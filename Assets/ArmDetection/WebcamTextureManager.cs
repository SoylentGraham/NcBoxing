using UnityEngine;
using System.Collections;

public class WebcamTextureManager : MonoBehaviour {

	private WebCamTexture	mWebcamTexture;
	public string DeviceName = "";
	public RenderTexture	mOutputTexture;
	public Material			mFlipMaterial;
	public bool				mFlip = false;
	public bool				mMirror = true;

	// Use this for initialization
	void Start () {
		#if UNITY_IOS && !UNITY_EDITOR
		mFlip =  true;
		#endif
	

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
			if ( mFlipMaterial )
			{
				mFlipMaterial.SetInt("Flip", mFlip?1:0 );
				mFlipMaterial.SetInt("Mirror",mMirror?1:0 );
				Graphics.Blit (mWebcamTexture, mOutputTexture, mFlipMaterial );
			}
			else
			{
				Graphics.Blit (mWebcamTexture, mOutputTexture );
			}
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

