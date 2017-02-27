﻿using UnityEngine;
using System.Collections;

public class ButtonWorldObject : WorldObject
{
  Animation _animation;

  bool _lockInteraction = false;

  string _animationName = "Action";

  float _animationSpeed = 4;

  public WorldObject ControlledObject;

  public ButtonWorldObject(string className, string prefabName, BehaviourWorldObject bmo)
  {
    BMO = bmo;

    _animation = BMO.GetComponentInParent<Animation>();
    if (_animation != null)
    {
      _animation[_animationName].time = 0;
      _animation[_animationName].speed = _animationSpeed;    
    }
  }

  public override void ActionHandler(object sender)
  {
    if (_animation != null && !_lockInteraction)
    {
      BMO.StartSound.Play();

      Job _job = new Job(ButtonToggleRoutine());

      _lockInteraction = true;
    }
  }

  IEnumerator ButtonToggleRoutine()
  {    
    _animation.Play(_animationName);

    while (_animation.IsPlaying(_animationName))
    {
      yield return null;
    }

    if (ControlledObject != null)
    {
      if (ControlledObject.ControlCallback != null)
        ControlledObject.ControlCallback(this);
    }

    _lockInteraction = false;
  }
}
