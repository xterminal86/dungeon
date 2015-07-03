using UnityEngine;
using System.Collections;

public class MapObject
{
  public virtual void PrintType()
  {
    Debug.Log (this.GetType());
  }
}
