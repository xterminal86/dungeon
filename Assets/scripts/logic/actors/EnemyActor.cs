using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyActor : ActorBase 
{
  public int AgroRange = 5;

  public EnemyActor(ModelMover model, App appRef, InputController inputRef) : base(model, appRef, inputRef)
  {
    ChangeState(new SearchingForPlayerState(this)); 
  }
}
