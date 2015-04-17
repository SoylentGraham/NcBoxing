Shader "Rewind/TrackFeatures" {
	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		FeaturesPrev ("FeaturesPrev", 2D) = "white" {}
		SampleRadius("SampleRadius",Int) = 4
		MaxHitCount("MaxHitCount",Range(1,40)) = 3	//	over this and we disregard this feature as non-unique
		MinScore("MinScore", Range(0,1)) = 0.7
	}
	SubShader {
	 Pass {
		CGPROGRAM

			#include "PopCommon.cginc"

			#pragma vertex vert
			#pragma fragment frag
	
	
			struct VertexInput {
				float4 Position : POSITION;
				float2 uv_MainTex : TEXCOORD0;
			};
			
			struct FragInput {
				float4 Position : SV_POSITION;
				float2	uv_MainTex : TEXCOORD0;
			};


			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			sampler2D FeaturesPrev;
			float4 FeaturesPrev_TexelSize;
			int SampleRadius;
			int MaxHitCount;
			float MinScore;
			
			FragInput vert(VertexInput In) {
				FragInput Out;
				Out.Position = mul (UNITY_MATRIX_MVP, In.Position );
				Out.uv_MainTex = In.uv_MainTex;
				return Out;
			}

			int GetMatchingBitCount(int a,int b,int Elements)
			{
				int MatchCount = 0;
				for ( int i=0;	i<Elements;	i++ )
				{
					MatchCount += ( HASBIT0(a) == HASBIT0(b) ) ? 1 : 0;
					//	shift down
					a = RIGHTSHIFTONCE(a);
					b = RIGHTSHIFTONCE(b);
				}
				return MatchCount;
			}
					
			
			float GetFeatureScore(int2 FeatureA,int2 FeatureB)
			{
				//	count matching bits
				//	todo: rotate ring by shifting/rolling
				int MatchInner = GetMatchingBitCount( FeatureA.x, FeatureB.x, InnerSampleCount );
				int MatchOuter = GetMatchingBitCount( FeatureA.y, FeatureB.y, OuterSampleCount );
				float ScoreWeightInner = 0.5f;
				float ScoreWeightOuter = 0.5f;
				float Score = 0.0f;
				Score += (MatchInner/(float)InnerSampleCount) * ScoreWeightInner;
				Score += (MatchOuter/(float)OuterSampleCount) * ScoreWeightOuter;
				return Score;
			}

			int2 GetFeature2(float4 Feature4)
			{
				int FeatureInner = 0;
				int FeatureOuter = 0;
				FeatureInner = OR( FeatureInner, Feature4.x * 255.f * 1.0f );
				FeatureInner = OR( FeatureInner, Feature4.y * 255.f * 256.0f );	//	shifted by 8
				FeatureOuter = OR( FeatureOuter, Feature4.z * 255.f * 1.0f );
				FeatureOuter = OR( FeatureOuter, Feature4.w * 255.f * 256.0f );	//	shifted by 8
				return int2(FeatureInner,FeatureOuter);
			}
						
			int2 GetPrevFeature(float2 Uv,int2 Offset)
			{
				Uv += Offset * FeaturesPrev_TexelSize.xy;
				float4 Feature4 = tex2D( FeaturesPrev, Uv );
				return GetFeature2( Feature4 );
			}
			
			int2 GetNewFeature(float2 Uv,int2 Offset)
			{
				Uv += Offset * FeaturesPrev_TexelSize.xy;
				float4 Feature4 = tex2D( FeaturesPrev, Uv );
				return GetFeature2( Feature4 );
			}
			
			float4 frag(FragInput In) : SV_Target 
			{
				float4 Result_TooManyHits = float4( 0,1,0,0 );
				float4 Result_NoHits = float4( 1,0,0,0 );
				
				int2 Feature = GetPrevFeature( In.uv_MainTex, int2(0,0) );
				//	todo: offset this with prediction from kalman or accellerometer or gyro
				float2 SampleOrigin = In.uv_MainTex;
				
				int2 BestIndex = int2(0,0);
				float BestScore = -1;
				int HitCount = 0;
				
				for ( int y=-SampleRadius;	y<=SampleRadius;	y++ )
				for ( int x=-SampleRadius;	x<=SampleRadius;	x++ )
				{
					int2 MatchFeature = GetNewFeature( SampleOrigin, int2(x,y) );
					float Score = GetFeatureScore( Feature, MatchFeature );
					if ( Score < MinScore )
						continue;
					if ( Score <= BestScore )
						continue;
					BestScore = Score;
					BestIndex = int2(x,y);
					HitCount++;
					if ( HitCount > MaxHitCount )
						return Result_TooManyHits;
				}
				
				if ( HitCount == 0 )
					return Result_NoHits;
				float2 MatchUvDelta = BestIndex*_MainTex_TexelSize.xy;
				float2 MatchUv = SampleOrigin + MatchUvDelta;
				return float4( MatchUvDelta.x, MatchUvDelta.y, BestScore, 1 );
			}

		ENDCG
		}
	} 
}