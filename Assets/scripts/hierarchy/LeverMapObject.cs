using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeverMapObject : MapObject 
{
  bool _lockInteraction = false;
  bool _isOn = false;

  string _animationName = "On";

  float _animationSpeed = 4;

  Animation _animation;

  public LeverMapObject(string className, string id, BehaviourMapObject bmo)
  {    
    ClassName = className;
    Id = id;
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
    if (_isOn)
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

    if (ActionCompleteCallback != null)
      ActionCompleteCallback(this);
  }

  public override void ActionCompleteHandler(object sender)
  {
    _lockInteraction = false;

    _isOn = !_isOn;
  }
}
