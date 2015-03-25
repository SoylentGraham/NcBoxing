﻿Shader "Rewind/MotionCalc" {
	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		LumLastTex ("LumLastTex", 2D) = "white" {}
	}
	SubShader {
	 Pass {
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


			sampler2D _MainTex;
			sampler2D LumLastTex;
			

			FragInput vert(VertexInput In) {
				FragInput Out;
				Out.Position = mul (UNITY_MATRIX_MVP, In.Position );
				Out.uv_MainTex = In.uv_MainTex;
				return Out;
			}

			float4 frag(FragInput In) : SV_Target {
				float LumLast = tex2D( LumLastTex, In.uv_MainTex ).r;
				float LumNew = tex2D( _MainTex, In.uv_MainTex ).r;
				
				float Tolerance = 0.02f;
				if ( abs(LumLast-LumNew) > Tolerance )
					return float4( 1,1,1,1 );
				return float4( 0,0,0,1 );
			}

		ENDCG
		}
	} 
}
