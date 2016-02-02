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

  public void OnMouseDown()
  {
    if (Input.GetMouseButtonDown(0))
    {
      if (_itemRef != null)
      {
        if (_itemRef.ActionCallback != null)
          _itemRef.ActionCallback(this);

        Icon.gameObject.SetActive(false);
        _itemRef = null;
      }
      else if (GUIManager.Instance.ItemTaken != null)
      {
        SoundManager.Instance.PlaySound(GlobalConstants.SFXItemPut);
    
        _itemRef = GUIManager.Instance.ItemTaken;
        Icon.sprite = GUIManager.Instance.GetIconFromAtlas(_itemRef.AtlasIcon);
        Icon.gameObject.SetActive(true);
        GUIManager.Instance.ItemTakenSprite.gameObject.SetActive(false);
        GUIManager.Instance.ItemTaken = null;
      }
    }
    else if (Input.GetMouseButtonDown(1))
    {
      Debug.Log("Right button");
    }
  }
}
