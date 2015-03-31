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
	public Material				mSubtractMaterial;
	public Material				mSubtractFillMaterial;
	public RenderTextureFormat	mTempTextureFormat = RenderTextureFormat.ARGBFloat;
	public FilterMode			mTempTextureFilterMode = FilterMode.Point;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		//	reset
		if (Input.GetMouseButtonDown (0)) {
			if (mBackgroundLearner != null)
				mBackgroundLearner.OnDisable ();
		}
	
		//	update intermediate textures
		if ( mBackgroundLearner != null )
			UpdateTempTexture( mBackgroundLearner.mBackgroundTexture, BackgroundJustLumShader, ref mJustLumTexture );
		
		if ( mBackgroundLearner != null )
			UpdateTempTexture( mBackgroundLearner.mBackgroundTexture, BackgroundJustScoreShader, ref mJustScoreTexture );
		
		if (mBackgroundLearner != null && mWebcamTextureManager != null) {
			//Texture LiveTexture = mWebcamTextureManager.mTextureOutput;
			Texture LiveTexture = mMotionTextureGenerator.mLumTexture;
			UpdateSubtractTexture (LiveTexture, mBackgroundLearner.mBackgroundTexture, ref mSubtractTexture);
		}
	}

	void UpdateSubtractTexture(Texture LiveTexture,Texture BackgroundTexture,ref RenderTexture TempTexture)
	{
		if (LiveTexture == null)
			return;
		if (BackgroundTexture == null)
			return;
		if (mSubtractMaterial == null)
			return;
		if (TempTexture == null) {
			TempTexture = new RenderTexture (LiveTexture.width, LiveTexture.height, 0, mTempTextureFormat);
			TempTexture.filterMode = mTempTextureFilterMode;
		}

		mSubtractMaterial.SetTexture ("LastBackgroundTex",BackgroundTexture);
		Graphics.Blit (LiveTexture, TempTexture, mSubtractMaterial);

		//	do a fill
		if (mSubtractFillMaterial != null) {
			RenderTexture FillTempTexture = RenderTexture.GetTemporary(LiveTexture.width, LiveTexture.height, 0, mTempTextureFormat);
			FillTempTexture.filterMode = mTempTextureFilterMode;
			Graphics.Blit( TempTexture, FillTempTexture, mSubtractFillMaterial );
			Graphics.Blit( FillTempTexture, TempTexture );
			RenderTexture.ReleaseTemporary( FillTempTexture);
		}

	}

	void UpdateTempTexture(Texture texture,Shader shader,ref RenderTexture TempTexture)
	{
		if (shader == null)
			return;
		
		if (TempTexture == null) {
			TempTexture = new RenderTexture (texture.width, texture.height, 0, mTempTextureFormat);
			TempTexture.filterMode = mTempTextureFilterMode;
		}

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
