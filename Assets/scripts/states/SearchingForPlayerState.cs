using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SearchingForPlayerState : GameObjectState 
{
  Vector3 _modelPosition = Vector3.zero;

  Job _moveJob, _rotateJob, _stepJob;

  RoadBuilder _roadBuilder;

  ModelMover _model;

  Int2 _currentMapPos = new Int2();

  public SearchingForPlayerState(ActorBase actor)
  {
    _actor = actor;
    _model = actor.Model;
    _roadBuilder = new RoadBuilder(App.Instance.GeneratedMap.Map, App.Instance.GeneratedMapWidth, App.Instance.GeneratedMapHeight);

    _modelPosition.x = _model.ModelPos.X * GlobalConstants.WallScaleFactor;
    _modelPosition.y = _model.transform.position.y;
    _modelPosition.z = _model.ModelPos.Y * GlobalConstants.WallScaleFactor;

    _model.AnimationComponent.Play(GlobalConstants.AnimationIdleName);
  }

  Int2 _oldPlayerPos = new Int2();

  bool _running = false, _firstStepSound = false, _moveDone = false;
  float _time = 0.0f;
  GeneratedMapCell _playerCell;
  public override void Run()
  {    
    _time += Time.smoothDeltaTime;

    if (_time > GlobalConstants.SearchingForPlayerRate)
    {
      _time = 0.0f;

      _playerCell = App.Instance.GeneratedMap.GetMapCellByPosition(InputController.Instance.PlayerMapPos);
      if (_playerCell.CellType != GeneratedCellType.ROOM)
      {
        bool playerInRange = IsPlayerInRange();

        //Debug.Log(string.Format("Searching for player {0} {1} - distance {2}", _model.ModelPos, InputController.Instance.PlayerMapPos, d));

        if (!_running && playerInRange)
        {
          //Debug.Log("Player Found!");

          _oldPlayerPos.X = InputController.Instance.PlayerMapPos.X;
          _oldPlayerPos.Y = InputController.Instance.PlayerMapPos.Y;

          _running = true;
          _moveJob = JobManager.Instance.CreateJob(ApproachPlayerRoutine());
        }
      }
    }

    _model.transform.position = _modelPosition;
  }

  bool IsPlayerInRange()
  {
    Int2 pos = _model.ModelPos;

    int lx = Mathf.Clamp(pos.X - (_actor as EnemyActor).AgroRange, 0, App.Instance.GeneratedMapHeight - 1);
    int ly = Mathf.Clamp(pos.Y - (_actor as EnemyActor).AgroRange, 0, App.Instance.GeneratedMapWidth - 1);
    int hx = Mathf.Clamp(pos.X + (_actor as EnemyActor).AgroRange, 0, App.Instance.GeneratedMapHeight - 1);
    int hy = Mathf.Clamp(pos.Y + (_actor as EnemyActor).AgroRange, 0, App.Instance.GeneratedMapWidth - 1);

    if (InputController.Instance.PlayerMapPos.X <= hx && InputController.Instance.PlayerMapPos.X >= lx 
     && InputController.Instance.PlayerMapPos.Y <= hy && InputController.Instance.PlayerMapPos.Y >= ly)
    {
      return true;
    }

    return false;
  }

  List<RoadBuilder.PathNode> _road;
  IEnumerator ApproachPlayerRoutine()
  {
    _modelPosition.x = _model.ModelPos.X * GlobalConstants.WallScaleFactor;
    _modelPosition.y = _model.transform.position.y;
    _modelPosition.z = _model.ModelPos.Y * GlobalConstants.WallScaleFactor;

    _roadBuilder.BuildRoadAsync(_actor.Model.ModelPos, InputController.Instance.PlayerMapPos, true);
    
    while ((_road = _roadBuilder.GetResult()) == null)
    {
      //Debug.Log("Waiting for result");
      yield return null;
    }    

    _currentMapPos.X = _model.ModelPos.X;
    _currentMapPos.Y = _model.ModelPos.Y;

    float angleStart = 0.0f, angleEnd = 0.0f;

    while (_road.Count > 1)
    {
      //Debug.Log("Approaching player");

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

      bool playerInRange = IsPlayerInRange();
      if (IsPlayerPositionChanged() && _playerCell.CellType != GeneratedCellType.ROOM && playerInRange)
      { 
        break;
      }

      yield return null;
    }

    _model.AnimationComponent.Play(GlobalConstants.AnimationIdleName);

    if (BlockDistance(_model.ModelPos, InputController.Instance.PlayerMapPos) == 1)
    {
      angleStart = _model.transform.rotation.eulerAngles.y;
      angleEnd = GetAngleToRotate(InputController.Instance.PlayerMapPos);

      if ((int)angleStart != (int)angleEnd)
      {
        _rotateJob = JobManager.Instance.CreateJob(RotateModel(angleEnd));
        
        while (!_rotateDone)
        {
          yield return null;
        }
      }
    }

    //Debug.Log("Approached player");

    _running = false;

    yield return null;
  }

  float GetAngleToRotate(Int2 cellToLook)
  {
    int dx = cellToLook.X - _currentMapPos.X;
    int dy = cellToLook.Y - _currentMapPos.Y;
    
    float angleStart = _model.transform.rotation.eulerAngles.y;
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

  int BlockDistance(Int2 from, Int2 to)
  {
    return Mathf.Abs(to.Y - from.Y) + Mathf.Abs(to.X - from.X);
  }

  bool IsPlayerPositionChanged()
  {
    if (InputController.Instance.PlayerMapPos.X != _oldPlayerPos.X || InputController.Instance.PlayerMapPos.Y != _oldPlayerPos.Y)
    {
      _oldPlayerPos.X = InputController.Instance.PlayerMapPos.X;
      _oldPlayerPos.Y = InputController.Instance.PlayerMapPos.Y;

      return true;
    }

    return false;
  }
}
