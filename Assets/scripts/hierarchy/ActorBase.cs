using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for actors (models)
/// </summary>
public abstract class ActorBase
{
  public string ActorName = string.Empty;

  public GameObjectState ActorState;

  public ActorBase()
  {

  }

  public virtual void ChangeState(GameObjectState newState)
  {
    ActorState = newState;
  }
}
