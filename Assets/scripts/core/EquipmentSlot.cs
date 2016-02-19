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

  public void OnMouseEnter()
  {
    if (_itemRef != null && GUIManager.Instance.ItemTaken == null)
    {
      GUIManager.Instance.ItemInfoForm.SetWindowTexts(_itemRef.ItemNameText, _itemRef.DescriptionText);            
    }
  }

  public void OnMouseExit()
  {
    GUIManager.Instance.ItemInfoForm.HideWindow();
  }

  public void OnMouseUp()
  {
    if (_itemRef != null && GUIManager.Instance.ItemTaken == null)
    {
      GUIManager.Instance.ItemInfoForm.SetWindowTexts(_itemRef.ItemNameText, _itemRef.DescriptionText);            
    }
  }

  public void OnMouseDown()
  {
    if (Input.GetMouseButtonDown(0))
    {  
      ProcessItem();
    }
  }

  public void ProcessItem()
  {
    // Something is in slot and we don't have anything "in hand"
    if (_itemRef != null && GUIManager.Instance.ItemTaken == null)
    { 
      SoundManager.Instance.PlaySound(GlobalConstants.SFXItemTake);

      if (_itemRef.LMBAction != null)
        _itemRef.LMBAction(this);

      GUIManager.Instance.ItemInfoForm.HideWindow();

      ItemImage.gameObject.SetActive(false);
      _itemRef = null;
    }
    // Put in slot
    else if (GUIManager.Instance.ItemTaken != null && CanBeEquipped())
    {
      SoundManager.Instance.PlaySound(GlobalConstants.SFXItemPut);

      if (_itemRef == null)
      {
        _itemRef = GUIManager.Instance.ItemTaken;
        GUIManager.Instance.ItemTakenSprite.gameObject.SetActive(false);
        GUIManager.Instance.ItemTaken = null;
      }
      else
      {
        ItemObject tmp = GUIManager.Instance.ItemTaken;
        GUIManager.Instance.ItemTaken = _itemRef;
        _itemRef = tmp;
        GUIManager.Instance.ItemTakenSprite.sprite = GUIManager.Instance.GetIconFromAtlas(GUIManager.Instance.ItemTaken.AtlasIcon);          
      }

      ItemImage.sprite = GUIManager.Instance.GetIconFromAtlas(_itemRef.AtlasIcon);          
      ItemImage.gameObject.SetActive(true);
    }
  }

  bool CanBeEquipped()
  {
    // If object to be equipped is of the same type as the slot or
    // if object is any weapon and this slot is a weapon slot
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
