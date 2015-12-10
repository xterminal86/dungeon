﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelMover : MonoBehaviour 
{
  public Int2 ModelStartingPos = new Int2();

  Dictionary<int, string> _animationsByIndex = new Dictionary<int, string>()
  {
    { 0, "Idle" },
    { 1, "Attack" },
    { 2, "Walk" },
    { 3, "Talk" },
    { 4, "Clueless" }
  };

  Animation _animationComponent;
  void Start () 
	{
    _animationComponent = GetComponent<Animation>();
    if (_animationComponent != null)
    {
      _animationComponent["Idle"].speed = 0.5f;
    }
	}
		
  bool _working = false;
  void Update () 
	{
    if (!_working)
    {
      //JobManager.Instance.CreateJob(PlayRandomAnimation());
      JobManager.Instance.CreateJob(FindPath());
      _working = true;
    }
	}

  IEnumerator FindPath()
  {
    RoadBuilder rb = new RoadBuilder(App.Instance.GeneratedMap.Map, App.Instance.GeneratedMapWidth, App.Instance.GeneratedMapHeight);

    Int2 destination = App.Instance.GeneratedMap.GetRandomUnoccupiedCell();

    Debug.Log("Going from " + ModelStartingPos + " to " + destination);

    var road = rb.BuildRoad(ModelStartingPos, destination, true);

    Vector3 tmpPos = Vector3.zero;

    tmpPos.x = transform.position.x;
    tmpPos.y = transform.position.y;
    tmpPos.z = transform.position.z;

    float delay = 0.0f;
    while (road.Count != 0)
    {
      delay += Time.smoothDeltaTime;

      if (delay > 1.0f)
      {
        delay = 0.0f;

        tmpPos.x = road[0].Coordinate.X * GlobalConstants.WallScaleFactor;
        tmpPos.z = road[0].Coordinate.Y * GlobalConstants.WallScaleFactor;

        transform.position = tmpPos;

        road.RemoveAt(0);
      }

      yield return null;
    }
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
