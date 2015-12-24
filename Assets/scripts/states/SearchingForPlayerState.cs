using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SearchingForPlayerState : GameObjectState 
{
  float _time = 0.0f;
  bool _working = false;
  Vector3 _modelPosition = Vector3.zero;

  RoadBuilder _roadBuilder;
  ModelMover _model;
  public SearchingForPlayerState(ActorBase actor)
  {
    _actor = actor;
    _model = _actor.Model;
    _time = 0.0f;
    _working = false;

    _modelPosition.x = _model.ModelPos.X * GlobalConstants.WallScaleFactor;
    _modelPosition.y = _model.transform.position.y;
    _modelPosition.z = _model.ModelPos.Y * GlobalConstants.WallScaleFactor;

    _roadBuilder = new RoadBuilder(App.Instance.GeneratedMap.Map, App.Instance.GeneratedMapWidth, App.Instance.GeneratedMapHeight);
    _model.AnimationComponent.Play(GlobalConstants.AnimationIdleName);
  }

  Job _mainJob, _stepJob, _rotateJob; 

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
        KillAllJobs();
        _actor.ChangeState(new ApproachingPlayerState(_actor));
      }
    }

    if (!_working)
    {
      _working = true;
      _mainJob = JobManager.Instance.CreateJob(MoveOnPath());
    }

    _model.transform.position = _modelPosition;
  }

  List<RoadBuilder.PathNode> _road;
  IEnumerator MoveOnPath()
  {
    _firstStepSound = false;
    
    _modelPosition.x = _model.ModelPos.X * GlobalConstants.WallScaleFactor;
    _modelPosition.y = _model.transform.position.y;
    _modelPosition.z = _model.ModelPos.Y * GlobalConstants.WallScaleFactor;
    
    Int2 destination = App.Instance.GeneratedMap.GetRandomUnoccupiedCell();
    
    //Debug.Log(name + ": going from " + ModelPos + " to " + destination);    
    
    _roadBuilder.BuildRoadAsync(_model.ModelPos, destination, true);
    
    while ((_road = _roadBuilder.GetResult()) == null)
    {
      // If player comes into range, while actor is still building his path,
      // we stop all activity and exit coroutine
      if (IsPlayerInRange())
      {
        _working = false;
        _roadBuilder.ProcessRoutine.KillJob();
        yield break;
      }

      yield return null;
    }
    
    _currentMapPos.X = _model.ModelPos.X;
    _currentMapPos.Y = _model.ModelPos.Y;
    
    _positionForTalk.X = _currentMapPos.X;
    _positionForTalk.Y = _currentMapPos.Y;
    
    float angleStart = 0.0f, angleEnd = 0.0f;
    
    while (_road.Count != 0)
    {
      angleStart = _model.transform.rotation.eulerAngles.y;
      angleEnd = GetAngleToRotate(_road[0].Coordinate);
      
      //Debug.Log("dx, dy " + dx + " " + dy + " " + road[0].Coordinate);
      //Debug.Log("Rotating from " + angleStart + " to " + angleEnd);
      
      if ((int)angleStart != (int)angleEnd)
      {
        _rotateJob = JobManager.Instance.CreateJob(RotateModel(angleEnd));
        
        while (!_rotateDone)
        {
          yield return null;
        }
        
        _firstStepSound = false;
      }
      
      _stepJob = JobManager.Instance.CreateJob(MoveModel(_road[0].Coordinate));
      
      while (!_moveDone)
      {
        yield return null;
      }
      
      _road.RemoveAt(0);
              
      // If player comes into range after actor made a step, we exit the coroutine
      if (IsPlayerInRange())
      {
        break;
      }

      yield return null;
    }
    
    _model.AnimationComponent.Play(GlobalConstants.AnimationIdleName);

    _working = false;

    yield return null;
  }

  bool _rotateDone = false;
  IEnumerator RotateModel(float angle)
  {
    _model.AnimationComponent.Play(GlobalConstants.AnimationIdleName);
    
    _rotateDone = false;
    
    Vector3 tmpRotation = _model.transform.rotation.eulerAngles;
    
    int d = (int)angle - (int)tmpRotation.y;
    
    // If model has 270 rotation around y and has to rotate to 0, 
    // we should rotate towards shortest direction, not from 270 to 0 backwards, 
    // so we introduce special condition.
    // Same thing when angles are reversed, because we rely on sign variable in tmpRotation.y change.
    //
    // TLDR:
    // Case 1: from = 270, to = 0, we have sign = -1, so we would decrement current rotation from 270 to 0, instead of just going to 0 (i.e. 360).
    // Case 2: from = 0, to = 270, we have sign = +1, so we would increment current rotation from 0 to 270 - same thing reversed.
    
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
  
  bool _firstStepSound = false;
  bool _moveDone = false;
  Int2 _currentMapPos = new Int2();
  Int2 _positionForTalk = new Int2();
  RaycastHit _raycastHit;
  IEnumerator MoveModel(Int2 newMapPos)
  {
    _model.AnimationComponent.Play(GlobalConstants.AnimationWalkName);
    
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

  public void KillAllJobs()
  {
    if (_roadBuilder.ProcessRoutine != null) _roadBuilder.ProcessRoutine.KillJob();
    if (_rotateJob != null) _rotateJob.KillJob();
    if (_stepJob != null) _stepJob.KillJob();
    if (_mainJob != null) _mainJob.KillJob();    
  }
}
