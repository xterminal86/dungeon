using UnityEngine;
using System.Collections;

public class DoorMapObject : MapObject
{
  public override void ActionHandler (object sender)
  {
    MapObject mo = sender as MapObject;
    if (mo != null)
    {
      Debug.Log(Name + ": I was toggled by " + mo.Name);
    }
  }
}
