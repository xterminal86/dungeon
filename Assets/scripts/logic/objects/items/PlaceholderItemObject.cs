using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Item that has no purpose (rock, note etc.)
/// </summary>
public class PlaceholderItemObject : ItemObject
{  
  public PlaceholderItemObject(string name, string desc, int atlasIcon, BehaviourItemObject bio, InputController inputController) 
    : base(name, desc, atlasIcon, bio, inputController)
  {    
  }
}