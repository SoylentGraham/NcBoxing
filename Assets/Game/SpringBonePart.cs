using UnityEngine;
using System.Collections;

public class SpringBonePart : MonoBehaviour {

	public SpringBoneController	Controller;
	private float SpringTimer = 0;	//	lerp amount, count down to 0. 2..1=going out, 1..0 going in. 0 in.
	private Vector3 SpringPos;		//	position we've been sprung to

	// Use this for initialization
	void Start () {
	
		//PushBone (new Vector3 (0, 0, 2));
	}

	public bool isSpringing()
	{
		//	debug
	//	if (name != "Head")
	//		return false;

		return SpringTimer > 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (isSpringing ()) {
			SpringTimer -= Time.deltaTime * Controller.SpringSpeedScalar;
		}
	}

	//	post-anim apply physics
	void LateUpdate()
	{
		if ( isSpringing() )
		{
			Vector3 AnimPos = transform.position;

			//	lerp time
			float LerpTime = SpringTimer > 1 ? 1-(SpringTimer-1) : SpringTimer;

			Quaternion SpringRot = Quaternion.LookRotation( SpringPos - AnimPos );
			transform.rotation = Quaternion.Slerp( transform.rotation, SpringRot, LerpTime*Controller.LerpScalar );
			//transform.position = Vector3.Lerp( AnimPos, SpringPos, LerpTime );
			//Debug.Log("pos(" + LerpTime + ") = " + transform.position );
		}
	}

	public void PushBone(Vector3 PushTarget)
	{
		if (isSpringing ())
			return;

		//	calc the spring pos and start timer
		SpringTimer = 2;
		//SpringPos = transform.position + (PushDelta * Controller.ForceDeltaScalar);
		SpringPos = PushTarget;
	}
}
