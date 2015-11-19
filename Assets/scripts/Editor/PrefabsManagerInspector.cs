using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(PrefabsManager))]
public class PrefabsManagerInspector : Editor 
{
  public override void OnInspectorGUI()
  {
    PrefabsManager pm = target as PrefabsManager;

    if (pm == null) return;

    string[] paths = { "Assets/prefabs-release" };

    DrawDefaultInspector();

    if (GUILayout.Button("Generate List"))
    {
      pm.Prefabs.Clear();

      string[] dirs = Directory.GetDirectories("Assets/prefabs-release", "*", SearchOption.AllDirectories);
      for (int i = 0; i < dirs.Length; i++)
      {
        string[] array = Directory.GetFiles(dirs[i], "*.prefab");

        for (int j = 0; j < array.Length; j++)
        {
          GameObject o = AssetDatabase.LoadAssetAtPath(array[j], typeof(GameObject)) as GameObject;
          pm.Prefabs.Add(o);
        }
      }
    }
  }

}
