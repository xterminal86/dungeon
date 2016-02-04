using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour 
{
  AsyncOperation _level;
	void Start() 
	{    
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
