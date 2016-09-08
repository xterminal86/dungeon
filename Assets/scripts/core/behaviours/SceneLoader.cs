using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour 
{
  AsyncOperation _level;
	void Start() 
	{ 
    GameData.Instance.Initialize();
    GUIManager.Instance.Initialize();
    JobManager.Instance.Initialize();
    PrefabsManager.Instance.Initialize();
    ThreadWatcher.Instance.Initialize();
    ScreenFader.Instance.Initialize();
    SoundManager.Instance.Initialize();

    // FIXME: remove after test
    //DateAndTime.Instance.Initialize();

    _level = SceneManager.LoadSceneAsync("title");

    StartCoroutine(LoadLevelRoutine());
	}

  IEnumerator LoadLevelRoutine()
  {
    while (!_level.isDone)
    {
      yield return null;
    }

    yield return null;
  }
}
