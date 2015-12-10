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
      _moveSpeed = length / speed;
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

    Debug.Log("Going from " + ModelPos + " to " + destination);

    var road = rb.BuildRoad(ModelPos, destination, true);

    int dx = road[0].Coordinate.X - ModelPos.X;
    int dy = road[0].Coordinate.Y - ModelPos.Y;

    float angleStart = transform.rotation.eulerAngles.y;
    float angleEnd = 0.0f;

    if (dy == 1 && dx == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.EAST];
    else if (dy == -1 && dx == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.WEST];
    else if (dx == 1 && dy == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.SOUTH];
    else if (dx == -1 && dy == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.NORTH];

    Debug.Log("dx, dy " + dx + " " + dy + " " + road[0].Coordinate);
    Debug.Log("Rotating from " + angleStart + " to " + angleEnd);

    Vector3 tmpRotation = transform.rotation.eulerAngles;

    float counter = 0.0f;
    while (counter < angleEnd)
    {
      counter += Time.smoothDeltaTime * GlobalConstants.CharacterRotationSpeed;
      tmpRotation.y += Time.smoothDeltaTime * GlobalConstants.CharacterRotationSpeed;

      transform.rotation = Quaternion.Euler(tmpRotation);      

      yield return null;
    }

    tmpRotation.y = angleEnd;
    transform.rotation = Quaternion.Euler(tmpRotation);      

    /*
    RoadBuilder rb = new RoadBuilder(App.Instance.GeneratedMap.Map, App.Instance.GeneratedMapWidth, App.Instance.GeneratedMapHeight);

    Int2 destination = App.Instance.GeneratedMap.GetRandomUnoccupiedCell();

    Debug.Log("Going from " + ModelPos + " to " + destination);

    var road = rb.BuildRoad(ModelPos, destination, true);
        
    _modelPosition.x = transform.position.x;
    _modelPosition.y = transform.position.y;
    _modelPosition.z = transform.position.z;

    _animationComponent.CrossFade("Walk");

    int nextPathCellIndex = 0;
        
    while (ModelPos.X != destination.X && ModelPos.Y != destination.Y)
    {
      int dx = road[nextPathCellIndex].Coordinate.X - ModelPos.X;
      int dy = road[nextPathCellIndex].Coordinate.Y - ModelPos.Y;

      _modelPosition.x += dx * (Time.smoothDeltaTime * _moveSpeed);
      _modelPosition.z += dy * (Time.smoothDeltaTime * _moveSpeed);

      ModelPos.X = Mathf.FloorToInt(_modelPosition.x / GlobalConstants.WallScaleFactor);
      ModelPos.Y = Mathf.FloorToInt(_modelPosition.z / GlobalConstants.WallScaleFactor);

      if (dx == 0 && dy == 0)
      {
        _modelPosition.x = ModelPos.X * GlobalConstants.WallScaleFactor;
        _modelPosition.z = ModelPos.Y * GlobalConstants.WallScaleFactor;

        nextPathCellIndex++;
      }

      yield return null;
    }

    _animationComponent.CrossFade("Idle");

    JobManager.Instance.CreateJob(DelayRoutine());
    */
  }

  float _delay = 5.0f;
  IEnumerator DelayRoutine()
  {
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
