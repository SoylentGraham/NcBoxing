Shader "Rewind/BackgroundLearner" {
	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		LastBackgroundTex ("LastBackgroundTex", 2D) = "white" {}
		AgeMax	("AgeMax", Int ) = 1000
		Init	("Init", Int ) = 1
		LumDiffMax	("LumDiffMax", Float ) = 0.99
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
				Truth = clamp( Truth, 0, 1 );
				//	lum is lum
				//	truth is 0..1 (integrety)
				//	age is frames/max frames
				float Agef = (float)Age / (float)AgeMax;
				Agef = clamp( Agef, 0, 1 );
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
				return MakeLumTruthAge( NewLum, 1.0/(float)AgeMax, 1 );
			}
			
			float4 UpdateLumTruthAge(FragInput In)
			{
				float4 OldLumTruthAge = GetLumTruthAge(In.uv_MainTex);
				float NewLumSample = tex2D( _MainTex, In.uv_MainTex ).r;


				//	get score of this lum
				float LumDiff = abs(NewLumSample - OldLumTruthAge.x);
				float LumScore = 1.0f - ( clamp( LumDiff / LumDiffMax, 0, 1 ) );
				bool BadLum = (LumDiff / LumDiffMax) >= 1.0f;
				
				float OldTruth = OldLumTruthAge.y;
				float NewTruth = OldTruth;
				
				//	use agemax for slow build up, use .z for very fast learn
				float FrameDelta = 1.0f / (float)AgeMax;
				//float FrameDelta = 1.0f / (float)OldLumTruthAge.z;
				
				//	if lum score is bad, we want to decrease the truth ("this pixel is noisy")
				if ( BadLum )
				{
					NewTruth -= FrameDelta;
				}
				else
				{
					NewTruth += LumScore * FrameDelta;
				}
												
				float NewLumInfluence = 5.0f;
				float NewWeight = LumScore * (FrameDelta*NewLumInfluence) * (1.0 - NewTruth);
				float OldWeight = 1.0f - NewWeight;
				
				float OldLum = OldLumTruthAge.x;
				float NewLum = (NewLumSample*NewWeight) + (OldLum*OldWeight);
								
				int NewAge = OldLumTruthAge.z + 1;
				
				return MakeLumTruthAge( NewLum, NewTruth, NewAge );
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
