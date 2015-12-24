using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SearchingForPlayerState : GameObjectState 
{
  float _time = 0.0f;
  public SearchingForPlayerState(ActorBase actor)
  {
    _actor = actor;
    _time = 0.0f;
  }

  GeneratedMapCell _playerCell;
  public override void Run()
  {
    _time += Time.smoothDeltaTime;
    
    if (_time > GlobalConstants.SearchingForPlayerRate)
    {
      _time = 0.0f;
      
      bool playerInRange = IsPlayerInRange();        
      
      // FIXME: improve algorithm to handle dynamic obstacles
      _playerCell = App.Instance.GeneratedMap.GetMapCellByPosition(InputController.Instance.PlayerMapPos);
      if (_playerCell.CellType != GeneratedCellType.ROOM 
       && playerInRange 
       && Utils.BlockDistance(_actor.Model.ModelPos, InputController.Instance.PlayerMapPos) > 1)
      {
        _actor.ChangeState(new ApproachingPlayerState(_actor));
      }
    }
  }

  bool IsPlayerInRange()
  {
    Int2 pos = _actor.Model.ModelPos;
    
    int lx = Mathf.Clamp(pos.X - (_actor as EnemyActor).AgroRange, 0, App.Instance.GeneratedMapHeight - 1);
    int ly = Mathf.Clamp(pos.Y - (_actor as EnemyActor).AgroRange, 0, App.Instance.GeneratedMapWidth - 1);
    int hx = Mathf.Clamp(pos.X + (_actor as EnemyActor).AgroRange, 0, App.Instance.GeneratedMapHeight - 1);
    int hy = Mathf.Clamp(pos.Y + (_actor as EnemyActor).AgroRange, 0, App.Instance.GeneratedMapWidth - 1);
    
    if (InputController.Instance.PlayerMapPos.X < hx && InputController.Instance.PlayerMapPos.X > lx 
        && InputController.Instance.PlayerMapPos.Y < hy && InputController.Instance.PlayerMapPos.Y > ly)
    {
      return true;
    }
    
    return false;
  }
}
