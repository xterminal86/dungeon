using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(SceneLoader))]
public class SceneLoaderInspector : Editor 
{
  public override void OnInspectorGUI()
  {
    SceneLoader sceneLoader = target as SceneLoader;

    if (sceneLoader == null) return;

    sceneLoader.SkipTitleScreen = EditorGUILayout.Toggle("Skip Title Screen", sceneLoader.SkipTitleScreen);

    if (sceneLoader.SkipTitleScreen)
    {
      sceneLoader.SceneToLoad = (ScenesList)EditorGUILayout.EnumPopup("Scene to Load", sceneLoader.SceneToLoad);
      sceneLoader.CharacterClass = (PlayerCharacter.CharacterClass)EditorGUILayout.EnumPopup("Character Class to Start With", sceneLoader.CharacterClass);
      sceneLoader.IsFemale = EditorGUILayout.Toggle("Female", sceneLoader.IsFemale);
    }

    if (GUI.changed)
    {
      EditorUtility.SetDirty(sceneLoader);
      EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
  }
}
