using UnityEngine;
using System.Collections;

public abstract class MapObject
{
  public string ClassName = string.Empty;
  public string PrefabName = string.Empty;
  public string ObjectId = string.Empty;

  public int Facing = -1;

  public bool IsBeingInteracted = false;
  public bool IsOpen = false;

  // Called when user interacts with object on a scene
  public CallbackO ActionCallback;
  public CallbackO ActionCompleteCallback;

  // Called when object on a scene is interacted via control (button, lever etc)
  public CallbackO ControlCallback;
  public CallbackO ControlCompleteCallback;

  public BehaviourMapObject BMO;

  public MapObject()
  {
    ClassName = "MapObject";  
    PrefabName = "None";
    ObjectId = "n/a";
  }

  public virtual void ActionHandler(object sender) { }
  public virtual void ActionCompleteHandler(object sender) { }
  public virtual void ControlHandler(object sender) { }
  public virtual void ControlCompleteHandler(object sender) { }
}
