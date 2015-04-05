using UnityEngine;
using System.Collections;

public class WebcamTextureManager : MonoBehaviour {

	private WebCamTexture	mWebcamTexture;
	public string DeviceName = "";
	public RenderTexture	mOutputTexture;
	public Shader			mFlipShader;

	// Use this for initialization
	void Start () {
	

	}
	
	// Update is called once per frame
	void Update () {
	
		if (!mWebcamTexture) {

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
				mWebcamTexture = new WebCamTexture (RealDeviceName);
			}
			else
			{
				string debug = "using default webcam device. Options: ";
				foreach( WebCamDevice w in WebCamTexture.devices )
					debug += "\n" + w.name;
				Debug.Log(debug);
				mWebcamTexture = new WebCamTexture ();
			}

			if ( mWebcamTexture != null )
				mWebcamTexture.Play ();
		}

		if (mOutputTexture && mWebcamTexture) {
			mOutputTexture.DiscardContents();

			//	ios camera seems to be upside down...
			bool Flip = false;
#if UNITY_IOS && !UNITY_EDITOR
			Flip = (mFlipShader != null);
#endif
			if ( Flip )
				Graphics.Blit (mWebcamTexture, mOutputTexture, new Material(mFlipShader) );
			else
				Graphics.Blit (mWebcamTexture, mOutputTexture );
		}
	}

	void OnDisable()
	{
		if (mWebcamTexture != null) {
			mWebcamTexture.Stop ();
			mWebcamTexture = null;
		}
	}
}

