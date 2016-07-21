using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TalkingState : GameObjectState 
{
  Job _rotateJob;

  public TalkingState(ActorBase actor)
  {
    _actor = actor;

    // Rotate actor to face the player
    Vector3 rotation = Camera.main.transform.eulerAngles;
    rotation.y = rotation.y - 180;

    _actor.Model.transform.eulerAngles = rotation;

    _actor.AnimationComponent.CrossFade(GlobalConstants.AnimationIdleName);
  }

  public override void ResetState()
  {
    // Rotate actor to face the player
    Vector3 rotation = Camera.main.transform.eulerAngles;
    rotation.y = rotation.y - 180;

    _actor.Model.transform.eulerAngles = rotation;
    _actor.AnimationComponent.CrossFade(GlobalConstants.AnimationIdleName);
  }

  public override void Run()
  {
    if (!_actor.AnimationComponent.IsPlaying(GlobalConstants.AnimationIdleName)
     && !_actor.AnimationComponent.IsPlaying(GlobalConstants.AnimationTalkName))
    {
      _actor.AnimationComponent.Play(GlobalConstants.AnimationIdleName);
    } 
  }  
}
