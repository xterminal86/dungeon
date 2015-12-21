using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScreenFader : MonoSingleton<ScreenFader> 
{
  public Callback FadeCompleteCallback;

  public Image FaderImage;

  Color _faderColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

  protected override void Init()
  {
    _alpha = 1.0f;
    FaderImage.gameObject.SetActive(true);
    FaderImage.color = _faderColor;    
  }

  public void FadeOut()
  {
    FaderImage.gameObject.SetActive(true);
    StartCoroutine(FadeOutRoutine());
  }

  public void FadeIn()
  {
    FaderImage.gameObject.SetActive(true);
    StartCoroutine(FadeInRoutine());
  }

  float _alpha = 0.0f;
  IEnumerator FadeOutRoutine()
  {
    _alpha = 0.0f;

    while (_alpha < 1.0f)
    {
      _alpha += Time.smoothDeltaTime * GlobalConstants.FadeSpeed;

      _faderColor.a = _alpha;

      FaderImage.color = _faderColor;
            
      yield return null;
    }

    if (FadeCompleteCallback != null)
      FadeCompleteCallback();
  }

  IEnumerator FadeInRoutine()
  {
    _alpha = 1.0f;

    while (_alpha > 0.0f)
    {
      _alpha -= Time.smoothDeltaTime * GlobalConstants.FadeSpeed;

      _faderColor.a = _alpha;
      
      FaderImage.color = _faderColor;

      yield return null;
    }

    FaderImage.gameObject.SetActive(false);

    if (FadeCompleteCallback != null)
      FadeCompleteCallback();
  }
}
