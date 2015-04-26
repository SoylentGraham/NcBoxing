using UnityEngine;
using System.Collections;

public class InputGlove : MonoBehaviour {

	public Rigidbody	body;

	// Use this for initialization
	void Start () {
		body.freezeRotation = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*
	public void OnHitRigidBody(GameObject BodyPart,Collision collision,StairDismount Character)
	{
		Debug.Log ("Hit " + BodyPart.name + " with player glove " + gameObject.name );
		Character.DoHit (BodyPart.GetComponent<Rigidbody>(), -collision.relativeVelocity);

		//	turn off collision for a bit to stop it spinning around
		body.isKinematic = true;
		body.velocity = Vector3.zero;
		body.angularVelocity = Vector3.zero;
		body.freezeRotation = true;

		body.isKinematic = false;
	}
	*/
}
