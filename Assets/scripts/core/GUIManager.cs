using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GUIManager : MonoSingleton<GUIManager> 
{
  public AudioSource ButtonClickSound;
  public AudioSource CharacterSpeakSound;
  
  public GameObject FormTalking;
  public GameObject FormCompass;

  public Image CompassImage;
  public Image PortraitImage;

  public Text FormTalkingName;
  public Text FormTalkingText;

  public List<Sprite> Portraits = new List<Sprite>();

  public Sprite FindPortraitByName(string name)
  {
    foreach (var item in Portraits)
    {
      if (item.name == name)
      {
        return item;
      }
    }

    return null;
  }

  public void SetCompassVisibility(bool showFlag)
  {
    FormCompass.SetActive(showFlag);
  }

  // FormTalking

  ActorBase _actorToTalk;
  VillagerInfo _villagerInfo;
  public void ShowFormTalking(ActorBase actorToTalk)
  {
    if (!FormTalking.activeSelf)
    {
      _actorToTalk = actorToTalk;

      SetupFormTalking();

      FormCompass.SetActive(false);      
      FormTalking.SetActive(true);      
    }
  }

  public void FormTalkingNameHandler()
  {
    ButtonClickSound.Play();
  }

  public void FormTalkingJobHandler()
  {
    ButtonClickSound.Play();
  }

  public void FormTalkingGossipHandler()
  {
    ButtonClickSound.Play();
  }

  public void FormTalkingByeHandler()
  {
    _textBuf = string.Empty;

    ButtonClickSound.Play();

    FormTalking.SetActive(false);
    FormCompass.SetActive(true);
  }

  void SetupFormTalking()
  {
    int hash = _actorToTalk.ActorName.GetHashCode();

    if (App.Instance.VillagersInfo.ContainsKey(hash))
    {
      _villagerInfo = App.Instance.VillagersInfo[hash];

      Sprite portraitSprite = FindPortraitByName(_villagerInfo.PortraitName);
      if (portraitSprite != null)
      {
        PortraitImage.sprite = portraitSprite;
      }

      FormTalkingName.text = _actorToTalk.ActorName;

      StartCoroutine(PrintTextRoutine(_villagerInfo.HailString));
    }
  }

  bool _skipFlag = false;
  string _textBuf = string.Empty;
  IEnumerator PrintTextRoutine(string textToPrint)
  {
    int count = 0;

    while (count < textToPrint.Length)
    {      
      _textBuf += textToPrint[count];

      count++;

      FormTalkingText.text = _textBuf;

      if (_skipFlag)
      {
        SoundManager.Instance.PlaySound(CharacterSpeakSound, CharacterSpeakSound.pitch);
      }

      _skipFlag = !_skipFlag;

      yield return new WaitForSeconds(0.01f);
    }
    
    yield return null;
  }  
}
