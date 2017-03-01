using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SearchingForPlayerState : GameObjectState 
{
  bool _working = false;

  int[,] _visitedCells;

  RoadBuilder _roadBuilder;
  ModelMover _model;
  public SearchingForPlayerState(ActorBase actor)
  {
    _actor = actor;
    _model = _actor.Model;
    _working = false;

    _visitedCells = new int[LevelLoader.Instance.LevelSize.X, LevelLoader.Instance.LevelSize.Z];

    _modelPosition.x = _model.ModelPos.X * GlobalConstants.WallScaleFactor;
    _modelPosition.y = _model.transform.position.y;
    _modelPosition.z = _model.ModelPos.Y * GlobalConstants.WallScaleFactor;

    //_roadBuilder = new RoadBuilder(_actor.AppRef.GeneratedMap.PathfindingMap, _actor.AppRef.GeneratedMapWidth, _actor.AppRef.GeneratedMapHeight);
  }

  public override void ResetState()
  {
    _working = false;

    for (int x = 0; x < LevelLoader.Instance.LevelSize.X; x++)
    {
      for (int y = 0; y < LevelLoader.Instance.LevelSize.Z; y++)
      {
        _visitedCells[x, y] = 0;
      }
    }
  }

  Job _mainJob, _stepJob, _rotateJob, _delayJob; 

  //GeneratedMapCell _playerCell;
  public override void Run()
  {
    if (!_working)
    {
      _working = true;
      _mainJob = new Job(WanderRoutine());
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
        _model.AnimationComponent.Play(GlobalConstants.AnimationIdleName);

        _rotateJob = new Job(RotateModel(angleEnd));

        while (!_rotateDone)
        {
          yield return null;
        }        
      }

      if (!_model.AnimationComponent.IsPlaying(GlobalConstants.AnimationWalkName))
      {
        _model.AnimationComponent.Play(GlobalConstants.AnimationWalkName);
      }

      _stepJob = new Job(MoveModel(_cellsAround[index]));

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

    Int2 pos = _model.ModelPos;

    for (int i = 0; i < 4; i++)
    {
      int x = pos.X + _direction[i, 0];
      int y = pos.Y + _direction[i, 1];

      Int2 cellCoord = new Int2(x, y);

      if (x < 0 || x > LevelLoader.Instance.LevelSize.X - 1 || y < 0 || y > LevelLoader.Instance.LevelSize.Z - 1)
      {
        continue;
      }

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

    // FIXME: fix!

    /*
    if (!_actor.AppRef.GeneratedMap.PathfindingMap[nextCellX, nextCellY].Walkable 
      || _actor.AppRef.GeneratedMap.PathfindingMap[modelX, modelY].SidesWalkability[orientation] == false
      || _actor.AppRef.GeneratedMap.PathfindingMap[nextCellX, nextCellY].SidesWalkability[oppositeOrientation] == false)
    {
      return false;
    }
    */

    return true;
  }

  List<RoadBuilder.PathNode> _road, _roadToPlayer;
  IEnumerator MoveOnPath()
  {
    _firstStepSound = false;
    
    _modelPosition.x = _model.ModelPos.X * GlobalConstants.WallScaleFactor;
    _modelPosition.y = _model.transform.position.y;
    _modelPosition.z = _model.ModelPos.Y * GlobalConstants.WallScaleFactor;
    
    //Int2 destination = _actor.AppRef.GeneratedMap.GetRandomUnoccupiedCell();
    Int2 destination = new Int2();

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
        _rotateJob = new Job(RotateModel(angleEnd));
        
        while (!_rotateDone)
        {
          yield return null;
        }
        
        _firstStepSound = false;
      }
      
      _stepJob = new Job(MoveModel(_road[0].Coordinate));
      
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

    _delayJob = new Job(DelayRoutine(() => { _working = false; }));

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
    /*
    //PathfindingCell cell = _actor.AppRef.GeneratedMap.PathfindingMap[coords.X, coords.Y];
    PathfindingCell cell = new PathfindingCell();

    string output = string.Format("{0} -> walkable: {1} sides: ", coords, cell.Walkable);
    foreach (var item in cell.SidesWalkability)
    {
      output += string.Format("| {0} {1} | ", item.Key, item.Value);
    }

    output += "\n";

    Debug.Log(output);
    */
  }
}
