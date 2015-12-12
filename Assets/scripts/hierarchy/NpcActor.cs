using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents villager, that randomly wanders around and can give random hints to player.
/// </summary>
public class NpcActor : ActorBase 
{
  public NpcActor(GameObjectState state)
  {
    ChangeState(state);
  }
}
