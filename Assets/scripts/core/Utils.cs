using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utils
{
  public static int BlockDistance(Int2 from, Int2 to)
  {
    return Mathf.Abs(to.Y - from.Y) + Mathf.Abs(to.X - from.X);
  }	
}
