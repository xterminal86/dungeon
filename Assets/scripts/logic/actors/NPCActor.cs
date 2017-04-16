using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents villager, that randomly wanders around and can give random hints to player.
/// </summary>
public class VillagerActor : ActorBase 
{
  public VillagerActor(string prefabName, Int3 position, GlobalConstants.Orientation o, GlobalConstants.ActorRole actorRole) : base(prefabName, position, o, actorRole)
  {
  }

  /*  
  public VillagerActor(ModelMover model) : base(model)
  {
    WanderingStateVar = new WanderingState(this);
    TalkingStateVar = new TalkingState(this);

    ChangeState(WanderingStateVar);
  }

  public override void Interact()
  {
    ShowFormTalking();
  }

  int _hash = -1;
  void ShowFormTalking()
  {
    _hash = ActorName.GetHashCode();

    if (!LevelLoader.Instance.NPCInfo.ContainsKey(_hash))
    {
      return;
    }

    if (!GUIManager.Instance.FormTalking.activeSelf)
    {
      GameData.Instance.PlayerMoveState = GameData.PlayerMoveStateEnum.HOLD_PLAYER;

      (ActorState as WanderingState).KillAllJobs();
      (ActorState as WanderingState).AdjustModelPosition();

      if (Model.ActorName != "???")
      {      
        SoundManager.Instance.PlaySound(Model.IsFemale ? GlobalConstants.SFXFemaleVillagerHuh : GlobalConstants.SFXMaleVillagerHuh, 
                                        Model.transform.position, true, Model.VoicePitch);
      }

      ChangeState(TalkingStateVar);      

      SetupFormTalking();
    }
  }

  NPCInfo _villagerInfo;
  Job _printTextJob;
  void SetupFormTalking()
  {
    GUIManager.Instance.InventoryFormWindow.gameObject.SetActive(false);

    GUIManager.Instance.FormTalking.SetActive(true);

    //Debug.Log(this.ActorName + ": FormTalking subscription");

    GUIManager.Instance.ButtonNameCallback += ButtonNameHandler;
    GUIManager.Instance.ButtonJobCallback += ButtonJobHandler;
    GUIManager.Instance.ButtonGossipCallback += ButtonGossipHandler;
    GUIManager.Instance.ButtonByeCallback += ButtonByeHandler;

    _villagerInfo = LevelLoader.Instance.NPCInfo[_hash];

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
      else if (AnimationComponent.IsPlaying(GlobalConstants.AnimationThinkingName))
      {
        AnimationComponent.Stop(GlobalConstants.AnimationThinkingName);
      }

      AnimationComponent.Play(GlobalConstants.AnimationTalkName);
    }
 
    int count = 0;

    _textBuf = string.Empty;

    _coroutineDone = false;

    float speakPitch = Model.SpeakPitch;

    while (count < textToPrint.Length)
    {
      _textBuf += textToPrint[count];

      GUIManager.Instance.FormTalkingText.text = _textBuf;

      if ((count % 3 == 0) && !formFirstOpen)
      {
        SoundManager.Instance.PlaySound(GUIManager.Instance.CharacterSpeakSound, speakPitch);
      }

      count++;

      yield return new WaitForSeconds(0.01f);
    }

    _coroutineDone = true;
        
    yield return null;
  }

  // Callback handlers

  void ButtonNameHandler()
  {
    if (_printTextJob != null)
    {
      _printTextJob.KillJob();
    }
        
    _printTextJob = JobManager.Instance.CreateCoroutine(PrintTextRoutine(_villagerInfo.Name));    
  }

  void ButtonJobHandler()
  {
    if (_printTextJob != null)
    {
      _printTextJob.KillJob();      
    }
        
    _printTextJob = JobManager.Instance.CreateCoroutine(PrintTextRoutine(_villagerInfo.Job));    
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
    _gossipListIndex %= _villagerInfo.GossipLines.Count;

    _printTextJob = JobManager.Instance.CreateCoroutine(PrintTextRoutine(_villagerInfo.GossipLines[_gossipListIndex]));

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
    
    //ActorState.RewindAnimation(GlobalConstants.AnimationTalkName);

    ChangeState(WanderingStateVar);
    
    GameData.Instance.PlayerMoveState = GameData.PlayerMoveStateEnum.NORMAL;
  }
  */
}
