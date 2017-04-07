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
    /*
    Int3 c = wall.ArrayCoordinates;

    GlobalConstants.Orientation wallOrientation = wall.ObjectOrientation;

    int lx = c.X - 1;
    int hx = c.X + 1;
    int ly = c.Y - 1;
    int hy = c.Y + 1;
    int lz = c.Z - 1;
    int hz = c.Z + 1;

    // Since HasWall is shared between blocks, we can use the current one.

    GlobalConstants.Orientation perpendicularOrientation = (GlobalConstants.Orientation)(((int)wallOrientation + 1) % 4);

    BlockEntity nextBlock = GetNextCellTowardsOrientation(c, wallOrientation, level);
    BlockEntity parallelBlock = GetNextCellTowardsOrientation(c, perpendicularOrientation, level);

    if (nextBlock != null && parallelBlock != null)
    {
      if (!nextBlock.HasWall[perpendicularOrientation]
        && !level.Level[c.X, c.Y, c.Z].HasWall[perpendicularOrientation]
        && !parallelBlock.HasWall[wallOrientation])
      {
        wall.BWO.WallColumnRight.gameObject.SetActive(true);
      }
    }
    */
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

    WorldObject res = null;

    if (lx >= 0)
    {
      res = DetectObject(level.Level[lx, y, z].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.WALL);

      if (res != null)
      {
        if (wallOrientation == GlobalConstants.Orientation.EAST)
        {
          wall.BWO.LeftQuad.gameObject.SetActive(false);
        }
        else if (wallOrientation == GlobalConstants.Orientation.WEST)
        {
          wall.BWO.RightQuad.gameObject.SetActive(false);
        }
      }
    }

    if (hx < level.MapX)
    {
      res = DetectObject(level.Level[hx, y, z].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.WALL);

      if (res != null)
      {
        if (wallOrientation == GlobalConstants.Orientation.EAST)
        {
          wall.BWO.RightQuad.gameObject.SetActive(false);
        }
        else if (wallOrientation == GlobalConstants.Orientation.WEST)
        {
          wall.BWO.LeftQuad.gameObject.SetActive(false);
        }
      }
    }

    if (lz >= 0)
    {      
      res = DetectObject(level.Level[x, y, lz].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.WALL);

      if (res != null)
      {
        if (wallOrientation == GlobalConstants.Orientation.SOUTH)
        {
          wall.BWO.RightQuad.gameObject.SetActive(false);
        }
        else if (wallOrientation == GlobalConstants.Orientation.NORTH)
        {
          wall.BWO.LeftQuad.gameObject.SetActive(false);
        }
      }
    }

    if (hz < level.MapZ)
    {
      res = DetectObject(level.Level[x, y, hz].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.WALL);

      if (res != null)
      {
        if (wallOrientation == GlobalConstants.Orientation.SOUTH)
        {
          wall.BWO.LeftQuad.gameObject.SetActive(false);
        }
        else if (wallOrientation == GlobalConstants.Orientation.NORTH)
        {
          wall.BWO.RightQuad.gameObject.SetActive(false);
        }
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
      res1 = DetectObject(level.Level[x, ly, z].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.WALL);

      if (res1 != null)
      {
        wall.BWO.BottomQuad.gameObject.SetActive(false);
      }
      else
      {
        // To disable Z fighting of door frame with bottom side of the wall, handle this case specifically.

        res2 = DetectObject(level.Level[x, ly, z].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
        res3 = DetectObject(level.Level[x, ly, z].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);

        if (res2 != null || res3 != null)
        {
          wall.BWO.BottomQuad.gameObject.SetActive(false);
        }
      }        
    }

    if (hy < level.MapY)
    {
      res1 = DetectObject(level.Level[x, hy, z].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.WALL);

      if (res1 != null)
      {
        wall.BWO.TopQuad.gameObject.SetActive(false);
      }
    }
  }

  static WorldObject DetectObject(List<WorldObject> worldObjects, GlobalConstants.Orientation orientation, GlobalConstants.WorldObjectClass objectClass)
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
