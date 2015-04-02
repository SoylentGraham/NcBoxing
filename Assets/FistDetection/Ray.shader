Shader "Rewind/Ray" {
	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
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
			float4 _MainTex_TexelSize;

			FragInput vert(VertexInput In) {
				FragInput Out;
				Out.Position = mul (UNITY_MATRIX_MVP, In.Position );
				Out.uv_MainTex = In.uv_MainTex;
				return Out;
			}
		
		
			int GetColumnHeight(float s)
			{
				int Height = _MainTex_TexelSize.w;
				for ( int i=0;	i<Height;	i++)
				{
					float t = (float)i * _MainTex_TexelSize.y;
					float Alpha = tex2D( _MainTex, float2( s, t ) ).r;
					if ( Alpha > 0.5f )
						return i;
				}
				return Height;
			}
							
			float4 frag(FragInput In) : SV_Target 
			{
				int Height = GetColumnHeight( In.uv_MainTex.x );
				bool Valid = (Height!=0) ;
				float HeightNorm = (float)Height / _MainTex_TexelSize.w;
				return float4( Valid?1:0, HeightNorm, 0, 1.0f );
			}
		ENDCG
	}
	} 
}
