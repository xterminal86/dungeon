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

  bool _isAnimationPlaying = false;
  public bool IsAnimationPlaying
  {
    get { return _isAnimationPlaying; }
  }

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
      if ((int)Mathf.Sign(_animation[_animationName].speed) == 1)
      {
        if (BWO.OffStateBeginSound != null)
          BWO.OffStateBeginSound.Stop();

        if (BWO.OnStateBeginSound != null)
          BWO.OnStateBeginSound.Play();
      }
      else
      {        
        if (BWO.OnStateBeginSound != null)
          BWO.OnStateBeginSound.Stop();

        if (BWO.OffStateBeginSound != null)
          BWO.OffStateBeginSound.Play();
      }
    }
    else
    {
      if ((int)Mathf.Sign(_animation[_animationName].speed) == 1)
      {
        if (BWO.OnStateBeginSound != null)
          BWO.OnStateBeginSound.Play();
      }
      else
      {
        if (BWO.OffStateBeginSound != null)
          BWO.OffStateBeginSound.Play();
      }
    }

    _animation.Play(_animationName);

    // Set animation type in Animation component to "Always animate"
    // to prevent animation not playing when model is not in the camera view.

    while (_animation.IsPlaying(_animationName))    
    { 
      _isAnimationPlaying = true;

      _animationTime = _animation[_animationName].normalizedTime;

      // If door is 90% opened, we can go
      // FIXME:  Possibly this is a hack?

      // We probably shouldn't force close/open both sides (far side of the current block and near side of the next block),
      // only set state for the current object's side.
      if (_animation[_animationName].normalizedTime > 0.9f)
      {        
        LevelLoader.Instance.LevelMap.Level[ArrayCoordinates.X, ArrayCoordinates.Y, ArrayCoordinates.Z].SidesWalkability[ObjectOrientation] = true;

        if (!IsSliding)
        {
          SetDoorSideWalkability(false);
        }
      }
      else
      {
        LevelLoader.Instance.LevelMap.Level[ArrayCoordinates.X, ArrayCoordinates.Y, ArrayCoordinates.Z].SidesWalkability[ObjectOrientation] = false;

        if (!IsSliding)
        {
          SetDoorSideWalkability(true);
        }
      }

      yield return null;
    }

    _isAnimationPlaying = false;

    if ((int)Mathf.Sign(_animation[_animationName].speed) == 1)
    {
      _isOpen = true;    
    }
    else
    {
      _isOpen = false;
    }

    if (IsSliding)
    {      
      if (_isOpen)
      {
        if (BWO.OnStateBeginSound != null)
          BWO.OnStateBeginSound.Stop();

        if (BWO.OnStateFinishedSound != null)
          BWO.OnStateFinishedSound.Play();
      }
      else
      {
        if (BWO.OffStateBeginSound != null)
          BWO.OffStateBeginSound.Stop();

        if (BWO.OffStateFinishedSound != null)
          BWO.OffStateFinishedSound.Play();
      }
    }
    else
    {
      if (_isOpen)
      {
        if (BWO.OnStateFinishedSound != null)
          BWO.OnStateFinishedSound.Play();
      }
      else
      {
        if (BWO.OffStateFinishedSound != null)
          BWO.OffStateFinishedSound.Play();
      }
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

  public void InitBWO()
  {    
    _animation = BWO.GetComponentInChildren<Animation>();
    if (_animation != null)
    {      
      _animation[_animationName].time = 0;
      _animation[_animationName].speed = AnimationOpenSpeed;
    }
  }
}

