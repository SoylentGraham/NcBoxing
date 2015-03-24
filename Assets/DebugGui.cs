using UnityEngine;
using System.Collections;

public class DebugGui : MonoBehaviour {

	public MotionTextureGenerator mMotionTextureGenerator;
	public WebcamTextureManager mWebcamTextureManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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


	void OnGUI()
	{
		if ( mWebcamTextureManager != null )
			DrawTexture( 0, 0, mWebcamTextureManager.mTextureOutput );
 
		if ( mMotionTextureGenerator != null )
			DrawTexture( 1, 0, mMotionTextureGenerator.mLumTexture );
		
		if ( mMotionTextureGenerator != null )
			DrawTexture( 0, 1, mMotionTextureGenerator.mMotionTexture );
		
	}
}
