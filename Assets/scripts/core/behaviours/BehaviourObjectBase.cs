using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourObjectBase : MonoBehaviour 
{
  // Sound that is played as ambience
  public AudioSource LoopingSound;

  protected Int2 _mapPosition = new Int2();
  public Int2 MapPosition
  {
    get { return _mapPosition; }
  }

  public void CalculateMapPosition()
  {    
    _mapPosition.X = (int)transform.position.x / GlobalConstants.WallScaleFactor;
    _mapPosition.Y = (int)transform.position.z / GlobalConstants.WallScaleFactor;
  }
}
