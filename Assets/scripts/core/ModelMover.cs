using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelMover : MonoBehaviour 
{
  public Int2 ModelPos = new Int2();

  Dictionary<int, string> _animationsByIndex = new Dictionary<int, string>()
  {
    { 0, "Idle" },
    { 1, "Attack" },
    { 2, "Walk" },
    { 3, "Talk" },
    { 4, "Clueless" }
  };

  float _moveSpeed = 0.0f;

  Animation _animationComponent;
  void Start () 
	{
    _animationComponent = GetComponent<Animation>();
    if (_animationComponent != null)
    {
      _animationComponent["Idle"].speed = 0.5f;

      _animationComponent.Play("Idle");

      float speed = _animationComponent["Walk"].speed;
      float length = _animationComponent["Walk"].length;

      _moveSpeed = (length / speed) * GlobalConstants.WallScaleFactor;

      _animationComponent["Walk"].speed = GlobalConstants.WallScaleFactor;
    }
	}

  Vector3 _modelPosition = Vector3.zero;

  bool _working = false;
  void Update () 
	{
    if (!_working)
    {
      //JobManager.Instance.CreateJob(PlayRandomAnimation());
      JobManager.Instance.CreateJob(MoveOnPath());
      _working = true;
    }

    transform.position = _modelPosition;
	}

  IEnumerator MoveOnPath()
  {    
    _modelPosition.x = ModelPos.X * GlobalConstants.WallScaleFactor;
    _modelPosition.y = transform.position.y;
    _modelPosition.z = ModelPos.Y * GlobalConstants.WallScaleFactor;

    RoadBuilder rb = new RoadBuilder(App.Instance.GeneratedMap.Map, App.Instance.GeneratedMapWidth, App.Instance.GeneratedMapHeight);

    Int2 destination = App.Instance.GeneratedMap.GetRandomUnoccupiedCell();

    //Debug.Log(name + ": going from " + ModelPos + " to " + destination);

    var road = rb.BuildRoad(ModelPos, destination, true);

    _currentMapPos.X = ModelPos.X;
    _currentMapPos.Y = ModelPos.Y;

    while (road.Count != 0)
    {
      int dx = road[0].Coordinate.X - _currentMapPos.X;
      int dy = road[0].Coordinate.Y - _currentMapPos.Y;

      float angleStart = transform.rotation.eulerAngles.y;
      float angleEnd = 0.0f;

      if (dy == 1 && dx == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.EAST];
      else if (dy == -1 && dx == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.WEST];
      else if (dx == 1 && dy == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.SOUTH];
      else if (dx == -1 && dy == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.NORTH];

      //Debug.Log("dx, dy " + dx + " " + dy + " " + road[0].Coordinate);
      //Debug.Log("Rotating from " + angleStart + " to " + angleEnd);

      if ((int)angleStart != (int)angleEnd)
      {
        StartCoroutine("RotateModel", angleEnd);

        while (!_rotateDone)
        {
          yield return null;
        }
      }

      StartCoroutine("MoveModel", road[0].Coordinate);

      while (!_moveDone)
      {
        yield return null;
      }

      road.RemoveAt(0);

      yield return null;
    }

    _animationComponent.CrossFade("Idle");

    yield return null;

    StartCoroutine(DelayRoutine());
  }

  bool _rotateDone = false;
  IEnumerator RotateModel(float angle)
  {
    _animationComponent.CrossFade("Idle");

    _rotateDone = false;

    Vector3 tmpRotation = transform.rotation.eulerAngles;

    int d = (int)angle - (int)tmpRotation.y;

    float counter = 0.0f;
    while (counter < Mathf.Abs(d))
    {
      counter += Time.smoothDeltaTime * GlobalConstants.CharacterRotationSpeed;

      if (d > 0)
      {
        tmpRotation.y += Time.smoothDeltaTime * GlobalConstants.CharacterRotationSpeed;
      }
      else 
      {
        tmpRotation.y -= Time.smoothDeltaTime * GlobalConstants.CharacterRotationSpeed;
      }

      transform.rotation = Quaternion.Euler(tmpRotation);      
      
      yield return null;
    }
    
    tmpRotation.y = angle;
    transform.rotation = Quaternion.Euler(tmpRotation);      

    _rotateDone = true;

    yield return null;
  }

  bool _moveDone = false;
  Int2 _currentMapPos = new Int2();
  IEnumerator MoveModel(Int2 newMapPos)
  {
    _animationComponent.CrossFade("Walk");

    _moveDone = false;

    _currentMapPos.X = (int)_modelPosition.x / GlobalConstants.WallScaleFactor;
    _currentMapPos.Y = (int)_modelPosition.z / GlobalConstants.WallScaleFactor;

    int dx = newMapPos.X - _currentMapPos.X;
    int dy = newMapPos.Y - _currentMapPos.Y;

    while (dx != 0 || dy != 0)
    {
      _modelPosition.x += dx * (Time.smoothDeltaTime * _moveSpeed);
      _modelPosition.z += dy * (Time.smoothDeltaTime * _moveSpeed);

      _currentMapPos.X = dx < 0 ? Mathf.CeilToInt(_modelPosition.x / GlobalConstants.WallScaleFactor) : Mathf.FloorToInt(_modelPosition.x / GlobalConstants.WallScaleFactor);
      _currentMapPos.Y = dy < 0 ? Mathf.CeilToInt(_modelPosition.z / GlobalConstants.WallScaleFactor) : Mathf.FloorToInt(_modelPosition.z / GlobalConstants.WallScaleFactor);

      dx = newMapPos.X - _currentMapPos.X;
      dy = newMapPos.Y - _currentMapPos.Y;

      yield return null;
    }

    _modelPosition.x = newMapPos.X * GlobalConstants.WallScaleFactor;
    _modelPosition.z = newMapPos.Y * GlobalConstants.WallScaleFactor;

    ModelPos.X = newMapPos.X;
    ModelPos.Y = newMapPos.Y;

    _moveDone = true;

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
  }

  float _time = 0.0f;
  IEnumerator PlayRandomAnimation()
  {
    int index = Random.Range(0, _animationComponent.GetClipCount());

    _animationComponent.Play(_animationsByIndex[index]);

    float speed = _animationComponent[_animationsByIndex[index]].speed;
    float length = _animationComponent[_animationsByIndex[index]].length;
    float cond = length / speed;
    while (_time < cond)
    {
      _time += Time.deltaTime;
      yield return null;
    }

    _working = false;
    _time = 0.0f;
  }
}
