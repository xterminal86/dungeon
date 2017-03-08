using UnityEngine;
using System.Collections;

public class DoorWorldObject : WorldObject
{  
  public float AnimationOpenSpeed = 1.0f;
  public float AnimationCloseSpeed = 1.0f;  

  string _animationName = "Open";
  
  Animation _animation;

  Job _job;

  bool _isSliding = false;
  bool _isOpening = false;

  public DoorWorldObject(string inGameName, string prefabName, bool isSliding) : base(inGameName, prefabName)
  {
    _isSliding = isSliding;
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

    if (_isSliding)
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
      }
      else
      {
        LevelLoader.Instance.LevelMap.Level[ArrayCoordinates.X, ArrayCoordinates.Y, ArrayCoordinates.Z].SidesWalkability[ObjectOrientation] =  false;
      }

      yield return null;
    }

    if (_isSliding)
    {
      BWO.StartSound.Stop();
      BWO.EndSound.Play();
    }

    if (ActionCompleteCallback != null)
      ActionCompleteCallback(this);
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

