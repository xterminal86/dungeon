using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EquipmentSlot : MonoBehaviour 
{
  public Image ItemImage;

  public GlobalConstants.WorldItemType SlotType;

  ItemObject _itemRef;
  public ItemObject ItemRef
  {
    get { return _itemRef; }
  }

  public void OnMouseDown()
  {
    if (Input.GetMouseButtonDown(0))
    {
      Debug.Log(SlotType);

      if (GUIManager.Instance.ItemTaken != null && CanBeEquipped())
      {
        _itemRef = GUIManager.Instance.ItemTaken;
        GUIManager.Instance.ItemTakenSprite.gameObject.SetActive(false);
        GUIManager.Instance.ItemTaken = null;

        ItemImage.sprite = GUIManager.Instance.GetIconFromAtlas(_itemRef.AtlasIcon);          
        ItemImage.gameObject.SetActive(true);
      }
    }
  }

  bool CanBeEquipped()
  {
    // If object to be equipped is the same type as slot or
    // if object is any weapon and this slot is weapon slot
    if (GUIManager.Instance.ItemTaken.ItemType == SlotType ||
      ( (GUIManager.Instance.ItemTaken.ItemType == GlobalConstants.WorldItemType.WEAPON_RANGED 
      || GUIManager.Instance.ItemTaken.ItemType == GlobalConstants.WorldItemType.WEAPON_MELEE) 
        && SlotType == GlobalConstants.WorldItemType.WEAPON_MELEE ) )
    {
      return true;
    }

    return false;
  }
}
