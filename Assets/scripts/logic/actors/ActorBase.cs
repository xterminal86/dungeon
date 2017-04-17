using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for actors (models) logic
/// </summary>
public abstract class ActorBase
{
  // Current state of this actor
  public GameObjectState ActorState;

  // Reference to the respective MonoBehaviour script for this actor
  public ModelMover Model;

  // Cached component
  public Animation AnimationComponent;

  // All possible states go here.
  // Create needed state objects in constructor of a specific Actor.
  // Use ChangeState(SomeStateVar) to change state.

  public WanderingState WanderingStateVar;
  public SearchingForPlayerState SearchingForPlayerStateVar;
  public IdleState IdleStateVar;
  public TalkingState TalkingStateVar;
  public AttackState AttackStateVar;
  public StoppedState StoppedStateVar;

  public Int3 ActorPosition = Int3.Zero;
  public Vector3 ActorWorldPosition = Vector3.zero;

  public readonly GlobalConstants.ActorRole ActorRole = GlobalConstants.ActorRole.DUMMY;

  // For footsteps dictionary access
  public string GameObjectName = string.Empty;

  public readonly string PrefabName = string.Empty;

  public float ModelMovementSpeed = GlobalConstants.WallScaleFactor;

  protected GlobalConstants.Orientation _actorOrientation = GlobalConstants.Orientation.EAST;
  public GlobalConstants.Orientation ActorOrientation
  {
    get { return _actorOrientation; }
  }

  public ActorBase(string prefabName, Int3 position, GlobalConstants.Orientation actorOrientation, GlobalConstants.ActorRole actorRole)
  {        
    PrefabName = prefabName;
    ActorRole = actorRole;
    _actorOrientation = actorOrientation;

    ActorPosition.Set(position);
    ActorWorldPosition.Set(position.X * GlobalConstants.WallScaleFactor, 
                           position.Y * GlobalConstants.WallScaleFactor,
                           position.Z * GlobalConstants.WallScaleFactor);    
  }

  public virtual void ChangeState(GameObjectState newState)
  {
    //Debug.Log("[" + this + "]: changing state: " + ActorState + " -> " + newState);

    ActorState = newState;

    ActorState.ResetState();
  }

  public virtual void Interact()
  {
  }

  public void Perform()
  {
    ActorState.Run();
  }

  public void InitBehaviour(ModelMover modelMover)
  {
    Model = modelMover;
    AnimationComponent = Model.GetComponent<Animation>();
  }
}
