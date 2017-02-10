using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourObjectBase : MonoBehaviour 
{
  // Sound that is played as ambience
  public AudioSource LoopingSound;

  protected Int3 _mapPosition = new Int3();
  public Int3 MapPosition
  {
    get { return _mapPosition; }
  }
}
