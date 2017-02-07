using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script should be placed on all items
/// </summary>
public class BehaviourItemObject : BehaviourObjectBase 
{
  // Can this object be picked up in the world?
  public bool CanBeTaken = true;
  	
  // Reference to 3D model - we're going to hide it when we take it in inventory
  public GameObject Model;

  // Logic class instance
  public ItemObject ItemObjectInstance;
}
