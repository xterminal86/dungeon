using UnityEngine;
using System.Collections;

/// <summary>
/// This script should be placed on all interactable objects
/// </summary>
public class BehaviourMapObject : MonoBehaviour
{
  // Sound that is played on interaction start (e.g. door opening)
  public AudioSource StartSound;

  // Sound that is played on interaction end (e.g. door closing)
  public AudioSource EndSound;

  // Sound that is played as ambience
  public AudioSource LoopingSound;

  // Which MapObject this sound is "attached" to
  public MapObject MapObjectInstance;

  void Start()
  {
    if (LoopingSound != null)
    {
      LoopingSound.Play();
    }
  }

  Int2 _mapPosition = new Int2();
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
