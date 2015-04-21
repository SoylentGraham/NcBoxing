using UnityEngine;
using System.Collections;

public class Debug_Input : MonoBehaviour {

	public Transform		mTargetGlove;
	private Vector3			mBasePosition;
	private float			mPunchTime = -1;	//	idle when <= 0
	private float			mPunchSpeed = 4.0f;
	[Range(0.0f,1.0f)]
	public float			mPunchDistance = 0.5f;
	public int 				mMouseButton = 0;

	// Use this for initialization
	void Start () {
		mBasePosition = mTargetGlove.localPosition;
	}

	void UpdateTransform()
	{
		//	make delta 0...1
		float Delta = Mathf.Sin (mPunchTime * 180.0f * Mathf.Deg2Rad);
		Vector3 Offset = Vector3.forward * Delta * mPunchDistance;
		mTargetGlove.localPosition = mBasePosition + Offset;
	}

	// Update is called once per frame
	void Update () {

		//	not punching yet
		if ( mPunchTime < 0.0f )
		{
			if ( Input.GetMouseButtonDown(mMouseButton) )
				mPunchTime = 0;
		}

		//	punching
		if (mPunchTime >= 0.0f) {
			mPunchTime += mPunchSpeed * Time.deltaTime;

			//	finished
			if (mPunchTime >= 1.0f)
				mPunchTime = -1.0f;

		}

		UpdateTransform ();
	}
}
