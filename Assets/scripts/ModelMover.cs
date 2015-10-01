using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelMover : MonoBehaviour 
{
  Dictionary<int, string> _animationsByIndex = new Dictionary<int, string>()
  {
    { 0, "Idle" },
    { 1, "Attack" },
    { 2, "Walk" }
  };

  Animation _animationComponent;
  void Start () 
	{
    _animationComponent = GetComponent<Animation>();
    if (_animationComponent != null)
    {
      _animationComponent["Idle"].speed = 0.5f;
      
      _animationComponent.Play("Idle");
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
    int index = Random.Range(0, 3);

    _animationComponent.Play(_animationsByIndex[index]);

    while (_time < _animationComponent[_animationsByIndex[index]].length)
    {
      _time += Time.deltaTime;
      yield return null;
    }

    _working = false;
    _time = 0.0f;
  }
}
