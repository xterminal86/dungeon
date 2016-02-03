using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script should be placed on all items
/// </summary>
public class BehaviourItemObject : MonoBehaviour 
{
  // Can this object be placed in inventory?
  public bool CanBeTaken = true;
  	
  // Reference to 3D model - we're going to hide it when we take it in inventory
  public GameObject Model;

  // Logic class instance
  public ItemObject ItemObjectInstance;

  Int2 _mapPosition = new Int2();
  public Int2 MapPosition
  {
    get { return _mapPosition; }
  }
  
  public void CalculateMapPosition()
  {
    _mapPosition.X = (int)transform.position.x / GlobalConstants.WallScaleFactor;
    _mapPosition.Y = (int)transform.position.z / GlobalConstants.WallScaleFactor;
  }
}

public enum InventoryItemType
{
  PLACEHOLDER = 0,
  ARMOR,
  ACCESSORY,
  CONSUMABLE,
  WEAPON
}