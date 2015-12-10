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
    RoadBuilder rb = new RoadBuilder(App.Instance.GeneratedMap.Map, App.Instance.GeneratedMapWidth, App.Instance.GeneratedMapHeight);

    Int2 destination = App.Instance.GeneratedMap.GetRandomUnoccupiedCell();

    Debug.Log("Going from " + ModelPos + " to " + destination);

    var road = rb.BuildRoad(ModelPos, destination, true);

    _modelPosition.x = transform.position.x;
    _modelPosition.y = transform.position.y;
    _modelPosition.z = transform.position.z;

    _animationComponent.CrossFade("Walk");
        
    float delay = 0.0f;
    while (road.Count != 0)
    {
      delay += Time.smoothDeltaTime;

      if (delay > _moveSpeed)
      {
        delay = 0.0f;

        _modelPosition.x = road[0].Coordinate.X * GlobalConstants.WallScaleFactor;
        _modelPosition.z = road[0].Coordinate.Y * GlobalConstants.WallScaleFactor;

        ModelPos.X = road[0].Coordinate.X;
        ModelPos.Y = road[0].Coordinate.Y;

        road.RemoveAt(0);
      }

      yield return null;
    }    

    _animationComponent.CrossFade("Idle");

    JobManager.Instance.CreateJob(DelayRoutine());
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
