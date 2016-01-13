using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FormInventory : MonoBehaviour 
{
  int _hungerBarMaxWidth = 0;

  public Image HungerBarBorder;
  public Image HungerBar;

  Vector2 _rectTransformSize = Vector2.zero;
  void Awake()
  {
    _hungerBarMaxWidth = (int)HungerBarBorder.rectTransform.rect.width - 6;
    _rectTransformSize.Set((int)HungerBar.rectTransform.rect.width, (int)HungerBar.rectTransform.rect.height);
  }

	void Start () 
	{	
	}
	

	void Update () 
	{
	
	}
}
