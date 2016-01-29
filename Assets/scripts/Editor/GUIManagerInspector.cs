using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

[CustomEditor(typeof(GUIManager))]
public class GUIManagerInspector : Editor
{
  string _path = "Assets\\Resources\\portraits";
  string _portraitsList = string.Empty;

  public override void OnInspectorGUI()
  {
    GUIManager m = target as GUIManager;

    if (m == null) return;

    DrawDefaultInspector();

    if (GUILayout.Button("Generate Portraits List"))
    {
      m.Portraits.Clear();

      string[] array = Directory.GetFiles(_path, "*.png");

      for (int i = 0; i < array.Length; i++)
      {
        int indexFront = array[i].LastIndexOf("\\") + 1;
        int indexBack = array[i].IndexOf(".png");

        string name = array[i].Substring(indexFront, indexBack - indexFront);
        Sprite res = Resources.Load<Sprite>("portraits/" + name);
        m.Portraits.Add(res);
      }
    }

    if (m.Portraits.Count != 0)
    {
      _portraitsList = string.Empty;

      int count = 0;
      foreach (var item in m.Portraits)
      {
        _portraitsList += string.Format("{0}: {1}\n", count, item.name);

        count++;
      }

      EditorGUILayout.HelpBox(_portraitsList, MessageType.None);
    }        
  }
}
