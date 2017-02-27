using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Model finds itself a destination point and goes there following Manhattan movement rule.
/// After that there is a delay and the process starts all over again.
/// </summary>
public class WanderingState : GameObjectState
{   
  RoadBuilder _roadBuilder;
  public WanderingState(ActorBase actor)
  {
    _actor = actor;
    _working = false;

    //_roadBuilder = new RoadBuilder(_actor.AppRef.GeneratedMap.PathfindingMap, _actor.AppRef.GeneratedMapWidth, _actor.AppRef.GeneratedMapHeight);
        
    _actor.Model.AnimationComponent.Play(GlobalConstants.AnimationIdleName);
  }

  public override void ResetState()
  {
    _working = false;
    _actor.Model.AnimationComponent.Play(GlobalConstants.AnimationIdleName);
  }

  Job _mainJob, _stepJob, _rotateJob, _delayJob;

  bool _working = false;
  public override void Run()
  {
    if (!_working)
    {      
      _working = true;
      _mainJob = new Job(MoveOnPath());
    }

    _actor.Model.transform.position = _modelPosition;
  }

  List<RoadBuilder.PathNode> _road;
  IEnumerator MoveOnPath()
  {
    _firstStepSound = false;

    _modelPosition.x = _actor.Model.ModelPos.X * GlobalConstants.WallScaleFactor;
    _modelPosition.y = _actor.Model.transform.position.y;
    _modelPosition.z = _actor.Model.ModelPos.Y * GlobalConstants.WallScaleFactor;

    //Int2 destination = _actor.AppRef.GeneratedMap.GetRandomUnoccupiedCell();

    Int2 destination = new Int2();

    //Debug.Log(name + ": going from " + ModelPos + " to " + destination);    

    _roadBuilder.BuildRoadAsync(_actor.Model.ModelPos, destination, true);

    while ((_road = _roadBuilder.GetResult()) == null)
    {
      yield return null;
    }

    _currentMapPos.X = _actor.Model.ModelPos.X;
    _currentMapPos.Y = _actor.Model.ModelPos.Y;

    _positionForTalk.X = _currentMapPos.X;
    _positionForTalk.Y = _currentMapPos.Y;

    float angleStart = 0.0f, angleEnd = 0.0f;

    while (_road.Count != 0)
    {
      angleStart = _actor.Model.transform.rotation.eulerAngles.y;
      angleEnd = GetAngleToRotate(_road[0].Coordinate);

      //Debug.Log("dx, dy " + dx + " " + dy + " " + road[0].Coordinate);
      //Debug.Log("Rotating from " + angleStart + " to " + angleEnd);

      if ((int)angleStart != (int)angleEnd)
      {
        _actor.Model.AnimationComponent.CrossFade(GlobalConstants.AnimationIdleName);

        _rotateJob = new Job(RotateModel(angleEnd));

        while (!_rotateDone)
        {
          yield return null;
        }

        _firstStepSound = false;
      }

      if (_actor.AnimationComponent.IsPlaying(GlobalConstants.AnimationThinkingName))
      {
        _actor.Model.AnimationComponent.CrossFade(GlobalConstants.AnimationWalkName);
      }
      else
      {
        _actor.Model.AnimationComponent.Play(GlobalConstants.AnimationWalkName);      
      }

      _stepJob = new Job(MoveModel(_road[0].Coordinate));

      while (!_moveDone)
      {
        yield return null;
      }

      _road.RemoveAt(0);

      yield return null;
    }

    if (Random.Range(0, 2) == 0 && _actor.Model.IsFemale)
    {
      _actor.Model.AnimationComponent.Play(GlobalConstants.AnimationThinkingName);
    }
    else
    {
      _actor.Model.AnimationComponent.CrossFade(GlobalConstants.AnimationIdleName);
    }
        
    _delayJob = new Job(DelayRoutine());
    
    yield return null;
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

  float _delay = 0.0f;
  IEnumerator DelayRoutine()
  {
    _delay = Random.Range(GlobalConstants.WanderingMinDelaySeconds + 1, GlobalConstants.WanderingMaxDelaySeconds + 1);

    float time = 0.0f;

    while (time < _delay)
    {
      time += Time.smoothDeltaTime;

      yield return null;
    }

    _working = false;

    yield return null;
  }
  
  public void KillAllJobs()
  {    
    if (_delayJob != null) _delayJob.KillJob();
    if (_rotateJob != null) _rotateJob.KillJob();
    if (_stepJob != null) _stepJob.KillJob();
    if (_mainJob != null) _mainJob.KillJob();    
  }

  public void AdjustModelPosition()
  {
    int modelX = InputController.Instance.PlayerMapPos.X;
    int modelZ = InputController.Instance.PlayerMapPos.Y;

    if (InputController.Instance.CameraOrientation == GlobalConstants.Orientation.EAST)
    {
      modelZ++;
    }
    else if (InputController.Instance.CameraOrientation == GlobalConstants.Orientation.SOUTH)
    {
      modelX++;
    }
    else if (InputController.Instance.CameraOrientation == GlobalConstants.Orientation.WEST)
    {
      modelZ--;
    }
    else if (InputController.Instance.CameraOrientation == GlobalConstants.Orientation.NORTH)
    {
      modelX--;
    }

    _modelPosition.x = modelX * GlobalConstants.WallScaleFactor;
    _modelPosition.z = modelZ * GlobalConstants.WallScaleFactor;

    _actor.Model.ModelPos.X = modelX;
    _actor.Model.ModelPos.Y = modelZ;

    _actor.Model.transform.position = _modelPosition;
  }
}
