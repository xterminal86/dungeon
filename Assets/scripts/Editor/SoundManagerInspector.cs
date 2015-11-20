using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(SoundManager))]
public class SoundManagerInspector : Editor
{
  string _musicList = string.Empty;
  string _soundsList = string.Empty;

  public override void OnInspectorGUI()
  {
    SoundManager sm = target as SoundManager;

    if (sm == null) return;

    sm.SoundVolume = EditorGUILayout.Slider("Sound Volume", sm.SoundVolume, 0.0f, 1.0f);
    sm.MusicVolume = EditorGUILayout.Slider("Music Volume", sm.MusicVolume, 0.0f, 1.0f);

    string musicPath = "Assets/sounds/music";
    string soundsPath = "Assets/sounds/sfx";

    if (GUILayout.Button("Generate Music List"))
    {
      sm.MusicTracks.Clear();

      string[] dirs = Directory.GetDirectories(musicPath, "*", SearchOption.AllDirectories);
      if (dirs.Length == 0)
      {
        LoadFiles(sm.MusicTracks, musicPath, "*.ogg");
      }
      else
      {
        for (int i = 0; i < dirs.Length; i++)
        {
          LoadFiles(sm.MusicTracks, dirs[i], "*.ogg");
        }
      }
    }

    if (sm.MusicTracks.Count != 0)
    {
      _musicList = string.Empty;
      
      int counter = 0;
      foreach (var item in sm.MusicTracks)
      {
        if (item != null)
        {
          _musicList += string.Format("{0}: {1}\n", counter, item.name);
          counter++;
        }
      }
      
      EditorGUILayout.HelpBox(_musicList, MessageType.None);
    }

    if (GUILayout.Button("Generate Sounds List"))
    {
      sm.SoundEffects.Clear();
      
      string[] dirs = Directory.GetDirectories(soundsPath, "*", SearchOption.AllDirectories);
      if (dirs.Length == 0)
      {
        LoadFiles(sm.SoundEffects, soundsPath, "*.wav");
      }
      else
      {
        for (int i = 0; i < dirs.Length; i++)
        {
          LoadFiles(sm.SoundEffects, dirs[i], "*.wav");
        }
      } 
    }

    if (sm.SoundEffects.Count != 0)
    {
      _soundsList = string.Empty;
      
      int counter = 0;
      foreach (var item in sm.SoundEffects)
      {
        if (item != null)
        {
          _soundsList += string.Format("{0}: {1}\n", counter, item.name);
          counter++;
        }
      }
      
      EditorGUILayout.HelpBox(_soundsList, MessageType.None);
    }
  }

  void LoadFiles(List<AudioClip> listToAdd, string path, string filter)
  {
    string[] files = Directory.GetFiles(path, filter);
    for (int j = 0; j < files.Length; j++)
    {
      AudioClip clip = AssetDatabase.LoadAssetAtPath(files[j], typeof(AudioClip)) as AudioClip;
      listToAdd.Add(clip);
    }
  }
}
