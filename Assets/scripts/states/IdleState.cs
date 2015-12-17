using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IdleState : GameObjectState 
{
  public IdleState(ActorBase actor)
  {
    _actor = actor;
  }  
}
