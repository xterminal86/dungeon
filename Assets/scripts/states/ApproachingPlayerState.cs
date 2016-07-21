using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ApproachingPlayerState : GameObjectState 
{
  Vector3 _modelPosition = Vector3.zero;

  Job _moveJob, _rotateJob, _stepJob;

  RoadBuilder _roadBuilder;

  ModelMover _model;

  Int2 _currentMapPos = new Int2();

  public ApproachingPlayerState(ActorBase actor)
  {
    _actor = actor;
    _model = actor.Model;
    _roadBuilder = new RoadBuilder(_actor.AppRef.GeneratedMap.PathfindingMap, _actor.AppRef.GeneratedMapWidth, _actor.AppRef.GeneratedMapHeight);
      
    _modelPosition.x = _model.ModelPos.X * GlobalConstants.WallScaleFactor;
    _modelPosition.y = _model.transform.position.y;
    _modelPosition.z = _model.ModelPos.Y * GlobalConstants.WallScaleFactor;

    _model.AnimationComponent.Play(GlobalConstants.AnimationIdleName);

    _running = false;
  }

  bool _running = false, _firstStepSound = false, _moveDone = false;
  public override void Run()
  {   
    if (!_running)
    {
      _running = true;
      _moveJob = JobManager.Instance.CreateCoroutine(ApproachPlayerRoutine());
    }

    _model.transform.position = _modelPosition;
  }

  List<RoadBuilder.PathNode> _road;
  IEnumerator ApproachPlayerRoutine()
  {    
    _modelPosition.x = _model.ModelPos.X * GlobalConstants.WallScaleFactor;
    _modelPosition.y = _model.transform.position.y;
    _modelPosition.z = _model.ModelPos.Y * GlobalConstants.WallScaleFactor;

    if (_roadBuilder.IsThreadWorking)
    {
      _roadBuilder.AbortThread();
    }
    
    _roadBuilder.BuildRoadAsync(_actor.Model.ModelPos, _actor.InputRef.PlayerMapPos, true);

    while ((_road = _roadBuilder.GetResult()) == null)
    {      
      // If we cannot find a path to the player, we get stuck in a pathfinding loop for a long time.
      // If during impossible pathfinding loop (e.g. standing before closed door) player moved outside
      // agro range, we stop all activity and change the state to SearchingForPlayerState.
      //
      // Interestingly enough, if we set "!playerInRange", actor will always go to the last detected
      // player position and will change its state only if player is found again during traversal.
      // If we set "playerInRange" we will always change the state, if player position has changed during pathbuilding,
      // which is very unlikely to happen in short ranges, while if we are standing before a closed door,
      // changing the state is meaningless, because we will still be unable to find path.

      if (IsPlayerPositionChanged() && IsPlayerInRange())
      {
        //_roadBuilder.ProcessRoutine.KillJob();
        _roadBuilder.AbortThread();
        _actor.ChangeState(new SearchingForPlayerState(_actor));
        yield break;
      }
      
      yield return null;
    }
        
    // If we could not find path to the player, path list will be empty
    if (_roadBuilder.ResultReady && _road.Count == 0)
    { 
      //Debug.Log("Path not found!");
      _actor.ChangeState(new SearchingForPlayerState(_actor));
      yield break;
    }
    
    _currentMapPos.X = _model.ModelPos.X;
    _currentMapPos.Y = _model.ModelPos.Y;

    float angleStart = 0.0f, angleEnd = 0.0f;

    while (_road.Count > 1)
    {      
      //Debug.Log("Approaching player");

      angleStart = _model.transform.rotation.eulerAngles.y;
      angleEnd = GetAngleToRotate(_road[0].Coordinate);

      //Debug.Log(_road[0].Coordinate);
      //Debug.Log("Rotating from " + angleStart + " to " + angleEnd);
      
      if ((int)angleStart != (int)angleEnd)
      {
        _rotateJob = JobManager.Instance.CreateCoroutine(RotateModel(angleEnd));
        
        while (!_rotateDone)
        {
          yield return null;
        }
        
        _firstStepSound = false;
      }

      if (CanMove(_road[0].Coordinate))
      {        
        _stepJob = JobManager.Instance.CreateCoroutine(MoveModel(_road[0].Coordinate));

        while (!_moveDone)
        {
          yield return null;
        }

        _road.RemoveAt(0);
      }
      else
      {
        _actor.ChangeState(new SearchingForPlayerState(_actor));
        yield break;
      }      

      // If player position changed after actor made a step,
      // go to the search mode again to build new path.

      if (IsPlayerInRange() && IsPlayerPositionChanged())
      {
        _actor.ChangeState(new SearchingForPlayerState(_actor));
        yield break;
      }

      yield return null;
    }

    _model.AnimationComponent.Play(GlobalConstants.AnimationIdleName);

    // Rotate to face player on last step...
    if (Utils.BlockDistance(_model.ModelPos, _actor.InputRef.PlayerMapPos) == 1)
    {
      angleStart = _model.transform.rotation.eulerAngles.y;
      angleEnd = GetAngleToRotate(_actor.InputRef.PlayerMapPos);

      if ((int)angleStart != (int)angleEnd)
      {
        _rotateJob = JobManager.Instance.CreateCoroutine(RotateModel(angleEnd));
        
        while (!_rotateDone)
        {
          yield return null;
        }
      }
            
      // ...and switch to attack mode if there is no wall before us...
      if (CanMove(_actor.InputRef.PlayerMapPos))
      {
        _actor.ChangeState(new AttackState(_actor));
        yield break;
      }
      else
      {
        // ...or go to search mode otherwise
        _actor.ChangeState(new SearchingForPlayerState(_actor));
        yield break;
      }
    }
    else
    {
      // If we got to the following line of code, it means we either moved to the last
      // detected player position and could not get close in order to switch to attack state 
      // (player already moved outside the range), or could not build a path to the player at all.
      // So we go to the searching mode again.

      _actor.ChangeState(new SearchingForPlayerState(_actor));
    }

    //Debug.Log("Approached player");

    _running = false;

    yield return null;
  }

  bool CanMove(Int2 nextCellCoord)
  {
    int modelFacing = (int)GlobalConstants.OrientationByAngle[(int)_model.transform.eulerAngles.y];
    int nextCellSide = (int)GlobalConstants.OppositeOrientationByAngle[(int)_model.transform.eulerAngles.y];

    int x = nextCellCoord.X;
    int y = nextCellCoord.Y;

    if (!_actor.AppRef.GeneratedMap.PathfindingMap[x, y].Walkable 
      || _actor.AppRef.GeneratedMap.PathfindingMap[_model.ModelPos.X, _model.ModelPos.Y].SidesWalkability[(GlobalConstants.Orientation)modelFacing] == false
      || _actor.AppRef.GeneratedMap.PathfindingMap[x, y].SidesWalkability[(GlobalConstants.Orientation)nextCellSide] == false)
    {
      return false;
    }

    return true;
  }

  float GetAngleToRotate(Int2 cellToLook)
  {
    int dx = cellToLook.X - _currentMapPos.X;
    int dy = cellToLook.Y - _currentMapPos.Y;
    
    float angleEnd = 0.0f;
    
    if (dy == 1 && dx == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.EAST];
    else if (dy == -1 && dx == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.WEST];
    else if (dx == 1 && dy == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.SOUTH];
    else if (dx == -1 && dy == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.NORTH];

    return angleEnd;
  }

  bool _rotateDone = false;
  IEnumerator RotateModel(float angle)
  {
    _model.AnimationComponent.Play(GlobalConstants.AnimationIdleName);
    
    _rotateDone = false;
    
    Vector3 tmpRotation = _model.transform.rotation.eulerAngles;
    
    int d = (int)angle - (int)tmpRotation.y;
    
    bool specialRotation = ((int)angle == 270 && (int)tmpRotation.y == 0) || ((int)angle == 0 && (int)tmpRotation.y == 270);
    int cond = specialRotation ? 90 : Mathf.Abs(d);
    
    float sign = Mathf.Sign(d);
    
    float counter = 0.0f;
    while (counter < cond)
    {
      counter += Time.smoothDeltaTime * GlobalConstants.CharacterRotationSpeed;
      
      if (specialRotation)
      {
        tmpRotation.y -= sign * (Time.smoothDeltaTime * GlobalConstants.CharacterRotationSpeed);
      }
      else
      {
        tmpRotation.y += sign * (Time.smoothDeltaTime * GlobalConstants.CharacterRotationSpeed);
      }
      
      _model.transform.rotation = Quaternion.Euler(tmpRotation);
      
      yield return null;
    }
    
    tmpRotation.y = angle;
    _model.transform.rotation = Quaternion.Euler(tmpRotation);
    
    _rotateDone = true;
    
    yield return null;
  }

  IEnumerator MoveModel(Int2 newMapPos)
  {
    if (!_model.AnimationComponent.IsPlaying(GlobalConstants.AnimationWalkName))
    {
      _model.AnimationComponent.Play(GlobalConstants.AnimationWalkName);
    }
    
    _moveDone = false;
    
    _currentMapPos.X = _model.ModelPos.X;
    _currentMapPos.Y = _model.ModelPos.Y;
    
    int dx = newMapPos.X - _currentMapPos.X;
    int dy = newMapPos.Y - _currentMapPos.Y;
    
    if (!_firstStepSound)
    {
      PlayFootstepSound3D(_model.ModelPos, _modelPosition);
      _firstStepSound = true;
    }
    
    while (dx != 0 || dy != 0)
    {
      _modelPosition.x += dx * (Time.smoothDeltaTime * _model.WalkingSpeed);
      _modelPosition.z += dy * (Time.smoothDeltaTime * _model.WalkingSpeed);
      
      _currentMapPos.X = dx < 0 ? Mathf.CeilToInt(_modelPosition.x / GlobalConstants.WallScaleFactor) : Mathf.FloorToInt(_modelPosition.x / GlobalConstants.WallScaleFactor);
      _currentMapPos.Y = dy < 0 ? Mathf.CeilToInt(_modelPosition.z / GlobalConstants.WallScaleFactor) : Mathf.FloorToInt(_modelPosition.z / GlobalConstants.WallScaleFactor);
      
      dx = newMapPos.X - _currentMapPos.X;
      dy = newMapPos.Y - _currentMapPos.Y;      
      
      yield return null;
    }
    
    _modelPosition.x = newMapPos.X * GlobalConstants.WallScaleFactor;
    _modelPosition.z = newMapPos.Y * GlobalConstants.WallScaleFactor;
    
    _model.ModelPos.X = newMapPos.X;
    _model.ModelPos.Y = newMapPos.Y;
    
    _moveDone = true;
    
    PlayFootstepSound3D(_model.ModelPos, _modelPosition);
    
    yield return null;
  }
}
