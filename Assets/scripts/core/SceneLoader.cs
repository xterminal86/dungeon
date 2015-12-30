using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour 
{
  AsyncOperation _level;
	void Start() 
	{
    _level = Application.LoadLevelAsync("title");

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
