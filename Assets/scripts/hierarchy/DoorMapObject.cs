using UnityEngine;
using System.Collections;

public class DoorMapObject : MapObject
{
  bool _lockInteraction = false;

  float _animationOpenSpeed = 2;
  float _animationCloseSpeed = 3;

  Animation _animation;

  Job _job;

  public DoorMapObject (string className, string prefabName, BehaviourMapObject bmo)
  {
    ClassName = className;
    PrefabName = prefabName;
    BMO = bmo;

    _animation = BMO.GetComponentInParent<Animation>();
    if (_animation != null)
    {      
      _animation["Open"].speed = _animationOpenSpeed;
      _animation["Close"].speed = _animationCloseSpeed;
    }
  }

  public override void ActionHandler(object sender)
  {
    if (_animation != null && !_lockInteraction)
    {
      if (!IsOpen)
      {
        BMO.StartSound.Play();
      }
      else
      {
        if (BMO.StartSound.isPlaying)
        {
          BMO.StartSound.Stop();
        }

        BMO.EndSound.Play();
      }

      _job = new Job(DoorToggleRoutine());

      _lockInteraction = true;
    }
  }

  IEnumerator DoorToggleRoutine()
  {
    IsBeingInteracted = true;

    string animationName = "Close";

    if (!IsOpen)
    {
      if (_animation != null)
      {
        _animation[animationName].speed = _animationOpenSpeed;
      }
    }
    else
    {
      if (_animation != null)
      {
        _animation[animationName].speed = -_animationCloseSpeed;
        _animation[animationName].time = _animation[animationName].length;
      }
    }

    _animation.Play(animationName);

    while (_animation.IsPlaying(animationName))
    {
      yield return null;
    }

    _lockInteraction = false;    
    IsOpen = !IsOpen;

    IsBeingInteracted = false;
  }
}

