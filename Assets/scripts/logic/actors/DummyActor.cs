using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyActor : ActorBase 
{
  public DummyActor(string prefabName, Int3 position, GlobalConstants.Orientation o, GlobalConstants.ActorRole actorRole) : base(prefabName, position, o, actorRole)
  {      
    WanderingStateVar = new WanderingState(this);
    StoppedStateVar = new StoppedState(this);

    ChangeState(WanderingStateVar);
  }
}
