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
