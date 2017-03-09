using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBase
{
  protected BlockEntity[,,] _level;
  public BlockEntity[,,] Level
  {
    get { return _level; }
  }

  protected int _mapX, _mapY, _mapZ;

  public int MapX
  {
    get { return _mapX; }
  }

  public int MapY
  {
    get { return _mapY; }
  }

  public int MapZ
  {
    get { return _mapZ; }
  }

  /// <summary>
  /// Position of player (might be starting pos and saved for subsequent loading).
  /// Indicates array coordinates of a block in which player is located.
  /// Footstep sound is calcualted from Y - 1 coordinates (see the end of InputController::CameraMoveRoutine())
  /// </summary>   
  protected Int3 _playerPos = new Int3();
  public Int3 PlayerPos
  {
    get { return _playerPos; }
  }

  public LevelBase(int x, int y, int z)
  {
    _mapX = x;
    _mapY = y;
    _mapZ = z;

    _level = new BlockEntity[x, y, z];

    InitArray();
  }

  /// <summary>
  /// Call this method to generate your level.
  /// </summary>
  public void Generate()
  {
    GenerateLevel();
    AdjustLiquidBlocks();
  }

  /// <summary>
  /// Code for generation of a level goes here.
  /// </summary>
  public virtual void GenerateLevel()
  {
  }

  void InitArray()
  {
    for (int y = 0; y < _mapY; y++)
    {
      for (int x = 0; x < _mapX; x++)
      {
        for (int z = 0; z < _mapZ; z++)
        {
          _level[x, y, z] = new BlockEntity();
          _level[x, y, z].BlockType = GlobalConstants.BlockType.AIR;
          _level[x, y, z].ArrayCoordinates.Set(x, y, z);
          _level[x, y, z].WorldCoordinates.Set(x * GlobalConstants.WallScaleFactor, y * GlobalConstants.WallScaleFactor, z * GlobalConstants.WallScaleFactor);
        }
      }
    }
  }

  // ************************ HELPER FUNCTIONS ************************ //

  /// <summary>
  /// Sets no-instantiate flag on blocks that are surrounded on all 6 sides.
  /// Works for all map height on a given area.
  /// </summary>
  /// <param name="areaStartX">Area start x.</param>
  /// <param name="areaEndX">Area end x.</param>
  /// <param name="areaStartZ">Area start z.</param>
  /// <param name="areaEndZ">Area end z.</param>
  protected void DiscardHiddenBlocks(int areaStartX, int areaEndX, 
                                     int areaStartY, int areaEndY, 
                                     int areaStartZ, int areaEndZ)
  {
    areaStartX = Mathf.Clamp(areaStartX, 0, _mapX - 1);
    areaEndX = Mathf.Clamp(areaEndX, 0, _mapX - 1);
    areaStartY = Mathf.Clamp(areaStartY, 0, _mapY - 1);
    areaEndY = Mathf.Clamp(areaEndY, 0, _mapY - 1);
    areaStartZ = Mathf.Clamp(areaStartZ, 0, _mapZ - 1);
    areaEndZ = Mathf.Clamp(areaEndZ, 0, _mapZ - 1);

    int lx, ly, lz, hx, hy, hz = 0;

    for (int y = 1; y < _mapY - 1; y++)
    {
      ly = y - 1;
      hy = y + 1;

      for (int x = areaStartX; x < areaEndX; x++)
      {
        lx = x - 1;
        hx = x + 1;

        for (int z = areaStartZ; z < areaEndZ; z++)
        {          
          // Skip if current block is air block
          if (_level[x, y, z].BlockType == GlobalConstants.BlockType.AIR)
          {
            continue;
          }

          lz = z - 1;
          hz = z + 1;

          // We cannot replace BlockId directly, since then on next loop iteration
          // the condition will fail.
          if (_level[lx, y, z].BlockType != GlobalConstants.BlockType.AIR &&  !_level[lx, y, z].IsLiquid 
            && _level[hx, y, z].BlockType != GlobalConstants.BlockType.AIR && !_level[hx, y, z].IsLiquid
            && _level[x, ly, z].BlockType != GlobalConstants.BlockType.AIR && !_level[x, ly, z].IsLiquid
            && _level[x, hy, z].BlockType != GlobalConstants.BlockType.AIR && !_level[x, hy, z].IsLiquid
            && _level[x, y, lz].BlockType != GlobalConstants.BlockType.AIR && !_level[x, y, lz].IsLiquid 
            && _level[x, y, hz].BlockType != GlobalConstants.BlockType.AIR && !_level[x, y, hz].IsLiquid)
          {
            _level[x, y, z].SkipInstantiation = true;
          }
        }
      }
    }
  }

  /// <summary>
  /// Sets blocks of given cube are to air.
  /// </summary>
  /// <param name="areaStartX">Area start x.</param>
  /// <param name="areaEndX">Area end x.</param>
  /// <param name="areaStartY">Area start y.</param>
  /// <param name="areaEndY">Area end y.</param>
  /// <param name="areaStartZ">Area start z.</param>
  /// <param name="areaEndZ">Area end z.</param>
  protected void ClearArea(int areaStartX, int areaEndX, 
                           int areaStartY, int areaEndY, 
                           int areaStartZ, int areaEndZ)
  {
    areaStartX = Mathf.Clamp(areaStartX, 0, _mapX - 1);
    areaEndX = Mathf.Clamp(areaEndX, 0, _mapX - 1);
    areaStartY = Mathf.Clamp(areaStartY, 0, _mapY - 1);
    areaEndY = Mathf.Clamp(areaEndY, 0, _mapY - 1);
    areaStartZ = Mathf.Clamp(areaStartZ, 0, _mapZ - 1);
    areaEndZ = Mathf.Clamp(areaEndZ, 0, _mapZ - 1);

    for (int y = areaStartY; y < areaEndY; y++)
    {
      for (int x = areaStartX; x < areaEndX; x++)
      {
        for (int z = areaStartZ; z < areaEndZ; z++)
        {          
          _level[x, y, z].BlockType = GlobalConstants.BlockType.AIR;
          _level[x, y, z].Walkable = true;
          _level[x, y, z].IsLiquid = false;
        }
      }
    }
  }

  /// <summary>
  /// Lowers liquid blocks down a little.
  /// </summary>
  void AdjustLiquidBlocks()
  {
    for (int y = 0; y < _mapY; y++)
    {
      for (int x = 0; x < _mapX; x++)
      {
        for (int z = 0; z < _mapZ; z++)
        {
          if (_level[x, y, z].IsLiquid)
          {
            float wx = _level[x, y, z].WorldCoordinates.x;
            float wy = _level[x, y, z].WorldCoordinates.y;
            float wz = _level[x, y, z].WorldCoordinates.z;

            _level[x, y, z].WorldCoordinates.Set(wx, wy - 0.3f, wz);
          }
        }
      }
    }
  }

  // ************************ WORLD GENERATION ************************ //

  /// <summary>
  /// Makes hill from layers of blocks on top of each other. Height should be odd.
  /// </summary>
  /// <param name="height">Hill height (should be odd)</param>
  protected void MakeHillLayered(GlobalConstants.BlockType blockType, Int3 arrayPos, int height)
  {
    int lx = arrayPos.X - height;
    int lz = arrayPos.Z - height;
    int hx = arrayPos.X + height;
    int hz = arrayPos.Z + height;

    lx = Mathf.Clamp(lx, 0, _mapX - 1);
    lz = Mathf.Clamp(lz, 0, _mapZ - 1);
    hx = Mathf.Clamp(hx, 0, _mapX - 1);
    hz = Mathf.Clamp(hz, 0, _mapZ - 1);

    int hy = 0;

    for (int h = 0; h < height; h++)
    {
      hy = h + arrayPos.Y;
      hy = Mathf.Clamp(hy, 0, _mapY - 1);

      for (int ax = lx + h; ax <= hx - h; ax++)
      {
        for (int ay = lz + h; ay <= hz - h; ay++)
        {               
          _level[ax, hy, ay].BlockType = blockType;
        }
      }
    }
  }

  protected void MakeHillQbert(int x, int z, int height)
  {
    int lx = x - 1;
    int lz = z - 1;
    int hx = x + 1;
    int hz = z + 1;

    if (height < 0 || lx < 0 || lz < 0 || hx >= _mapX - 1 || hz >= _mapZ - 1)
    {
      return;
    }

    if (_level[x, height, z].BlockType == GlobalConstants.BlockType.AIR)
    {
      _level[x, height, z].BlockType = GlobalConstants.BlockType.GRASS;
    }

    if (_level[lx, height, z].BlockType == GlobalConstants.BlockType.AIR)
    {
      _level[lx, height, z].BlockType = GlobalConstants.BlockType.GRASS;
    }

    if (_level[x, height, lz].BlockType == GlobalConstants.BlockType.AIR)
    {
      _level[x, height, lz].BlockType = GlobalConstants.BlockType.GRASS;
    }

    if (_level[hx, height, z].BlockType == GlobalConstants.BlockType.AIR)
    {
      _level[hx, height, z].BlockType = GlobalConstants.BlockType.GRASS;
    }

    if (_level[x, height, hz].BlockType == GlobalConstants.BlockType.AIR)
    {
      _level[x, height, hz].BlockType = GlobalConstants.BlockType.GRASS;
    }

    MakeHillQbert(lx, z, height - 1);
    MakeHillQbert(hx, z, height - 1);
    MakeHillQbert(x, lz, height - 1);
    MakeHillQbert(x, hz, height - 1);
  }

  protected WorldObject PlaceWorldObject(Int3 arrayPos, GlobalConstants.WorldObjectClass objectClass, GlobalConstants.WorldObjectPrefabType prefabType, GlobalConstants.Orientation orientation, WorldObject objectToControl = null)
  {
    string prefabStringName = GlobalConstants.WorldObjectPrefabByType[prefabType];

    if (PrefabsManager.Instance.FindPrefabByName(prefabStringName) == null)
    {
      Debug.LogWarning("Couldn't find prefab " + prefabStringName);
      return null;
    }

    WorldObject wo = null;

    switch (objectClass)
    {
      case GlobalConstants.WorldObjectClass.WALL:
        wo = new WallWorldObject(GlobalConstants.WorldObjectInGameNameByType[prefabType], prefabStringName);
        break;

      case GlobalConstants.WorldObjectClass.DOOR_OPENABLE:
        wo = new DoorWorldObject("", prefabStringName, false);
        (wo as DoorWorldObject).AnimationOpenSpeed = 4.0f;
        (wo as DoorWorldObject).AnimationCloseSpeed = 4.0f;
        wo.ActionCallback += wo.ActionHandler;
        break;

      case GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE:
        wo = new DoorWorldObject("", prefabStringName, false);
        (wo as DoorWorldObject).AnimationOpenSpeed = 4.0f;
        (wo as DoorWorldObject).AnimationCloseSpeed = 4.0f;
        break;

      case GlobalConstants.WorldObjectClass.LEVER:
        wo = new LeverWorldObject("", prefabStringName);
        (wo as LeverWorldObject).ControlledObject = objectToControl;
        (wo as LeverWorldObject).ActionCallback += (wo as LeverWorldObject).ActionHandler;
        (wo as LeverWorldObject).ActionCompleteCallback += objectToControl.ActionHandler;
        break;
    }

    if (wo != null)
    {      
      wo.ArrayCoordinates = arrayPos;
      wo.ObjectClass = objectClass;
      wo.ObjectOrientation = orientation;
      _level[arrayPos.X, arrayPos.Y, arrayPos.Z].WorldObjects.Add(wo);
      _level[arrayPos.X, arrayPos.Y, arrayPos.Z].ArrayCoordinates.Set(arrayPos.X, arrayPos.Y, arrayPos.Z);
      _level[arrayPos.X, arrayPos.Y, arrayPos.Z].WorldCoordinates.Set(arrayPos.X * GlobalConstants.WallScaleFactor, arrayPos.Y * GlobalConstants.WallScaleFactor, arrayPos.Z * GlobalConstants.WallScaleFactor);

      if (wo.ObjectClass != GlobalConstants.WorldObjectClass.PLACEHOLDER)
      {
        _level[arrayPos.X, arrayPos.Y, arrayPos.Z].SidesWalkability[orientation] = false;
      }
      else
      {
        _level[arrayPos.X, arrayPos.Y, arrayPos.Z].Walkable = false;
      }
    }

    return wo;
  }
}
