using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelMover : MonoBehaviour 
{
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
      JobManager.Instance.CreateJob(PlayRandomAnimation());
      _working = true;
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
