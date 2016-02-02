using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ItemObject
{
  public BehaviourItemObject BIO;

  public int AtlasIcon = -1;

  public CallbackO ActionCallback;

  public string ItemNameText = string.Empty;
  public string DescriptionText = string.Empty;

  public ItemObject()
  {
  }

  public virtual void SaveReference() { }
  public virtual void ActionHandler(object sender) { }
}
