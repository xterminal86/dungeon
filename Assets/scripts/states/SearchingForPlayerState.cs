using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SearchingForPlayerState : GameObjectState 
{
  bool _working = false;
  Vector3 _modelPosition = Vector3.zero;

  int[,] _visitedCells;

  RoadBuilder _roadBuilder;
  ModelMover _model;
  public SearchingForPlayerState(ActorBase actor)
  {
    _actor = actor;
    _model = _actor.Model;
    _working = false;

    _visitedCells = new int[_actor.AppRef.GeneratedMapWidth, _actor.AppRef.GeneratedMapHeight];

    _modelPosition.x = _model.ModelPos.X * GlobalConstants.WallScaleFactor;
    _modelPosition.y = _model.transform.position.y;
    _modelPosition.z = _model.ModelPos.Y * GlobalConstants.WallScaleFactor;

    _roadBuilder = new RoadBuilder(_actor.AppRef.GeneratedMap.PathfindingMap, _actor.AppRef.GeneratedMapWidth, _actor.AppRef.GeneratedMapHeight);
  }

  public override void ResetState()
  {
    _working = false;

    for (int x = 0; x < _actor.AppRef.GeneratedMapWidth; x++)
    {
      for (int y = 0; y < _actor.AppRef.GeneratedMapHeight; y++)
      {
        _visitedCells[x, y] = 0;
      }
    }
  }

  Job _mainJob, _stepJob, _rotateJob, _delayJob; 

  GeneratedMapCell _playerCell;
  public override void Run()
  {
    if (!_working)
    {
      _working = true;
      //_mainJob = JobManager.Instance.CreateCoroutine(MoveOnPath());
      _mainJob = JobManager.Instance.CreateCoroutine(WanderRoutine());
    }

    _model.transform.position = _modelPosition;
  }
  
  IEnumerator WanderRoutine()
  {    
    LookAround();

    float angleStart = 0.0f, angleEnd = 0.0f;

    while (_cellsAround.Count != 0)
    {      
      _visitedCells[_model.ModelPos.X, _model.ModelPos.Y] = 1;

      int index = Random.Range(0, _cellsAround.Count);

      angleStart = _model.transform.rotation.eulerAngles.y;
      angleEnd = GetAngleToRotate(_cellsAround[index]);

      //PrintCellInfo(_cellsAround[index]);

      //Debug.Log(angleStart + " " + angleEnd);

      if ((int)angleStart != (int)angleEnd)
      {
        _rotateJob = JobManager.Instance.CreateCoroutine(RotateModel(angleEnd));

        while (!_rotateDone)
        {
          yield return null;
        }        
      }

      _stepJob = JobManager.Instance.CreateCoroutine(MoveModel(_cellsAround[index]));

      while (!_moveDone)
      {
        yield return null;
      }

      LookAround();

      yield return null;
    }

    _actor.ChangeState(_actor.IdleStateVar);

    yield return null;
  }

  List<Int2> _cellsAround = new List<Int2>();
  sbyte[,] _direction = new sbyte[4, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
  void LookAround()
  {
    _cellsAround.Clear();

    var map = _actor.AppRef.GeneratedMap.PathfindingMap;

    Int2 pos = _model.ModelPos;

    for (int i = 0; i < 4; i++)
    {
      int x = pos.X + _direction[i, 0];
      int y = pos.Y + _direction[i, 1];

      Int2 cellCoord = new Int2(x, y);

      if (x < 0 || x > _actor.AppRef.GeneratedMapWidth - 1 || y < 0 || y > _actor.AppRef.GeneratedMapHeight - 1)
      {
        continue;
      }

      //CanMove(cellCoord);

      if (CanMove(cellCoord) && _visitedCells[x, y] != 1)
      {
        _cellsAround.Add(cellCoord);
      }
    }
  }

  // Model may be facing in different direction than randomly chosen cell,
  // so CanMove() was tweaked accordingly.
  bool CanMove(Int2 nextCellCoord)
  {    
    int nextCellX = nextCellCoord.X;
    int nextCellY = nextCellCoord.Y;

    int modelX = _model.ModelPos.X;
    int modelY = _model.ModelPos.Y;

    GlobalConstants.Orientation orientation = 0;
    GlobalConstants.Orientation oppositeOrientation = 0;

    if (modelX < nextCellX)
    {
      // Going SOUTH

      orientation = GlobalConstants.Orientation.SOUTH;
      oppositeOrientation = GlobalConstants.Orientation.NORTH;
    }
    else if (modelX > nextCellX)
    {
      // Going NORTH

      orientation = GlobalConstants.Orientation.NORTH;
      oppositeOrientation = GlobalConstants.Orientation.SOUTH;
    }
    else if (modelY < nextCellY)
    {
      // Going EAST

      orientation = GlobalConstants.Orientation.EAST;
      oppositeOrientation = GlobalConstants.Orientation.WEST;
    }
    else if (modelY > nextCellY)
    {
      // Going WEST

      orientation = GlobalConstants.Orientation.WEST;
      oppositeOrientation = GlobalConstants.Orientation.EAST;
    }

    //string output = string.Format("Model pos: {0} ", _model.ModelPos);

    //output += string.Format("modelFacing: {0} nextCellSide: {1} ", GlobalConstants.OrientationByAngle[(int)_model.transform.eulerAngles.y], GlobalConstants.OppositeOrientationByAngle[(int)_model.transform.eulerAngles.y]);

    //Debug.Log(output);
    //PrintCellInfo(nextCellCoord);

    if (!_actor.AppRef.GeneratedMap.PathfindingMap[nextCellX, nextCellY].Walkable 
      || _actor.AppRef.GeneratedMap.PathfindingMap[modelX, modelY].SidesWalkability[orientation] == false
      || _actor.AppRef.GeneratedMap.PathfindingMap[nextCellX, nextCellY].SidesWalkability[oppositeOrientation] == false)
    {
      return false;
    }

    return true;
  }

  List<RoadBuilder.PathNode> _road, _roadToPlayer;
  IEnumerator MoveOnPath()
  {
    _firstStepSound = false;
    
    _modelPosition.x = _model.ModelPos.X * GlobalConstants.WallScaleFactor;
    _modelPosition.y = _model.transform.position.y;
    _modelPosition.z = _model.ModelPos.Y * GlobalConstants.WallScaleFactor;
    
    Int2 destination = _actor.AppRef.GeneratedMap.GetRandomUnoccupiedCell();
    
    //Debug.Log(name + ": going from " + ModelPos + " to " + destination);    

    _roadBuilder.BuildRoadAsync(_model.ModelPos, destination, true);    

    while ((_road = _roadBuilder.GetResult()) == null)
    {      
      // If player comes into range, while actor is still building his path,
      // we stop all activity and exit coroutine

      if (IsPlayerInRange())
      {
        //_roadBuilder.ProcessRoutine.KillJob();
        _roadBuilder.AbortThread();
        _actor.ChangeState(new ApproachingPlayerState(_actor));
        yield break;
      }

      yield return null;
    }

    float angleStart = 0.0f, angleEnd = 0.0f;
    
    while (_road.Count != 0)
    {
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
      
      _stepJob = JobManager.Instance.CreateCoroutine(MoveModel(_road[0].Coordinate));
      
      while (!_moveDone)
      {
        yield return null;
      }
      
      _road.RemoveAt(0);
              
      // If player comes into range after actor made a step, 
      // we go to the "approach" mode
      if (IsPlayerInRange())
      {
        _actor.ChangeState(new ApproachingPlayerState(_actor));
        yield break;
      }

      yield return null;
    }

    _model.AnimationComponent.Play(GlobalConstants.AnimationIdleName);

    _delayJob = JobManager.Instance.CreateCoroutine(DelayRoutine(() => { _working = false; }));

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
  RaycastHit _raycastHit;
  IEnumerator MoveModel(Int2 newMapPos)
  {
    if (!_model.AnimationComponent.IsPlaying(GlobalConstants.AnimationWalkName))
    {
      _model.AnimationComponent.Play(GlobalConstants.AnimationWalkName);
    }

    _moveDone = false;
    
    int dx = newMapPos.X - _model.ModelPos.X;
    int dy = newMapPos.Y - _model.ModelPos.Y;

    if (!_firstStepSound)
    {
      PlayFootstepSound3D(_model.ModelPos, _modelPosition);
      _firstStepSound = true;
    }
    
    while (dx != 0 || dy != 0)
    {
      _modelPosition.x += dx * (Time.smoothDeltaTime * _model.WalkingSpeed);
      _modelPosition.z += dy * (Time.smoothDeltaTime * _model.WalkingSpeed);
      
      _model.ModelPos.X = dx < 0 ? Mathf.CeilToInt(_modelPosition.x / GlobalConstants.WallScaleFactor) : Mathf.FloorToInt(_modelPosition.x / GlobalConstants.WallScaleFactor);
      _model.ModelPos.Y = dy < 0 ? Mathf.CeilToInt(_modelPosition.z / GlobalConstants.WallScaleFactor) : Mathf.FloorToInt(_modelPosition.z / GlobalConstants.WallScaleFactor);
      
      dx = newMapPos.X - _model.ModelPos.X;
      dy = newMapPos.Y - _model.ModelPos.Y;      
      
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

  float _delay = 0.0f;
  IEnumerator DelayRoutine(Callback cb = null)
  {
    _delay = Random.Range(GlobalConstants.WanderingMinDelaySeconds + 1, GlobalConstants.WanderingMaxDelaySeconds + 1);

    float time = 0.0f;

    while (time < _delay)
    {
      time += Time.smoothDeltaTime;

      /*
      if (IsPlayerInRange())
      {
        _actor.ChangeState(new ApproachingPlayerState(_actor));
        yield break;
      }
      */

      yield return null;
    }

    if (cb != null)
      cb();
    
    yield return null;
  }

  float GetAngleToRotate(Int2 cellToLook)
  {
    int dx = cellToLook.X - _model.ModelPos.X;
    int dy = cellToLook.Y - _model.ModelPos.Y;
    
    float angleEnd = 0.0f;
    
    if (dy == 1 && dx == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.EAST];
    else if (dy == -1 && dx == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.WEST];
    else if (dx == 1 && dy == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.SOUTH];
    else if (dx == -1 && dy == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.NORTH];
    
    return angleEnd;
  }

  void PrintCellInfo(Int2 coords)
  {    
    PathfindingCell cell = _actor.AppRef.GeneratedMap.PathfindingMap[coords.X, coords.Y];

    string output = string.Format("{0} -> walkable: {1} sides: ", coords, cell.Walkable);
    foreach (var item in cell.SidesWalkability)
    {
      output += string.Format("| {0} {1} | ", item.Key, item.Value);
    }

    output += "\n";

    Debug.Log(output);
  }
}
