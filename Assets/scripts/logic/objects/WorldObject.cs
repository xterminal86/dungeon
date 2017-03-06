using UnityEngine;
using System.Collections;

/// <summary>
/// Logic class of a scene interactable object
/// </summary>
public abstract class WorldObject
{
  // So that we can have different prefabs for doors, walls etc.
  public GlobalConstants.WorldObjectClass ObjectClass = GlobalConstants.WorldObjectClass.PLACEHOLDER;

  public GlobalConstants.Orientation ObjectOrientation = GlobalConstants.Orientation.NORTH;

  // Called when user interacts with object on a scene
  public CallbackO ActionCallback;
  public CallbackO ActionCompleteCallback;

  // Called when object on a scene is interacted via control (button, lever etc)
  public CallbackO ControlCallback;
  public CallbackO ControlCompleteCallback;

  // MonoBehaviour script reference to the respective interactable object
  public BehaviourWorldObject BMO;

  public string InGameName = "n\a";
  public string PrefabName = string.Empty;

  public WorldObject(string inGameName, string prefabName)
  { 
    InGameName = inGameName;
    PrefabName = prefabName;
  }

  // These sould be overriden for respective interaction special handling.
  // Subscription should be done during instantiation of a specific derived class object
  // and routed on these methods accordingly.

  public virtual void ActionHandler(object sender) { }
  public virtual void ActionCompleteHandler(object sender) { }
  public virtual void ControlHandler(object sender) { }
  public virtual void ControlCompleteHandler(object sender) { }
}