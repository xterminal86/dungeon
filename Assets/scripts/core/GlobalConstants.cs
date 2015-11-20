using UnityEngine;
using System.Collections.Generic;

public delegate void Callback();
public delegate void CallbackO(object sender);

public static class GlobalConstants 
{
  public static int WallScaleFactor = 2;
  public static int CameraTurnSpeed = 350;
  public static int CameraMoveSpeed = 4;
  public static int CameraCannotMoveSpeed = 2;
  public static float DoorOpenSpeed = 1.0f;
  public static float CameraBobSpeed = 0.5f;
  
  public static Dictionary<MapAttributes, string> MapAttributesDictionary = new Dictionary<MapAttributes, string>()
  {
    { MapAttributes.Floor, "floor" }, { MapAttributes.Wall, "wall" }, { MapAttributes.Ceiling, "ceiling" },
    { MapAttributes.CoordX, "x" }, { MapAttributes.CoordY, "y" }
  };

  public static Dictionary<int, Orientation> OrientationsMap = new Dictionary<int, Orientation>()
  {
    { 0, Orientation.NORTH },
    { 1, Orientation.EAST },
    { 2, Orientation.SOUTH },
    { 3, Orientation.WEST }
  };

  public static Dictionary<Orientation, float> OrientationAngles = new Dictionary<Orientation, float>()
  {
    { Orientation.NORTH, 270.0f },
    { Orientation.EAST, 0.0f },
    { Orientation.SOUTH, 90.0f },
    { Orientation.WEST, 180.0f }
  };

  public static List<int> FootstepsDummy = new List<int>()
  {
    "fs-dummy1".GetHashCode(),
    "fs-dummy2".GetHashCode()
  };

  public static List<int> FootstepsGrass = new List<int>()
  {
    "fs-grass1".GetHashCode(),
    "fs-grass2".GetHashCode(),
    "fs-grass3".GetHashCode(),
    "fs-grass4".GetHashCode()
  };

  public static List<int> FootstepsTile = new List<int>()
  {
    "fs-tile1".GetHashCode(),
    "fs-tile2".GetHashCode(),
    "fs-tile3".GetHashCode(),
    "fs-tile4".GetHashCode()
  };

  public static List<int> FootstepsWood = new List<int>()
  {
    "fs-wood1".GetHashCode(),
    "fs-wood2".GetHashCode(),
    "fs-wood3".GetHashCode(),
    "fs-wood4".GetHashCode(),
    "fs-wood5".GetHashCode()
  };

  public static Dictionary<FootstepSoundType, List<int>> FootstepsListByType = new Dictionary<FootstepSoundType, List<int>>()
  {
    { FootstepSoundType.DUMMY, FootstepsDummy },
    { FootstepSoundType.DIRT, FootstepsDummy },
    { FootstepSoundType.GRASS, FootstepsGrass },
    { FootstepSoundType.STONE, FootstepsDummy },
    { FootstepSoundType.TILE, FootstepsTile },
    { FootstepSoundType.WOOD, FootstepsWood },
    { FootstepSoundType.METAL, FootstepsDummy }
  };

  public enum FootstepSoundType
  {
    DUMMY = 0,
    DIRT,
    GRASS,
    STONE,
    TILE,
    WOOD,
    METAL
  }

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
