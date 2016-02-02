using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ItemObject
{
  public BehaviourItemObject BIO;

  public int AtlasIcon = -1;

  public CallbackO ActionCallback;

  public ItemObject()
  {
    
  }

  public virtual void ActionHandler(object sender) { }
}
