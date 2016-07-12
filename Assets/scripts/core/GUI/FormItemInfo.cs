using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FormItemInfo : MonoBehaviour 
{
  public RectTransform Window;
  public CanvasScaler Scaler;

  public Text HeadText;
  public Text DescriptionText;

  public int MaxLettersInRow;
  public int RowHeight;

  int _windowMinWidth;
  int _windowMinHeight;

  Vector2 _sizeDelta = Vector2.zero;
  Vector2 _position = Vector2.zero;

  float _scaleFactor = 1.0f;
  float _diff = 1.0f;
  void Awake()
  {
    _windowMinWidth = (int)Window.sizeDelta.x;
    _windowMinHeight = (int)Window.sizeDelta.y;

    _scaleFactor = Scaler.scaleFactor;

    float w = (float)Screen.width;
    _diff = w / Scaler.referenceResolution.x;

    //Debug.Log(Window.sizeDelta + " " + _scaleFactor + " " + _diff);
  }

  public void SetWindowTexts(string head, string desc)
  {
    HeadText.text = head;
    DescriptionText.text = desc;

    int newLines = desc.Split('\n').Length - 1;
    int rows = desc.Length / MaxLettersInRow + newLines;

    _sizeDelta.x = _windowMinWidth;
    _sizeDelta.y = _windowMinHeight + rows * RowHeight;

    Window.sizeDelta = _sizeDelta;
            
    Window.gameObject.SetActive(true);
  }

  public void HideWindow()
  {
    Window.gameObject.SetActive(false);
  }

  void Update()
  {
    _position = Input.mousePosition;
    _position.x -= (Window.sizeDelta.x / 2) * _diff + 10;

    Window.position = _position;
  }
}
