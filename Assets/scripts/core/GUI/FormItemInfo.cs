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

  // You should experimentally find these out:
  // MaxLettersInRow are simply number of characters that fits in line before text starts to wrap.
  // RowHeight is the height in pixels of one line of text (see comments in SetWindowTexts())
  //
  // Probably works the best on monospaced fonts only.

  public int MaxLettersInRow;
  public int RowHeight;

  Vector2 _sizeDelta = Vector2.zero;
  Vector2 _position = Vector2.zero;

  float _scaleFactor = 1.0f;
  float _diff = 1.0f;
  void Awake()
  {
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
      } 
      else
      {        
        rows = (splitString[i].Length / MaxLettersInRow);

        // If our line of text is equal to MaxLettersInRow, we shouldn't add new line
        // since such text fits.
        if (splitString[i].Length != MaxLettersInRow)
        {
          rows++;
        }

        newLines += rows;
      }
    }

    //Debug.Log("new lines: " + newLines + " | rows: " + rows);

    // Even if font is monospaced, its size in Text component is not equal
    // to its height. You should experimentally find out height in pixels of the specific font.
    // In case of Old_Terminal it's 15 and only works on Constant Pixel Size canvas scaler
    // with scale factor of 1

    _sizeDelta.x = Window.sizeDelta.x;

    // We should consider the offset from the top of the window where item name is located.
    // Right now it's -45, so we add 45 to the window height
    // and 15 more for one blank line of height.
    _sizeDelta.y = newLines * RowHeight + 60;

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
    _position.x -= (Window.sizeDelta.x / 2);

    Window.position = _position;
  }
}
