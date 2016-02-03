using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour 
{	
  public Image Icon;

  ItemObject _itemRef;

	void Awake() 
	{	
	}
		
	void Update () 
	{    
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
      // Take item from slot
      if (_itemRef != null && GUIManager.Instance.ItemTaken == null)
      {
        if (_itemRef.LMBAction != null)
          _itemRef.LMBAction(this);

        GUIManager.Instance.ItemInfoForm.HideWindow();

        Icon.gameObject.SetActive(false);
        _itemRef = null;
      }
      // Put item in slot
      else if (GUIManager.Instance.ItemTaken != null)
      {
        SoundManager.Instance.PlaySound(GlobalConstants.SFXItemPut);

        // If slot is empty
        if (_itemRef == null)
        {
          _itemRef = GUIManager.Instance.ItemTaken;
          GUIManager.Instance.ItemTakenSprite.gameObject.SetActive(false);
          GUIManager.Instance.ItemTaken = null;
        }
        // If slot has something, swap it
        else
        {
          ItemObject tmp = GUIManager.Instance.ItemTaken;
          GUIManager.Instance.ItemTaken = _itemRef;
          _itemRef = tmp;
          GUIManager.Instance.ItemTakenSprite.sprite = GUIManager.Instance.GetIconFromAtlas(GUIManager.Instance.ItemTaken.AtlasIcon);          
        }

        Icon.sprite = GUIManager.Instance.GetIconFromAtlas(_itemRef.AtlasIcon);          
        Icon.gameObject.SetActive(true);          
      }
    }
    else if (Input.GetMouseButtonDown(1))
    {
      Debug.Log("Right button");
    }
  }
}
