using UnityEngine;
using System.Collections;

public class DoorMapObject : MapObject
{
  bool _lockInteraction = false;
  bool _isOpened = false;

  float _animationOpenSpeed = 2;
  float _animationCloseSpeed = 3;

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
      Debug.Log(_animation);

      _animation["Open"].speed = _animationOpenSpeed;
      _animation["Close"].speed = _animationCloseSpeed;
    }
  }

  public override void ActionHandler(object sender)
  {
    if (_animation != null && !_lockInteraction)
    {
      if (!_isOpened)
      {
        BMO.StartSound.Play();
      }
      else
      {
        if (BMO.StartSound.isPlaying)
        {
          BMO.StartSound.Stop();
        }

        BMO.EndSound.Play();
      }

      _job = new Job(DoorToggleRoutine());

      _lockInteraction = true;
    }
  }

  public override void ActionCompleteHandler(object sender)
  {
    _lockInteraction = false;

    _isOpened = !_isOpened;
  }

  IEnumerator DoorToggleRoutine()
  {
    string animationName = "Close";

    if (!_isOpened)
    {
      if (_animation != null)
      {
        _animation[animationName].speed = _animationOpenSpeed;
      }
    }
    else
    {
      if (_animation != null)
      {
        _animation[animationName].speed = -_animationCloseSpeed;
        _animation[animationName].time = _animation[animationName].length;
      }
    }

    _animation.Play(animationName);

    while (_animation.IsPlaying(animationName))
    {
      yield return null;
    }

    if (ActionCompleteCallback != null)
      ActionCompleteCallback(this);
  }
}

/*
public class DoorMapObject : MapObject
{
  public bool DoorIsOpen = false;

  const float _doorMinY = 1.0f;
  const float _doorMaxY = 2.9f;
  const float _doorOpenSpeed = 0.5f;

  Job _job;
  public override void ActionCompleteHandler (object sender)
  {
    MapObject mo = sender as MapObject;
    if (mo != null)
    {
      Debug.Log(string.Format("[{0}] was toggled by [{1}]", ClassName, mo.ClassName));
      if (_job != null)
      {
        _job.KillJob();
      }

      if (BMO.ContinuousSound != null && !BMO.ContinuousSound.isPlaying)
      {
        BMO.ContinuousSound.Play();
      }

      _job = new Job(DoorToggleRoutine());
    }
  }

  DoorMovingState _doorMovingState = DoorMovingState.STILL;
  IEnumerator DoorToggleRoutine()
  {    
    Vector3 position = GameObjectToControl.transform.localPosition;

    if (!DoorIsOpen && _doorMovingState == DoorMovingState.STILL)
    {
      _doorMovingState = DoorMovingState.OPENING;

      while (position.y < _doorMaxY)
      {
        position.y += Time.smoothDeltaTime * _doorOpenSpeed;

        GameObjectToControl.transform.localPosition = position;

        yield return null;
      }

      _doorMovingState = DoorMovingState.STILL;
      DoorIsOpen = true;

      position.y = _doorMaxY;
      GameObjectToControl.transform.localPosition = position;
    }
    else if (!DoorIsOpen && _doorMovingState == DoorMovingState.OPENING)
    {
      _doorMovingState = DoorMovingState.CLOSING;
      DoorIsOpen = false;

      while (position.y > _doorMinY)
      {
        position.y -= Time.smoothDeltaTime * _doorOpenSpeed;
        
        GameObjectToControl.transform.localPosition = position;
        
        yield return null;
      }

      _doorMovingState = DoorMovingState.STILL;

      position.y = _doorMinY;
      GameObjectToControl.transform.localPosition = position;
    }
    else if (DoorIsOpen && _doorMovingState == DoorMovingState.STILL)
    {
      _doorMovingState = DoorMovingState.CLOSING;
      DoorIsOpen = false;

      while (position.y > _doorMinY)
      {
        position.y -= Time.smoothDeltaTime * _doorOpenSpeed;
        
        GameObjectToControl.transform.localPosition = position;
        
        yield return null;
      }

      _doorMovingState = DoorMovingState.STILL;
      
      position.y = _doorMinY;
      GameObjectToControl.transform.localPosition = position;
    }
    else if (!DoorIsOpen && _doorMovingState == DoorMovingState.CLOSING)
    {
      _doorMovingState = DoorMovingState.OPENING;     
      DoorIsOpen = false;

      while (position.y < _doorMaxY)
      {
        position.y += Time.smoothDeltaTime * _doorOpenSpeed;
        
        GameObjectToControl.transform.localPosition = position;
        
        yield return null;
      }
      
      _doorMovingState = DoorMovingState.STILL;
      DoorIsOpen = true;
      
      position.y = _doorMaxY;
      GameObjectToControl.transform.localPosition = position;
    }

    if (BMO.ShortSound != null)
    {
      if (BMO.ContinuousSound != null)
      {
        BMO.ContinuousSound.Stop();
      }

      BMO.ShortSound.Play();
    }

    Debug.Log (string.Format("[{0}] open status: {1}", ClassName, DoorIsOpen));
  }
}

enum DoorMovingState
{
  OPENING = 0,
  CLOSING,
  STILL
}
*/