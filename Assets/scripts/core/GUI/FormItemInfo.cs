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

    int newLines = 0;
    int rows = 0;

    string[] splitString = desc.Split('\n');
    for (int i = 0; i < splitString.Length; i++)
    {
      if (splitString[i].Length == 0)
      {
        newLines++;
      } else
      {        
        rows = (splitString[i].Length / MaxLettersInRow) + 1;
        newLines += rows;
      }

      //Debug.Log(splitString[i].Length);
    }

    /*
    int newLines = 0;

    int charactersCount = 0;
    for (int i = 0; i < desc.Length; i++)
    {
      if (desc[i] != '\n')
      {
        charactersCount++;
      }
      else
      {
        charactersCount = 0;
      }

      if (charactersCount == MaxLettersInRow)
      {
        newLines++;
        charactersCount = 0;
      }
    }
    */

    //int rows = desc.Length / MaxLettersInRow + newLines;

    Debug.Log("new lines: " + newLines + " | rows: " + rows);

    _sizeDelta.x = _windowMinWidth;
    _sizeDelta.y = _windowMinHeight + newLines * RowHeight + 48 + 36;
    //_sizeDelta.y = _windowMinHeight + 2 * RowHeight;

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
    _position.x -= (Window.sizeDelta.x / 2) * _diff;

    Window.position = _position;
  }
}
