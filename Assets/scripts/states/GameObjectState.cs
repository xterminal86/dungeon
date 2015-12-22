using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for managing specific actors' functionalities
/// </summary>
public abstract class GameObjectState
{
  protected ActorBase _actor;

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
}
