using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	
public class LevelLoader : StateMachine
{
	public List<string>				globalSceneList = new List<string>();
	public List<string>				processSceneList = new List<string>();



	enum LoadLevelProgressStates
	{
		Idling = 0,
		LoadExtraScenes,
		ProcessExtraScenes,
		Done
	}
	
	private void RegisterStates()
	{
		AddState((int)LoadLevelProgressStates.Idling,				null,						null,							null	);
		AddState((int)LoadLevelProgressStates.LoadExtraScenes,		LoadExtraScenes_Begin,		null,							null	);
		AddState((int)LoadLevelProgressStates.ProcessExtraScenes,	ProcessExtraScenes_Begin,	ProcessExtraScenes_Update,		null	);
		AddState((int)LoadLevelProgressStates.Done,					null,						null,							null	);
	}
	
	protected override void Start()
	{
		BootSequence.extraScenesLoaded = false;

		//make a copy of the scene list so we can remove each as they are processed
		foreach (string sceneName in this.globalSceneList)
		{
			GameObject sceneObj = GameObject.Find("__Scene_" + sceneName);
		
			if (sceneObj==null)
			{				
				//Debug.Log("LevelLoader:::::Adding Scene '" + sceneName + "' to load list");
				processSceneList.Add(sceneName);
			}
			else
			{
				//Debug.Log("LevelLoader:::::Scene '" + sceneName + "' Already there!!!!");
			}
			
		}
		
		RegisterStates();
		SetState((int)LoadLevelProgressStates.ProcessExtraScenes);
	}
	
	public void LoadExtraScenes_Begin(int PrevMode)
	{
	
		
		
	}

	public void ProcessExtraScenes_Begin(int PrevMode)
	{
		foreach (string sceneName in this.processSceneList)
		{
			Application.LoadLevelAdditive(sceneName);
		}
	}

	public void ProcessExtraScenes_Update()
	{
		//wait for each scene to be loaded
		for (int s = this.processSceneList.Count-1; s>=0; s--)
		{
			GameObject sceneObj = GameObject.Find("__Scene_" + this.processSceneList[s]);
		
			if (sceneObj!=null)
			{
				DontDestroyOnLoad(sceneObj);
				this.processSceneList.RemoveAt(s);
			}
			else
			{
				//Debug.Log("LevelLoader:::::WARNING : Unable to process extra scene '" + this.processSceneList[s] + "'");
			}
		}
		
		if (this.processSceneList.Count==0)
		{
			//Debug.Log("LevelLoader:::::ALL SCENES PROCESSED!!!!!!");
			SetState((int)LoadLevelProgressStates.Done);

			BootSequence.extraScenesLoaded = true;
		}
		
	}

}