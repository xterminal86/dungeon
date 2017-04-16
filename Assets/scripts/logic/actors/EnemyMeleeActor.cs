using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyActor : ActorBase 
{
  public int AgroRange = 5;

  public EnemyActor(string prefabName, Int3 position, GlobalConstants.Orientation o, GlobalConstants.ActorRole actorRole) : base(prefabName, position, o, actorRole)
  {
    SearchingForPlayerStateVar = new SearchingForPlayerState(this);
    IdleStateVar = new IdleState(this);

    ChangeState(SearchingForPlayerStateVar);
  }
}
