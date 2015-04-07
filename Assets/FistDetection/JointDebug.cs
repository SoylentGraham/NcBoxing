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

	//	draw circle around point
	public static void DrawCircle(Vector2 Center, float Radius, Color color,int Segments=10)
	{
		float AngStep = 360.0f / (float)Segments;
		for (float a=0; a<=360.0f+AngStep; a+=AngStep) {
			float a_rad = Mathf.Deg2Rad * a;
			float b_rad = Mathf.Deg2Rad * (a+AngStep);
			var aOff = new Vector2( Mathf.Cos (a_rad) * Radius, Mathf.Sin (a_rad) * Radius );
			var bOff = new Vector2( Mathf.Cos (b_rad) * Radius, Mathf.Sin (b_rad) * Radius );
			DrawLine ( Center+aOff, Center+bOff, color, 1);
		}
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



public class JointDebug : MonoBehaviour {

	public JointGenerator	mJointGenerator;

	
	void DrawTexture(int ScreenSectionX,int ScreenSectionY,Texture texture)
	{
		if (texture == null)
			return;
		
		float Sectionsx = Screen.width / 2;
		float Sectionsy = Screen.height / 2;
		Rect rect = new Rect( Sectionsx*ScreenSectionX, Sectionsy*ScreenSectionY, Sectionsx, Sectionsy );
		
		GUI.DrawTexture (rect, texture);
	}
	
	static void FitToRect(ref Vector2 PosNorm,Rect rect)
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
	
	static Rect GetScreenRect(int ScreenSectionX,int ScreenSectionY)
	{
		float Sectionsx = Screen.width / 2;
		float Sectionsy = Screen.height / 2;
		Rect rect = new Rect( Sectionsx*ScreenSectionX, Sectionsy*ScreenSectionY, Sectionsx, Sectionsy );
		return rect;
	}
	
	public static void DrawJoint(TJoint joint,int ScreenSectionX,int ScreenSectionY)
	{
		Rect rect = GetScreenRect (ScreenSectionX, ScreenSectionY);
		DrawJoint (joint, rect);
	}

	public static void DrawJoint(TJoint joint,Rect ScreenRect)
	{
		Vector2 Start = new Vector2 (joint.mStart.x, joint.mStart.y);
		Vector2 Middle = new Vector2 (joint.mMiddle.x, joint.mMiddle.y);
		Vector2 RayEnd = new Vector2 (joint.mRayEnd.x, joint.mRayEnd.y);
		Vector2 End = new Vector2 (joint.mEnd.x, joint.mEnd.y);
		FitToRect( ref Start, ScreenRect );
		FitToRect( ref Middle, ScreenRect );
		FitToRect( ref RayEnd, ScreenRect );
		FitToRect( ref End, ScreenRect );

		GuiHelper.DrawLine( Start, Middle, Color.red );
		GuiHelper.DrawLine( Middle, RayEnd, Color.green );
		GuiHelper.DrawCircle (End, joint.mEndRadius, Color.magenta);
	}

	void Update()
	{
		/*
		Texture mInputTexture = mJointGenerator ? mJointGenerator.mInputTexture : null;
		if (mInputTexture != null) {
			//	update mask test
			if (mMaskTestMaterial ) {
				if ( mMaskTestTexture == null )
					mMaskTestTexture = new RenderTexture (mInputTexture.width, mInputTexture.height, 0, RenderTextureFormat.ARGB32);
				Graphics.Blit (mInputTexture, mMaskTestTexture, mMaskTestMaterial);
			}
		}
*/
	}

	void OnGUI()
	{
		if (mJointGenerator != null) {
			DrawTexture (0, 0, mJointGenerator.mMaskTexture);
			DrawTexture (1, 0, mJointGenerator.mRayTexture);
			DrawTexture (1, 1, mJointGenerator.mSecondJointTexture);
		//	DrawTexture (0, 1, mMaskTestTexture);
		
			for (int i=0; mJointGenerator.mJoints!=null && i<mJointGenerator.mJoints.Count; i++) {
				DrawJoint (mJointGenerator.mJoints [i], 0, 1);
			}
		
			GUI.Label (GetScreenRect (0, 1), "Joints: " + mJointGenerator.mJoints.Count);
		}
	}
}

