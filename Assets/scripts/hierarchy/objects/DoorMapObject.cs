using UnityEngine;
using System.Collections;

public class DoorMapObject : MapObject
{
  public string DoorSoundType = string.Empty;

  bool _lockInteraction = false;

  public float AnimationOpenSpeed = 1.0f;
  public float AnimationCloseSpeed = 1.0f;  

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
      _animation[_animationName].speed = AnimationOpenSpeed;
    }
  }

  public override void ActionHandler(object sender)
  {
    if (_animation != null && !_lockInteraction)
    {
      if (DoorSoundType == "openable")
      {
        if (!IsOpen)
        {
          BMO.StartSound.Play();
        }
        else
        {
          BMO.EndSound.Play();
        }
      }
      else if (DoorSoundType == "sliding")
      {
        BMO.StartSound.Play();
      }
      
      _job = new Job(DoorToggleRoutine());

      _lockInteraction = true;
    }
  }

  public override void ActionCompleteHandler(object sender)
  {
    int x = BMO.MapPosition.X;
    int y = BMO.MapPosition.Y;

    Debug.Log("changing map cell type: " + x + " " + y + " to " + IsOpen);

    App.Instance.ObstaclesByPosition[x, y].CellType = IsOpen ? GeneratedCellType.NONE : GeneratedCellType.OBSTACLE;
  }

  IEnumerator DoorToggleRoutine()
  {
    IsBeingInteracted = true;

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

    _animation.Play(_animationName);

    // Set animation type in Animation component to "Always animate"
    // to prevent animation not playing when model is not in the camera view.

    while (_animation.IsPlaying(_animationName))    
    {      
      yield return null;
    }

    _lockInteraction = false;    
    IsOpen = !IsOpen;

    if (ActionCompleteCallback != null)
      ActionCompleteCallback(this);

    IsBeingInteracted = false;

    if (DoorSoundType == "sliding")
    {
      if (BMO.StartSound.isPlaying)
      {
        BMO.StartSound.Stop();
      }

      BMO.EndSound.Play();
    }
  }
}

