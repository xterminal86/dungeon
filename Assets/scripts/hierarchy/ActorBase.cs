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

  public ModelMover Model;
  public Animation AnimationComponent;

  public ActorBase(ModelMover model)
  {
    Model = model;
    AnimationComponent = Model.GetComponent<Animation>();
  }

  public virtual void ChangeState(GameObjectState newState)
  {
    ActorState = newState;
  }

  public virtual void Perform()
  {
    ActorState.Run();
  }

  public virtual void Interact()
  {
  }
}
