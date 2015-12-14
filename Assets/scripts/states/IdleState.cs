using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IdleState : GameObjectState 
{
  public IdleState()
  {
    _actor.AnimationComponent.Play(GlobalConstants.AnimationIdleName);
  }  
}
