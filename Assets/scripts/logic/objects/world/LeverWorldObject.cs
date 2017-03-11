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
      if (BWO.OnStateBeginSound != null)
        BWO.OnStateBeginSound.Play();

      Job _job = new Job(LeverToggleRoutine());

      _lockInteraction = true;
    }

    if (_animation == null)
    {
      Debug.LogWarning("No animation on " + this + " at " + ArrayCoordinates);
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

    ProcessControlledObjectCallback();
  }

  public void InitBWO()
  {    
    _animation = BWO.GetComponentInChildren<Animation>();
  }

  void ProcessControlledObjectCallback()
  {
    // FIXME: probably dirtyish hack (hard-coded door type of controller object)
    //
    // Door should react to control only when their "on" states are different:
    // e.g. lever is up - door closed. We pull the lever down, it becomes "on", 
    // while door is still "off", so we execute callback and door also becomes "on".
    // Thus if we open a door from one side and then flip lever on the other side,
    // it won't do anything.

    // ControlledObject is assigned in PlaceWorldObject, but theoretically you can
    // assign ActionCompleteCallback to invoke something else:
    // e.g. (wo as DoorWorldObject).ActionCompleteCallback += Something;
    if (ControlledObject != null)
    {
      if (ControlledObject is DoorWorldObject)
      { 
        // Sliding door can be "toggled" when door is playing its animation, but when it's done
        // lever toggles corresponding animation only according to its state
        if ((ControlledObject as DoorWorldObject).IsSliding)    
        {
          if ((ControlledObject as DoorWorldObject).IsAnimationPlaying)              
          {
            if (ActionCompleteCallback != null)
              ActionCompleteCallback(this);
          }
          else if (!(ControlledObject as DoorWorldObject).IsAnimationPlaying 
            && (ControlledObject as DoorWorldObject).IsOpen != _isOn)
          {
            if (ActionCompleteCallback != null)
              ActionCompleteCallback(this);
          }
        }
        else
        {
          // If door is a simple swing type, just toggle it according to lever's current state
          if ((ControlledObject as DoorWorldObject).IsOpen != _isOn)
          {
            if (ActionCompleteCallback != null)
              ActionCompleteCallback(this);
          }
        }
      }
    }
    else
    {
      if (ActionCompleteCallback != null)
        ActionCompleteCallback(this);
    }
  }
}
