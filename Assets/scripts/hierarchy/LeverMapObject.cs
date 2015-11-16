using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeverMapObject : MapObject 
{
  bool _lockInteraction = false;

  string _animationName = "On";

  float _animationSpeed = 4;

  Animation _animation;

  public MapObject ControlledObject;

  public LeverMapObject(string className, string prefabName, BehaviourMapObject bmo)
  {    
    ClassName = className;
    PrefabName = prefabName;
    BMO = bmo;

    _animation = BMO.GetComponent<Animation>();    
  }
    
  public override void ActionHandler(object sender)
  {
    if (_animation != null && !_lockInteraction)
    {
      BMO.StartSound.Play();

      Job _job = new Job(LeverToggleRoutine());

      _lockInteraction = true;
    }
  }

  IEnumerator LeverToggleRoutine()
  {    
    if (IsOpen)
    {
      _animation[_animationName].time = _animation[_animationName].length;
      _animation[_animationName].speed = -_animationSpeed;
    }
    else
    {
      _animation[_animationName].time = 0;
      _animation[_animationName].speed = _animationSpeed;
    }

    _animation.Play(_animationName);

    while (_animation.IsPlaying(_animationName))
    {
      yield return null;
    }

    if (ControlledObject != null)
    {
      if (ControlledObject.ControlCallback != null && IsOpen == ControlledObject.IsOpen)
        ControlledObject.ControlCallback(this);
    }

    _lockInteraction = false;    
    IsOpen = !IsOpen;
  }
}
