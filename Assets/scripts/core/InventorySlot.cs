using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour 
{	
  public Image Icon;

  PlaceholderMapObject _item;

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
      /*
      if (GUIManager.Instance.DraggedObject.IsActive())
      {
        GUIManager.Instance.DraggedObject.gameObject.SetActive(false);
      }
      else
      {
        Sprite s = GUIManager.Instance.GetItemIcon("atlas_2");
        GUIManager.Instance.DraggedObject.sprite = s;
        GUIManager.Instance.DraggedObject.gameObject.SetActive(true);
      }
      */
    }
    else if (Input.GetMouseButtonDown(1))
    {
      Debug.Log("Right button");
    }
  }
}
