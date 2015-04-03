using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class GuiHelper
{
	// The texture used by DrawLine(Color)
	private static Texture2D _coloredLineTexture;
	
	// The color used by DrawLine(Color)
	private static Color _coloredLineColor;
	
	/// <summary>
	/// Draw a line between two points with the specified color and a thickness of 1
	/// </summary>
	/// <param name="lineStart">The start of the line</param>
	/// <param name="lineEnd">The end of the line</param>
	/// <param name="color">The color of the line</param>
	public static void DrawLine(Vector2 lineStart, Vector2 lineEnd, Color color)
	{
		DrawLine(lineStart, lineEnd, color, 1);
	}
	
	/// <summary>
	/// Draw a line between two points with the specified color and thickness
	/// Inspired by code posted by Sylvan
	/// http://forum.unity3d.com/threads/17066-How-to-draw-a-GUI-2D-quot-line-quot?p=407005&viewfull=1#post407005
	/// </summary>
	/// <param name="lineStart">The start of the line</param>
	/// <param name="lineEnd">The end of the line</param>
	/// <param name="color">The color of the line</param>
	/// <param name="thickness">The thickness of the line</param>
	public static void DrawLine(Vector2 lineStart, Vector2 lineEnd, Color color, int thickness)
	{
		if (_coloredLineTexture == null || _coloredLineColor != color)
		{
			_coloredLineColor = color;
			_coloredLineTexture = new Texture2D(1, 1);
			_coloredLineTexture.SetPixel(0, 0, _coloredLineColor);
			_coloredLineTexture.wrapMode = TextureWrapMode.Repeat;
			_coloredLineTexture.Apply();
		}
		DrawLineStretched(lineStart, lineEnd, _coloredLineTexture, thickness);
	}
	
	/// <summary>
	/// Draw a line between two points with the specified texture and thickness.
	/// The texture will be stretched to fill the drawing rectangle.
	/// Inspired by code posted by Sylvan
	/// http://forum.unity3d.com/threads/17066-How-to-draw-a-GUI-2D-quot-line-quot?p=407005&viewfull=1#post407005
	/// </summary>
	/// <param name="lineStart">The start of the line</param>
	/// <param name="lineEnd">The end of the line</param>
	/// <param name="texture">The texture of the line</param>
	/// <param name="thickness">The thickness of the line</param>
	public static void DrawLineStretched(Vector2 lineStart, Vector2 lineEnd, Texture2D texture, int thickness)
	{
		Vector2 lineVector = lineEnd - lineStart;
		float angle = Mathf.Rad2Deg * Mathf.Atan(lineVector.y / lineVector.x);
		if (lineVector.x < 0)
		{
			angle += 180;
		}
		
		if (thickness < 1)
		{
			thickness = 1;
		}
		
		// The center of the line will always be at the center
		// regardless of the thickness.
		int thicknessOffset = (int)Mathf.Ceil(thickness / 2);
		
		GUIUtility.RotateAroundPivot(angle,
		                             lineStart);
		GUI.DrawTexture(new Rect(lineStart.x,
		                         lineStart.y - thicknessOffset,
		                         lineVector.magnitude,
		                         thickness),
		                texture);
		GUIUtility.RotateAroundPivot(-angle, lineStart);
	}
	
	/// <summary>
	/// Draw a line between two points with the specified texture and a thickness of 1
	/// The texture will be repeated to fill the drawing rectangle.
	/// </summary>
	/// <param name="lineStart">The start of the line</param>
	/// <param name="lineEnd">The end of the line</param>
	/// <param name="texture">The texture of the line</param>
	public static void DrawLine(Vector2 lineStart, Vector2 lineEnd, Texture2D texture)
	{
		DrawLine(lineStart, lineEnd, texture, 1);
	}
	
	/// <summary>
	/// Draw a line between two points with the specified texture and thickness.
	/// The texture will be repeated to fill the drawing rectangle.
	/// Inspired by code posted by Sylvan and ArenMook
	/// http://forum.unity3d.com/threads/17066-How-to-draw-a-GUI-2D-quot-line-quot?p=407005&viewfull=1#post407005
	/// http://forum.unity3d.com/threads/28247-Tile-texture-on-a-GUI?p=416986&viewfull=1#post416986
	/// </summary>
	/// <param name="lineStart">The start of the line</param>
	/// <param name="lineEnd">The end of the line</param>
	/// <param name="texture">The texture of the line</param>
	/// <param name="thickness">The thickness of the line</param>
	public static void DrawLine(Vector2 lineStart, Vector2 lineEnd, Texture2D texture, int thickness)
	{
		Vector2 lineVector = lineEnd - lineStart;
		float angle = Mathf.Rad2Deg * Mathf.Atan(lineVector.y / lineVector.x);
		if (lineVector.x < 0)
		{
			angle += 180;
		}
		
		if (thickness < 1)
		{
			thickness = 1;
		}
		
		// The center of the line will always be at the center
		// regardless of the thickness.
		int thicknessOffset = (int)Mathf.Ceil(thickness / 2);
		
		Rect drawingRect = new Rect(lineStart.x,
		                            lineStart.y - thicknessOffset,
		                            Vector2.Distance(lineStart, lineEnd),
		                            (float) thickness);
		GUIUtility.RotateAroundPivot(angle,
		                             lineStart);
		GUI.BeginGroup(drawingRect);
		{
			int drawingRectWidth = Mathf.RoundToInt(drawingRect.width);
			int drawingRectHeight = Mathf.RoundToInt(drawingRect.height);
			
			for (int y = 0; y < drawingRectHeight; y += texture.height)
			{
				for (int x = 0; x < drawingRectWidth; x += texture.width)
				{
					GUI.DrawTexture(new Rect(x,
					                         y,
					                         texture.width,
					                         texture.height),
					                texture);
				}
			}
		}
		GUI.EndGroup();
		GUIUtility.RotateAroundPivot(-angle, lineStart);
	}
}

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

