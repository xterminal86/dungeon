using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Model finds itself a destination point and goes there following Manhattan movement rule.
/// After that there is a delay and the process starts all over again.
/// </summary>
public class WanderingState : GameObjectState
{  
  Vector3 _modelPosition = Vector3.zero;

  ModelMover _model;

  public WanderingState(ActorBase actor)
  {
    _actor = actor;
    _model = actor.Model;
    _working = false;
  }

  Job _mainJob, _stepJob, _rotateJob, _delayJob;

  bool _working = false;
  public override void Run()
  {
    if (!_working)
    {      
      _mainJob = JobManager.Instance.CreateJob(MoveOnPath());
      _working = true;
    }

    _model.transform.position = _modelPosition;
  }

  void PlayFootstepSound3D(Int2 mapPos, Vector3 position3D)
  {
    if (App.Instance.FloorSoundTypeByPosition[mapPos.X, mapPos.Y] != -1)
    {
      SoundManager.Instance.PlayFootstepSound(_model.name, (GlobalConstants.FootstepSoundType)App.Instance.FloorSoundTypeByPosition[mapPos.X, mapPos.Y], position3D);
    }
  }

  List<RoadBuilder.PathNode> _road;
  IEnumerator MoveOnPath()
  {
    _firstStepSound = false;

    _modelPosition.x = _model.ModelPos.X * GlobalConstants.WallScaleFactor;
    _modelPosition.y = _model.transform.position.y;
    _modelPosition.z = _model.ModelPos.Y * GlobalConstants.WallScaleFactor;

    RoadBuilder rb = new RoadBuilder(App.Instance.GeneratedMap.Map, App.Instance.GeneratedMapWidth, App.Instance.GeneratedMapHeight);

    Int2 destination = App.Instance.GeneratedMap.GetRandomUnoccupiedCell();

    //Debug.Log(name + ": going from " + ModelPos + " to " + destination);

    _road = rb.BuildRoad(_model.ModelPos, destination, true);

    _currentMapPos.X = _model.ModelPos.X;
    _currentMapPos.Y = _model.ModelPos.Y;

    _positionForTalk.X = _currentMapPos.X;
    _positionForTalk.Y = _currentMapPos.Y;

    while (_road.Count != 0)
    {
      int dx = _road[0].Coordinate.X - _currentMapPos.X;
      int dy = _road[0].Coordinate.Y - _currentMapPos.Y;

      float angleStart = _model.transform.rotation.eulerAngles.y;
      float angleEnd = 0.0f;

      if (dy == 1 && dx == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.EAST];
      else if (dy == -1 && dx == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.WEST];
      else if (dx == 1 && dy == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.SOUTH];
      else if (dx == -1 && dy == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.NORTH];

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

      yield return null;
    }

    _model.AnimationComponent.Play(GlobalConstants.AnimationIdleName);

    _delayJob = JobManager.Instance.CreateJob(DelayRoutine());
    
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

    /*
    if (_model.RaycastPoint != null)
    {
      int dirX = (int)Mathf.Sin(_model.transform.eulerAngles.y * Mathf.Deg2Rad);
      int dirZ = (int)Mathf.Cos(_model.transform.eulerAngles.y * Mathf.Deg2Rad);

      Vector3 raycastDir = new Vector3(_model.RaycastPoint.position.x + dirX, 
                                _model.RaycastPoint.position.y, 
                                _model.RaycastPoint.position.z + dirZ);

      Ray r = new Ray(_model.RaycastPoint.position, raycastDir);
      if (Physics.Raycast(r.origin, r.direction, out _raycastHit))
      {
        if (_raycastHit.collider != null)
        {
          ModelMover mm = _raycastHit.collider.gameObject.GetComponent<ModelMover>();
          if (mm != null)
          {
            _road.Clear();
            _moveDone = true;
            yield break;
          }
        }
      }
    }
    */

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

  // Helper Functions

  void RewindAnimation(string animationName)
  {
    _model.AnimationComponent[animationName].time = 0.0f;
    _model.AnimationComponent.Sample();
    _model.AnimationComponent.Stop(animationName);
  }

  public void KillAllJobs()
  {
    if (_mainJob != null) _mainJob.KillJob();
    if (_stepJob != null) _stepJob.KillJob();
    if (_rotateJob != null) _rotateJob.KillJob();
    if (_delayJob != null) _delayJob.KillJob();
  }

  public void AdjustModelPosition()
  {
    int modelX = InputController.Instance.PlayerMapPos.X;
    int modelZ = InputController.Instance.PlayerMapPos.Y;

    if (App.Instance.CameraOrientation == (int)GlobalConstants.Orientation.EAST)
    {
      modelZ++;
    }
    else if (App.Instance.CameraOrientation == (int)GlobalConstants.Orientation.SOUTH)
    {
      modelX++;
    }
    else if (App.Instance.CameraOrientation == (int)GlobalConstants.Orientation.WEST)
    {
      modelZ--;
    }
    else if (App.Instance.CameraOrientation == (int)GlobalConstants.Orientation.NORTH)
    {
      modelX--;
    }

    _modelPosition.x = modelX * GlobalConstants.WallScaleFactor;
    _modelPosition.z = modelZ * GlobalConstants.WallScaleFactor;

    _model.ModelPos.X = modelX;
    _model.ModelPos.Y = modelZ;

    _model.transform.position = _modelPosition;
  }
}
