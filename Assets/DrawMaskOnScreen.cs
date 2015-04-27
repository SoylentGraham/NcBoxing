using UnityEngine;
using System.Collections;

public class DrawMaskOnScreen : MonoBehaviour {

	public RenderTexture	mBackgroundLearnerTexture;
	public RenderTexture	mInputLum;
	private RenderTexture	mSubtractTexture;
	public Material			mSubtractMaterial;
	public Material			mSubtractFillMaterial;
	public Rect				mDrawRect = new Rect(0,0,1,1);
	public RenderTextureFormat	mTempTextureFormat = RenderTextureFormat.ARGBFloat;
	public FilterMode			mTempTextureFilterMode = FilterMode.Point;

	[Range(0,1)]
	public float			BadTruthLumDiff = 0.7f;
	[Range(0,1)]
	public float			GoodTruthLumDiff = 0.1f;
	[Range(0,1)]
	public float			TruthMin = 0.01f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		UpdateSubtractTexture (mInputLum, mBackgroundLearnerTexture, ref mSubtractTexture);
	
	
		mSubtractMaterial.SetFloat ("BadTruthLumDiff", BadTruthLumDiff);
		mSubtractMaterial.SetFloat ("GoodTruthLumDiff", GoodTruthLumDiff);
		mSubtractMaterial.SetFloat ("TruthMin", TruthMin);

	}

	void UpdateSubtractTexture(Texture LiveTexture,Texture BackgroundTexture,ref RenderTexture TempTexture)
	{
		if (LiveTexture == null)
			return;
		if (BackgroundTexture == null)
			return;
		if (mSubtractMaterial == null)
			return;
		if (TempTexture == null) {
			TempTexture = new RenderTexture (LiveTexture.width, LiveTexture.height, 0, mTempTextureFormat);
			TempTexture.filterMode = mTempTextureFilterMode;
		}
		
		mSubtractMaterial.SetTexture ("LastBackgroundTex",BackgroundTexture);
		Graphics.Blit (LiveTexture, TempTexture, mSubtractMaterial);
		
		//	do a fill
		if (mSubtractFillMaterial != null) {
			RenderTexture FillTempTexture = RenderTexture.GetTemporary(LiveTexture.width, LiveTexture.height, 0, mTempTextureFormat);
			FillTempTexture.filterMode = mTempTextureFilterMode;
			Graphics.Blit( TempTexture, FillTempTexture, mSubtractFillMaterial );
			Graphics.Blit( FillTempTexture, TempTexture );
			RenderTexture.ReleaseTemporary( FillTempTexture);
		}
		
	}

	void OnGUI()
	{
		float w=Screen.width,h=Screen.height;

		if ( mSubtractTexture != null )
			GUI.DrawTexture (new Rect (w * mDrawRect.x, h * mDrawRect.y, w * mDrawRect.width, h * mDrawRect.height), mSubtractTexture);
	}


}
