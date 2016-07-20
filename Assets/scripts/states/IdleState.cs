using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IdleState : GameObjectState 
{
  public IdleState(ActorBase actor)
  {
    _actor = actor;
    _working = false;

    _actor.AnimationComponent.Play(GlobalConstants.AnimationIdleName);
  }  

  bool _working = false;
  public override void Run()
  {
    if (!_working)
    {
      _working = true;
      JobManager.Instance.StartCoroutine(DelayRoutine(() => 
        {
          _actor.ChangeState(new SearchingForPlayerState(_actor));
        }));
    }
  }

  float _delay = 0.0f;
  IEnumerator DelayRoutine(Callback cb = null)
  {
    _delay = Random.Range(GlobalConstants.WanderingMinDelaySeconds + 1, GlobalConstants.WanderingMaxDelaySeconds + 1);

    float time = 0.0f;

    while (time < _delay)
    {
      time += Time.smoothDeltaTime;

      /*
      if (IsPlayerInRange())
      {
        _actor.ChangeState(new ApproachingPlayerState(_actor));
        yield break;
      }
      */

      yield return null;
    }

    if (cb != null)
      cb();

    yield return null;
  }
}
