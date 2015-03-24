Shader "Rewind/VideoToLum" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		BlockWidth("BlockWidth", int) = 16
		BlockHeight("BlockHeight", int) = 16
		MainTexWidth("MainTexWidth", int) = 512
		MainTexHeight("MainTexHeight", int) = 512
	}
	SubShader {
	Pass
	{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			int MaintexWidth;
			int MaintexHeight;
			int BlockWidth;
			int BlockHeight;
	
			struct VertexInput {
				float2 uv_MainTex : TEXCOORD0;
			};
			
			struct FragInput {
				float2	uv_MainTex : TEXCOORD0;
			};

			
			float RgbaToLum(fixed4 Rgba)
			{
				//	gr: get proper lum weights
				float Lumr = Rgba.x * 0.3333f;
				float Lumg = Rgba.y * 0.3333f;
				float Lumb = Rgba.z * 0.3333f;
				return Lumr + Lumg + Lumb;
			}
			

			float GetLuminance(float2 BlockUv,int XOffset,int YOffset)
			{
				//	blocks start at top left, so add pixels (as uv)
			//	BlockUv.x += XOffset / MaintexWidth;
			//	BlockUv.y += YOffset / MaintexHeight;
				fixed4 Rgba = tex2D ( _MainTex, BlockUv );
				return RgbaToLum( Rgba );
			}
			
			FragInput vert(VertexInput In) {
				FragInput Out;
	//			Out.Position = mul (UNITY_MATRIX_MVP, In.Position );
				Out.uv_MainTex = In.uv_MainTex;
				return Out;
			}

			float4 frag(FragInput In) : SV_Target {
				
				float Lum = GetLuminance( In.uv_MainTex, 0, 0 );
				/*
				//	for this pixel, sample the block from the main texture
				float Lum = 0.f;
				int SampleCount = BlockWidth * BlockHeight;
				for ( int y=0;	y<BlockHeight;	y++) 
				for ( int x=0;	x<BlockWidth;	x++ )
				{
					Lum += GetLuminance( In.uv_MainTex, x, y );
					SampleCount ++;
				}
				
				//	get average lum
				Lum /= (float)SampleCount;
				*/
		
				return float4( Lum, Lum, Lum, 1 );
			}

		ENDCG
			}	
	} 
}
