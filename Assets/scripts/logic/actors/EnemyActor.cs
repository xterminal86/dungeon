using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyActor : ActorBase 
{
  public int AgroRange = 5;

  public EnemyActor(ModelMover model, App appRef, InputController inputRef) : base(model, appRef, inputRef)
  {
    SearchingForPlayerStateVar = new SearchingForPlayerState(this);
    IdleStateVar = new IdleState(this);

    ChangeState(SearchingForPlayerStateVar);
  }
}
