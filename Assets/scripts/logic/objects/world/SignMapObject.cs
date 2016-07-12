using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignMapObject : MapObject 
{
  public SignMapObject(string className, string prefabName, BehaviourMapObject bmo, string signText)
  {
    ClassName = className;
    PrefabName = prefabName;
    BMO = bmo;

    var tm = BMO.GetComponentInChildren<TextMesh>();
    if (tm != null)
    {
      tm.text = signText;
    }
  }
}
