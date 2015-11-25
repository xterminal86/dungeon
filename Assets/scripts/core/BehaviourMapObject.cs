using UnityEngine;
using System.Collections;

public class BehaviourMapObject : MonoBehaviour
{
  public AudioSource StartSound;
  public AudioSource EndSound;
  public AudioSource LoopingSound;

  public MapObject MapObjectInstance;

  void Start()
  {
    if (LoopingSound != null)
    {
      LoopingSound.Play();
    }
  }
}
