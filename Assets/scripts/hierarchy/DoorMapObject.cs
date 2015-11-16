using UnityEngine;
using System.Collections;

public class DoorMapObject : MapObject
{
  bool _lockInteraction = false;

  float _animationOpenSpeed = 2;
  float _animationCloseSpeed = 3;

  string _animationName = "Open";

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
      _animation[_animationName].time = 0;
      _animation[_animationName].speed = _animationOpenSpeed;
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

    if (IsOpen)
    {
      _animation[_animationName].time = _animation[_animationName].length;
      _animation[_animationName].speed = -_animationCloseSpeed;
    }
    else
    {
      _animation[_animationName].time = 0;
      _animation[_animationName].speed = _animationOpenSpeed;
    }

    _animation.Play(_animationName);

    // Set animation type in Animation component to "Always animate"

    while (_animation.IsPlaying(_animationName))    
    {      
      yield return null;
    }

    _lockInteraction = false;    
    IsOpen = !IsOpen;

    IsBeingInteracted = false;
  }
}

