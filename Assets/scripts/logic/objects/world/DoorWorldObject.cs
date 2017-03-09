using UnityEngine;
using System.Collections;

public class DoorWorldObject : WorldObject
{  
  public float AnimationOpenSpeed = 1.0f;
  public float AnimationCloseSpeed = 1.0f;  

  string _animationName = "Open";
  
  Animation _animation;

  Job _job;

  public bool IsSliding = false;

  bool _isOpening = false;

  bool _isOpen = false;
  public bool IsOpen
  {
    get { return _isOpen; }
  }

  public DoorWorldObject(string inGameName, string prefabName) : base(inGameName, prefabName)
  {    
  }

  public override void ActionHandler(object sender)
  {
    if (_animation != null)
    {      
      if (_job != null)
      {
        _job.KillJob();
        _isOpening = !_isOpening;
      }

      _job = new Job(DoorToggleRoutine());
    }
    else
    {
      Debug.LogWarning("No animation on " + this + " at " + ArrayCoordinates);
    }
  }

  float _animationTime = 0.0f;
  IEnumerator DoorToggleRoutine()
  { 
    if (_isOpening)
    {
      _animation[_animationName].normalizedTime = _animationTime;
      _animation[_animationName].speed = -AnimationCloseSpeed;
    }
    else
    {
      _animation[_animationName].normalizedTime = _animationTime;
      _animation[_animationName].speed = AnimationOpenSpeed;
    }

    if (IsSliding)
    {
      if (!BWO.StartSound.isPlaying)
      {
        BWO.StartSound.Play();
      }
    }
    else
    {
      if ((int)Mathf.Sign(_animation[_animationName].speed) == 1)
      {
        BWO.StartSound.Play();
      }
      else
      {
        BWO.EndSound.Play();
      }
    }

    _animation.Play(_animationName);

    // Set animation type in Animation component to "Always animate"
    // to prevent animation not playing when model is not in the camera view.

    while (_animation.IsPlaying(_animationName))    
    {           
      _animationTime = _animation[_animationName].normalizedTime;

      // If door is 80% opened, we can go
      // FIXME:  Possibly this is a hack?

      // We probably shouldn't force close/open both sides (far side of the current block and near side of the next block),
      // only set state for the current object's side.
      if (_animation[_animationName].normalizedTime > 0.8f)
      {
        LevelLoader.Instance.LevelMap.Level[ArrayCoordinates.X, ArrayCoordinates.Y, ArrayCoordinates.Z].SidesWalkability[ObjectOrientation] = true;

        if (!IsSliding)
        {
          SetDoorSideWalkability(false);
        }
      }
      else
      {
        LevelLoader.Instance.LevelMap.Level[ArrayCoordinates.X, ArrayCoordinates.Y, ArrayCoordinates.Z].SidesWalkability[ObjectOrientation] =  false;

        if (!IsSliding)
        {
          SetDoorSideWalkability(true);
        }
      }

      yield return null;
    }

    if (IsSliding)
    {
      BWO.StartSound.Stop();
      BWO.EndSound.Play();
    }

    if ((int)Mathf.Sign(_animation[_animationName].speed) == 1)
    {
      _isOpen = true;    
    }
    else
    {
      _isOpen = false;
    }

    if (ActionCompleteCallback != null)
      ActionCompleteCallback(this);
  }

  void SetDoorSideWalkability(bool status)
  {
    int newOrientation = (int)ObjectOrientation;

    newOrientation++;
    newOrientation %= 4;

    int newX = ArrayCoordinates.X;
    int newZ = ArrayCoordinates.Z;

    // Hardcoded

    if (ObjectOrientation == GlobalConstants.Orientation.EAST)
    {
      newZ++;
    }
    else if (ObjectOrientation == GlobalConstants.Orientation.SOUTH)
    {
      newX++;
    }
    else if (ObjectOrientation == GlobalConstants.Orientation.WEST)
    {
      newZ--;
    }
    else if (ObjectOrientation == GlobalConstants.Orientation.NORTH)    
    {
      newX--;
    }

    LevelLoader.Instance.LevelMap.Level[newX, ArrayCoordinates.Y, newZ].SidesWalkability[(GlobalConstants.Orientation)newOrientation] = status;
  }

  public void InitBWO(BehaviourWorldObject bwo)
  {
    BWO = bwo;

    _animation = BWO.GetComponentInChildren<Animation>();
    if (_animation != null)
    {      
      _animation[_animationName].time = 0;
      _animation[_animationName].speed = AnimationOpenSpeed;
    }
  }
}

