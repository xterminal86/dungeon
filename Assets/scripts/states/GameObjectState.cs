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

  public GameObjectState()
  {
    _oldPlayerPos.X = InputController.Instance.PlayerMapPos.X;
    _oldPlayerPos.Y = InputController.Instance.PlayerMapPos.Y;
  }

  public virtual void Run()
  {
  }

  protected void PlayFootstepSound3D(Int2 mapPos, Vector3 position3D)
  {
    if (App.Instance.FloorSoundTypeByPosition[mapPos.X, mapPos.Y] != -1)
    {
      SoundManager.Instance.PlayFootstepSound(_actor.Model.name, (GlobalConstants.FootstepSoundType)App.Instance.FloorSoundTypeByPosition[mapPos.X, mapPos.Y], position3D);
    }
  }

  protected bool IsPlayerPositionChanged()
  {
    if (InputController.Instance.PlayerMapPos.X != _oldPlayerPos.X || InputController.Instance.PlayerMapPos.Y != _oldPlayerPos.Y)
    {
      _oldPlayerPos.X = InputController.Instance.PlayerMapPos.X;
      _oldPlayerPos.Y = InputController.Instance.PlayerMapPos.Y;  
      
      return true;
    }
    
    return false;
  }
  
  protected bool IsPlayerInRange()
  {
    Int2 pos = _actor.Model.ModelPos;
    
    int lx = Mathf.Clamp(pos.X - (_actor as EnemyActor).AgroRange, 0, App.Instance.GeneratedMapHeight - 1);
    int ly = Mathf.Clamp(pos.Y - (_actor as EnemyActor).AgroRange, 0, App.Instance.GeneratedMapWidth - 1);
    int hx = Mathf.Clamp(pos.X + (_actor as EnemyActor).AgroRange, 0, App.Instance.GeneratedMapHeight - 1);
    int hy = Mathf.Clamp(pos.Y + (_actor as EnemyActor).AgroRange, 0, App.Instance.GeneratedMapWidth - 1);
    
    if (InputController.Instance.PlayerMapPos.X <= hx && InputController.Instance.PlayerMapPos.X >= lx 
     && InputController.Instance.PlayerMapPos.Y <= hy && InputController.Instance.PlayerMapPos.Y >= ly)
    {
      return true;
    }
    
    return false;
  }
}
