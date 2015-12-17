using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TalkingState : GameObjectState 
{
  Job _rotateJob;

  public TalkingState(ActorBase actor)
  {
    _actor = actor;

    Vector3 rotation = Camera.main.transform.eulerAngles;
    rotation.y = rotation.y - 180;

    _actor.Model.transform.eulerAngles = rotation;    
  }

  public void StopTalkingAnimation()
  {
    _actor.Model.AnimationComponent[GlobalConstants.AnimationTalkName].time = 0.0f;
    _actor.Model.AnimationComponent.Sample();
    _actor.Model.AnimationComponent.Stop(GlobalConstants.AnimationTalkName);
  }
}
