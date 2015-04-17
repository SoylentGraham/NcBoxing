using UnityEngine;
using System.Collections;

public class FeatureTracker : MonoBehaviour {

	public Material			mMakeFeaturesShader;
	private RenderTexture	mFeaturesPrev;			//	feature per pixel
	public RenderTexture	mFeatures;
	public Material			mTrackFeaturesShader;
	public RenderTexture	mTrackedFeatures;	//	per pixel feature's best match (offset)
	public RenderTexture	mInput;
	public RenderTexture	mInputBlurred;
	private Texture2D		mTrackedFeaturesDataTexture;
	private TextureFormat	mTrackedFeaturesDataTextureFormat = TextureFormat.ARGB32;
	private Color32[]		mTrackedFeaturesData;

	// Use this for initialization
	void Start () {
		mFeaturesPrev = null;
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
		if (!mMakeFeaturesShader || !mInput || !mFeatures)
			return;

		if (mInputBlurred) {
			Graphics.Blit (mInput, mInputBlurred);
			Graphics.Blit (mInputBlurred, mFeatures, mMakeFeaturesShader);
		} else {
			Graphics.Blit (mInput, mFeatures, mMakeFeaturesShader);
		}

		//	find the feature's best match
		if (mFeaturesPrev && mTrackFeaturesShader && mTrackedFeatures ) 
		{
			if (! mTrackedFeaturesDataTexture )
				mTrackedFeaturesDataTexture = new Texture2D( mTrackedFeatures.width, mTrackedFeatures.height, mTrackedFeaturesDataTextureFormat, false );
			mTrackFeaturesShader.SetTexture("FeaturesPrev", mFeaturesPrev );
			Graphics.Blit( mFeatures, mTrackedFeatures, mTrackFeaturesShader );

			//	extract data for debug rendering
			RenderTexture.active = mTrackedFeatures;
			mTrackedFeaturesDataTexture.ReadPixels(new Rect (0, 0, mTrackedFeatures.width, mTrackedFeatures.height), 0, 0);
			mTrackedFeaturesDataTexture.Apply (true);
			mTrackedFeaturesData = mTrackedFeaturesDataTexture.GetPixels32();
			RenderTexture.active = null;
		}

		//	copy current features to last features
		if ( !mFeaturesPrev )
			mFeaturesPrev = new RenderTexture( mFeatures.width, mFeatures.height, mFeatures.depth, mFeatures.format );
		Graphics.Blit (mFeatures, mFeaturesPrev);
	}

	void OnGUI()
	{
		Rect a = new Rect(0,0,Screen.width/2,Screen.height);
		GUI.DrawTexture (a, mFeatures);
	
		Rect b = new Rect(Screen.width/2,0,Screen.width/2,Screen.height);
		GUI.DrawTexture (b, mInputBlurred? mInputBlurred : mInput);
		GUI.DrawTexture (b, mTrackedFeatures);

		//	count matches
		int MatchedFeatures = 0;
		for (int i=0; i<mTrackedFeaturesData.Length; i++)
			MatchedFeatures += (mTrackedFeaturesData [i].a > 0) ? 1 : 0;
		GUI.Label( a, "matched features: " + MatchedFeatures );
	}
}
