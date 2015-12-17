using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for managing specific actors' functionalities
/// </summary>
public abstract class GameObjectState
{
  protected ActorBase _actor;
  protected string _currentAnimationName = string.Empty;

  public virtual void Run()
  {
  }
}
