using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackState : GameObjectState 
{
  public AttackState(ActorBase actor) : base()
  {
    _actor = actor;

    if (IsPlayerReachable() && !HasWall(_actor.Model.ModelPos))
    {
      _actor.Model.AnimationComponent.Play(GlobalConstants.AnimationAttackName);
    }
  }

  float _timer = 0.0f;
  public override void Run()
  {
    _timer += Time.smoothDeltaTime;

    if (_timer > GlobalConstants.AttackCooldown)
    {
      _timer = 0.0f;

      if (IsPlayerReachable() && !HasWall(_actor.Model.ModelPos))
      {
        _actor.Model.AnimationComponent.Play(GlobalConstants.AnimationAttackName);
      }
      else
      {
        _actor.ChangeState(new SearchingForPlayerState(_actor));
      }
    }

    if (!_actor.Model.AnimationComponent.IsPlaying(GlobalConstants.AnimationAttackName))
    {
      _actor.Model.AnimationComponent.Play(GlobalConstants.AnimationIdleName);
    }

    if (!_actor.Model.AnimationComponent.IsPlaying(GlobalConstants.AnimationAttackName) && !IsPlayerReachable())
    {
      _actor.ChangeState(new SearchingForPlayerState(_actor));
    }
  }

  bool IsPlayerReachable()
  {
    float d = Vector3.Distance(_actor.Model.transform.position, App.Instance.CameraPos);

    return (Utils.BlockDistance(_actor.Model.ModelPos, InputController.Instance.PlayerMapPos) == 1 && (int)d <= GlobalConstants.WallScaleFactor);    
  }

  bool HasWall(Int2 nextCellCoord)
  {
    int modelFacing = (int)GlobalConstants.OrientationByAngle[(int)_actor.Model.transform.eulerAngles.y];
    int nextCellSide = (int)GlobalConstants.OppositeOrientationByAngle[(int)_actor.Model.transform.eulerAngles.y];
        
    int x = nextCellCoord.X;
    int y = nextCellCoord.Y;

    if (!App.Instance.GeneratedMap.PathfindingMap[x, y].Walkable
      || App.Instance.GeneratedMap.PathfindingMap[_actor.Model.ModelPos.X, _actor.Model.ModelPos.Y].SidesWalkability[(GlobalConstants.Orientation)modelFacing] == false
      || App.Instance.GeneratedMap.PathfindingMap[x, y].SidesWalkability[(GlobalConstants.Orientation)nextCellSide] == false)
    {
      return true;
    }

    return false;
  }
}
