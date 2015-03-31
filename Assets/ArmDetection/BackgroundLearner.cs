using UnityEngine;
using System.Collections;

public class BackgroundLearner : MonoBehaviour {

	public MotionTextureGenerator	mMotionTextureGenerator;
	public RenderTexture			mBackgroundTexture;
	public RenderTexture			mLastBackgroundTexture;
	public Material					mBackgroundLearnerMat;
	public RenderTextureFormat		mRenderTextureFormat = RenderTextureFormat.ARGBFloat;
	public FilterMode				mRenderTextureFilterMode = FilterMode.Point;

	// Use this for initialization
	void Start () {
	
	}
	
	public void OnDisable()
	{
		if (mBackgroundTexture != null) {
			mBackgroundTexture.DiscardContents ();
			mBackgroundTexture = null;
		}
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
			mBackgroundTexture.filterMode = mRenderTextureFilterMode;
			mBackgroundLearnerMat.SetInt("Init",1);
			Graphics.Blit (LumTexture, mBackgroundTexture, mBackgroundLearnerMat);
			mBackgroundLearnerMat.SetInt("Init",0);
		}

		if (mLastBackgroundTexture == null) {
			mLastBackgroundTexture = new RenderTexture (mBackgroundTexture.width, mBackgroundTexture.height, 0, mRenderTextureFormat );
			mLastBackgroundTexture.filterMode = mBackgroundTexture.filterMode;
		}
		Graphics.Blit (mBackgroundTexture, mLastBackgroundTexture);
		mBackgroundLearnerMat.SetInt("Init",0);
		mBackgroundLearnerMat.SetTexture ("LastBackgroundTex", mLastBackgroundTexture);
		Graphics.Blit (LumTexture, mBackgroundTexture, mBackgroundLearnerMat);
	}
}
