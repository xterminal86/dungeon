using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallWorldObject : WorldObject 
{
  public WallWorldObject(string inGameName, string prefabName) : base(inGameName, prefabName)
  {    
  }

  public override void ActionCompleteHandler(object sender)
  {
  }

  public override void ActionHandler(object sender)
  {
    Debug.Log("You see: " + InGameName);
  }
}
