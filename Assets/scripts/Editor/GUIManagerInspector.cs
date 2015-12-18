using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

[CustomEditor(typeof(GUIManager))]
public class GUIManagerInspector : Editor
{
  string _path = "Assets\\Resources";
  string _portraitsList = string.Empty;

  public override void OnInspectorGUI()
  {
    GUIManager m = target as GUIManager;

    if (m == null) return;
        
    m.ButtonClickSound = (AudioSource)EditorGUILayout.ObjectField("Button Click Sound", m.ButtonClickSound, typeof(AudioSource));
    m.CharacterSpeakSound = (AudioSource)EditorGUILayout.ObjectField("Character Speak Sound", m.CharacterSpeakSound, typeof(AudioSource));
    
    m.FormTalking = (GameObject)EditorGUILayout.ObjectField("Form-Talking", m.FormTalking, typeof(GameObject));
    m.FormCompass = (GameObject)EditorGUILayout.ObjectField("Form-Compass", m.FormCompass, typeof(GameObject));
    m.FormGameMenu = (GameObject)EditorGUILayout.ObjectField("Form-Game Menu", m.FormGameMenu, typeof(GameObject));

    m.CompassImage = (Image)EditorGUILayout.ObjectField("Form-Talking Compass Image", m.CompassImage, typeof(Image));
    m.PortraitImage = (Image)EditorGUILayout.ObjectField("Form-Talking Portrait Image", m.PortraitImage, typeof(Image));

    m.FormTalkingName = (Text)EditorGUILayout.ObjectField("Form-Talking Name", m.FormTalkingName, typeof(Text));
    m.FormTalkingText = (Text)EditorGUILayout.ObjectField("Form-Talking Text", m.FormTalkingText, typeof(Text));    

    if (GUILayout.Button("Generate Portraits List"))
    {
      m.Portraits.Clear();

      string[] array = Directory.GetFiles(_path, "*.png");

      for (int i = 0; i < array.Length; i++)
      {
        int indexFront = array[i].LastIndexOf("\\") + 1;
        int indexBack = array[i].IndexOf(".png");

        string name = array[i].Substring(indexFront, indexBack - indexFront);                
        Sprite res = Resources.Load<Sprite>(name);
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
