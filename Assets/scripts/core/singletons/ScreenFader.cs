using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScreenFader : MonoSingleton<ScreenFader> 
{
  public Image FaderImage;
  public Text LoadingText;
  public Image WhiteFlasher;

  Color _faderColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
  Color _textColor = Color.white;

  protected override void Init()
  {
    _alpha = 1.0f;
    _faderColor.a = 1.0f;
    FaderImage.gameObject.SetActive(true);
    FaderImage.color = _faderColor;
    LoadingText.color = _textColor;
  }

  public void FlashScreen()
  {
    StopCoroutine(FlashScreenRoutine());
    StartCoroutine(FlashScreenRoutine());
  }

  IEnumerator FlashScreenRoutine()
  { 
    Color c = Color.white;

    WhiteFlasher.color = c;

    WhiteFlasher.gameObject.SetActive(true);

    float alpha = 1.0f;
    while (alpha > 0.0f)
    {      
      alpha -= Time.smoothDeltaTime * 3.0f;

      c.a = alpha;

      WhiteFlasher.color = c;

      yield return null;
    }

    WhiteFlasher.gameObject.SetActive(false);

    yield return null;
  }

  public void FadeOut(Callback cb = null)
  {
    FaderImage.gameObject.SetActive(true);
    StartCoroutine(FadeOutRoutine(cb));
  }

  public void FadeIn(Callback cb = null)
  {
    FaderImage.gameObject.SetActive(true);
    StartCoroutine(FadeInRoutine(cb));
  }

  float _alpha = 0.0f;
  IEnumerator FadeOutRoutine(Callback cb)
  {
    _alpha = 0.0f;

    while (_alpha < 1.0f)
    {
      _alpha += Time.smoothDeltaTime * GlobalConstants.FadeSpeed;

      _faderColor.a = _alpha;
      _textColor.a = _alpha;

      FaderImage.color = _faderColor;
      LoadingText.color = _textColor;

      yield return null;
    }

    if (cb != null)
      cb();
  }

  IEnumerator FadeInRoutine(Callback cb)
  {
    _alpha = 1.0f;

    while (_alpha > 0.0f)
    {
      _alpha -= Time.smoothDeltaTime * GlobalConstants.FadeSpeed;

      _faderColor.a = _alpha;
      _textColor.a = _alpha;

      FaderImage.color = _faderColor;
      LoadingText.color = _textColor;

      yield return null;
    }

    FaderImage.gameObject.SetActive(false);

    if (cb != null)
      cb();
  }
}
