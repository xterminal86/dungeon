using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for managing specific actors' functionalities
/// </summary>
public abstract class GameObjectState
{
  protected ActorBase _actor;

  protected bool _lock = false;

  // Avoid starting coroutines in state ctor since changing of states is carried out
  // via _actor.ChangeState(new SomeState());
  // Because ctor of SomeState will be called first, coroutine will start immediately before
  // actual change of state happens, resulting in running of old Run() method for several frames,
  // which may be not desirable.
  // Use trigger flag to start coroutine in Run() instead.
  public GameObjectState()
  {    
    //Debug.Log("[GameObjectState] ctor [" + _actor + "]");
  }

  /// <summary>
  /// Since we cache state variables, override this method to reset state to its default values.
  /// Usually it's almost the same code as in constructor.
  /// </summary>
  public virtual void ResetState()
  {
  }

  public virtual void Run()
  {
  }

  protected bool IsPlayerPositionChanged()
  {
    /*
    if (_actor.InputRef.PlayerMapPos.X != _oldPlayerPos.X || _actor.InputRef.PlayerMapPos.Y != _oldPlayerPos.Y)
    {
      _oldPlayerPos.X = _actor.InputRef.PlayerMapPos.X;
      _oldPlayerPos.Y = _actor.InputRef.PlayerMapPos.Y;  
      
      return true;
    }
    */

    return false;
  }
  
  protected bool IsPlayerInRange()
  {
    /*
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
    */

    return false;
  }

  protected float GetAngleToRotate(Int3 cellToLook)
  {
    int dx = cellToLook.X - _actor.ActorPosition.X;
    int dz = cellToLook.Z - _actor.ActorPosition.Z;

    float angleEnd = 0.0f;

    if (dz == 1 && dx == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.EAST];
    else if (dz == -1 && dx == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.WEST];
    else if (dx == 1 && dz == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.SOUTH];
    else if (dx == -1 && dz == 0) angleEnd = GlobalConstants.OrientationAngles[GlobalConstants.Orientation.NORTH];

    return angleEnd;
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
  protected Vector3 _currentModelPos = Vector3.zero;
  protected Vector3 _newModelPos = Vector3.zero;
  protected Int3 _positionForTalk = new Int3();
  protected bool _moveDone = false;
  protected IEnumerator MoveModel(Int3 newMapPos)
  {      
    _moveDone = false;

    _currentModelPos.Set(_actor.ActorWorldPosition.x, _actor.ActorWorldPosition.y, _actor.ActorWorldPosition.z);
    _newModelPos.Set(newMapPos.X * GlobalConstants.WallScaleFactor, newMapPos.Y * GlobalConstants.WallScaleFactor, newMapPos.Z * GlobalConstants.WallScaleFactor);

    _modelPosition.Set(_actor.ActorWorldPosition.x, _actor.ActorWorldPosition.y, _actor.ActorWorldPosition.z);

    int dx = newMapPos.X - _actor.ActorPosition.X;
    int dz = newMapPos.Z - _actor.ActorPosition.Z;

    float cond = 0.0f;
    float speed = 0.0f;
    while (cond < GlobalConstants.WallScaleFactor)
    { 
      // cond and model position must be incremented equally,
      // in order for model movement to be in sync with loop condition increment.

      speed = Time.smoothDeltaTime * _actor.ModelMovementSpeed;
      cond += speed;

      _modelPosition.x += dx * speed;
      _modelPosition.z += dz * speed;

      if (_currentModelPos.x > _newModelPos.x)
      {
        _modelPosition.x = Mathf.Clamp(_modelPosition.x, _newModelPos.x, _currentModelPos.x);
      }
      else
      {
        _modelPosition.x = Mathf.Clamp(_modelPosition.x, _currentModelPos.x, _newModelPos.x);
      }

      if (_currentModelPos.z > _newModelPos.z)
      {
        _modelPosition.z = Mathf.Clamp(_modelPosition.z, _newModelPos.z, _currentModelPos.z);
      }
      else
      {
        _modelPosition.z = Mathf.Clamp(_modelPosition.z, _currentModelPos.z, _newModelPos.z);
      }

      _actor.Model.transform.position = _modelPosition;

      yield return null;
    }

    _moveDone = true;

    _actor.ActorWorldPosition.Set(_newModelPos.x, _newModelPos.y, _newModelPos.z);

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
