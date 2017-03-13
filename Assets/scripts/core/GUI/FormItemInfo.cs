using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FormItemInfo : MonoBehaviour 
{
  public RectTransform Window;
  public RectTransform TextBackground;
  public RectTransform ParchmentBackground;

  public Text HeadText;
  public Text DescriptionText;

  Vector2 _sizeDelta = Vector2.zero;
  Vector2 _windowPosition = Vector2.zero;
  Vector2 _headPosition = Vector2.zero;

  public void SetWindowTexts(string head, string desc)
  {     
    HeadText.text = head;
    DescriptionText.text = desc;

    Window.gameObject.SetActive(true);
  }

  public void HideWindow()
  {
    Window.gameObject.SetActive(false);
  }

  void Update()
  {
    AdjustTextBackground();

    _windowPosition = Input.mousePosition;
    _windowPosition.x -= (Window.sizeDelta.x / 2);

    Window.position = _windowPosition;
  }

  float _headOffsetY = 20;
  float _extraHeightForHead = 30;

  void AdjustTextBackground()
  {
    _headPosition.x = -(TextBackground.sizeDelta.x / 2) - HeadText.rectTransform.sizeDelta.x / 2;
    _headPosition.y = _headOffsetY;

    _sizeDelta.x = TextBackground.sizeDelta.x;
    _sizeDelta.y = TextBackground.sizeDelta.y + _extraHeightForHead;

    Vector2 parchmentPosition = ParchmentBackground.anchoredPosition;
    parchmentPosition.y = _extraHeightForHead;

    ParchmentBackground.sizeDelta = _sizeDelta;
    ParchmentBackground.anchoredPosition = parchmentPosition;

    HeadText.rectTransform.anchoredPosition = _headPosition;
  }
}
