using UnityEngine;
using System.Collections;

public class FeatureTracker : MonoBehaviour {

	public Material			mMakeFeaturesShader;
	public RenderTexture	mFeaturesLast;			//	feature per pixel
	public RenderTexture	mFeatures;
	public Material			mTrackFeaturesShader;
	public RenderTexture	mTrackedFeatures;	//	per pixel feature's best match (offset)
	public RenderTexture	mInput;
	private Texture2D		mTrackedFeaturesDataTexture;
	private Color32[]		mTrackedFeaturesData;

	// Use this for initialization
	void Start () {
		mFeaturesLast = null;
		mTrackedFeaturesDataTexture = null;
		mTrackedFeaturesData = null;
	}

	public void OnDisable()
	{
		Start();
	}

	// Update is called once per frame
	void Update () {
	
		//	generate features
		Graphics.Blit (mInput, mFeatures, mMakeFeaturesShader);

		//	find the feature's best match
		if (mFeaturesLast) {
			mTrackFeaturesShader.SetTexture("_mainTex_Prev", mFeaturesLast );
			Graphics.Blit( mFeatures, mTrackedFeatures, mTrackFeaturesShader );

			//	extract data for debug rnedering
			RenderTexture.active = mTrackedFeatures;
			mTrackedFeaturesDataTexture.ReadPixels(new Rect (0, 0, mTrackedFeatures.width, mTrackedFeatures.height), 0, 0);
			mTrackedFeaturesDataTexture.Apply (true);
			mTrackedFeaturesData = mTrackedFeaturesDataTexture.GetPixels32();
			RenderTexture.active = null;
		}

		//	copy current features to last features
		Graphics.Blit (mFeatures, mFeaturesLast);
	}

	void OnGUI()
	{
		//	render mTrackedFeaturesData as delta lines
	}
}
