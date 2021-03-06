﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Logic class of a scene interactable object
/// </summary>
public abstract class WorldObject
{
  // So that we can have different prefabs for doors, walls etc.
  public GlobalConstants.WorldObjectClass ObjectClass = GlobalConstants.WorldObjectClass.PLACEHOLDER;

  // Object orientation
  public GlobalConstants.Orientation ObjectOrientation = GlobalConstants.Orientation.NORTH;

  // Called when user interacts with object on a scene
  public CallbackO ActionCallback;

  // Use this to call something after ActionCallback method is done
  public CallbackO ActionCompleteCallback;

  // Position in a map grid array
  public Int3 ArrayCoordinates = new Int3();

  // MonoBehaviour script reference to the respective interactable object
  public BehaviourWorldObject BWO;

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

  // Handles the object's interaction
  public virtual void ActionHandler(object sender) { }
  // Called at the end of the above handler
  public virtual void ActionCompleteHandler(object sender) { }
}