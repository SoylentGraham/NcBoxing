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

		Graphics.Blit (LumTextureNew, mMotionTexture, new Material(mMotionInitShader) );

		/*
		if (!LumTexturePrev) {
			//	run init motion texture shader
			mTextureProcessor.RenderTextureWithShader (LumTextureNew, mMotionTexture, mMotionInitShader );
		}
		else{
			//	run normal motion generator
			mTextureProcessor.RenderTextureWithShader (LumTextureNew, mMotionTexture, mLumToMotionShader );
		}
		*/
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

	}
}
