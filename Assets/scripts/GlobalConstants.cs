using UnityEngine;
using System.Collections;

public delegate void Callback();

public static class GlobalConstants 
{
  public static int WallScaleFactor = 2;
  public static int CameraTurnSpeed = 250;
  public static int CameraMoveSpeed = 6;
  public static Color FogColor = Color.black;
  public static float FogDensity = 0.2f;
}
