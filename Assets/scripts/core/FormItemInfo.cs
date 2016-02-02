using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FormItemInfo : MonoBehaviour 
{
  public Image Window;

  public Text HeadText;
  public Text DescriptionText;

  public int MaxLettersInRow;
  public int WindowMinWidth;
  public int WindowMinHeight;

  Vector2 _sizeDelta = Vector2.zero;
  Vector2 _position = Vector2.zero;
  public void SetWindowTexts(string head, string desc)
  {
    HeadText.text = head;
    DescriptionText.text = desc;

    int rows = desc.Length / MaxLettersInRow;
        
    _sizeDelta.x = WindowMinWidth;
    _sizeDelta.y = WindowMinHeight + (rows + 1) * 15;

    Window.rectTransform.sizeDelta = _sizeDelta;
            
    Window.gameObject.SetActive(true);
  }

  public void HideWindow()
  {
    Window.gameObject.SetActive(false);
  }

  void Update()
  {
    _position = Input.mousePosition;
    _position.x -= (Window.rectTransform.sizeDelta.x / 2 + 10);
    _position.y -= Window.rectTransform.sizeDelta.y / 2 + 10;
    
    Window.rectTransform.position = _position;
  }
}
