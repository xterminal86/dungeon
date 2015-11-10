using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabsManager : MonoSingleton<PrefabsManager>
{
  public List<GameObject> Prefabs;

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

    return null;
  }
}
