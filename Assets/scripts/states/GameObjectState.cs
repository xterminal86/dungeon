using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for managing specific actors' functionalities
/// </summary>
public abstract class GameObjectState
{
  protected ActorBase _actor;
  protected Int2 _oldPlayerPos = new Int2();

  // Avoid starting coroutines in state ctor since changing of states is carried out
  // via _actor.ChangeState(new SomeState());
  // Because ctor of SomeState will be called first, coroutine will start immediately before
  // actual change of state happens, resulting in running of old Run() method for several frames,
  // which may be not desirable.
  // Use trigger flag to start coroutine in Run() instead.
  public GameObjectState()
  {    
    //Debug.Log("[GameObjectState] ctor [" + _actor + "]");

    if (_actor != null)
    {
      _oldPlayerPos.X = _actor.InputRef.PlayerMapPos.X;
      _oldPlayerPos.Y = _actor.InputRef.PlayerMapPos.Y;
    }
  }

  /// <summary>
  /// Override it in certain states to reset all needed variables to default values,
  /// so that state becomes clean.
  /// Usually it's almost the same code as in constructor.
  /// </summary>
  public virtual void ResetState()
  {
  }

  public virtual void Run()
  {
  }

  protected void PlayFootstepSound3D(Int2 mapPos, Vector3 position3D)
  {
    if (_actor.AppRef.FloorSoundTypeByPosition[mapPos.X, mapPos.Y] != -1)
    {
      SoundManager.Instance.PlayFootstepSound(_actor.Model.name, (GlobalConstants.FootstepSoundType)_actor.AppRef.FloorSoundTypeByPosition[mapPos.X, mapPos.Y], position3D);
    }
  }

  protected bool IsPlayerPositionChanged()
  {
    if (_actor.InputRef.PlayerMapPos.X != _oldPlayerPos.X || _actor.InputRef.PlayerMapPos.Y != _oldPlayerPos.Y)
    {
      _oldPlayerPos.X = _actor.InputRef.PlayerMapPos.X;
      _oldPlayerPos.Y = _actor.InputRef.PlayerMapPos.Y;  
      
      return true;
    }
    
    return false;
  }
  
  protected bool IsPlayerInRange()
  {
    Int2 pos = _actor.Model.ModelPos;
    
    int lx = Mathf.Clamp(pos.X - (_actor as EnemyActor).AgroRange, 0, _actor.AppRef.GeneratedMapHeight - 1);
    int ly = Mathf.Clamp(pos.Y - (_actor as EnemyActor).AgroRange, 0, _actor.AppRef.GeneratedMapWidth - 1);
    int hx = Mathf.Clamp(pos.X + (_actor as EnemyActor).AgroRange, 0, _actor.AppRef.GeneratedMapHeight - 1);
    int hy = Mathf.Clamp(pos.Y + (_actor as EnemyActor).AgroRange, 0, _actor.AppRef.GeneratedMapWidth - 1);
    
    if (_actor.InputRef.PlayerMapPos.X <= hx && _actor.InputRef.PlayerMapPos.X >= lx 
     && _actor.InputRef.PlayerMapPos.Y <= hy && _actor.InputRef.PlayerMapPos.Y >= ly)
    {
      return true;
    }
    
    return false;
  }

  protected bool _rotateDone = false;
  protected IEnumerator RotateModel(float angle)
  {    
    _rotateDone = false;

    Vector3 tmpRotation = _actor.Model.transform.rotation.eulerAngles;

    int fromAngle = (int)tmpRotation.y;
    int toAngle = (int)angle;

    if (fromAngle == 270 && toAngle == 0)
    {
      toAngle = 360;
    }
    else if (fromAngle == 0 && toAngle == 270)
    {
      toAngle = -90;
    }

    int d = (int)angle - (int)tmpRotation.y;

    // If model has 270 rotation around y and has to rotate to 0, 
    // we should rotate towards shortest direction, not from 270 to 0 backwards, 
    // so we introduce special condition.
    // Same thing when angles are reversed, because we rely on sign variable in tmpRotation.y change.
    //
    // TLDR:
    // Case 1: from = 270, to = 0, we have sign = -1, so we would decrement current rotation from 270 to 0, instead of just going to 0 (i.e. 360).
    // Case 2: from = 0, to = 270, we have sign = +1, so we would increment current rotation from 0 to 270 - same thing reversed.

    bool specialRotation = ((int)angle == 270 && (int)tmpRotation.y == 0) || ((int)angle == 0 && (int)tmpRotation.y == 270);
    int cond = specialRotation ? 90 : Mathf.Abs(d);

    float sign = Mathf.Sign(d);

    float counter = 0.0f;
    while (counter < cond)
    {
      counter += Time.smoothDeltaTime * GlobalConstants.CharacterRotationSpeed;

      if (specialRotation)
      {
        tmpRotation.y -= sign * (Time.smoothDeltaTime * GlobalConstants.CharacterRotationSpeed);
      }
      else
      {
        tmpRotation.y += sign * (Time.smoothDeltaTime * GlobalConstants.CharacterRotationSpeed);
      }

      // Removing jitter caused by manual adjustment
      // of tmpRotation.y after the while loop (also removed)
      if (fromAngle > toAngle)
      {
        tmpRotation.y = Mathf.Clamp(tmpRotation.y, toAngle, fromAngle);
      }
      else
      {
        tmpRotation.y = Mathf.Clamp(tmpRotation.y, fromAngle, toAngle);
      }

      _actor.Model.transform.rotation = Quaternion.Euler(tmpRotation);

      yield return null;
    }

    _actor.Model.transform.rotation = Quaternion.Euler(tmpRotation);

    _rotateDone = true;

    yield return null;
  }

