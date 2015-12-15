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

    if (_coroutineDone)
    {
      StartCoroutine(PrintTextRoutine(_villagerInfo.VillagerName));
    }
  }

  public void FormTalkingJobHandler()
  {
    ButtonClickSound.Play();

    if (_coroutineDone)
    {
      StartCoroutine(PrintTextRoutine(_villagerInfo.VillagerJob));
    }
  }

  int _gossipListIndex = 0;
  public void FormTalkingGossipHandler()
  {
    ButtonClickSound.Play();

    if (_coroutineDone)
    {
      // In case some villagers have more gossip lines than others,
      // we first check for overflow
      _gossipListIndex %= _villagerInfo.VillagerGossipLines.Count;
            
      StartCoroutine(PrintTextRoutine(_villagerInfo.VillagerGossipLines[_gossipListIndex]));

      _gossipListIndex++;
    }
  }

  public void FormTalkingByeHandler()
  {
    //StopCoroutine(PrintTextRoutine(string.Empty));

    ButtonClickSound.Play();

    FormTalking.SetActive(false);
    FormCompass.SetActive(true);
  }

  int _hash = -1;
  void SetupFormTalking()
  {
    _hash = _actorToTalk.ActorName.GetHashCode();

    if (App.Instance.VillagersInfo.ContainsKey(_hash))
    {
      _villagerInfo = App.Instance.VillagersInfo[_hash];

      Sprite portraitSprite = FindPortraitByName(_villagerInfo.PortraitName);
      if (portraitSprite != null)
      {
        PortraitImage.sprite = portraitSprite;
      }

      FormTalkingName.text = _actorToTalk.ActorName;

      StartCoroutine(PrintTextRoutine(_villagerInfo.HailString));
    }
  }

  bool _coroutineDone = true;
  bool _skipFlag = false;
  string _textBuf = string.Empty;
  IEnumerator PrintTextRoutine(string textToPrint)
  {
    int count = 0;

    _textBuf = string.Empty;

    _coroutineDone = false;

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

    _coroutineDone = true;

    yield return null;
  }  
}
