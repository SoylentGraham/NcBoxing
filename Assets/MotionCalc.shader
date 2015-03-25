Shader "Rewind/MotionCalc" {
	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		LumLastTex ("LumLastTex", 2D) = "white" {}
		DiffTolerance ("Tolerance", Float )= 0.03
		DiffRadius("DiffRadius",Int) = 5
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
			float DiffTolerance;
			int DiffRadius;			

			FragInput vert(VertexInput In) {
				FragInput Out;
				Out.Position = mul (UNITY_MATRIX_MVP, In.Position );
				Out.uv_MainTex = In.uv_MainTex;
				return Out;
			}
			
			float3 GetDeltaDiff(float BaseLum,int2 SampleOffsetPixels,FragInput In)
			{
				float2 PixelsToUv = float2( 256.0/1.0, 256.0/1.0 );
				float2 SampleOffsetUv = SampleOffsetPixels * PixelsToUv;
				float PrevLum = tex2D( LumLastTex, In.uv_MainTex + SampleOffsetUv ).r;
				return float3( SampleOffsetPixels.x, SampleOffsetPixels.y, BaseLum - PrevLum );
			}
			
			float3 GetBestDeltaDiff(float3 a,float3 b)
			{
				if ( abs(a.z) <= abs(b.z) )
					return a;
				else
					return b;
			}
			
			float3 GetRadiusBestDeltaDiff(float BaseLum,int Radius,FragInput In,float3 BestDeltaDiff)
			{
			
				//	top row
				for ( int x=-Radius;	x<=Radius;	x++ )
				{
					int y = -Radius;
					float3 RadDiff = GetDeltaDiff( BaseLum, int2( x,y ), In );
					BestDeltaDiff = GetBestDeltaDiff( BestDeltaDiff, RadDiff );
				}
				
				//	bottom row
				for ( int x=-Radius;	x<=Radius;	x++ )
				{
					int y = Radius;
					float3 RadDiff = GetDeltaDiff( BaseLum, int2( x,y ), In );
					BestDeltaDiff = GetBestDeltaDiff( BestDeltaDiff, RadDiff );
				}
				
				//	left col
				for ( int y=-Radius+1;	y<=Radius-1;	y++ )
				{
					int x = -Radius;
					float3 RadDiff = GetDeltaDiff( BaseLum, int2( x,y ), In );
					BestDeltaDiff = GetBestDeltaDiff( BestDeltaDiff, RadDiff );
				}
				
				//	right col
				for ( int y=-Radius+1;	y<=Radius-1;	y++ )
				{
					int x = Radius;
					float3 RadDiff = GetDeltaDiff( BaseLum, int2( x,y ), In );
					BestDeltaDiff = GetBestDeltaDiff( BestDeltaDiff, RadDiff );
				}
				
				return BestDeltaDiff;
			}
			
			float4 frag(FragInput In) : SV_Target 
			{
				float LumNew = tex2D( _MainTex, In.uv_MainTex ).r;
				float BaseLum = LumNew;

				//	initial value 
				float3 DeltaDiff = GetDeltaDiff( BaseLum, int2(0,0), In );
			
				//	search radius for best result
				for ( int r=1;	r<=DiffRadius;	r++ )
					DeltaDiff = GetRadiusBestDeltaDiff( BaseLum, r, In, DeltaDiff );
					
					
				//	normalise (-r...r) to 0...1
				float2 Delta = DeltaDiff.xy;			//	-r...r
				
				Delta /= float2(DiffRadius,DiffRadius);	//	-1...1
				Delta /= float2(2,2);	//	-0.5...0.5
				Delta += float2(0.5,0.5);
			
					
				float Diff = abs(DeltaDiff.z);
				//float Diff = 0;
					
				return float4( Delta.x, Delta.y, Diff, 1.0 );
				
				/*			
				float LumLast = tex2D( LumLastTex, In.uv_MainTex ).r;
				
				if ( (LumLast-LumNew) > DiffTolerance )
					return float4( 1,0,0,1 );
					
				if ( (LumLast-LumNew) < -DiffTolerance )
					return float4( 0,1,0,1 );
					
				return float4( 0,0,0,1 );
				*/
			}

		ENDCG
		}
	} 
}
