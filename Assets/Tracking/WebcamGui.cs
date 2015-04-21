using UnityEngine;
using System.Collections;

public class WebcamGui : MonoBehaviour {

	public Texture		mInputTexture;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnGUI () {
	
		Camera camera = GetComponent<Camera> ();
		if (camera && mInputTexture) {
			Rect rect = camera.pixelRect;
			GUI.DrawTexture (rect, mInputTexture);
		}
	}
}
