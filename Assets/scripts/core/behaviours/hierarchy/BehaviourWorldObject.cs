using UnityEngine;
using System.Collections;

/// <summary>
/// This script should be placed on all interactable objects
/// </summary>
public class BehaviourWorldObject : BehaviourWorldObjectBase
{
  [Header("Sound that is played when object starts the \"on\" state (e.g. door opening)")]
  public AudioSource OnStateBeginSound;
  [Header("Sound that is played when \"on\" state completes")]
  public AudioSource OnStateFinishedSound;

  [Header("Sound that is played when object starts the \"off\" state (e.g. door closing)")]
  public AudioSource OffStateBeginSound;
  [Header("Sound that is played when \"off\" state completes")]
  public AudioSource OffStateFinishedSound;

  // Which MapObject this sound is "attached" to
  public WorldObject WorldObjectInstance;

  void Start()
  {
    if (AmbientSound != null)
    {
      AmbientSound.Play();
    }
  }
}
