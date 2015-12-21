using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(App))]
public class AppInspector : Editor
{
  string _path = "Assets/prefabs/characters";
  string _prefabsList = string.Empty;

  public override void OnInspectorGUI()
  {
    App app = target as App;

    if (app == null) return;

    DrawDefaultInspector();

    if (GUILayout.Button("Generate Character Prefabs List"))
    {
      app.Characters.Clear();

      string[] array = Directory.GetFiles(_path, "*.prefab");

      for (int i = 0; i < array.Length; i++)
      {
        GameObject o = AssetDatabase.LoadAssetAtPath(array[i], typeof(GameObject)) as GameObject;
        app.Characters.Add(o);
      }
    }

    if (app.Characters.Count != 0)
    {
      _prefabsList = string.Empty;
      
      int counter = 0;
      foreach (var item in app.Characters)
      {
        if (item != null)
        {
          _prefabsList += string.Format("{0}: {1}\n", counter, item.name);
          counter++;
        }
      }
      
      EditorGUILayout.HelpBox(_prefabsList, MessageType.None);
    }

    if (GUI.changed)
    {
      EditorUtility.SetDirty(target);
    }
  }
}
