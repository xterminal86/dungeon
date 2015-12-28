using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackState : GameObjectState 
{
  public AttackState(ActorBase actor) : base()
  {
    _actor = actor;

    _actor.Model.AnimationComponent.Play(GlobalConstants.AnimationIdleName);

  }

  public override void Run()
  {
    if (Utils.BlockDistance(_actor.Model.ModelPos, InputController.Instance.PlayerMapPos) > 1)
    {
      _actor.ChangeState(new SearchingForPlayerState(_actor));
    }
  }
}
