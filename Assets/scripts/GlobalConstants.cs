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

public class Int2
{
  int _x = 0;
  int _y = 0;

  public Int2()
  {
    _x = 0;
    _y = 0;
  }

  public Int2(int x, int y)
  {
    _x = x;
    _y = y;
  }

  public Int2(float x, float y)
  {
    _x = (int)x;
    _y = (int)y;
  }

  public Int2(Vector2 v2)
  {
    _x = (int)v2.x;
    _y = (int)v2.y;
  }

  public int X
  {
    set { _x = value; }
    get { return _x; }
  }

  public int Y
  {
    set { _y = value; }
    get { return _y; }
  }

  public override string ToString()
  {
    return string.Format("[Int2: X={0}, Y={1}]", X, Y);
  }
}

public class RoomBounds
{
  public Int2 FirstPoint = new Int2();
  public Int2 SecondPoint = new Int2();

  public RoomBounds(Int2 p1, Int2 p2)
  {
    FirstPoint.X = p1.X;
    FirstPoint.Y = p1.Y;

    SecondPoint.X = p2.X;
    SecondPoint.Y = p2.Y;
  }

  public override string ToString()
  {
    return string.Format("[RoomBounds] -> [" + FirstPoint + " " + SecondPoint + "]");
  }
}