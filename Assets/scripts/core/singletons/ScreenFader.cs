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
    _faderColor.a = 1.0f;
    FaderImage.gameObject.SetActive(true);
    FaderImage.color = _faderColor;    
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

      FaderImage.color = _faderColor;
            
      yield return null;
    }

    if (cb != null)
      cb();
    
    //if (FadeCompleteCallback != null)
    //  FadeCompleteCallback();
  }

  IEnumerator FadeInRoutine(Callback cb)
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

    if (cb != null)
      cb();
    
    //if (FadeCompleteCallback != null)
    //  FadeCompleteCallback();
  }
}
