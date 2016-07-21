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

  protected App _appReference;
  public App AppRef
  {
    get { return _appReference; }
  }

  protected InputController _inputRef;
  public InputController InputRef
  {
    get { return _inputRef; }
  }

  // All possible states go here.
  // Create needed state objects in constructor of a specific Actor.
  // Use ChangeState(SomeStateVar) to change state.

  public WanderingState WanderingStateVar;
  public SearchingForPlayerState SearchingForPlayerStateVar;
  public IdleState IdleStateVar;
  public TalkingState TalkingStateVar;
  public AttackState AttackStateVar;
  public StoppedState StoppedStateVar;

  public ActorBase(ModelMover model, App appRef, InputController inputRef)
  {    
    _appReference = appRef;
    _inputRef = inputRef;

    Model = model;
    AnimationComponent = Model.GetComponent<Animation>();

    StoppedStateVar = new StoppedState(this);
  }

  public virtual void ChangeState(GameObjectState newState)
  {
    //Debug.Log("[" + this + "]: changing state: " + ActorState + " -> " + newState);

    ActorState = newState;

    ActorState.ResetState();
  }

  public virtual void Perform()
  {
    ActorState.Run();
  }

  public virtual void Interact()
  {
  }
}
