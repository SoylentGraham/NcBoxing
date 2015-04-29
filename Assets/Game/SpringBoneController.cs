using UnityEngine;
using System.Collections;

public class SpringBoneController : MonoBehaviour {


	[Range(0.5f,10.0f)]
	public float SpringSpeedScalar = 1.0f;
	[Range(0.1f,2.0f)]
	public float ForceDeltaScalar = 1.0f;
	[Range(0,1)]
	public float LerpScalar = 1.0f;

	// Update is called once per frame
	void Update () {
		//	check for collisions
	}

	void Start () {
		
		//Get all the rigid bodies that belong to the ragdoll
		Rigidbody[] rigidBodies=GetComponentsInChildren<Rigidbody>();
		
		//Add the RagdollPartScript to all the gameobjects that also have the a rigid body
		foreach (Rigidbody body in rigidBodies)
		{
			SpringBonePart part=body.gameObject.AddComponent<SpringBonePart>();
			part.Controller=this;
		}
	}
}
