Shader "Rewind/BackgroundLearner" {
	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		LastBackgroundTex ("LastBackgroundTex", 2D) = "white" {}
		AgeMax	("AgeMax", Int ) = 100
		Init	("Init", Int ) = 1
		LumDiffMax	("LumDiffMax", Float ) = 0.30
	}
	SubShader {
	
	pass
	{
		CGPROGRAM

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

			int Init;
			int AgeMax;
			float LumDiffMax;
			sampler2D _MainTex;	//	new lum
			sampler2D LastBackgroundTex;

			FragInput vert(VertexInput In) {
				FragInput Out;
				Out.Position = mul (UNITY_MATRIX_MVP, In.Position );
				Out.uv_MainTex = In.uv_MainTex;
				return Out;
			}
			
			float Lerp(float Min,float Max,float Time)
			{
				return Min + (( Max - Min )* Time);
			}
			
			float4 MakeLumTruthAge(float Lum,float Truth,int Age)
			{
				//	lum is lum
				//	truth is 0..1 (integrety)
				//	age is frames/max frames
				float Agef = (float)Age / (float)AgeMax;
				Agef = min( 1.0, Agef );
				Agef = max( 0.0, Agef );
				return float4(Lum,Truth,Agef,1).xyzw;
			}

			float4 GetLumTruthAge(float2 Uv)
			{
				float4 LumTruthAge = tex2D( LastBackgroundTex, Uv ).xyzw;
				LumTruthAge.z *= (float)AgeMax;
				return LumTruthAge;
			}

			float4 InitLumTruthAge(FragInput In)
			{
				float NewLum = tex2D( _MainTex, In.uv_MainTex );
				return MakeLumTruthAge( NewLum, 1.0, 1 );
			}
			
			float4 UpdateLumTruthAge(FragInput In)
			{
				float4 OldLumTruthAge = GetLumTruthAge(In.uv_MainTex);
				float NewLumSample = tex2D( _MainTex, In.uv_MainTex ).r;


				//	get score of this lum
				float LumDiff = NewLumSample - OldLumTruthAge.x;
				float LumScore = 1.0f - ( clamp( abs(LumDiff) / LumDiffMax, 0, 1 ) );
				
				float NewWeight = LumScore / OldLumTruthAge.z;
				float OldWeight = 1.0f - NewWeight;
				
				//	merge with old score
				float OldScore = OldLumTruthAge.y;
				float NewScore = (LumScore*NewWeight) + (OldScore*OldWeight);
				
				float OldLum = OldLumTruthAge.x;
				float NewLum = (NewLumSample*NewWeight) + (OldLum*OldWeight);
				
				int NewAge = OldLumTruthAge.z + 1;
				
				return MakeLumTruthAge( NewLum, NewScore, NewAge );
				/*
				
				//	do we use this new lum, or is it wildly different that we ignore it?
				float LumDiff = NewLum - OldLumTruthAge.x;
				
				//	gr: lose some integrety here?
				if ( abs(LumDiff) > LumDiffMax )
				{
					int NewAge = OldLumTruthAge.z-1;
					float NewTruth = OldLumTruthAge.y;
					float NewAvgLum = OldLumTruthAge.x;
					return MakeLumTruthAge( NewAvgLum, NewTruth, NewAge );
				}
					
				//	update by
				//	increasing age
				//	averaging Lum
				//	increase/decreate truth by the variance
				float FrameWeight = 1.0f / OldLumTruthAge.z;
				float NewAvgLum = Lerp( NewLum, OldLumTruthAge.x, FrameWeight ); 	//	10 frames old, merge 1/10th 
				float NewTruth = OldLumTruthAge.y + (((1.0f-abs(LumDiff))/LumDiffMax) * FrameWeight);
				int NewAge = OldLumTruthAge.z+1;
				
				return MakeLumTruthAge( NewAvgLum, NewTruth, NewAge );
				*/
			}
						
			float4 frag(FragInput In) : SV_Target 
			{
				if ( Init != 0 )
					return InitLumTruthAge(In);
				else
					return UpdateLumTruthAge(In);
			}
		ENDCG
	}
	} 
}
