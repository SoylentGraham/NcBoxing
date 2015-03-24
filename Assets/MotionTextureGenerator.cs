using UnityEngine;
using System.Collections;
using System.Collections.Generic;


class TextureProcessor
{
	bool	Init(int Width,int Height)
	{
		return true;
	}

	public bool	RenderTextureWithShader(Texture InputTexture,RenderTexture OutputTexture,Shader CopyShader)
	{
		if (CopyShader == null)
			return false;

		return RenderTextureWithShader (InputTexture, OutputTexture, new Material (CopyShader));
	}

	public bool	RenderTextureWithShader(Texture InputTexture,RenderTexture OutputTexture,Material CopyMaterial)
	{
		if (!CopyMaterial)
			return false;

		if (!Init (OutputTexture.width, OutputTexture.height))
			return false;
		
		/*
		RenderTexture.active = mTempRenderTexture;
		mCamera.SetTargetBuffers (mTempRenderTexture.colorBuffer, null);
		mCamera.orthographic = true;
*/
		Graphics.Blit ( InputTexture, OutputTexture, CopyMaterial );
		
		//	read pixels from active render texture
		//		OutputTexture.ReadPixels (new Rect (0, 0, InputTexture.width, InputTexture.height), 0, 0, false) );
		
		return true;
	}
	

};

public class MotionTextureGenerator : MonoBehaviour {

	public WebcamTextureManager mWebcamTextureManager;
	private TextureProcessor mTextureProcessor = new TextureProcessor ();
	public RenderTexture mLumTexture;
	public RenderTexture mLumTextureLast;
	public RenderTexture mMotionTexture;
	public int mLumBlockWidth = 16;
	public int mLumBlockHeight = 16;


	public Shader mVideoToLumShader;
	public Material mVideoToLumMaterial;
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

		if ( !mLumTexture )
		{
			int Width = (int)(VideoTexture.width / mLumBlockWidth);
			int Height = (int)(VideoTexture.height / mLumBlockHeight);
		
			mLumTexture = new RenderTexture( Width, Height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Default );
		}

		if (!mVideoToLumMaterial) {
			mVideoToLumMaterial = new Material (mVideoToLumShader);
			//mVideoToLumMaterial.SetTexture( "MainTex", VideoTexture );
			mVideoToLumMaterial.SetInt ("MainTexWidth", VideoTexture.width);
			mVideoToLumMaterial.SetInt ("MainTexHeight", VideoTexture.height);
			mVideoToLumMaterial.SetInt ("BlockWidth", mLumBlockWidth );
			mVideoToLumMaterial.SetInt ("BlockHeight", mLumBlockHeight);
		}

		mTextureProcessor.RenderTextureWithShader (VideoTexture, mLumTexture, mVideoToLumMaterial);

		return true;
	}

	bool ExecuteShaderLumToMotion(Texture LumTextureNew,Texture LumTexturePrev)
	{
		if (!LumTextureNew)
			return false;

		if (!mMotionTexture) {
			int Width = (int)(LumTextureNew.width / 1);
			int Height = (int)(LumTextureNew.height / 1);
			
			mMotionTexture = new RenderTexture( Width, Height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Default );
		}

		mTextureProcessor.RenderTextureWithShader (LumTextureNew, mMotionTexture, mMotionInitShader );
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

		//	make current lum texture the old one
		if (mLumTexture) {
			//	gr: test swapping
			//Texture2D PoolTexture = mLumTextureLast;
			RenderTexture PoolTexture = null;
			mLumTextureLast = mLumTexture;
			mLumTexture = PoolTexture;
		}

		if (!ExecuteShaderVideoToLum (VideoTexture))
			return;

		if ( !ExecuteShaderLumToMotion( mLumTexture, mLumTextureLast ) )
			return;

	}
}
