﻿Shader "Rewind/AlphaFiller" {
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
			const int SampleRadius = 2;
			//#define SampleRadius 2
			const int HitCountMin = 7;


			FragInput vert(VertexInput In) {
				FragInput Out;
				Out.Position = mul (UNITY_MATRIX_MVP, In.Position );
				Out.uv_MainTex = In.uv_MainTex;
				return Out;
			}
			
			bool HasHit(FragInput In,int2 offi)
			{
				float2 offf = offi * _MainTex_TexelSize.xy;
				float2 uv = In.uv_MainTex + offf;
				float Alpha = tex2D( _MainTex, uv ).w;
				return Alpha > 0.1f;
			}
			
			int GetRadiusHitCount(int Radius,FragInput In)
			{
				int HitCount = 0;
			
				//	top & bottom row
				for ( int x=-Radius;	x<=Radius;	x++ )
				{
					HitCount += HasHit( In, int2( x,-Radius ) );
					HitCount += HasHit( In, int2( x,Radius ) );
					
					if ( x == -Radius || x == Radius )
					continue;
					HitCount += HasHit( In, int2( -Radius,x ) );
					HitCount += HasHit( In, int2( Radius,x ) );
				}
			
				/*	
				//	left & right col
				for ( int y=(-Radius)+1;	y<=Radius-1;	y++ )
				{
					HitCount += HasHit( In, int2( -Radius,y ) );
					HitCount += HasHit( In, int2( Radius,y ) );
				}
				*/
				
				return HitCount;
			}
							
			float4 frag(FragInput In) : SV_Target 
			{
				float4 Sample = tex2D( _MainTex, In.uv_MainTex );
				
				//	already alpha
				if ( Sample.w > 0 )
					return Sample;
	
				//	should we be alpha?
				//int HitCount = Sample.w>0 ? 1 : 0;
				int HitCount = 0;
				for ( int r=1;	r<=SampleRadius;	r++ )
					HitCount += GetRadiusHitCount(r, In );
			
				Sample.w = ( HitCount >= HitCountMin ) ? 1 : 0;
				return Sample;
			}
		ENDCG
	}
	} 
}
