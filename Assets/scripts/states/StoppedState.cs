using UnityEngine;
using System.Collections;

/// <summary>
/// Sort of end state - changing to this simply does nothing 
/// and cannot be exited from.
/// </summary>
public class StoppedState : GameObjectState
{
  public StoppedState(ActorBase actor)
  {    
    _actor = actor;
  }

  public override void ResetState()
  {
  }
}