  protected Vector3 _modelPosition = Vector3.zero;
  protected Int2 _currentMapPos = new Int2();
  protected Int2 _positionForTalk = new Int2();
  protected bool _moveDone = false;
  protected bool _firstStepSound = false;
  protected IEnumerator MoveModel(Int2 newMapPos)
  {
    _moveDone = false;

    _currentMapPos.X = _actor.Model.ModelPos.X;
    _currentMapPos.Y = _actor.Model.ModelPos.Y;

    Int2 oldWorldPos = new Int2(_currentMapPos.X * GlobalConstants.WallScaleFactor, _currentMapPos.Y * GlobalConstants.WallScaleFactor);
    Int2 newWorldPos = new Int2(newMapPos.X * GlobalConstants.WallScaleFactor, newMapPos.Y * GlobalConstants.WallScaleFactor);

    int dx = newMapPos.X - _currentMapPos.X;
    int dy = newMapPos.Y - _currentMapPos.Y;

    if (!_firstStepSound)
    {
      PlayFootstepSound3D(_actor.Model.ModelPos, _modelPosition);
      _firstStepSound = true;      
    }

    while (dx != 0 || dy != 0)
    {
      // Constantly moving 3d model
      _modelPosition.x += dx * (Time.smoothDeltaTime * _actor.Model.WalkingSpeed);
      _modelPosition.z += dy * (Time.smoothDeltaTime * _actor.Model.WalkingSpeed);

      // We have to calculate map position from 3d coordinates every frame, so we have to perform a division.
      // In order to properly determine map position when model is less than halfway from the center of the cell,
      // we use Ceil or Floor.
      _currentMapPos.X = dx < 0 ? Mathf.CeilToInt(_modelPosition.x / GlobalConstants.WallScaleFactor) : Mathf.FloorToInt(_modelPosition.x / GlobalConstants.WallScaleFactor);
      _currentMapPos.Y = dy < 0 ? Mathf.CeilToInt(_modelPosition.z / GlobalConstants.WallScaleFactor) : Mathf.FloorToInt(_modelPosition.z / GlobalConstants.WallScaleFactor);

      dx = newMapPos.X - _currentMapPos.X;
      dy = newMapPos.Y - _currentMapPos.Y;      

      if (oldWorldPos.X > newWorldPos.X)
      {
        _modelPosition.x = Mathf.Clamp(_modelPosition.x, newWorldPos.X, oldWorldPos.X);
      }
      else
      {
        _modelPosition.x = Mathf.Clamp(_modelPosition.x, oldWorldPos.X, newWorldPos.X);
      }

      if (oldWorldPos.Y > newWorldPos.Y)
      {
        _modelPosition.z = Mathf.Clamp(_modelPosition.z, newWorldPos.Y, oldWorldPos.Y);
      }
      else
      {
        _modelPosition.z = Mathf.Clamp(_modelPosition.z, oldWorldPos.Y, newWorldPos.Y);
      }

      yield return null;
    }

    _actor.Model.ModelPos.X = newMapPos.X;
    _actor.Model.ModelPos.Y = newMapPos.Y;

    _moveDone = true;

    PlayFootstepSound3D(_actor.Model.ModelPos, _modelPosition);

    yield return null;
  }

  // Sometimes when change of animation happens, it gets stuck in previous state
  // regardless of wrap mode and stuff, so we should manually rewind previously
  // playing animation before playing new one, using this method.
  //
  // N.B. Make sure "Key All Bones" is checked during FBX export in Blender (v. 2.75).
  public void RewindAnimation(string animationName)
  {
    if (_actor.Model.AnimationComponent.GetClip(animationName) != null)
    {
      _actor.Model.AnimationComponent[animationName].time = 0.0f;
      _actor.Model.AnimationComponent.Sample();
      _actor.Model.AnimationComponent.Stop(animationName);
    }
  }
}
