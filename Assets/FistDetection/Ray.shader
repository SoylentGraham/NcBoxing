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
			const int RayPad = 2;

			FragInput vert(VertexInput In) {
				FragInput Out;
				Out.Position = mul (UNITY_MATRIX_MVP, In.Position );
				Out.uv_MainTex = In.uv_MainTex;
				return Out;
			}
		
			bool IsMask(float2 st)
			{
				//	gr: change this to a texture border to stop
				if ( st.x < 0.0f || st.x > 1.0f || st.y < 0.0f || st.y > 1.0f )
					return false;
				float Alpha = tex2D( _MainTex, st ).a;
				return ( Alpha > 0.5f );
			}
			
			int GetColumnHeight(float s)
			{
				int Height = _MainTex_TexelSize.w;
				for ( int i=0;	i<Height;	i++)
				{
					float t = (float)i * _MainTex_TexelSize.y;
					if ( !IsMask( float2(s,t) ) )
						return max( (i-1)-RayPad,0);
				}
				int i = Height;
				return max( (i-1)-RayPad,0);
			}
							
			float4 frag(FragInput In) : SV_Target 
			{
				int Height = GetColumnHeight( In.uv_MainTex.x );
				Height /= 2;
				float HeightNorm = (float)Height / _MainTex_TexelSize.w;
				return float4( HeightNorm, 0, 0, 1.0f );
			}
		ENDCG
	}
	} 
}
