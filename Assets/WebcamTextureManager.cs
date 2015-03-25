using UnityEngine;
using System.Collections;

public class WebcamTextureManager : MonoBehaviour {

	public WebCamTexture mTextureOutput = null;
	public string DeviceName = "";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (!mTextureOutput) {

			if ( DeviceName.Length > 0 )
			{
				mTextureOutput = new WebCamTexture (DeviceName);
			}
			else
			{
				string debug = "using default webcam device. Options: ";
				foreach( WebCamDevice w in WebCamTexture.devices )
					debug += "\n" + w.name;
				Debug.Log(debug);
				mTextureOutput = new WebCamTexture ();
			}

			mTextureOutput.Play ();
		}
	}

	void OnDisable()
	{
		mTextureOutput.Stop ();
		mTextureOutput = null;
	}
}

