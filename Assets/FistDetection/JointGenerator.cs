using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public struct float2
{
	public float	x;
	public float	y;
	
	public float2(float _x,float _y)
	{
		x = _x;
		y = _y;
	}
};

public struct TJoint
{
	public float2	mStart;
	public float2	mMiddle;
	public float2	mEnd;
};



public class TJointCalculator
{
	private Texture2D		mSecondJointTextureCopy;
	
	float radians(float Degrees)
	{
		return Degrees * Mathf.Deg2Rad;
	}

	void OnDisable()
	{
	}

	public List<TJoint> CalculateJoints(Texture MaskTexture,RenderTexture mRayTexture,Material mRayMaterial,RenderTexture mSecondJointTexture,Material mSecondJointMaterial)
	{
		if (MaskTexture == null)
			return null;
		
		if (mRayTexture == null || mRayMaterial == null)
			return null;

		mRayTexture.DiscardContents ();
		Graphics.Blit (MaskTexture, mRayTexture, mRayMaterial);
		
		
		
		if (mSecondJointTexture == null || mSecondJointMaterial == null)
			return null;
		mSecondJointMaterial.SetTexture ("_RayTex", mRayTexture);
		mSecondJointMaterial.SetInt ("TargetHeight", mSecondJointTexture.height);
		mSecondJointTexture.DiscardContents ();
		Graphics.Blit (MaskTexture, mSecondJointTexture, mSecondJointMaterial);
		
		
		//	calc joints frmo pixel data
		if (mSecondJointTextureCopy == null) {
			mSecondJointTextureCopy = new Texture2D (mSecondJointTexture.width, mSecondJointTexture.height, TextureFormat.ARGB32, false);
		}
		
		RenderTexture.active = mSecondJointTexture;
		mSecondJointTextureCopy.ReadPixels (new Rect (0, 0, mSecondJointTexture.width, mSecondJointTexture.height), 0, 0);
		RenderTexture.active = null; 
		
		Color32[] SecondJointPixels = mSecondJointTextureCopy.GetPixels32 ();
		int MaxJointLength = mSecondJointMaterial.GetInt("MaxJointLength");
		float AngleDegMin = mSecondJointMaterial.GetFloat ("AngleDegMin");
		float AngleDegMax = mSecondJointMaterial.GetFloat ("AngleDegMax");
		
		return CalculateJoints (SecondJointPixels, mSecondJointTextureCopy, MaxJointLength, AngleDegMin, AngleDegMax);
	}
	
	
	List<TJoint> CalculateJoints(Color32[] SecondJointPixels,Texture InputTexture,int MaxJointLength,float AngleDegMin,float AngleDegMax)
	{
		int MaxJoints = 400;
		List<TJoint> Joints = new List<TJoint> ();
		
		//	gr: verify w/h agains pixels size?
		int Width = InputTexture.width;
		int PixelCount = SecondJointPixels.Length;
		if (PixelCount % Width != 0)
			return Joints;
		
		//	should match mSecondJointTexture.height
		int PixelsHeight = PixelCount / Width;
		
		//	gr: could probbaly be more cache friendly by approaching this row-by-row...
		for (int x=0; x<Width; x++) 
		{
			//	height is the same for each angle so we can skip that quick
			int g = SecondJointPixels[x].g;	//	x+angstep*width == x
			float Height = ( g / 255.0f ) * InputTexture.height;
			if ( Height < 1 )
				continue;
			
			//	get the longest joint for this X
			float BestJointLength = 0.0f;
			int BestJointAng = -1;
			
			for ( int angstep=0;	angstep<PixelsHeight;	angstep++ )
			{
				int p = x + (angstep*Width);
				int r = SecondJointPixels[p].r;
			//	float AngleDeg = Mathf.Lerp( AngleDegMin, AngleDegMax, (float)angstep / (float)PixelsHeight );
				
				float JointLength = ( r / 255.0f) * MaxJointLength;
				if ( JointLength < BestJointLength )
					continue;
				
				BestJointLength = JointLength;
				BestJointAng = angstep;
			}
			
			if ( BestJointAng < 0 )
				continue;
			if ( BestJointLength < 1 )
				continue;
			
			{
				int angstep = BestJointAng;
				int p = x + (angstep*Width);
				int r = SecondJointPixels[p].r;
				float AngleDeg = Mathf.Lerp( AngleDegMin, AngleDegMax, (float)angstep / (float)PixelsHeight );
				float JointLength = ( (float)r / 255.0f) * MaxJointLength;
				
				//	gr: something wrong in this calc? half seems to look right
			//	JointLength /= 2.0f;
				
				float AngleRad = radians(AngleDeg);
				Vector2 AngleVector = new Vector2( Mathf.Sin(AngleRad), Mathf.Cos(AngleRad) );
				AngleVector.Normalize();
				AngleVector *= JointLength;
				
				float2 UvScalar = new float2( 1.0f / InputTexture.width, 1.0f / InputTexture.height );
				TJoint joint = new TJoint();
				joint.mStart = new float2( x*UvScalar.x, 0 );
				joint.mMiddle = new float2( x*UvScalar.x, Height*UvScalar.y );
				joint.mEnd = new float2( joint.mMiddle.x + (AngleVector.x*UvScalar.x), joint.mMiddle.y + (AngleVector.y*UvScalar.y) );
				Joints.Add( joint );
				
				if ( Joints.Count >= MaxJoints )
					return Joints;
			}
		}
		return Joints;
	}
	
}



public class JointGenerator : MonoBehaviour {
	
	public RenderTexture mRayTexture;
	public Material mRayMaterial;
	public RenderTexture mSecondJointTexture;
	public Material mSecondJointMaterial;
	private TJointCalculator	mJointCalculator = new TJointCalculator();
	public Texture			mMaskTexture;
	public List<TJoint>		mJoints = new List<TJoint>();

	void OnDisable()
	{
		mJoints.Clear ();
		mJointCalculator = null;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (mMaskTexture == null)
			return;
		
		if (mJointCalculator == null) {
			mJointCalculator = new TJointCalculator ();
		}
		
		mJoints = mJointCalculator.CalculateJoints (mMaskTexture, mRayTexture, mRayMaterial, mSecondJointTexture, mSecondJointMaterial);
	}
	
}
