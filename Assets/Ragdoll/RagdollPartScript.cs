using UnityEngine;
using System.Collections;

public class RagdollPartScript : MonoBehaviour {

	public RagdollHelper mainScript;
	private float RagdollTimer = 0;

	public bool IsRagdoll()
	{
		return RagdollTimer > 0;
	}

	void Update()
	{
		if ( RagdollTimer > 0 )
		{
			RagdollTimer -= Time.deltaTime;
			if ( RagdollTimer <= 0 )
			{
				mainScript.OnRagdollChange(this);
			}
		}

		ApplyRagdoll ();
	}

	void ApplyRagdoll()
	{
		bool Enable = IsRagdoll ();
		Rigidbody rigid = GetComponent<Rigidbody> ();

		//	set ragdoll properties
		//	gr: sometimes want to lock pos/rotation of some objects
		bool EnableKinematic = !Enable;
		if ( !EnableKinematic && rigid.isKinematic )
		{
			Debug.Log ("Made " + name + " kinematic");
			rigid.isKinematic = EnableKinematic;
			rigid.velocity = Vector3.zero;
			rigid.angularVelocity = Vector3.zero;
		}
		else if ( EnableKinematic && !rigid.isKinematic )
		{
			Debug.Log ("Made " + name + "non kinematic");
			rigid.isKinematic = EnableKinematic;
			rigid.velocity = Vector3.zero;
			rigid.angularVelocity = Vector3.zero;
			//rigid.freezeRotation = true;
		}

	}

	void Start () {
		//string UpperOrLower = IsUpperBody () ? "upper" : "lower";
		//Debug.Log (gameObject.name + " is " + UpperOrLower);
	}

	void OnCollisionEnter(Collision collision)
	{
		//	self intersection
		if (transform.root == collision.transform.root)
			return;

		//	did we get hit by a player glove?
		InputGlove PlayerGlove = collision.transform.GetComponent<InputGlove> ();
		if (PlayerGlove != null) {
			RagdollTimer = mainScript.RagdollPartTime;
			mainScript.OnRagdollChange(this);
		}
		/*
		var Helper = mainScript.GetComponent<RagdollHelper> ();
		if (Helper.ragdolled) {
		//	Debug.Log ("Collision during ragdolling");
			return;
		}

			PlayerGlove.OnHitRigidBody(gameObject,collision,mainScript);
		}
		*/
	}
}
