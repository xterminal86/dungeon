using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EquipmentSlot : MonoBehaviour 
{
  public Image ItemImage;

  public SlotType Type;

  ItemObject _itemRef;
  public ItemObject ItemRef
  {
    get { return _itemRef; }
  }

  public void OnMouseDown()
  {
    if (Input.GetMouseButtonDown(0))
    {
    }
  }
}

public enum SlotType
{
  WEAPON,
  ACCESSORY,
  ARMOR_HEAD,
  ARMOR_CHEST,
  ARMOR_PANTS,
  ARMOR_BOOTS,
  CLOAK,
  NECKLACE
}
