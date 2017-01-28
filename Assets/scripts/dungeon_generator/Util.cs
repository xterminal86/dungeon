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

public class Int3
{
  int _x = 0;
  int _y = 0;
  int _z = 0;

  public Int3()
  {
    _x = 0;
    _y = 0;
    _z = 0;
  }

  public Int3(int x, int y, int z)
  {
    _x = x;
    _y = y;
    _z = z;
  }

  public Int3(float x, float y, float z)
  {
    _x = (int)x;
    _y = (int)y;
    _z = (int)z;
  }

  public Int3(Vector3 v3)
  {
    _x = (int)v3.x;
    _y = (int)v3.y;
    _z = (int)v3.z;
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

  public int Z
  {
    set { _z = value; }
    get { return _z; }
  }

  public void Set(float x, float y, float z)
  {
    _x = (int)x;
    _y = (int)y;
    _z = (int)z;
  }

  public override string ToString()
  {
    return string.Format("[Int3: X={0}, Y={1}, Z={2}]", X, Y, Z);
  }
}

public enum SerializableBlockType
{
  NONE = 0,
  WINDOW
}

// ********************* SERIALIZATION ********************* //

// Holds data for map generation 

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

// Represents any kind of obstacle (column, wall etc.)

[Serializable]
public class SerializableBlock
{
  // Map position X
  public int X = -1;
  // Map position Z
  public int Y = -1;
  // Y height
  public int Layer = -1;
  // Direction of object's front view
  public int Facing = 0;
  // Name of the prefab to instantiate
  public string PrefabName = string.Empty;
  // Should we rotate the object 180 around X? (useful for making ceiling from floor)
  public bool FlipFlag = false;
  // Hash of the footstep sound on this tile
  public int FootstepSoundType = -1;
  // For generator algorithms to distinguish specific blocks
  public SerializableBlockType BlockType = SerializableBlockType.NONE;

  public override string ToString()
  {
    return string.Format("X = {0} Y = {1} Layer = {2} Facing = {3} PrefabName = {4} FlipFlag = {5}", 
                          X, Y, Layer, Facing, PrefabName, FlipFlag);
  }
}

// Any kind of interactable object (lever, door etc.)

[Serializable]
public class SerializableObject
{
  // Map position X
  public int X = -1;
  // Map position Z
  public int Y = -1;
  // Y height
  public int Layer = -1;
  // Direction of object's front view
  public int Facing = 0;
  // In-game object name
  public string ObjectName = string.Empty;
  // Name of the prefab to instantiate
  public string PrefabName = string.Empty;
  // Name of the logic class that will control this object
  public string ObjectClassName = string.Empty;
  // Door related data follows - might be my bad design ¯\_(ツ)_/¯
  public string DoorSoundType = string.Empty;
  public float AnimationOpenSpeed = 1.0f;
  public float AnimationCloseSpeed = 1.0f;
  // Id (i.e. unique name) of this object
  public string ObjectId = string.Empty;
  // Id of the object that this object controls
  public string ObjectToControlId = string.Empty;
  // Some text field for in-game usage (description, signs etc.)
  public string TextField = string.Empty;

  public override string ToString()
  {
    return string.Format("X = {0} Y = {1} Layer = {2} Facing = {3} PrefabName = {4}", X, Y, Layer, Facing, PrefabName);
  }
}

// Serializable*Item classes are made to be used in conjunction with random map generation
// similarily to SerializableObject and SerializableBlock.

// TODO: item durability?

[Serializable]
public class SerializableItem
{
  public int X = -1;
  public int Y = -1;
  public int Layer = -1;
  public int Facing = 0;
  public int AtlasIconIndex = 0;
  public int Cost = 0;  // Money cost value
  public int StrReq = 0;
  public int IntReq = 0;
  public int WillReq = 0;
  public string PrefabName = string.Empty;
  public string ItemName = string.Empty;
  public string ItemDescription = string.Empty;
  public GlobalConstants.WorldItemType ItemType = GlobalConstants.WorldItemType.PLACEHOLDER;

  public SerializableItem()
  {    
  }

  public SerializableItem(SerializableItem rhs)
  {
    X = rhs.X;
    Y = rhs.Y;
    Layer = rhs.Layer;
    Facing = rhs.Facing;
    AtlasIconIndex = rhs.AtlasIconIndex;
    Cost = rhs.Cost;
    StrReq = rhs.StrReq;
    IntReq = rhs.IntReq;
    WillReq = rhs.WillReq;
    PrefabName = rhs.PrefabName;
    ItemName = rhs.ItemName;
    ItemDescription = rhs.ItemDescription;
    // TODO: We can just hardcode specific WorldItemType in constructors of corresponding classes
    ItemType = rhs.ItemType;
  }
}

[Serializable]
public class SerializableFoodItem : SerializableItem
{
  public int Saturation = 0;

  public SerializableFoodItem()
  {    
  }

  public SerializableFoodItem(SerializableFoodItem rhs) : base(rhs)
  {
    Saturation = rhs.Saturation;
  }
}

[Serializable]
public class SerializableWeaponItem : SerializableItem
{  
  public int MinimumDamage = 0;
  public int MaximumDamage = 0;
  public int Cooldown = 0;

  public SerializableWeaponItem()
  {
  }

  public SerializableWeaponItem(SerializableWeaponItem rhs) : base(rhs)
  {
    MinimumDamage = rhs.MinimumDamage;
    MaximumDamage = rhs.MaximumDamage;
    Cooldown = rhs.Cooldown;
  }
}

[Serializable]
public class SerializableArmorItem : SerializableItem
{
  public int ArmorClassModifier = 0;

  public SerializableArmorItem()
  {    
  }

  public SerializableArmorItem(SerializableArmorItem rhs) : base(rhs)
  {
    ArmorClassModifier = rhs.ArmorClassModifier;
  }
}

[Serializable]
public class CameraStartingPos
{
  public int X = -1;
  public int Y = -1;
  public int Facing = 0;
}