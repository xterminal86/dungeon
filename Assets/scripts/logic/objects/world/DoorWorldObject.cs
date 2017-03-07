using UnityEngine;
using System.Collections;

public class DoorWorldObject : WorldObject
{  
  bool _lockInteraction = false;

  public float AnimationOpenSpeed = 1.0f;
  public float AnimationCloseSpeed = 1.0f;  

  public bool IsOpen = false;

  string _animationName = "Open";
  
  Animation _animation;

  Job _job;

  bool _isSliding = false;

  public DoorWorldObject (string inGameName, string prefabName, bool isSliding, BehaviourWorldObject bmo) : base(inGameName, prefabName)
  {
    BWO = bmo;
    _isSliding = isSliding;

    _animation = BWO.GetComponentInParent<Animation>();
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
      if (_isSliding)
      {
        BWO.StartSound.Play();
      }
      else
      {
        if (!IsOpen)
        {
          BWO.StartSound.Play();
        }
        else
        {
          BWO.EndSound.Play();
        }
      }

      _job = new Job(DoorToggleRoutine());

      _lockInteraction = true;
    }
  }

  public override void ActionCompleteHandler(object sender)
  {
    int x = BWO.MapPosition.X;
    int y = BWO.MapPosition.Y;
    int z = BWO.MapPosition.Z;

    //Debug.Log("changing map cell type: " + x + " " + y + " to " + IsOpen + " facing: " + Facing + " " + (GlobalConstants.Orientation)Facing);

    LevelLoader.Instance.LevelMap.Level[x, y, z].SidesWalkability[ObjectOrientation] = IsOpen;    
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

    if (_isSliding)
    {
      BWO.StartSound.Stop();
      BWO.EndSound.Play();
    }

    _lockInteraction = false;    
    
    if (ActionCompleteCallback != null)
      ActionCompleteCallback(this);
  }
}

