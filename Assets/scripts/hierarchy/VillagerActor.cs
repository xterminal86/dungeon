using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents villager, that randomly wanders around and can give random hints to player.
/// </summary>
public class VillagerActor : ActorBase 
{
  public VillagerActor(ModelMover model) : base(model)
  {    
  }

  public override void Interact()
  {    
    GUIManager.Instance.ShowFormTalking(this);
  }
}
