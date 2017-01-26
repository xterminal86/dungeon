using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class for storing and finding all to be instantiated prefabs 
/// </summary>
public class PrefabsManager : MonoSingleton<PrefabsManager>
{
  public List<GameObject> Prefabs = new List<GameObject>();

  protected override void Init()
  {
    base.Init();
  }

  public GameObject FindPrefabByName(string name)
  {
    foreach (var item in Prefabs)
    {
      if (item.name == name)
      {
        return item;
      }
    }

    Debug.LogWarning("Could not find prefab " + name);

    return null;
  }
}
