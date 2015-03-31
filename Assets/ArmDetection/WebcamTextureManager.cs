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

			Application.RequestUserAuthorization(UserAuthorization.WebCam);

			string RealDeviceName = DeviceName;
#if UNITY_ANDROID
			RealDeviceName = "";
#endif
#if UNITY_IOS
			RealDeviceName = "";
#endif
			if ( RealDeviceName.Length > 0 )
			{
				mTextureOutput = new WebCamTexture (RealDeviceName);
			}
			else
			{
				string debug = "using default webcam device. Options: ";
				foreach( WebCamDevice w in WebCamTexture.devices )
					debug += "\n" + w.name;
				Debug.Log(debug);
				mTextureOutput = new WebCamTexture ();
			}

			if ( mTextureOutput != null )
				mTextureOutput.Play ();
		}
	}

	void OnDisable()
	{
		if (mTextureOutput != null) {
			mTextureOutput.Stop ();
			mTextureOutput = null;
		}
	}
}

