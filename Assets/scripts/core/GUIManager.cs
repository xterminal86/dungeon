﻿using UnityEngine;
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
  int _hash = -1;
  public void ShowFormTalking(ActorBase actorToTalk)
  {
    _actorToTalk = actorToTalk;

    _hash = _actorToTalk.ActorName.GetHashCode();
    
    if (!App.Instance.VillagersInfo.ContainsKey(_hash))
    {
      return;
    }

    if (!FormTalking.activeSelf)
    {
      if (_actorToTalk.ActorState is WanderingState)
      {
        App.Instance.PlayerMoveState = App.GameState.HOLD_PLAYER;

        (_actorToTalk.ActorState as WanderingState).KillAllJobs();
        (_actorToTalk.ActorState as WanderingState).AdjustModelPosition();

        _actorToTalk.ChangeState(new TalkingState(_actorToTalk));
      }

      SetupFormTalking();
    }
  }

  Job _printTextJob;
  public void FormTalkingNameHandler()
  {
    ButtonClickSound.Play();

    if (_coroutineDone)
    {
      _actorToTalk.AnimationComponent.Play(GlobalConstants.AnimationTalkName);

      _printTextJob = JobManager.Instance.CreateJob(PrintTextRoutine(_villagerInfo.VillagerName));
    }
  }

  public void FormTalkingJobHandler()
  {
    ButtonClickSound.Play();

    if (_coroutineDone)
    {
      _actorToTalk.AnimationComponent.Play(GlobalConstants.AnimationTalkName);

      _printTextJob = JobManager.Instance.CreateJob(PrintTextRoutine(_villagerInfo.VillagerJob));
    }
  }

  int _gossipListIndex = 0;
  public void FormTalkingGossipHandler()
  {
    ButtonClickSound.Play();

    if (_printTextJob != null)
    {
      _printTextJob.KillJob();
    }

    _actorToTalk.AnimationComponent.Play(GlobalConstants.AnimationTalkName);

    // In case some villagers have more gossip lines than others,
    // we first check for overflow
    _gossipListIndex %= _villagerInfo.VillagerGossipLines.Count;
            
    _printTextJob = JobManager.Instance.CreateJob(PrintTextRoutine(_villagerInfo.VillagerGossipLines[_gossipListIndex]));

    _gossipListIndex++;
  }

  public void FormTalkingByeHandler()
  {
    ButtonClickSound.Play();

    if (_printTextJob != null)
    {
      _printTextJob.KillJob();
    }

    if (FormTalking.activeSelf)
    {
      FormTalking.SetActive(false);
      FormCompass.SetActive(true);
    }

    if (_actorToTalk != null)
    {
      _actorToTalk.ChangeState(new WanderingState(_actorToTalk));
    }

    App.Instance.PlayerMoveState = App.GameState.NORMAL;
  }

  void SetupFormTalking()
  {
    FormCompass.SetActive(false);
    FormTalking.SetActive(true);      

    _villagerInfo = App.Instance.VillagersInfo[_hash];

    Sprite portraitSprite = FindPortraitByName(_villagerInfo.PortraitName);

    PortraitImage.gameObject.SetActive(portraitSprite != null);

    if (portraitSprite != null)
    {
      PortraitImage.sprite = portraitSprite;
    }

    FormTalkingName.text = _actorToTalk.ActorName;

    _printTextJob = JobManager.Instance.CreateJob(PrintTextRoutine(_villagerInfo.HailString, true));
  }

  bool _coroutineDone = true;  
  string _textBuf = string.Empty;
  IEnumerator PrintTextRoutine(string textToPrint, bool formFirstOpen = false)
  {
    int count = 0;

    _textBuf = string.Empty;

    _coroutineDone = false;

    float speakPitch = _actorToTalk.Model.IsFemale ? 2.0f : 1.0f;

    while (count < textToPrint.Length)
    {      
      _textBuf += textToPrint[count];

      count++;

      FormTalkingText.text = _textBuf;

      if ( (count % 3 == 0)  && !formFirstOpen)
      {
        SoundManager.Instance.PlaySound(CharacterSpeakSound, speakPitch);
      }

      yield return new WaitForSeconds(0.01f);
    }

    _coroutineDone = true;

    yield return null;
  }  
}
