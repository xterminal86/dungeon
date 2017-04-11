using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallWorldObject : WorldObject 
{
  public bool LeftColumnVisible = true;
  public bool RightColumnVisible = true;

  public WallWorldObject(string inGameName, string prefabName) : base(inGameName, prefabName)
  {    
  }
}
