using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour 
{
  public bool SkipTitleScreen = false;

  void Start() 
	{ 
    GameData.Instance.Initialize();
    GUIManager.Instance.Initialize();
    JobManager.Instance.Initialize();
    PrefabsManager.Instance.Initialize();
    ThreadWatcher.Instance.Initialize();
    ScreenFader.Instance.Initialize();
    SoundManager.Instance.Initialize();
    DateAndTime.Instance.Initialize();
    LevelLoader.Instance.Initialize();
    InputController.Instance.Initialize();

    LevelLoader.Instance.LoadLevel();

    if (SkipTitleScreen)
    {
      GameData.Instance.PlayerCharacterVariable.ResetToDefault();
      SceneManager.LoadSceneAsync("main");
    }
    else
    {
      SceneManager.LoadSceneAsync("title");
    }
	}
}
