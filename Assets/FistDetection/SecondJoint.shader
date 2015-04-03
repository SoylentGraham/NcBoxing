Shader "Rewind/SecondJoint" {
	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		_RayTex ("_RayTex", 2D) = "white" {}
		AngleDegMin("AngleDegMin", Float ) = -90
		AngleDegMax("AngleDegMax", Float ) = 90
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

			sampler2D _RayTex;	//	height of mask at x
			sampler2D _MainTex;	//	original mask
			float4 _MainTex_TexelSize;
			float AngleDegMin;
			float AngleDegMax;
			const int MaxJointLength = 256;

			FragInput vert(VertexInput In) {
				FragInput Out;
				Out.Position = mul (UNITY_MATRIX_MVP, In.Position );
				Out.uv_MainTex = In.uv_MainTex;
				return Out;
			}
		
			bool IsMask(float2 st)
			{
				float Alpha = tex2D( _MainTex, st ).r;
				return ( Alpha < 0.7f );
			}
		
			int GetRayLength(float2 StartUv,float AngleDeg)
			{
				float2 AngleStep = float2( sin(radians( AngleDeg )), cos(radians(AngleDeg) ) );
				AngleStep = normalize( AngleStep );
				
				for ( int i=1;	i<MaxJointLength;	i++ )
				{
					float2 Delta = AngleStep * (float)i;
					if ( !IsMask( StartUv + Delta ) )
						return (i-1);
				}
				return MaxJointLength;
			}
					
			float Lerp(float from,float to,float step)
			{
				return ((to-from)*step) + from;
				}
					
			float4 frag(FragInput In) : SV_Target 
			{
				float AngleDeg = Lerp( AngleDegMin, AngleDegMax, In.uv_MainTex.y );
				int HeightMax = _MainTex_TexelSize.w;	//	original texture height used to normalise height
				float Heightf = tex2D( _RayTex, float2(In.uv_MainTex.x,0) ).r;
				int Height = Heightf * (float)HeightMax;
				
				//	starting uv
				float2 StartUv = float2( In.uv_MainTex.x, Heightf );
				int RayLength = GetRayLength( StartUv, AngleDeg );
			
				float LengthNorm = clamp( (float)RayLength / (float)MaxJointLength, 0, 1 );
				return float4( LengthNorm, Heightf, LengthNorm, 1.0f );
			}
		ENDCG
	}
	} 
}
