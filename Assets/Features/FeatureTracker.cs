using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FeatureTracker : MonoBehaviour {

	public Material			mMakeFeaturesShader;
	public int				mTrackFrameLag = 5;			//	may need this? but to test algo we need the tracking over X frames to get significant movement
	private List<RenderTexture>	mFeaturesPrev;			//	feature per pixel
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
		mFeaturesPrev = new List<RenderTexture> ();
		mTrackedFeaturesDataTexture = null;
		mTrackedFeaturesData = null;
		Resources.UnloadUnusedAssets ();
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

		//	pop oldest prev features
		RenderTexture FeaturesPrev = null;
		if (mFeaturesPrev.Count >= mTrackFrameLag) {
			FeaturesPrev = mFeaturesPrev [0];
			mFeaturesPrev.RemoveAt (0);
		}

			//	find the feature's best match
		if (FeaturesPrev && mTrackFeaturesShader && mTrackedFeatures ) 
		{
			if (! mTrackedFeaturesDataTexture )
				mTrackedFeaturesDataTexture = new Texture2D( mTrackedFeatures.width, mTrackedFeatures.height, mTrackedFeaturesDataTextureFormat, false );
			mTrackFeaturesShader.SetTexture("FeaturesPrev", FeaturesPrev );
			Graphics.Blit( mFeatures, mTrackedFeatures, mTrackFeaturesShader );

			//	extract data for debug rendering
			RenderTexture.active = mTrackedFeatures;
			mTrackedFeaturesDataTexture.ReadPixels(new Rect (0, 0, mTrackedFeatures.width, mTrackedFeatures.height), 0, 0);
			mTrackedFeaturesDataTexture.Apply (true);
			mTrackedFeaturesData = mTrackedFeaturesDataTexture.GetPixels32();
			RenderTexture.active = null;
		}

		//	push features onto prev list
		if (mFeaturesPrev.Count < mTrackFrameLag) {
			//	re-use texture
			if (!FeaturesPrev)
				FeaturesPrev = new RenderTexture (mFeatures.width, mFeatures.height, mFeatures.depth, mFeatures.format);
			Graphics.Blit (mFeatures, FeaturesPrev);
			mFeaturesPrev.Add (FeaturesPrev);
		}
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
		if (mTrackedFeaturesData!=null) {
			for (int i=0; i<mTrackedFeaturesData.Length; i++)
				MatchedFeatures += (mTrackedFeaturesData [i].a > 0) ? 1 : 0;
		}
		GUI.Label( a, "matched features: " + MatchedFeatures );
	}
}
