using UnityEngine;
using System.Collections;

public class InputGlove : MonoBehaviour {

	public PlayerManager	Player;
	public Transform		PowPrefab;
	public Rigidbody		body;

	// Use this for initialization
	void Start () {
		body.freezeRotation = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnCollisionEnter(Collision collision)
	{
		var SpringBone = collision.gameObject.GetComponent<SpringBonePart> ();
		if ( SpringBone && !SpringBone.isSpringing() )
		{
			//	make sure it goes in the right direction
			Vector3 Delta = collision.relativeVelocity.normalized;
			Vector3 PlayerForward = (Player.transform.position - transform.position).normalized;
			if ( Vector3.Dot(Delta,PlayerForward) > 0 )
				Delta *= -1;

			ContactPoint contact = collision.contacts[0];
			//Vector3 Reflection = Vector3.Reflect( transform.forward, -contact.normal );
			Vector3 Reflection = Vector3.Cross(-transform.forward, contact.normal );

			Vector3 PushPos = contact.point + ((Reflection + (Vector3.up*4.0f))*4.0f);
			if ( PowPrefab )
				Instantiate( PowPrefab, PushPos, Quaternion.identity);

			SpringBone.PushBone( PushPos );
			Debug.Log("Fist collided with spring bone " + collision.gameObject.name);
		}
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
