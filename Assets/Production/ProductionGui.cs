using UnityEngine;
using System.Collections;


public class ProductionGui : MonoBehaviour {

	public JointGenerator		mJointGenerator;
	public MaskGenerator		mMaskGenerator;
	public BackgroundLearner	mBackgroundLearner;
	public Texture				mCameraTexture;
	public bool					mAutoReset = false;
	private float				mResetCountdownInterval = 5.0f;
	private float				mResetCountdown = 0.0f;
	private int					mCycleJointIndex = 0;
	public bool					mCycleJoints = false;
	public bool					mDebugTextures = false;

	[Range(0,1)]
	public float 				mBackgroundAlpha = 0.5f;

	void Update()
	{
		bool Reset = Input.GetMouseButtonDown (0);

		//	every so often, reset for testing
		if (mAutoReset) {
			mResetCountdown -= Time.deltaTime;
			if (mResetCountdown < 0.0f) {
				Reset = true;
				mResetCountdown = mResetCountdownInterval;
			}
		}

		//	reset
		if (Reset) {
			if (mBackgroundLearner != null)
				mBackgroundLearner.OnDisable ();
		}

		//	fix texture leak by forcing unity to unload assets
		Resources.UnloadUnusedAssets();
	}

	// Update is called once per frame
	void OnGUI () {
		Camera camera = this.GetComponent<Camera> ();
		if (!camera)
			return;

		Rect ScreenRect = camera.pixelRect;

		Texture BackgroundTexture = mCameraTexture;
		if (BackgroundTexture == null && mBackgroundLearner != null && mBackgroundLearner.mBackgroundTexture != null)
			BackgroundTexture = mBackgroundLearner.mBackgroundTexture;

		if (BackgroundTexture == null && mJointGenerator != null )
			BackgroundTexture = mJointGenerator.GetCopyTexture();

		if (BackgroundTexture) {
			var OldColour = GUI.color;
			GUI.color = new Color(1,1,1,mBackgroundAlpha);
			GUI.DrawTexture (ScreenRect, BackgroundTexture);
			GUI.color = OldColour;
		}

		//	render joints
		if (mJointGenerator != null && mJointGenerator.mJoints!=null)
		{
			mCycleJointIndex = (mCycleJointIndex+1) % Mathf.Max(mJointGenerator.mJoints.Count,1);

			for (int i=0; i<mJointGenerator.mJoints.Count; i++) {
				if ( mCycleJointIndex == i || !mCycleJoints )
					JointDebug.DrawJoint (mJointGenerator.mJoints [i], ScreenRect );
			}

		}
	
		if (mJointGenerator) {

			string debug = "";
			Texture2D[] AllTextures = GameObject.FindObjectsOfType<Texture2D>();
			int Texture2DCount = AllTextures.Length;
			debug += "Texture2D count: " +  Texture2DCount + "\n";

			if ( mDebugTextures )
			{
				for ( int i=0;	i<Mathf.Min(30,AllTextures.Length);	i++ )
					debug += AllTextures[i].name + " ";
				debug += "\n";
			}

			debug += mJointGenerator.mDebug;
			GUI.Label (ScreenRect, debug);
		}
	}
}
