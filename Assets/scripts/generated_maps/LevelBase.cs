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

  protected void MakeHillCubed(GlobalConstants.BlockType blockType, Int3 arrayPos, int width, int height)
  {
    int lx = arrayPos.X - width;
    int lz = arrayPos.Z - width;
    int hx = arrayPos.X + width;
    int hz = arrayPos.Z + width;

    lx = Mathf.Clamp(lx, 0, _mapX - 1);
    lz = Mathf.Clamp(lz, 0, _mapZ - 1);
    hx = Mathf.Clamp(hx, 0, _mapX - 1);
    hz = Mathf.Clamp(hz, 0, _mapZ - 1);

    int hy = height + arrayPos.Y;
    hy = Mathf.Clamp(hy, 0, _mapY - 1);

    for (int h = arrayPos.Y; h < hy; h++)
    { 
      for (int ax = lx; ax <= hx; ax++)
      {
        for (int ay = lz; ay <= hz; ay++)
        {               
          if (blockType == GlobalConstants.BlockType.GRASS)
          {
            _level[ax, h, ay].BlockType = (h != hy - 1) ? GlobalConstants.BlockType.DIRT : GlobalConstants.BlockType.GRASS;
          }
          else
          {
            _level[ax, h, ay].BlockType = blockType;
          }
        }
      }
    }
  }

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

  // ********************************* World object placement methods ********************************* //

  /// <summary>
  /// Places placeholder type WorldObject (no wall sides blocking).
  /// </summary>
  protected WorldObject PlaceObject(Int3 arrayPos, GlobalConstants.WorldObjectPrefabType prefabType, GlobalConstants.Orientation orientation)
  {
    string prefabStringName = GlobalConstants.WorldObjectPrefabByType[prefabType];

    if (TryFindPrefab(prefabStringName))
    {
      WorldObject wo = new PlaceholderWorldObject(string.Empty, prefabStringName);
      SetWorldObjectParams(wo, arrayPos, GlobalConstants.WorldObjectClass.PLACEHOLDER, orientation);

      return wo;
    }

    return null;
  }

  // FIXME: Factually duplication of code except for WorldObject type. 
  // Think on universal design (introduce type of shrine enum?)

  protected WorldObject PlaceShrine(Int3 arrayPos, GlobalConstants.WorldObjectPrefabType prefabType, GlobalConstants.Orientation orientation)
  {
    string prefabStringName = GlobalConstants.WorldObjectPrefabByType[prefabType];

    if (TryFindPrefab(prefabStringName))
    {      
      WorldObject wo = new ShrineWorldObject(string.Empty, prefabStringName);
      (wo as ShrineWorldObject).ActionCallback += (wo as ShrineWorldObject).ActionHandler;
      _level[arrayPos.X, arrayPos.Y, arrayPos.Z].Walkable = false;
      SetWorldObjectParams(wo, arrayPos, GlobalConstants.WorldObjectClass.SHRINE, orientation);

      return wo;
    }

    return null;
  }

  protected WorldObject PlaceWall(Int3 arrayPos, GlobalConstants.WorldObjectPrefabType prefabType, GlobalConstants.Orientation orientation)
  {
    string prefabStringName = GlobalConstants.WorldObjectPrefabByType[prefabType];

    if (TryFindPrefab(prefabStringName))
    {
      WorldObject wo = new WallWorldObject(string.Empty, prefabStringName);
      wo.ArrayCoordinates.Set(arrayPos);
      wo.ObjectClass = GlobalConstants.WorldObjectClass.WALL;
      wo.ObjectOrientation = orientation;

      _level[arrayPos.X, arrayPos.Y, arrayPos.Z].SidesWalkability[orientation] = false;
      _level[arrayPos.X, arrayPos.Y, arrayPos.Z].WallsByOrientation[orientation] = wo as WallWorldObject;

      BlockEntity nextBlock = Utils.GetNextCellTowardsOrientation(arrayPos, orientation, this);
      if (nextBlock != null)
      {
        var o = Utils.GetOppositeOrientation(orientation);
        WallWorldObject sharedWall = new WallWorldObject(string.Empty, string.Empty);
        sharedWall.ArrayCoordinates = new Int3(nextBlock.ArrayCoordinates);
        sharedWall.ObjectClass = GlobalConstants.WorldObjectClass.WALL;
        sharedWall.ObjectOrientation = o;
        nextBlock.WallsByOrientation[o] = sharedWall;
        nextBlock.SidesWalkability[o] = false;
      }

      _level[arrayPos.X, arrayPos.Y, arrayPos.Z].ArrayCoordinates.Set(arrayPos.X, arrayPos.Y, arrayPos.Z);
      _level[arrayPos.X, arrayPos.Y, arrayPos.Z].WorldCoordinates.Set(arrayPos.X * GlobalConstants.WallScaleFactor, arrayPos.Y * GlobalConstants.WallScaleFactor, arrayPos.Z * GlobalConstants.WallScaleFactor);

      return wo;
    }

    return null;
  }

  /// <summary>
  /// Places door world object.
  /// </summary>
  /// <returns>WorldObject reference</returns>
  /// <param name="arrayPos">Array position</param>
  /// <param name="prefabType">Prefab type</param>
  /// <param name="orientation">Orientation</param>
  /// <param name="canBeOpenedByHand">If set to <c>true</c> can be opened by hand - door action callback is subscribed to its own handler.</param>
  /// <param name="isSlidingAnimated">If set to <c>true</c> indicates that door is "sliding" type, 
  /// which determines walkability of the corresponding cell sides: sliding doors (e.g. portcullis) set own side to true/false, 
  /// while swing doors set side where door was before opening to true/false and where door is now to false/true.
  /// </param>
  /// <param name="animationOpenSpeed">Opening animation speed</param>
  /// <param name="animationCloseSpeed">Closing animation speed</param>
  protected WorldObject PlaceDoor(Int3 arrayPos, GlobalConstants.WorldObjectPrefabType prefabType, GlobalConstants.Orientation orientation, bool canBeOpenedByHand, bool isSlidingAnimated, float animationOpenSpeed = 1.0f, float animationCloseSpeed = 1.0f)
  { 
    string prefabStringName = GlobalConstants.WorldObjectPrefabByType[prefabType];

    if (TryFindPrefab(prefabStringName))
    {
      WorldObject wo = new DoorWorldObject(string.Empty, prefabStringName);
      (wo as DoorWorldObject).AnimationOpenSpeed = animationOpenSpeed;
      (wo as DoorWorldObject).AnimationCloseSpeed = animationCloseSpeed;
      (wo as DoorWorldObject).IsSliding = isSlidingAnimated;

      _level[arrayPos.X, arrayPos.Y, arrayPos.Z].SidesWalkability[orientation] = false;

      if (canBeOpenedByHand)
      {
        (wo as DoorWorldObject).ActionCallback += (wo as DoorWorldObject).ActionHandler;
        SetWorldObjectParams(wo, arrayPos, GlobalConstants.WorldObjectClass.DOOR_OPENABLE, orientation);
      }
      else
      {
        SetWorldObjectParams(wo, arrayPos, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE, orientation);
      }

      return wo;
    }  

    return null;
  }

  protected WorldObject PlaceControl(Int3 arrayPos, GlobalConstants.WorldObjectClass controlClass, GlobalConstants.WorldObjectPrefabType prefabType, GlobalConstants.Orientation orientation, params WorldObject[] objectsToControl)
  {
    string prefabStringName = GlobalConstants.WorldObjectPrefabByType[prefabType];

    if (TryFindPrefab(prefabStringName))
    {
      WorldObject wo = null;
        
      switch (controlClass)
      {
        case GlobalConstants.WorldObjectClass.LEVER:
          wo = new LeverWorldObject(string.Empty, prefabStringName);
          (wo as LeverWorldObject).ActionCallback += (wo as LeverWorldObject).ActionHandler;

          (wo as LeverWorldObject).AssignControlledObjects(objectsToControl);

          SetWorldObjectParams(wo, arrayPos, GlobalConstants.WorldObjectClass.LEVER, orientation);

          break;

        case GlobalConstants.WorldObjectClass.BUTTON:
          wo = new ButtonWorldObject(string.Empty, prefabStringName);
          (wo as ButtonWorldObject).ActionCallback += (wo as ButtonWorldObject).ActionHandler;

          (wo as ButtonWorldObject).ControlledObjects = objectsToControl;

          foreach (var obj in objectsToControl)
          {
            (wo as ButtonWorldObject).ActionCompleteCallback += obj.ActionHandler;
          }

          SetWorldObjectParams(wo, arrayPos, GlobalConstants.WorldObjectClass.BUTTON, orientation);

          break;
      }

      return wo;
    }

    return null;
  }

  protected WorldObject PlaceSign(Int3 arrayPos, GlobalConstants.WorldObjectPrefabType prefabType, GlobalConstants.Orientation orientation, string textToDisplay)
  {
    string prefabStringName = GlobalConstants.WorldObjectPrefabByType[prefabType];

    if (TryFindPrefab(prefabStringName))
    {
      WorldObject wo = new SignWorldObject(string.Empty, prefabStringName);
      (wo as SignWorldObject).SignText = textToDisplay;

      SetWorldObjectParams(wo, arrayPos, GlobalConstants.WorldObjectClass.SIGN, orientation);

      return wo;
    }

    return null;
  }

  protected void PlaceTeleporter(Int3 arrayPos, Int3 destination)
  {
    string prefabStringName = GlobalConstants.WorldObjectPrefabByType[GlobalConstants.WorldObjectPrefabType.TELEPORTER];

    if (TryFindPrefab(prefabStringName))
    {
      WorldObject wo = new TeleporterWorldObject(string.Empty, prefabStringName);

      (wo as TeleporterWorldObject).CoordinatesToTeleport = destination;

      wo.ArrayCoordinates = arrayPos;

      _level[arrayPos.X, arrayPos.Y, arrayPos.Z].ArrayCoordinates.Set(arrayPos.X, arrayPos.Y, arrayPos.Z);
      _level[arrayPos.X, arrayPos.Y, arrayPos.Z].WorldCoordinates.Set(arrayPos.X * GlobalConstants.WallScaleFactor, arrayPos.Y * GlobalConstants.WallScaleFactor, arrayPos.Z * GlobalConstants.WallScaleFactor);
      _level[arrayPos.X, arrayPos.Y, arrayPos.Z].Teleporter = wo as TeleporterWorldObject;
    }
  }

  // ********************************* Helper Functions ********************************* //

  bool TryFindPrefab(string prefabStringName)
  {    
    if (PrefabsManager.Instance.FindPrefabByName(prefabStringName) == null)
    {
      Debug.LogWarning("Couldn't find prefab " + prefabStringName);
      return false;
    }

    return true;
  }

  /// <summary>
  /// Sets world object common parameters.
  /// </summary>
  /// <param name="wo">WorldObject to modify</param>
  /// <param name="arrayPos">Array coordinates</param>
  /// <param name="objectClass">Object class</param>
  /// <param name="orientation">Orientation</param>
  void SetWorldObjectParams(WorldObject wo, Int3 arrayPos, GlobalConstants.WorldObjectClass objectClass, GlobalConstants.Orientation orientation)
  {
    // Avoid reference passing via = for complex types
    wo.ArrayCoordinates.Set(arrayPos);

    wo.ObjectClass = objectClass;
    wo.ObjectOrientation = orientation;

    _level[arrayPos.X, arrayPos.Y, arrayPos.Z].WorldObjects.Add(wo);
    _level[arrayPos.X, arrayPos.Y, arrayPos.Z].ArrayCoordinates.Set(arrayPos.X, arrayPos.Y, arrayPos.Z);
    _level[arrayPos.X, arrayPos.Y, arrayPos.Z].WorldCoordinates.Set(arrayPos.X * GlobalConstants.WallScaleFactor, arrayPos.Y * GlobalConstants.WallScaleFactor, arrayPos.Z * GlobalConstants.WallScaleFactor);
  }
}
