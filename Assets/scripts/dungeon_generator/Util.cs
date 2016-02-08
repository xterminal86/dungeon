using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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

public enum SerializableBlockType
{
  NONE = 0,
  WINDOW
}

// ********************* SERIALIZATION ********************* //

[Serializable]
public class SerializableMap
{
  public int MapWidth = 0;
  public int MapHeight = 0;

  public string MusicTrack = string.Empty;

  public CameraStartingPos CameraPos = new CameraStartingPos();
  public List<SerializableBlock> SerializableBlocksList = new List<SerializableBlock>();
  public List<SerializableObject> SerializableObjectsList = new List<SerializableObject>();
}

[Serializable]
public class SerializableBlock
{
  public int X = -1;
  public int Y = -1;
  public int Layer = -1;
  public int Facing = 0;
  public string PrefabName = string.Empty;
  public bool FlipFlag = false;
  public int FootstepSoundType = -1;
  public SerializableBlockType BlockType = SerializableBlockType.NONE;

  public override string ToString()
  {
    return string.Format("X = {0} Y = {1} Layer = {2} Facing = {3} PrefabName = {4} FlipFlag = {5}", 
                          X, Y, Layer, Facing, PrefabName, FlipFlag);
  }
}

[Serializable]
public class SerializableObject
{
  public int X = -1;
  public int Y = -1;
  public int Layer = -1;
  public int Facing = 0;
  public string ObjectName = string.Empty;
  public string PrefabName = string.Empty;
  public string ObjectClassName = string.Empty;
  public string DoorSoundType = string.Empty;
  public float AnimationOpenSpeed = 1.0f;
  public float AnimationCloseSpeed = 1.0f;
  public string ObjectId = string.Empty;
  public string ObjectToControlId = string.Empty;
  public string TextField = string.Empty;

  public override string ToString()
  {
    return string.Format("X = {0} Y = {1} Layer = {2} Facing = {3} PrefabName = {4}", X, Y, Layer, Facing, PrefabName);
  }
}

[Serializable]
public class SerializableItem
{
  public int X = -1;
  public int Y = -1;
  public int Layer = -1;
  public int Facing = 0;
  public int AtlasIconHash = -1;
  public int Value = 0;  // Money cost value
  public string PrefabName = string.Empty;
  public string ItemName = string.Empty;
  public string ItemDescription = string.Empty;
  public string ItemType = string.Empty;
}

[Serializable]
public class SerializableFoodItem : SerializableItem
{
  public int HungerDecreaseValue = 0;

  // TODO: food rotting?
  // Have to calculate all food objects and decrease food "durability" over time.
}

[Serializable]
public class SerializableWeaponItem : SerializableItem
{
}

/*
public enum ObjectItemType
{
  PLACEHOLDER = 0,
  ARMOR,
  ACCESSORY,
  CONSUMABLE,
  WEAPON
}
*/

[Serializable]
public class CameraStartingPos
{
  public int X = -1;
  public int Y = -1;
  public int Facing = 0;
}