[ExecuteInEditMode]
public class FistDebug : MonoBehaviour {

	public Texture			mInputTexture;
	public RenderTexture	mRayTexture;
	public Material			mRayMaterial;
	public RenderTexture	mSecondJointTexture;
	public Material			mSecondJointMaterial;
	public Texture2D		mSecondJointTextureCopy;
	public List<TJoint>		mJoints = new List<TJoint>();

	// Use this for initialization
	void Start () {
	
	}

	void OnDisable()
	{
		mJoints.Clear ();
		mSecondJointTextureCopy = null;
	}

	float radians(float Degrees)
	{
		return Degrees * Mathf.Deg2Rad;
	}

	List<TJoint> CalculateJoints(Color32[] SecondJointPixels,Texture InputTexture,int MaxJointLength,float AngleDegMin,float AngleDegMax)
	{
		int MaxJoints = 100;
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
			for ( int angstep=0;	angstep<PixelsHeight;	angstep++ )
			{
				float AngleDeg = Mathf.Lerp( AngleDegMin, AngleDegMax, (float)angstep / (float)PixelsHeight );
				int p = x + (angstep*Width);
				int r = SecondJointPixels[p].r;
				int g = SecondJointPixels[p].g;
				int b = SecondJointPixels[p].b;
				int a = SecondJointPixels[p].a;
				float JointLength = ( r / 255.0f) * MaxJointLength;
				float Height = ( g / 255.0f ) * InputTexture.height;
				if ( Height < 4 )
					continue;

				if ( JointLength < 4 )
					continue;
				//JointLength = 20;
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

	// Update is called once per frame
	void Update () {
	
		if (mInputTexture == null)
			return;


		if (mRayTexture == null || mRayMaterial == null)
			return;
		Graphics.Blit (mInputTexture, mRayTexture, mRayMaterial);



		if (mSecondJointTexture == null || mSecondJointMaterial == null)
			return;
		mSecondJointMaterial.SetTexture ("_RayTex", mRayTexture);
		mSecondJointMaterial.SetInt ("TargetHeight", mSecondJointTexture.height);
		Graphics.Blit (mInputTexture, mSecondJointTexture, mSecondJointMaterial);


		//	read joints

		if (UnityEditor.EditorApplication.isPlaying && mJoints.Count== 0) {

			if (mSecondJointTextureCopy == null) {
				mSecondJointTextureCopy = new Texture2D (mSecondJointTexture.width, mSecondJointTexture.height, TextureFormat.ARGB32, false);
			}
			RenderTexture.active = mSecondJointTexture;
			mSecondJointTextureCopy.ReadPixels (new Rect (0, 0, mSecondJointTexture.width, mSecondJointTexture.height), 0, 0);
			RenderTexture.active = null; 

			Color32[] SecondJointPixels = mSecondJointTextureCopy.GetPixels32 ();
			int MaxJointLength = 256;	//	in shader
			float AngleDegMin = mSecondJointMaterial.GetFloat ("AngleDegMin");
			float AngleDegMax = mSecondJointMaterial.GetFloat ("AngleDegMax");
	
			mJoints = CalculateJoints (SecondJointPixels, mSecondJointTextureCopy, MaxJointLength, AngleDegMin, AngleDegMax);
		}
	}

	void DrawTexture(int ScreenSectionX,int ScreenSectionY,Texture texture)
	{
		if (texture == null)
			return;
		
		float Sectionsx = Screen.width / 2;
		float Sectionsy = Screen.height / 2;
		Rect rect = new Rect( Sectionsx*ScreenSectionX, Sectionsy*ScreenSectionY, Sectionsx, Sectionsy );
		
		GUI.DrawTexture (rect, texture);
	}

	void FitToRect(ref Vector2 PosNorm,Rect rect)
	{
		if (PosNorm.x < 0)
			PosNorm.x = 0;
		if (PosNorm.y < 0)
			PosNorm.y = 0;
		if (PosNorm.x > 1)
			PosNorm.x = 1;
		if (PosNorm.y > 1)
			PosNorm.y = 1;
		
		//	y is inverted... at source? not the line drawing code?
		PosNorm.y = 1.0f - PosNorm.y;
		
		PosNorm.x *= rect.width;
		PosNorm.y *= rect.height;
		PosNorm.x += rect.xMin;
		PosNorm.y += rect.yMin;
	}

	Rect GetScreenRect(int ScreenSectionX,int ScreenSectionY)
	{
		float Sectionsx = Screen.width / 2;
		float Sectionsy = Screen.height / 2;
		Rect rect = new Rect( Sectionsx*ScreenSectionX, Sectionsy*ScreenSectionY, Sectionsx, Sectionsy );
		return rect;
	}

	void DrawJoint(TJoint joint,int ScreenSectionX,int ScreenSectionY)
	{
		Rect rect = GetScreenRect (ScreenSectionX, ScreenSectionY);

		Vector2 Start = new Vector2 (joint.mStart.x, joint.mStart.y);
		Vector2 Middle = new Vector2 (joint.mMiddle.x, joint.mMiddle.y);
		Vector2 End = new Vector2 (joint.mEnd.x, joint.mEnd.y);
		FitToRect( ref Start, rect );
		FitToRect( ref Middle, rect );
		FitToRect( ref End, rect );

		GuiHelper.DrawLine( Start, Middle, Color.red );
		GuiHelper.DrawLine( Middle, End, Color.green );
	}

	void OnGUI()
	{
		DrawTexture (0, 0, mInputTexture);
		DrawTexture (1, 0, mRayTexture);
		DrawTexture (1, 1, mSecondJointTexture);
		//DrawTexture (0, 1, mSecondJointTextureCopy);
		for ( int i=0;	i<mJoints.Count;	i++ )
		{
			DrawJoint( mJoints[i], 0, 0 );
		}

		GUI.Label( GetScreenRect( 0, 1 ), "Joints: " + mJoints.Count );
	}
}
