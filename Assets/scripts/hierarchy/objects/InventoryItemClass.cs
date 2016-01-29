using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryItemClass
{
  public InventoryItemClass()
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