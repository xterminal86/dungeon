using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for managing specific actors' functionalities
/// </summary>
public abstract class GameObjectState
{
  protected ModelMover _model;
  
  public virtual void Run()
  {
  }
}
