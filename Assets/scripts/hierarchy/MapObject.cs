using UnityEngine;
using System.Collections;

public abstract class MapObject
{
  public string ClassName = string.Empty;
  public string Id = string.Empty;

  public int Facing = -1;

  public CallbackO ActionCallback;
  public CallbackO ActionCompleteCallback;

  public BehaviourMapObject BMO;
  public GameObject GameObjectToControl;

  public MapObject()
  {
    ClassName = "MapObject";  
    Id = "None";
  }

  public virtual void ActionHandler(object sender)
  {
  }

  public virtual void ActionCompleteHandler(object sender)
  {
  }
}
