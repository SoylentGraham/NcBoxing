using UnityEngine;
using System.Collections;

public class BackgroundLearner : MonoBehaviour {

	public MotionTextureGenerator	mMotionTextureGenerator;
	public RenderTexture			mBackgroundTexture;
	public Material					mBackgroundLearnerMat;
	public RenderTextureFormat		mRenderTextureFormat = RenderTextureFormat.ARGBFloat;
	// Use this for initialization
	void Start () {
	
	}

	void OnDisable()
	{
		mBackgroundTexture = null;
	}
	
	// Update is called once per frame
	void Update () {

		//	need a lum texture to update
		Texture LumTexture = (mMotionTextureGenerator != null) ? mMotionTextureGenerator.mLumTexture : null;
		if (LumTexture == null)
			return;
		if (mBackgroundLearnerMat == null)
			return;

		//	first run
		if (mBackgroundTexture == null) {
			mBackgroundTexture = new RenderTexture (LumTexture.width, LumTexture.height, 0, mRenderTextureFormat );
			mBackgroundLearnerMat.SetInt("Init",1);
			Graphics.Blit (LumTexture, mBackgroundTexture, mBackgroundLearnerMat);
			mBackgroundLearnerMat.SetInt("Init",0);
		}

		mBackgroundLearnerMat.SetTexture ("LastBackgroundTex", mBackgroundTexture);
		Graphics.Blit (LumTexture, mBackgroundTexture, mBackgroundLearnerMat);
	}
}
