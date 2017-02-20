using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallWorldObject : WorldObject 
{
  public WallWorldObject(string name, string prefabName)
  {
  }

  public override void ActionCompleteHandler(object sender)
  {
  }

  public override void ActionHandler(object sender)
  {
    Debug.Log("You see: " + GetType());
  }
}
