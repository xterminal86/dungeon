﻿using UnityEngine;
using System.Collections;

public abstract class MapObject
{
  public string Name = string.Empty;
  public int HashCode = -1;  
  public int Facing = -1;

  public CallbackO ActionCallback;
  public CallbackO ActionCompleteCallback;

  public GameObject GameObjectToControl;

  public virtual void ActionHandler(object sender)
  {
  }

  public virtual void ActionCompleteHandler(object sender)
  {
  }
}
