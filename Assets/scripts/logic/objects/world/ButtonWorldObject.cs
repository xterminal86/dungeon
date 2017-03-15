using UnityEngine;
using System.Collections;

public class ButtonWorldObject : WorldObject
{
  Animation _animation;

  bool _lockInteraction = false;

  string _animationName = "Action";

  float _animationSpeed = 4;

  public WorldObject[] ControlledObjects;

  public ButtonWorldObject(string inGameName, string prefabName) : base(inGameName, prefabName)
  {    
    
  }

  public override void ActionHandler(object sender)
  {
    if (_animation != null && !_lockInteraction)
    {
      if (BWO.OnStateBeginSound != null)
        BWO.OnStateBeginSound.Play();

      Job _job = new Job(ButtonToggleRoutine());

      _lockInteraction = true;
    }

    if (_animation == null)
    {
      Debug.LogWarning("No animation on " + this + " at " + ArrayCoordinates);
    }
  }

  IEnumerator ButtonToggleRoutine()
  {    
    _animation.Play(_animationName);

    while (_animation.IsPlaying(_animationName))
    {
      yield return null;
    }

    _lockInteraction = false;

    if (ActionCompleteCallback != null)
      ActionCompleteCallback(this);    
  }

  public void InitBWO()
  {
    _animation = BWO.GetComponentInChildren<Animation>();
    if (_animation != null)
    {
      _animation[_animationName].time = 0;
      _animation[_animationName].speed = _animationSpeed;    
    }
  }
}
