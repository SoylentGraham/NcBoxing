Shader "Rewind/BackgroundSubtract" {
	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		BackgroundTex ("LastBackgroundTex", 2D) = "white" {}
		AgeMax	("AgeMax", Int ) = 100 
		BackgroundTruthMin("BackgroundTruthMin", Float ) = 0.03
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
				return float4(Lum,Truth,Agef,1);
			}

			float4 GetLumTruthAge(float2 Uv)
			{
				float4 LumTruthAge = tex2D( LastBackgroundTex, Uv );
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
				
				//	do we use this new lum, or is it wildly different that we ignore it?
				float NewLum = tex2D( _MainTex, In.uv_MainTex ).r;
				float LumDiff = NewLum - OldLumTruthAge.x;
				
				//	gr: lose some integrety here?
				if ( abs(LumDiff) > LumDiffMax )
					return OldLumTruthAge;
					
				//	update by
				//	increasing age
				//	averaging Lum
				//	increase/decreate truth by the variance
				float FrameWeight = 1.0f / OldLumTruthAge.z;
				float NewAvgLum = Lerp( NewLum, OldLumTruthAge.x, FrameWeight ); 	//	10 frames old, merge 1/10th 
				float NewTruth = OldLumTruthAge.y + (((1.0f-abs(LumDiff))/LumDiffMax) * FrameWeight);
				
				return MakeLumTruthAge( NewAvgLum, NewTruth, OldLumTruthAge.z+1 );
			}
						
			float4 frag(FragInput In) : SV_Target 
			{
			//	if ( Init != 0 )
					return InitLumTruthAge(In);
			//	else
			//		return UpdateLumTruthAge(In);
			}
		ENDCG
	}
	} 
}
