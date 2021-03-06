﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourWorldObjectBase : MonoBehaviour 
{
  [Header("Looping sound that is played as ambience")]
  public AudioSource AmbientSound;

  // Which logic WorldObject this sound is "attached" to
  public WorldObject WorldObjectInstance;

  protected Int3 _mapPosition = new Int3();
  public Int3 MapPosition
  {
    get { return _mapPosition; }
  }

  void Start()
  {
    if (AmbientSound != null)
    {
      AmbientSound.Play();
    }
  }
}
