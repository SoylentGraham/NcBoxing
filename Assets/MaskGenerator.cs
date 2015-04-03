using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class MaskGenerator : MonoBehaviour {

	public Texture			mInputTexture;
	public RenderTexture	mMaskTexture;
	public BackgroundLearner	mBackgroundLearner;
	public Material				mSubtractMaterial;
	private RenderTextureFormat	mTempTextureFormat = RenderTextureFormat.ARGB32;
	private FilterMode			mTempTextureFilterMode = FilterMode.Point;
	public Material				mSubtractFillMaterial;

	void Update () {

		//	take input and subtract from background learner to generate mask
		if (!mInputTexture || !mBackgroundLearner || !mMaskTexture)
			return;

		Texture BackgroundTexture = mBackgroundLearner.mBackgroundTexture;
		if (!BackgroundTexture || !mSubtractMaterial )
			return;

		RenderTexture SubtractTempTexture = RenderTexture.GetTemporary (mInputTexture.width, mInputTexture.height, 0, mTempTextureFormat);
		mSubtractMaterial.SetTexture ("LastBackgroundTex",BackgroundTexture);
		Graphics.Blit (mInputTexture, SubtractTempTexture, mSubtractMaterial);

		//	do a fill to remove noise etc
		if (mSubtractFillMaterial != null) {
			RenderTexture FillTempTexture = RenderTexture.GetTemporary (mInputTexture.width, mInputTexture.height, 0, mTempTextureFormat);
			FillTempTexture.filterMode = mTempTextureFilterMode;
			FillTempTexture.DiscardContents();
			Graphics.Blit (SubtractTempTexture, FillTempTexture, mSubtractFillMaterial);
			SubtractTempTexture.DiscardContents();
			mMaskTexture.DiscardContents();
			Graphics.Blit (FillTempTexture, mMaskTexture);
			RenderTexture.ReleaseTemporary (FillTempTexture);
		} else {
			Graphics.Blit( SubtractTempTexture, mMaskTexture );
		}

		SubtractTempTexture.DiscardContents ();
		RenderTexture.ReleaseTemporary (SubtractTempTexture);
	}

	/*
	
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
*/

}
