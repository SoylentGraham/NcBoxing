using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class MotionTextureGenerator : MonoBehaviour {

	public WebcamTextureManager mWebcamTextureManager;
	public RenderTexture mLumTexture;
	public RenderTexture mLumTextureLast;
	public RenderTexture mMotionTexture;


	public Shader mVideoToLumShader;
	public Shader mLumToMotionShader;
	public Shader mMotionInitShader;

	// Use this for initialization
	void Start () {
	
	}

	void OnDisable()
	{
		mLumTexture = null;
		mLumTextureLast = null;
		mMotionTexture = null;
	}

	bool ExecuteShaderVideoToLum(Texture VideoTexture)
	{
		if (!VideoTexture)
			return false;

		if (!mVideoToLumShader)
			return false;

		if (!mLumTexture)
			return false;

		Graphics.Blit (VideoTexture, mLumTexture, new Material (mVideoToLumShader) );

		return true;
	}

	bool ExecuteShaderLumToMotion(Texture LumTextureNew,Texture LumTexturePrev)
	{
		if (!LumTextureNew)
			return false;

		if (!mMotionTexture)
			return false;

		if (!LumTexturePrev) {
			//	run init motion texture shader
			Graphics.Blit (LumTextureNew, mMotionTexture, new Material(mMotionInitShader) );
		}
		else{
			Material MotionCalcMat = new Material( mLumToMotionShader );
			MotionCalcMat.SetTexture("LumLastTex", mLumTextureLast );

			//	run normal motion generator
			Graphics.Blit (LumTextureNew, mMotionTexture, MotionCalcMat );
		}

		return true;
	}

	// Update is called once per frame
	void Update () {

		if (mWebcamTextureManager == null)
			return;

		Texture VideoTexture = mWebcamTextureManager.mTextureOutput;

		if (!ExecuteShaderVideoToLum (VideoTexture))
			return;
	
		if ( !ExecuteShaderLumToMotion( mLumTexture, mLumTextureLast ) )
			return;

		if (!mLumTextureLast) {
			mLumTextureLast = new RenderTexture (mLumTexture.width, mLumTexture.height, mLumTexture.depth, mLumTexture.format, RenderTextureReadWrite.Default);
		}
		Graphics.Blit (mLumTexture, mLumTextureLast);
	}
}
