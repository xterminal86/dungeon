using UnityEngine;
using System.Collections;

public class BehaviourMapObject : MonoBehaviour
{
  public AudioSource StartSound;
  public AudioSource EndSound;
  public AudioSource LoopingSound;

  public MapObject MapObjectInstance;

  Collider _collider;
  void Start()
  {
    _collider = GetComponentInChildren<BoxCollider>();
    if (LoopingSound != null)
    {
      LoopingSound.Play();
    }
  }
}
