Shader "Rewind/SecondJoint" {
	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		_RayTex ("_RayTex", 2D) = "white" {}
		AngleDegMin("AngleDegMin", Range(-180,180) ) = -90
		AngleDegMax("AngleDegMax", Range(-180,180) ) = 90
		MaxJointLength("MaxJointLength", Int ) = 40
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
			int MaxJointLength;
			const int RayPad = 3;

			FragInput vert(VertexInput In) {
				FragInput Out;
				Out.Position = mul (UNITY_MATRIX_MVP, In.Position );
				Out.uv_MainTex = In.uv_MainTex;
				return Out;
			}
		
			bool IsMask(float2 st)
			{
				//	gr: change this to a texture border to stop
				if ( st.x < 0.0f || st.x > 1.0f )
					return false;
				if ( st.y < 0.0f || st.y > 1.0f )
					return false;
				float Alpha = tex2D( _MainTex, st ).r;
				return ( Alpha < 0.7f );
			}
		
			int GetRayLength(float2 StartUv,float AngleDeg)
			{
				//	angles seem to be backwards to renderer...
				AngleDeg += 0.0f;
				float2 AngleStep = float2( sin(radians( AngleDeg )), cos(radians(AngleDeg) ) );
				AngleStep = normalize( AngleStep );
				//	step needs to be in pixels!
				AngleStep *= _MainTex_TexelSize.x;	//	errr x or y... hmm kinda require square textures
				
				for ( int i=0;	i<MaxJointLength;	i++ )
				{
					float2 Delta = AngleStep * (float)i;
					if ( !IsMask( StartUv + Delta ) )
						return max( (i-1)-RayPad, 0 );
				}
				int i = MaxJointLength;
				return max( (i-1)-RayPad,0);
			}
		
			float SoyLerp(float from,float to,float step)
			{
				return ((to-from)*step) + from;
			}
					
			float4 frag(FragInput In) : SV_Target 
			{
				float AngleDeg = SoyLerp( AngleDegMin, AngleDegMax, In.uv_MainTex.y );
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
