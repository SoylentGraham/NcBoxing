Shader "Rewind/BackgroundSubtract" {
	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		BackgroundTex ("LastBackgroundTex", 2D) = "white" {}

		TruthMin("TruthMin", Float )=  0.10			//	only compare with BG lum if this good
		LumDiffMin("LumDiffMin", Float ) = 0.40		//	must be foreground contrasting against bg
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

			float LumDiffMin;
			float TruthMin;
			sampler2D _MainTex;	//	live
			sampler2D LastBackgroundTex;

			FragInput vert(VertexInput In) {
				FragInput Out;
				Out.Position = mul (UNITY_MATRIX_MVP, In.Position );
				Out.uv_MainTex = In.uv_MainTex;
				return Out;
			}
						
			float4 frag(FragInput In) : SV_Target 
			{
				//	get input lum
				float LiveLum = tex2D( _MainTex, In.uv_MainTex ).x;
				
				float BgLum =  tex2D( LastBackgroundTex, In.uv_MainTex ).x;
				float BgTruth = tex2D( LastBackgroundTex, In.uv_MainTex ).y;

				//	very similar to background, and we trust background
				if ( abs(LiveLum-BgLum) < LumDiffMin && BgTruth > TruthMin )
					return float4(1,0,0,0 );
				
				return float4( LiveLum, LiveLum, LiveLum, 1 );
			}
		ENDCG
	}
	} 
}
