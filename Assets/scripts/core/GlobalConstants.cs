using UnityEngine;
using System.Collections.Generic;

public delegate void Callback();
public delegate void CallbackO(object sender);

public static class GlobalConstants 
{
  public static int WallScaleFactor = 2;
  public static int CameraTurnSpeed = 350;
  public static int CameraMoveSpeed = 4;
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

  public static Dictionary<SoundNames, int> SoundHashByName = new Dictionary<SoundNames, int>()
  {
    { SoundNames.FS_GRASS1, "fs-grass1".GetHashCode() },
    { SoundNames.FS_GRASS2, "fs-grass2".GetHashCode() },
    { SoundNames.FS_GRASS3, "fs-grass3".GetHashCode() },
    { SoundNames.FS_GRASS4, "fs-grass4".GetHashCode() },
    { SoundNames.FS_TILE1, "fs-tile1".GetHashCode() },
    { SoundNames.FS_TILE2, "fs-tile2".GetHashCode() },
    { SoundNames.FS_TILE3, "fs-tile3".GetHashCode() },
    { SoundNames.FS_TILE4, "fs-tile4".GetHashCode() },
    { SoundNames.ACT_WOODEN_DOOR_OPEN, "act-door-wooden-open".GetHashCode() },
    { SoundNames.ACT_WOODEN_DOOR_CLOSE, "act-door-wooden-close".GetHashCode() },
    { SoundNames.PLAYER_CANNOT_MOVE, "player-cannot-move".GetHashCode() }
  };
    
  public static List<int> FootstepsGrass = new List<int>()
  {
    SoundHashByName[SoundNames.FS_GRASS1],
    SoundHashByName[SoundNames.FS_GRASS2],
    SoundHashByName[SoundNames.FS_GRASS3],
    SoundHashByName[SoundNames.FS_GRASS4]
  };

  public static List<int> FootstepsTile = new List<int>()
  {
    SoundHashByName[SoundNames.FS_TILE1],
    SoundHashByName[SoundNames.FS_TILE2],
    SoundHashByName[SoundNames.FS_TILE3],
    SoundHashByName[SoundNames.FS_TILE4]
  };

  public enum SoundNames
  {
    FS_GRASS1 = 0,
    FS_GRASS2,
    FS_GRASS3,
    FS_GRASS4,
    FS_TILE1,
    FS_TILE2,
    FS_TILE3,
    FS_TILE4,
    ACT_WOODEN_DOOR_OPEN,
    ACT_WOODEN_DOOR_CLOSE,
    PLAYER_CANNOT_MOVE
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
