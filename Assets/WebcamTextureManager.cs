using UnityEngine;
using System.Collections;

public class WebcamTextureManager : MonoBehaviour {

	public WebCamTexture mTextureOutput = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (!mTextureOutput) {
			mTextureOutput = new WebCamTexture ();
			mTextureOutput.Play ();
		}
	}

	void OnDisable()
	{
		mTextureOutput.Stop ();
		mTextureOutput = null;
	}
}

