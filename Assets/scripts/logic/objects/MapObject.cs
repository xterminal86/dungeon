using UnityEngine;
using System.Collections;

/// <summary>
/// Logic class of a scene interactable object
/// </summary>
public abstract class MapObject
{
  // Used for deciding which type of MapObject to instantiate (App::CreateMapObject())
  public string ClassName = string.Empty;

  // Unused (for distinguishing objects in Debug.Log)
  public string PrefabName = string.Empty;

  public GlobalConstants.Orientation ObjectOrientation = GlobalConstants.Orientation.NORTH;

  public bool IsOpen = false;

  // Called when user interacts with object on a scene
  public CallbackO ActionCallback;
  public CallbackO ActionCompleteCallback;

  // Called when object on a scene is interacted via control (button, lever etc)
  public CallbackO ControlCallback;
  public CallbackO ControlCompleteCallback;

  // MonoBehaviour script reference to the respective interactable object
  public BehaviourMapObject BMO;

  protected App _appRef;

  public MapObject()
  {
    ClassName = "MapObject";  
    PrefabName = "None";
  }

  // These sould be overriden for respective interaction special handling.
  // Subscription should be done during instantiation of a specific derived class object
  // and routed on these methods accordingly.

  public virtual void ActionHandler(object sender) { }
  public virtual void ActionCompleteHandler(object sender) { }
  public virtual void ControlHandler(object sender) { }
  public virtual void ControlCompleteHandler(object sender) { }
}