using UnityEngine;
using System.Collections;


public class ProductionGui : MonoBehaviour {

	public JointGenerator		mJointGenerator;
	public MaskGenerator		mMaskGenerator;
	public BackgroundLearner	mBackgroundLearner;
	public Texture				mCameraTexture;
	private float				mResetCountdownInterval = 5.0f;
	private float				mResetCountdown = 0.0f;
	private int					mRenderJointIndex = 0;


	void Update()
	{
		bool Reset = Input.GetMouseButtonDown (0);

		//	every so often, reset for testing
		mResetCountdown -= Time.deltaTime;
		if (mResetCountdown < 0.0f) {
			Reset = true;
			mResetCountdown = mResetCountdownInterval;
		}

		//	reset
		if (Reset) {
			if (mBackgroundLearner != null)
				mBackgroundLearner.OnDisable ();
		}
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

		if ( BackgroundTexture )
			GUI.DrawTexture ( ScreenRect, BackgroundTexture);

		//	render joints
		if (mJointGenerator != null && mJointGenerator.mJoints!=null)
		{
			mRenderJointIndex = (mRenderJointIndex+1) % Mathf.Max(mJointGenerator.mJoints.Count,1);

			for (int i=0; i<mJointGenerator.mJoints.Count; i++) {
				if ( mRenderJointIndex == i )
					JointDebug.DrawJoint (mJointGenerator.mJoints [i], ScreenRect );
			}
		}
	
	}
}
