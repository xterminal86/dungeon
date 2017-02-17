using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallMapObject : MapObject 
{
  public WallMapObject(string name, string prefabName)
  {
    ClassName = name;
    PrefabName = prefabName;
  }

  public override void ActionCompleteHandler(object sender)
  {
  }

  public override void ActionHandler(object sender)
  {
    Debug.Log("You see: " + ClassName + " " + PrefabName);
  }
}
