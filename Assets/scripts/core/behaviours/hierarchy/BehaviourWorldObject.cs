using UnityEngine;
using System.Collections;

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

  [Space(32)]
  [Header("For walls")]

  public Transform FrontQuad;
  public Transform BackQuad;
  public Transform LeftQuad;
  public Transform RightQuad;
  public Transform TopQuad;
  public Transform BottomQuad;

  public Transform WallColumnLeft;
  public Transform WallColumnRight;

  void Update()
  {
    if (AmbientSound != null)
    {
      float d = Vector3.Distance(transform.position, InputController.Instance.CameraPos);

      d = Mathf.Clamp(d, 0.0f, GlobalConstants.SoundHearingMaxDistance);

      float volume = _ambientSoundMaxVolume - (d / GlobalConstants.SoundHearingMaxDistance) * _ambientSoundMaxVolume;

      AmbientSound.volume = volume;
    }
  }

  public void SetLayer(string layerName)
  {
    LeftQuad.gameObject.layer = LayerMask.NameToLayer(layerName);
    RightQuad.gameObject.layer = LayerMask.NameToLayer(layerName);
    FrontQuad.gameObject.layer = LayerMask.NameToLayer(layerName);
    BackQuad.gameObject.layer = LayerMask.NameToLayer(layerName);
    TopQuad.gameObject.layer = LayerMask.NameToLayer(layerName);
    BottomQuad.gameObject.layer = LayerMask.NameToLayer(layerName);
  }
}
