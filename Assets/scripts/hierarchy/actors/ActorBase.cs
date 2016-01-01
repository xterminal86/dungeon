using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for actors (models) logic
/// </summary>
public abstract class ActorBase
{
  public string ActorName = string.Empty;

  // Current state of this actor
  public GameObjectState ActorState;

  // Reference to the respective MonoBehaviour script for this actor
  public ModelMover Model;

  // Cached component
  public Animation AnimationComponent;

  public ActorBase(ModelMover model)
  {
    Model = model;
    AnimationComponent = Model.GetComponent<Animation>();
  }

  public virtual void ChangeState(GameObjectState newState)
  {
    //Debug.Log("[" + this + "]: changing state: " + ActorState + " -> " + newState);

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
