using UnityEngine;
using System.Collections.Generic;

public delegate void Callback();

public static class GlobalConstants 
{
  public static int WallScaleFactor = 2;
  public static int CameraTurnSpeed = 350;
  public static int CameraMoveSpeed = 4;
  public static Color FogColor = Color.black;
  public static float FogDensity = 0.2f;

  public static Dictionary<MapAttributes, string> MapAttributesDictionary = new Dictionary<MapAttributes, string>()
  {
    { MapAttributes.Floor, "floor" }, { MapAttributes.Wall, "wall" }, { MapAttributes.Ceiling, "ceiling" },
    { MapAttributes.CoordX, "x" }, { MapAttributes.CoordY, "y" }
  };

  /*
  public Dictionary<Orientation, Vector2> OrientationAngles = new Dictionary<Orientation, Vector2>()
  {
    //{ Orientation.NORTH, new Vector2
  };
  */

  public enum MapAttributes
  {
    Floor = 0,
    Wall,
    Ceiling,
    CoordX,
    CoordY    
  }

  public enum Orientation
  {
    NORTH = 0,
    EAST,
    SOUTH,
    WEST
  }
}

public class MapCell
{
  public int CoordX = -1;
  public int CoordY = -1;
  public int FloorId = -1;
  public int WallId = -1;
  public int CeilingId = -1;
}