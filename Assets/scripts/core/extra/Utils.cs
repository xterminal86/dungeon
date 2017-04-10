using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utils
{
  public static int BlockDistance(Int2 from, Int2 to)
  {
    return Mathf.Abs(to.Y - from.Y) + Mathf.Abs(to.X - from.X);
  }	

  /// <summary>
  /// Hide appropriate sides of a block to prevent side doubling and lighting artifacts (also to reduce draw calls).
  /// </summary>
  /// <param name="blockSidesList">Block to operate on</param>
  /// <param name="coordinates">Current block coordinates</param>
  /// <param name="level">LevelBase based class</param>
  public static void HideLevelBlockSides(MinecraftBlock block, Vector3 coordinates, LevelBase level)
  {
    Vector3[] arrayCoordinatesAdds = new Vector3[6] 
    { 
      new Vector3(-1.0f, 0.0f, 0.0f),
      new Vector3(1.0f, 0.0f, 0.0f),
      new Vector3(0.0f, -1.0f, 0.0f),
      new Vector3(0.0f, 1.0f, 0.0f),
      new Vector3(0.0f, 0.0f, -1.0f),
      new Vector3(0.0f, 0.0f, 1.0f),
    };

    int x = (int)coordinates.x;
    int y = (int)coordinates.y;
    int z = (int)coordinates.z;

    for (int side = 0; side < 6; side++)
    { 
      int nx = x + (int)arrayCoordinatesAdds[side][0];
      int ny = y + (int)arrayCoordinatesAdds[side][1];
      int nz = z + (int)arrayCoordinatesAdds[side][2];

      // Check for map size limits
      if (nx != x && nx >= 0 && nx <= level.MapX - 1)
      {
        // Disable shadow receiving if submerged
        if (!level.Level[x, y, z].IsLiquid && level.Level[nx, y, z].IsLiquid)
        {
          if ((int)arrayCoordinatesAdds[side][0] == -1)
          {
            block.LeftQuadRenderer.receiveShadows = false;
          }
          else if ((int)arrayCoordinatesAdds[side][0] == 1)
          {
            block.RightQuadRenderer.receiveShadows = false;
          }
        }

        // Hide corresponding side of the current block if:
        //
        // 1) Current block is solid and neighbouring block is solid
        // 2) Current block is liquid and neighbouring block is liquid
        // 3) Current block is liquid and neighbouring block is solid

        if ((!level.Level[x, y, z].IsLiquid && !level.Level[nx, y, z].IsLiquid && level.Level[nx, y, z].BlockType != GlobalConstants.BlockType.AIR)
          || (level.Level[x, y, z].IsLiquid &&  level.Level[nx, y, z].IsLiquid)
          || (level.Level[x, y, z].IsLiquid && !level.Level[nx, y, z].IsLiquid))
        {
          if ((int)arrayCoordinatesAdds[side][0] == -1)
          {
            block.LeftQuad.gameObject.SetActive(false);
          }
          else if ((int)arrayCoordinatesAdds[side][0] == 1)
          {
            block.RightQuad.gameObject.SetActive(false);
          }
        }
      }

      if (nz != z && nz >= 0 && nz <= level.MapZ - 1)
      {
        // Disable shadow casting if submerged
        if (!level.Level[x, y, z].IsLiquid && level.Level[x, y, nz].IsLiquid)
        {
          if ((int)arrayCoordinatesAdds[side][2] == -1)
          {
            block.ForwardQuadRenderer.receiveShadows = false;
          }
          else if ((int)arrayCoordinatesAdds[side][2] == 1)
          {
            block.BackQuadRenderer.receiveShadows = false;
          }
        }

        if ((!level.Level[x, y, z].IsLiquid && !level.Level[x, y, nz].IsLiquid && level.Level[x, y, nz].BlockType != GlobalConstants.BlockType.AIR)
          || (level.Level[x, y, z].IsLiquid && level.Level[x, y, nz].IsLiquid)
          || (level.Level[x, y, z].IsLiquid && !level.Level[x, y, nz].IsLiquid))
        {
          if ((int)arrayCoordinatesAdds[side][2] == -1)
          {
            block.ForwardQuad.gameObject.SetActive(false);
          }
          else if ((int)arrayCoordinatesAdds[side][2] == 1)
          {
            block.BackQuad.gameObject.SetActive(false);
          }
        }
      }

      // arrayCoordinatesAdds[side][1] == -1 means that ny will be less that current block y,
      // thus below

      if (ny != y && ny >= 0 && ny <= level.MapY - 1)
      {        
        // Disable shadow casting if submerged
        if (!level.Level[x, y, z].IsLiquid && level.Level[x, ny, z].IsLiquid)
        {
          // If current block is above liquid block
          if ((int)arrayCoordinatesAdds[side][1] == -1)
          {
            // Disable shadow receiving of down quad
            block.DownQuadRenderer.receiveShadows = false;
          }
          else if ((int)arrayCoordinatesAdds[side][1] == 1)
          {
            block.UpQuadRenderer.receiveShadows = false;
          }
        }

        if ((!level.Level[x, y, z].IsLiquid && !level.Level[x, ny, z].IsLiquid && level.Level[x, ny, z].BlockType != GlobalConstants.BlockType.AIR)
          || (level.Level[x, y, z].IsLiquid && level.Level[x, ny, z].IsLiquid))
        {          
          // If ny is lower block
          if ((int)arrayCoordinatesAdds[side][1] == -1)
          {            
            // Hide down side of current block
            block.DownQuad.gameObject.SetActive(false);
          }
          else if ((int)arrayCoordinatesAdds[side][1] == 1)
          {
            block.UpQuad.gameObject.SetActive(false);
          }
        }
      }
    }
  }

  static Int3 _nextCellCoordsTowardsCurrentOrientation = Int3.Zero;
  public static BlockEntity GetNextCellTowardsOrientation(Int3 currentPos, GlobalConstants.Orientation currentOrientation, LevelBase level)
  {
    _nextCellCoordsTowardsCurrentOrientation.Set(currentPos);

    // South - X+, East - Z+

    if (currentOrientation == GlobalConstants.Orientation.NORTH)
    {
      _nextCellCoordsTowardsCurrentOrientation.X--;
    }
    else if (currentOrientation == GlobalConstants.Orientation.EAST)
    {
      _nextCellCoordsTowardsCurrentOrientation.Z++;
    }
    else if (currentOrientation == GlobalConstants.Orientation.SOUTH)
    {
      _nextCellCoordsTowardsCurrentOrientation.X++;
    }
    else if (currentOrientation == GlobalConstants.Orientation.WEST)
    {
      _nextCellCoordsTowardsCurrentOrientation.Z--;
    }

    if (_nextCellCoordsTowardsCurrentOrientation.X >= 0 && _nextCellCoordsTowardsCurrentOrientation.X < level.MapX
      && _nextCellCoordsTowardsCurrentOrientation.Z >= 0 && _nextCellCoordsTowardsCurrentOrientation.Z < level.MapZ)
    {
      return level.Level[_nextCellCoordsTowardsCurrentOrientation.X, _nextCellCoordsTowardsCurrentOrientation.Y, _nextCellCoordsTowardsCurrentOrientation.Z];
    }

    return null;
  }

  public static GlobalConstants.Orientation GetOppositeOrientation(GlobalConstants.Orientation orientation)
  {
    int oppositeOrientation = (int)orientation;

    oppositeOrientation += 2;
    oppositeOrientation %= 4;

    return (GlobalConstants.Orientation)oppositeOrientation;
  }

  public static void SetWallColumns(WallWorldObject wall, LevelBase level)
  {    
    int x = wall.ArrayCoordinates.X;
    int y = wall.ArrayCoordinates.Y;
    int z = wall.ArrayCoordinates.Z;

    int lx = wall.ArrayCoordinates.X - 1;
    int lz = wall.ArrayCoordinates.Z - 1;
    int hx = wall.ArrayCoordinates.X + 1;
    int hz = wall.ArrayCoordinates.Z + 1;

    GlobalConstants.Orientation wallOrientation = wall.ObjectOrientation;
    GlobalConstants.Orientation perpendicularOrientation1 = GlobalConstants.Orientation.EAST;
    GlobalConstants.Orientation perpendicularOrientation2 = GlobalConstants.Orientation.EAST;

    bool parallelWall = false;
    WorldObject parallelDoorType1 = null;
    WorldObject parallelDoorType2 = null;
    bool perpendicularWallCurrent = false;
    bool perpendicularWallNext = false;

    // South - X+, East - Z+

    SetColumnsExtractedMethod(wall, level);

    /*
    if (wallOrientation == GlobalConstants.Orientation.EAST)
    {
      perpendicularOrientation1 = GlobalConstants.Orientation.NORTH;
      perpendicularOrientation2 = GlobalConstants.Orientation.SOUTH;

      if (lx >= 0 && hz < level.MapZ)
      {
        parallelWall = (level.Level[lx, y, z].WallsByOrientation[wallOrientation] != null);
        parallelDoorType1 = FindObject(level.Level[lx, y, z].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);
        parallelDoorType2 = FindObject(level.Level[lx, y, z].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
        perpendicularWallCurrent = (level.Level[x, y, z].WallsByOrientation[perpendicularOrientation1] != null);
        perpendicularWallNext = (level.Level[x, y, hz].WallsByOrientation[perpendicularOrientation1] != null);

        if ( (parallelWall || parallelDoorType1 != null || parallelDoorType2 != null)
          || (perpendicularWallCurrent && perpendicularWallNext))
        {
          wall.BWO.WallColumnLeft.gameObject.SetActive(false);
        }
      }

      if (hx < level.MapX && hz < level.MapZ)
      {
        if (level.Level[hx, y, z].WallsByOrientation[wallOrientation] != null
          || (level.Level[x, y, z].WallsByOrientation[perpendicularOrientation2] != null
          && level.Level[x, y, hz].WallsByOrientation[perpendicularOrientation2] != null))
        {
          wall.BWO.WallColumnRight.gameObject.SetActive(false);
        }
      }
    }
    else if (wallOrientation == GlobalConstants.Orientation.SOUTH)
    {
    }
    else if (wallOrientation == GlobalConstants.Orientation.WEST)
    {
    }
    else if (wallOrientation == GlobalConstants.Orientation.NORTH)
    {
    }
    */
  }

  static Dictionary<GlobalConstants.Orientation, Int2> _offsetsForParallelLeft = new Dictionary<GlobalConstants.Orientation, Int2>()
  {
    { GlobalConstants.Orientation.EAST, new Int2(-1, 0) },
    { GlobalConstants.Orientation.SOUTH, new Int2(0, 1) },
    { GlobalConstants.Orientation.WEST, new Int2(1, 0) },
    { GlobalConstants.Orientation.NORTH, new Int2(0, -1) }
  };

  static Dictionary<GlobalConstants.Orientation, Int2> _offsetsForParallelRight = new Dictionary<GlobalConstants.Orientation, Int2>()
  {
    { GlobalConstants.Orientation.EAST, new Int2(1, 0) },
    { GlobalConstants.Orientation.SOUTH, new Int2(0, -1) },
    { GlobalConstants.Orientation.WEST, new Int2(-1, 0) },
    { GlobalConstants.Orientation.NORTH, new Int2(0, 1) }
  };

  static void SetColumnsExtractedMethod(WallWorldObject wallToCheck, LevelBase level)
  {
    GlobalConstants.Orientation wallOrientation = wallToCheck.ObjectOrientation;

    int pol = ((int)wallOrientation - 1) == -1 ? 3 : ((int)wallOrientation - 1);
    int por = ((int)wallOrientation + 1) % 4;

    GlobalConstants.Orientation perpendicularOrientationLeft = (GlobalConstants.Orientation)pol;
    GlobalConstants.Orientation perpendicularOrientationRight = (GlobalConstants.Orientation)por;

    int x = wallToCheck.ArrayCoordinates.X;
    int y = wallToCheck.ArrayCoordinates.Y;
    int z = wallToCheck.ArrayCoordinates.Z;

    int lpnx = x + _offsetsForParallelLeft[wallOrientation].X;
    int lpnz = z + _offsetsForParallelLeft[wallOrientation].Y;
    int rpnx = x + _offsetsForParallelRight[wallOrientation].X;
    int rpnz = z + _offsetsForParallelRight[wallOrientation].Y;

    bool parallelWallLeft = (lpnx >= 0 && lpnx < level.MapX && lpnz >= 0 && lpnz < level.MapZ) ? (level.Level[lpnx, y, lpnz].WallsByOrientation[wallOrientation] != null) : false;
    bool parallelWallRight = (rpnx >= 0 && rpnx < level.MapX && rpnz >= 0 && rpnz < level.MapZ) ? (level.Level[rpnx, y, rpnz].WallsByOrientation[wallOrientation] != null) : false;
    bool perpendicularWallCurrentLeft = (level.Level[x, y, z].WallsByOrientation[perpendicularOrientationLeft] != null);
    bool perpendicularWallCurrentRight = (level.Level[x, y, z].WallsByOrientation[perpendicularOrientationRight] != null);
    BlockEntity nextBlock = GetNextCellTowardsOrientation(wallToCheck.ArrayCoordinates, wallOrientation, level);
    bool perpendicularWallNextLeft = (nextBlock != null) ? (nextBlock.WallsByOrientation[perpendicularOrientationLeft] != null) : false;
    bool perpendicularWallNextRight = (nextBlock != null) ? (nextBlock.WallsByOrientation[perpendicularOrientationRight] != null) : false;
    WorldObject parallelLeftDoorType1 = (lpnx >= 0 && lpnx < level.MapX && lpnz >= 0 && lpnz < level.MapZ) ? FindObject(level.Level[lpnx, y, lpnz].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.DOOR_OPENABLE) : null;
    WorldObject parallelLeftDoorType2 = (lpnx >= 0 && lpnx < level.MapX && lpnz >= 0 && lpnz < level.MapZ) ? FindObject(level.Level[lpnx, y, lpnz].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE) : null;
    WorldObject parallelRightDoorType1 = (rpnx >= 0 && rpnx < level.MapX && rpnz >= 0 && rpnz < level.MapZ) ? FindObject(level.Level[rpnx, y, rpnz].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.DOOR_OPENABLE) : null;
    WorldObject parallelRightDoorType2 = (rpnx >= 0 && rpnx < level.MapX && rpnz >= 0 && rpnz < level.MapZ) ? FindObject(level.Level[rpnx, y, rpnz].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE) : null;

    if ((parallelWallLeft || (parallelLeftDoorType1 != null || parallelLeftDoorType2 != null)) || (perpendicularWallCurrentLeft && perpendicularWallNextLeft))
    {      
      wallToCheck.BWO.WallColumnLeft.gameObject.SetActive(false);
    }

    if ((parallelWallRight || (parallelRightDoorType1 != null || parallelRightDoorType2 != null)) || (perpendicularWallCurrentRight && perpendicularWallNextRight))
    {      
      wallToCheck.BWO.WallColumnRight.gameObject.SetActive(false);
    }
  }

  public static void CheckPerpendicularWallsForColumns(WallWorldObject wallToCheck, LevelBase level)
  {
    GlobalConstants.Orientation wallOrientation = wallToCheck.ObjectOrientation;

    int pol = ((int)wallOrientation - 1) == -1 ? 3 : ((int)wallOrientation - 1);
    int por = ((int)wallOrientation + 1) % 4;

    GlobalConstants.Orientation perpendicularOrientationLeft = (GlobalConstants.Orientation)pol;
    GlobalConstants.Orientation perpendicularOrientationRight = (GlobalConstants.Orientation)por;

    int x = wallToCheck.ArrayCoordinates.X;
    int y = wallToCheck.ArrayCoordinates.Y;
    int z = wallToCheck.ArrayCoordinates.Z;

    bool perpendicularWallCurrentLeft = (level.Level[x, y, z].WallsByOrientation[perpendicularOrientationLeft] != null);
    bool perpendicularWallCurrentRight = (level.Level[x, y, z].WallsByOrientation[perpendicularOrientationRight] != null);

    if (perpendicularWallCurrentLeft)
    {
      BehaviourWorldObject bwol = level.Level[x, y, z].WallsByOrientation[perpendicularOrientationLeft].BWO;

      if (bwol.WallColumnRight.gameObject.activeSelf)
      {
        wallToCheck.BWO.WallColumnLeft.gameObject.SetActive(false);
      }
    }

    if (perpendicularWallCurrentRight)
    {
      BehaviourWorldObject bwor = level.Level[x, y, z].WallsByOrientation[perpendicularOrientationRight].BWO;

      if (bwor.WallColumnLeft.gameObject.activeSelf)
      {
        wallToCheck.BWO.WallColumnRight.gameObject.SetActive(false);
      }
    }
  }

  /// <summary>
  /// Hides the wall sides if it has adjacent walls to save draw calls and minimize lighting artefacts.
  /// </summary>
  public static void HideWallSides(WallWorldObject wall, LevelBase level)
  {     
    // Increment of X is equivalent of going to the south, increment of Z - east.
    // So, we have four cases: neighbouring Xs for east and west walls, and neighbouring Zs for south and north.

    TryHideLeftRightSides(wall, level);
    TryHideTopBottomSides(wall, level);
  }

  /// <summary>
  /// If we have neighbouring walls, hide appropriate lateral sides of the current wall.
  /// This is to reduce draw calls and get rid of potential lighting artefacts:
  /// if you have two cubes adjacent to each other, at their "shared" side there will be situations
  /// when lighting can sometimes highlight "shared" side of the adjacent cube, which result in a seam that
  /// clearly shows that two objects are separate.
  /// \
  ///  \
  ///   \ here
  ///    |\ _
  ///    | |_|
  ///    |/
  ///   /
  ///  /
  /// /
  /// </summary>
  static void TryHideLeftRightSides(WallWorldObject wall, LevelBase level)
  {
    int x = wall.ArrayCoordinates.X;
    int y = wall.ArrayCoordinates.Y;
    int z = wall.ArrayCoordinates.Z;

    int lx = wall.ArrayCoordinates.X - 1;
    int lz = wall.ArrayCoordinates.Z - 1;
    int hx = wall.ArrayCoordinates.X + 1;
    int hz = wall.ArrayCoordinates.Z + 1;

    GlobalConstants.Orientation wallOrientation = wall.ObjectOrientation;

    // South - X+, East - Z+

    if (wallOrientation == GlobalConstants.Orientation.EAST)
    {
      if (lx >= 0 && level.Level[lx, y, z].WallsByOrientation[wallOrientation] != null)
      {
        wall.BWO.LeftQuad.gameObject.SetActive(false);
      }

      if (hx < level.MapX && level.Level[hx, y, z].WallsByOrientation[wallOrientation] != null)
      {
        wall.BWO.RightQuad.gameObject.SetActive(false);
      }
    }
    else if (wallOrientation == GlobalConstants.Orientation.SOUTH)
    {
      if (lz >= 0 && level.Level[x, y, lz].WallsByOrientation[wallOrientation] != null)
      {
        wall.BWO.RightQuad.gameObject.SetActive(false);
      }

      if (hz < level.MapZ && level.Level[x, y, hz].WallsByOrientation[wallOrientation] != null)
      {
        wall.BWO.LeftQuad.gameObject.SetActive(false);
      }
    }
    else if (wallOrientation == GlobalConstants.Orientation.WEST)
    {      
      if (lx >= 0 && level.Level[lx, y, z].WallsByOrientation[wallOrientation] != null)
      {
        wall.BWO.RightQuad.gameObject.SetActive(false);
      }

      if (hx < level.MapX && level.Level[hx, y, z].WallsByOrientation[wallOrientation] != null)
      {
        wall.BWO.LeftQuad.gameObject.SetActive(false);
      }
    }
    else if (wallOrientation == GlobalConstants.Orientation.NORTH)
    {
      if (lz >= 0 && level.Level[x, y, lz].WallsByOrientation[wallOrientation] != null)
      {
        wall.BWO.LeftQuad.gameObject.SetActive(false);
      }

      if (hz < level.MapZ && level.Level[x, y, hz].WallsByOrientation[wallOrientation] != null)
      {
        wall.BWO.RightQuad.gameObject.SetActive(false);
      }
    }
  }

  /// <summary>
  /// If we have neighbouring walls, hide appropriate top and bottom sides of the current wall.
  /// </summary>
  static void TryHideTopBottomSides(WallWorldObject wall, LevelBase level)
  {
    int x = wall.ArrayCoordinates.X;
    int y = wall.ArrayCoordinates.Y;
    int z = wall.ArrayCoordinates.Z;

    int ly = wall.ArrayCoordinates.Y - 1;
    int hy = wall.ArrayCoordinates.Y + 1;

    GlobalConstants.Orientation wallOrientation = wall.ObjectOrientation;

    WorldObject res1 = null;
    WorldObject res2 = null;
    WorldObject res3 = null;

    if (ly >= 0)
    {
      if (level.Level[x, ly, z].WallsByOrientation[wallOrientation] != null)
      {
        wall.BWO.BottomQuad.gameObject.SetActive(false);
      }
      else
      {
        // To disable Z fighting of door frame with bottom side of the wall, handle this case specifically.

        res2 = FindObject(level.Level[x, ly, z].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
        res3 = FindObject(level.Level[x, ly, z].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);

        if (res2 != null || res3 != null)
        {          
          wall.BWO.BottomQuad.gameObject.SetActive(false);
        }
      }
    }

    if (hy < level.MapY)
    {
      if (level.Level[x, hy, z].WallsByOrientation[wallOrientation] != null)
      {
        wall.BWO.TopQuad.gameObject.SetActive(false);
      }
    }
  }

  static WorldObject FindObject(List<WorldObject> worldObjects, GlobalConstants.Orientation orientation, GlobalConstants.WorldObjectClass objectClass)
  {
    if (worldObjects == null)
    {
      return null;
    }
    
    foreach (var item in worldObjects)
    {
      if ( item.ObjectClass == objectClass && item.ObjectOrientation == orientation)
      {
        return item;
      }
    }

    return null;
  }
}
