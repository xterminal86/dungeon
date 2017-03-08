using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeverWorldObject : WorldObject 
{
  bool _lockInteraction = false;

  string _animationName = "On";

  float _animationSpeed = 4;

  Animation _animation;

  public WorldObject ControlledObject;

  bool _isOn = false;

  public LeverWorldObject(string inGameName, string prefabName) : base(inGameName, prefabName)
  { 
  }
    
  public override void ActionHandler(object sender)
  {
    if (_animation != null && !_lockInteraction)
    {
      BWO.StartSound.Play();

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

    _lockInteraction = false;    
    _isOn = !_isOn;

    if (ActionCompleteCallback != null)
      ActionCompleteCallback(this);
  }

  public void InitBWO(BehaviourWorldObject bwo)
  {
    BWO = bwo;

    _animation = BWO.GetComponentInChildren<Animation>();
  }
}
