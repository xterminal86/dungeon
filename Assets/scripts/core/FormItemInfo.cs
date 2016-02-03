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
  public int RowHeight;

  int _windowMinWidth;
  int _windowMinHeight;

  Vector2 _sizeDelta = Vector2.zero;
  Vector2 _position = Vector2.zero;
  void Awake()
  {
    _windowMinWidth = (int)Window.rectTransform.sizeDelta.x;
    _windowMinHeight = (int)Window.rectTransform.sizeDelta.y;
  }

  public void SetWindowTexts(string head, string desc)
  {
    HeadText.text = head;
    DescriptionText.text = desc;

    int newLines = desc.Split('\n').Length - 1;
    int rows = desc.Length / MaxLettersInRow + newLines;

    _sizeDelta.x = _windowMinWidth;
    _sizeDelta.y = _windowMinHeight + rows * RowHeight;

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
    //_position.y += Window.rectTransform.sizeDelta.y / 2 + 10;
    
    Window.rectTransform.position = _position;
  }
}
