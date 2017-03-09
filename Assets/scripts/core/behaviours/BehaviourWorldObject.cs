﻿using UnityEngine;
using System.Collections;

/// <summary>
/// This script should be placed on all interactable objects
/// </summary>
public class BehaviourWorldObject : BehaviourWorldObjectBase
{
  // Sound that is played on interaction start (e.g. door opening)
  public AudioSource StartSound;

  // Sound that is played on interaction end (e.g. door closing)
  public AudioSource EndSound;

  // Which MapObject this sound is "attached" to
  public WorldObject WorldObjectInstance;

  void Start()
  {
    if (LoopingSound != null)
    {
      LoopingSound.Play();
    }
  }
}