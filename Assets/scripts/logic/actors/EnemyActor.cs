using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyActor : ActorBase 
{
  public int AgroRange = 5;

  public EnemyActor(ModelMover model) : base(model)
  {
    SearchingForPlayerStateVar = new SearchingForPlayerState(this);
    IdleStateVar = new IdleState(this);

    ChangeState(SearchingForPlayerStateVar);
  }
}
