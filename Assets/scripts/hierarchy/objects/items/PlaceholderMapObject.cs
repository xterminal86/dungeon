using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceholderMapObject : MapObject
{
  public PlaceholderMapObject()
  {    
  }
}

public enum InventoryItemType
{
  PLACEHOLDER = 0,
  CLOAK,
  HEAD,
  CHEST,
  PANTS,
  BOOTS,
  ACCESSORY,
  CONSUMABLE,
  WEAPON
}