using UnityEngine;
using System.Collections;


public class ProductionGui : MonoBehaviour {

	public JointGenerator		mJointGenerator;
	public MaskGenerator		mMaskGenerator;
	public BackgroundLearner	mBackgroundLearner;


	// Use this for initialization
	void Start () {
	
	}

	void Update()
	{
		//	reset
		if (Input.GetMouseButtonDown (0)) {
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

		if ( mMaskGenerator != null && mMaskGenerator.mInputTexture != null )
			GUI.DrawTexture ( ScreenRect, mMaskGenerator.mMaskTexture);

		//	render joints
		if (mJointGenerator != null) {
			for (int i=0; mJointGenerator.mJoints!=null && i<mJointGenerator.mJoints.Count; i++) {
				JointDebug.DrawJoint (mJointGenerator.mJoints [i], ScreenRect );
			}
		}
	}
}
