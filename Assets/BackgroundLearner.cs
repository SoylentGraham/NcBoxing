using UnityEngine;
using System.Collections;

public class BackgroundLearner : MonoBehaviour {

	public Texture					mInputTexture;
	public RenderTexture			mLumTexture;
	public Material					mLumMaterial;
	public RenderTexture			mBackgroundTexture;
	private RenderTexture			mLastBackgroundTexture;
	public Material					mBackgroundLearnerMat;
	public RenderTextureFormat		mRenderTextureFormat = RenderTextureFormat.ARGBFloat;
	public FilterMode				mRenderTextureFilterMode = FilterMode.Point;
	private bool					mInitBackgroundTexture = true;

	// Use this for initialization
	void Start () {
		mInitBackgroundTexture = true;
		mLastBackgroundTexture = null;
	}
	
	public void OnDisable()
	{
		mInitBackgroundTexture = true;
		mLastBackgroundTexture = null;
	}
	
	// Update is called once per frame
	void Update () {

		if (mInputTexture == null)
			return;

		//	update lum texture
		if (mLumMaterial == null || mLumTexture == null)
			return;
		mLumTexture.DiscardContents ();
		Graphics.Blit (mInputTexture, mLumTexture, mLumMaterial);

		if (mBackgroundLearnerMat == null)
			return;

		if (!mBackgroundTexture)
			return;

		//	first run
		if (mInitBackgroundTexture) {
			//mBackgroundTexture = new RenderTexture (mLumTexture.width, mLumTexture.height, 0, mRenderTextureFormat );
			mBackgroundTexture.filterMode = mRenderTextureFilterMode;
			mBackgroundLearnerMat.SetInt("Init",1);
			mBackgroundTexture.DiscardContents ();
			Graphics.Blit (mLumTexture, mBackgroundTexture, mBackgroundLearnerMat);
			mBackgroundLearnerMat.SetInt("Init",0);
			mInitBackgroundTexture = false;
		}

		if (mLastBackgroundTexture == null) {
			mLastBackgroundTexture = new RenderTexture (mBackgroundTexture.width, mBackgroundTexture.height, 0, mRenderTextureFormat );
			mLastBackgroundTexture.filterMode = mBackgroundTexture.filterMode;
		}
		mLastBackgroundTexture.DiscardContents ();
		Graphics.Blit (mBackgroundTexture, mLastBackgroundTexture);
		mBackgroundLearnerMat.SetInt("Init",0);
		mBackgroundLearnerMat.SetTexture ("LastBackgroundTex", mLastBackgroundTexture);
		mBackgroundTexture.DiscardContents ();
		Graphics.Blit (mLumTexture, mBackgroundTexture, mBackgroundLearnerMat);
	}
}
