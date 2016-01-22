using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour 
{	
  public Image Icon;

  InventoryItemBase _item;

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
      Debug.Log("Left button");
    }
    else if (Input.GetMouseButtonDown(1))
    {
      Debug.Log("Right button");
    }
  }
}
