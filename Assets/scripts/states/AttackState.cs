using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackState : GameObjectState 
{
  float _attackHalfway = 0.0f;
  public AttackState(ActorBase actor) : base()
  {
    _actor = actor;

    _timer = GlobalConstants.AttackCooldown;
    _attackHalfway = _actor.AnimationComponent[GlobalConstants.AnimationAttackName].length / 2.0f;
  }

  float _timer = 0.0f;
  bool _trigger = false;
  public override void Run()
  {
    if (!_trigger 
     && _actor.AnimationComponent.IsPlaying(GlobalConstants.AnimationAttackName) 
     && _actor.AnimationComponent[GlobalConstants.AnimationAttackName].time > _attackHalfway)
    {
      _trigger = true;
      PlayerData.Instance.PlayerCharacterVariable.AddDamage(-1);
      SoundManager.Instance.PlaySound(GlobalConstants.SFXPunch.GetHashCode(), 0.0f, _actor.Model.transform.position, true);
    }

    _timer += Time.smoothDeltaTime;

    if (_timer > GlobalConstants.AttackCooldown)
    {
      _timer = 0.0f;

      if (IsPlayerReachable() && !HasWall(_actor.Model.ModelPos))
      {
        _actor.Model.AnimationComponent.Play(GlobalConstants.AnimationAttackName);
        _trigger = false;
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

  bool HasWall(Int2 actorPos)
  {
    var modelFacing = GlobalConstants.OrientationByAngle[(int)_actor.Model.transform.eulerAngles.y];
    var nextCellSide = GlobalConstants.OppositeOrientationByAngle[(int)_actor.Model.transform.eulerAngles.y];

    int nextCellX = actorPos.X;
    int nextCellY = actorPos.Y;

    if (modelFacing == GlobalConstants.Orientation.NORTH) nextCellX--;
    else if (modelFacing == GlobalConstants.Orientation.EAST) nextCellY++;
    else if (modelFacing == GlobalConstants.Orientation.SOUTH) nextCellX++;
    else if (modelFacing == GlobalConstants.Orientation.WEST) nextCellY--;

    bool nextCellWalkability = App.Instance.GeneratedMap.PathfindingMap[nextCellX, nextCellY].Walkable;
    bool currentCellWall = App.Instance.GeneratedMap.PathfindingMap[actorPos.X, actorPos.Y].SidesWalkability[modelFacing];
    bool nextCellWall = App.Instance.GeneratedMap.PathfindingMap[nextCellX, nextCellY].SidesWalkability[nextCellSide];

    //Debug.Log("Checking cells " + modelFacing + " " + nextCellSide + " " + nextCellX + " " + nextCellY + " " + nextCellWalkability + " " + currentCellWall + " " + nextCellWall);

    if (!nextCellWalkability || !currentCellWall || !nextCellWall)
    {
      return true;
    }

    return false;
  }
}
