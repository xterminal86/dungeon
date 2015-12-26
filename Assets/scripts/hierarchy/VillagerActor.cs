using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents villager, that randomly wanders around and can give random hints to player.
/// </summary>
public class VillagerActor : ActorBase 
{
  public VillagerActor(ModelMover model) : base(model)
  {
    ChangeState(new WanderingState(this));
  }

  public override void Interact()
  {
    ShowFormTalking();
  }

  int _hash = -1;
  void ShowFormTalking()
  {
    _hash = ActorName.GetHashCode();

    if (!App.Instance.VillagersInfo.ContainsKey(_hash))
    {
      return;
    }

    if (!GUIManager.Instance.FormTalking.activeSelf)
    {
      App.Instance.PlayerMoveState = App.PlayerMoveStateEnum.HOLD_PLAYER;

      (ActorState as WanderingState).KillAllJobs();
      (ActorState as WanderingState).AdjustModelPosition();

      ChangeState(new TalkingState(this));      

      SetupFormTalking();
    }
  }

  VillagerInfo _villagerInfo;
  Job _printTextJob;
  void SetupFormTalking()
  {
    GUIManager.Instance.FormCompass.SetActive(false);
    GUIManager.Instance.FormTalking.SetActive(true);

    //Debug.Log(this.ActorName + ": FormTalking subscription");

    GUIManager.Instance.ButtonNameCallback += ButtonNameHandler;
    GUIManager.Instance.ButtonJobCallback += ButtonJobHandler;
    GUIManager.Instance.ButtonGossipCallback += ButtonGossipHandler;
    GUIManager.Instance.ButtonByeCallback += ButtonByeHandler;

    _villagerInfo = App.Instance.VillagersInfo[_hash];

    Sprite portraitSprite = GUIManager.Instance.FindPortraitByName(_villagerInfo.PortraitName);

    GUIManager.Instance.PortraitImage.gameObject.SetActive(portraitSprite != null);

    if (portraitSprite != null)
    {
      GUIManager.Instance.PortraitImage.sprite = portraitSprite;
    }

    GUIManager.Instance.FormTalkingName.text = ActorName;

    _printTextJob = JobManager.Instance.CreateCoroutine(PrintTextRoutine(_villagerInfo.HailString, true));
  }

  bool _coroutineDone = true;
  string _textBuf = string.Empty;
  IEnumerator PrintTextRoutine(string textToPrint, bool formFirstOpen = false)
  {
    if (textToPrint != "..." && !formFirstOpen)
    {
      if (AnimationComponent.IsPlaying(GlobalConstants.AnimationTalkName))
      {
        AnimationComponent.Stop(GlobalConstants.AnimationTalkName);
      }

      AnimationComponent.Play(GlobalConstants.AnimationTalkName);
    }

    int count = 0;

    _textBuf = string.Empty;

    _coroutineDone = false;

    float speakPitch = Model.IsFemale ? 2.0f : 1.0f;

    while (count < textToPrint.Length)
    {
      _textBuf += textToPrint[count];

      count++;

      GUIManager.Instance.FormTalkingText.text = _textBuf;

      if ((count % 3 == 0) && !formFirstOpen)
      {
        SoundManager.Instance.PlaySound(GUIManager.Instance.CharacterSpeakSound, speakPitch);
      }

      yield return new WaitForSeconds(0.01f);
    }

    _coroutineDone = true;

    yield return null;
  }

  // Callback handlers

  void ButtonNameHandler()
  {    
    if (_coroutineDone)
    {
      _printTextJob = JobManager.Instance.CreateCoroutine(PrintTextRoutine(_villagerInfo.VillagerName));
    }
  }

  void ButtonJobHandler()
  {    
    if (_coroutineDone)
    {
      _printTextJob = JobManager.Instance.CreateCoroutine(PrintTextRoutine(_villagerInfo.VillagerJob));
    }
  }

  int _gossipListIndex = 0;
  void ButtonGossipHandler()
  {   
    if (_printTextJob != null)
    {
      _printTextJob.KillJob();
    }

    // In case some villagers have more gossip lines than others,
    // we first check for overflow.
    _gossipListIndex %= _villagerInfo.VillagerGossipLines.Count;

    _printTextJob = JobManager.Instance.CreateCoroutine(PrintTextRoutine(_villagerInfo.VillagerGossipLines[_gossipListIndex]));

    _gossipListIndex++;
  }

  void ButtonByeHandler()
  {   
    if (_printTextJob != null)
    {
      _printTextJob.KillJob();
    }

    //Debug.Log(this.ActorName + ": FormTalking unsubscribing.");

    GUIManager.Instance.ButtonNameCallback -= ButtonNameHandler;
    GUIManager.Instance.ButtonJobCallback -= ButtonJobHandler;
    GUIManager.Instance.ButtonGossipCallback -= ButtonGossipHandler;
    GUIManager.Instance.ButtonByeCallback -= ButtonByeHandler;

    GUIManager.Instance.FormTalking.SetActive(false);
    GUIManager.Instance.FormCompass.SetActive(true);    

    ChangeState(new WanderingState(this));
    
    App.Instance.PlayerMoveState = App.PlayerMoveStateEnum.NORMAL;
  }
}
