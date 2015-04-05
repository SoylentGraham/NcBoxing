﻿Shader "Rewind/Flip" {
	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		Flip("Flip", Int ) = 0
		Mirror("Mirror", Int ) = 0
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

			sampler2D _MainTex;	//	new lum
			int	Flip;
			int	Mirror;
			
			FragInput vert(VertexInput In) {
				FragInput Out;
				Out.Position = mul (UNITY_MATRIX_MVP, In.Position );
				Out.uv_MainTex = In.uv_MainTex;
				if ( Flip )
					Out.uv_MainTex.y = 1.0f - Out.uv_MainTex.y;
				if ( Mirror )
					Out.uv_MainTex.x = 1.0f - Out.uv_MainTex.x;
				return Out;
			}
	
			float4 frag(FragInput In) : SV_Target 
			{
				return tex2D( _MainTex, In.uv_MainTex );
			}
		ENDCG
	}
	} 
}
