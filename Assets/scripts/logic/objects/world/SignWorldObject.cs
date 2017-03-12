using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignWorldObject : WorldObject 
{
  public string SignText = string.Empty;

  public SignWorldObject(string inGameName, string prefabName) : base(inGameName, prefabName)
  { 
  }

  public void InitBWO()
  {
    var tm = BWO.GetComponentInChildren<TextMesh>();
    if (tm != null)
    {
      tm.text = SignText;
    }
  }
}
