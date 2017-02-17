using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSlidingMapObject : MapObject 
{
  bool _lockInteraction = false;

  public float AnimationOpenSpeed = 1.0f;
  public float AnimationCloseSpeed = 1.0f;  

  string _animationName = "Open";

  Animation _animation;

  Job _job;

  public DoorSlidingMapObject(string className, string prefabName, BehaviourMapObject bmo, App appRef)
  {
    _appRef = appRef;

    ClassName = className;
    PrefabName = prefabName;
    BMO = bmo;

    _animation = BMO.GetComponentInParent<Animation>();
    if (_animation != null)
    {      
      _animation[_animationName].time = 0;
      _animation[_animationName].speed = AnimationOpenSpeed;
    }
  }

  public override void ActionHandler(object sender)
  {
    if (_animation != null && !_lockInteraction)
    {
      BMO.StartSound.Play();

      _job = new Job(DoorToggleRoutine());

      _lockInteraction = true;
    }
  }

  public override void ActionCompleteHandler(object sender)
  {
    int x = BMO.MapPosition.X;
    int y = BMO.MapPosition.Y;

    //Debug.Log("changing map cell type: " + x + " " + y + " to " + IsOpen + " facing: " + Facing + " " + (GlobalConstants.Orientation)Facing);

    _appRef.GeneratedMap.PathfindingMap[x, y].SidesWalkability[ObjectOrientation] = IsOpen;    
  }

  IEnumerator DoorToggleRoutine()
  {    
    if (IsOpen)
    {
      _animation[_animationName].time = _animation[_animationName].length;
      _animation[_animationName].speed = -AnimationCloseSpeed;
    }
    else
    {
      _animation[_animationName].time = 0;
      _animation[_animationName].speed = AnimationOpenSpeed;
    }

    IsOpen = !IsOpen;

    _animation.Play(_animationName);

    // Set animation type in Animation component to "Always animate"
    // to prevent animation not playing when model is not in the camera view.

    while (_animation.IsPlaying(_animationName))    
    {      
      yield return null;
    }

    _lockInteraction = false;    

    BMO.StartSound.Stop();
    BMO.EndSound.Play();

    if (ActionCompleteCallback != null)
      ActionCompleteCallback(this);
  }
}
