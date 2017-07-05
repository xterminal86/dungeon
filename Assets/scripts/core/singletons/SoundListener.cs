using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundListener : MonoSingleton<SoundListener> 
{
  public Vector3 Position = Vector3.zero;
  public Vector3 EulerAngles = Vector3.zero;

  public void UpdateTransform(Vector3 pos, Vector3 eulerAngles)
  {
    Position.Set(pos.x, pos.y, pos.z);
    EulerAngles.Set(eulerAngles.x, eulerAngles.y, eulerAngles.z);

    transform.position = Position;
    //transform.eulerAngles = EulerAngles;
  }
}
