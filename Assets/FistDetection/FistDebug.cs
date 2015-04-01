using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class FistDebug : MonoBehaviour {

	public Texture	mInputTexture;
	public RenderTexture	mRayTexture;
	public Material	mRayMaterial;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (mInputTexture == null)
			return;
		if (mRayTexture == null)
			return;
		if (mRayMaterial == null)
			return;

		Graphics.Blit (mInputTexture, mRayTexture, mRayMaterial);
	}

	void DrawTexture(int ScreenSectionX,int ScreenSectionY,Texture texture)
	{
		if (texture == null)
			return;
		
		float Sectionsx = Screen.width / 2;
		float Sectionsy = Screen.height / 2;
		Rect rect = new Rect( Sectionsx*ScreenSectionX, Sectionsy*ScreenSectionY, Sectionsx, Sectionsy );
		
		GUI.DrawTexture (rect, texture);
	}
	

	void OnGUI()
	{
		DrawTexture (0, 0, mInputTexture);
		DrawTexture (1, 0, mRayTexture);
	}
}
