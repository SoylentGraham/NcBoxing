using UnityEngine;
using System.Collections;

public class PopCommon : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Resources.UnloadUnusedAssets ();
	}

	static public bool CheckCurrentCamera()
	{
		if (!Camera.current)
			Camera.SetupCurrent (Camera.main);
		return Camera.current != null;
	}
}
