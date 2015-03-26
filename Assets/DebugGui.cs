using UnityEngine;
using System.Collections;

public class DebugGui : MonoBehaviour {

	public MotionTextureGenerator mMotionTextureGenerator;
	public WebcamTextureManager mWebcamTextureManager;
	public BackgroundLearner	mBackgroundLearner;
	public Shader BackgroundJustLumShader;
	public Shader BackgroundJustScoreShader;
	private RenderTexture		mJustLumTexture;
	private RenderTexture		mJustScoreTexture;
	private RenderTexture		mSubtractTexture;
	public Shader	SubtractShader;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		//	update intermediate textures
		if ( mBackgroundLearner != null )
			UpdateTempTexture( mBackgroundLearner.mBackgroundTexture, BackgroundJustLumShader, ref mJustLumTexture );
		
		if ( mBackgroundLearner != null )
			UpdateTempTexture( mBackgroundLearner.mBackgroundTexture, BackgroundJustScoreShader, ref mJustScoreTexture );
		
		if ( mBackgroundLearner != null && mWebcamTextureManager != null )
			UpdateSubtractTexture( mWebcamTextureManager.mTextureOutput, mBackgroundLearner.mBackgroundTexture, SubtractShader, ref mSubtractTexture );
	}
	void UpdateSubtractTexture(Texture LiveTexture,Texture BackgroundTexture,Shader shader,ref RenderTexture TempTexture)
	{
		if (shader == null)
			return;
		
		if (TempTexture == null)
			TempTexture = new RenderTexture (LiveTexture.width, LiveTexture.height, 0, RenderTextureFormat.ARGBFloat);

		Material SubtractMat = new Material (shader);
		SubtractMat.SetTexture ("LastBackgroundTex",BackgroundTexture);
		Graphics.Blit (LiveTexture, TempTexture, SubtractMat);
	}

	void UpdateTempTexture(Texture texture,Shader shader,ref RenderTexture TempTexture)
	{
		if (shader == null)
			return;
		
		if (TempTexture == null)
			TempTexture = new RenderTexture (texture.width, texture.height, 0, RenderTextureFormat.ARGBFloat);
		
		Graphics.Blit (texture, TempTexture, new Material (shader));
	}

	void DrawTexture(int ScreenSectionX,int ScreenSectionY,Texture texture)
	{
		if (texture == null)
			return;

		float Sectionsx = Screen.width / 3;
		float Sectionsy = Screen.height / 3;
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

		if (mBackgroundLearner != null)
			DrawTexture (1, 1, mBackgroundLearner.mBackgroundTexture);

		if ( mJustLumTexture != null )
			DrawTexture( 2, 1, mJustLumTexture );

		if ( mJustScoreTexture != null )
			DrawTexture( 2, 2, mJustScoreTexture );

		if (mSubtractTexture != null)
			DrawTexture (0, 2, mSubtractTexture);
	}
}
