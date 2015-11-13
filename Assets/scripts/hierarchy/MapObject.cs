using UnityEngine;
using System.Collections;

public abstract class MapObject
{
  public string ClassName = string.Empty;
  public string PrefabName = string.Empty;
  public string ObjectId = string.Empty;

  public int Facing = -1;

  public CallbackO ActionCallback;
  public CallbackO ActionCompleteCallback;

  public CallbackO ControlCallback;
  public CallbackO ControlCompleteCallback;

  public BehaviourMapObject BMO;
  public GameObject GameObjectToControl;

  public MapObject()
  {
    ClassName = "MapObject";  
    PrefabName = "None";
    ObjectId = "n/a";
  }

  public virtual void ActionHandler(object sender)
  {
  }

  public virtual void ActionCompleteHandler(object sender)
  {
  }

  public virtual void ControlHandler(object sender)
  {
  }

  public virtual void ControlCompleteHandler(object sender)
  {
  }
}
