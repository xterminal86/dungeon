using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignWorldObject : WorldObject 
{
  public SignWorldObject(string inGameName, string prefabName, BehaviourWorldObject bmo, string signText) : base(inGameName, prefabName)
  {
    BWO = bmo;

    var tm = BWO.GetComponentInChildren<TextMesh>();
    if (tm != null)
    {
      tm.text = signText;
    }
  }
}
