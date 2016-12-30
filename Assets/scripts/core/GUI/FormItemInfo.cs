using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FormItemInfo : MonoBehaviour 
{
  public RectTransform Window;

  public Text HeadText;
  public Text DescriptionText;

  // This is resize according to the head window width to properly position anchored description text
  public RectTransform DescriptionTextHolderWindow;

  // Border and background holder
  public RectTransform TextWindow;

  // Minimal width of description window (hard-coded)
  // If head text is too long, width will dynamically change accordingly.
  int _windowMinWidth = 400;

  // Should be found out manually for every font size value
  float _headLetterWidth = 12.0f;
  float _headLetterHeight = 28.4f;

  // Inner indent of text from both sides of window border to compensate for it
  int _headTextPadding = 20;
  int _descriptionTextPadding = 20;

  // Should be found out manually for every font size value
  float _descriptionLetterWidth = 7.45f;
  float _descriptionLetterHeight = 16.4f;

  public void SetWindowTexts(string headText, string descriptionText)
  {
    HeadText.text = headText;
    DescriptionText.text = descriptionText;

    // Calculate pixel width of head GUI text element
    float headTextwidth = (float)headText.Length * _headLetterWidth;

    Vector2 size = new Vector2(headTextwidth, _headLetterHeight);
    HeadText.rectTransform.sizeDelta = size;

    float windowWidth = _windowMinWidth + _headTextPadding;

    // If head text is longer than minimum window width, calculate new window width
    if (headTextwidth > _windowMinWidth)
    {     
      windowWidth = headTextwidth + _headTextPadding;
    }

    size.x = windowWidth;

    // It's important to set size of this parent transform
    // for child description text to be positioned correctly.
    // DescriptionTextHolderWindow is anchored to top, while the text itself is anchored to upper left corner.
    DescriptionTextHolderWindow.sizeDelta = size;

    // Calculate GUI text element pixel width (compensate for horizontal padding for text itself and head text)
    float descriptionTextWidth = windowWidth - _headTextPadding - _descriptionTextPadding;

    //Debug.Log("Description Text GUI width : " + descriptionTextWidth);

    // Compensate for head height and descritption text initial offset
    float windowHeight = 50.0f;

    int totalLines = 0;

    string[] lines = descriptionText.Split('\n');
    for (int i = 0; i < lines.Length; i++)
    {
      //Debug.Log("Split line " + i + " length : " + lines[i].Length);

      float lineWidth = (float)lines[i].Length * _descriptionLetterWidth;

      //Debug.Log("Line width : " + lineWidth);

      int linesNumber = (int)(lineWidth / descriptionTextWidth);

      if (linesNumber != 0)
      {
        // We cannot do a "+ 1" here because there might be a situation, when text is
        // exactly equal to the line width before wrapping occurs. In such case linesNumber will be 1,
        // and additional + 1 will fuck newline counter up.
        totalLines += linesNumber;

        // If line will be wrapped to one and a half, linesNumber will return 1, so we will lose
        // actual end of line. To compensate this the following is written.
        // Second condition compensates newline for only one element of string.Split().
        if (linesNumber == 1 || lines.Length == 1 || (linesNumber > 1 && i == lines.Length - 1 && lines.Length != 1))
        {
          totalLines++;
        }
      }
      else
      {
        totalLines++;
      }
    }

    //Debug.Log("Total lines : " + totalLines);

    // Add some additional padding at the end
    windowHeight += _descriptionLetterHeight * totalLines + 5.0f;

    size.y = windowHeight;

    TextWindow.sizeDelta = size;

    size.x = descriptionTextWidth;
    size.y = totalLines * _descriptionLetterHeight;

    DescriptionText.rectTransform.sizeDelta = size;

    Window.gameObject.SetActive(true);
    Cursor.visible = false;
  }

  public void HideWindow()
  {
    Window.gameObject.SetActive(false);
    Cursor.visible = true;
  }

  Vector2 _position = Vector2.zero;
  void Update()
  {
    _position = Input.mousePosition;
    _position.x -= (TextWindow.sizeDelta.x / 4);

    Window.position = _position;
  }
}
