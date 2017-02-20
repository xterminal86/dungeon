using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignWorldObject : WorldObject 
{
  public SignWorldObject(string className, string prefabName, BehaviourMapObject bmo, string signText)
  {
    BMO = bmo;

    var tm = BMO.GetComponentInChildren<TextMesh>();
    if (tm != null)
    {
      tm.text = signText;
    }
  }
}
