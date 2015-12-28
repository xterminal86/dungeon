using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackState : GameObjectState 
{
  public AttackState(ActorBase actor) : base()
  {
    _actor = actor;

    if (Utils.BlockDistance(_actor.Model.ModelPos, InputController.Instance.PlayerMapPos) == 1)
    {
      _actor.Model.AnimationComponent.Play(GlobalConstants.AnimationAttackName);
    }
  }

  float _timer = 0.0f;
  public override void Run()
  {
    _timer += Time.smoothDeltaTime;

    if (_timer > GlobalConstants.AttackCooldown)
    {
      _timer = 0.0f;

      if (Utils.BlockDistance(_actor.Model.ModelPos, InputController.Instance.PlayerMapPos) == 1)
      {
        _actor.Model.AnimationComponent.Play(GlobalConstants.AnimationAttackName);
      }
    }

    if (!_actor.Model.AnimationComponent.IsPlaying(GlobalConstants.AnimationAttackName) 
     && Utils.BlockDistance(_actor.Model.ModelPos, InputController.Instance.PlayerMapPos) > 1)
    {
      _actor.ChangeState(new SearchingForPlayerState(_actor));
    }
  }
}
