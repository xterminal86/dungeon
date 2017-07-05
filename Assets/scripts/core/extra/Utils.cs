using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utils
{
  public static int BlockDistance(Int2 from, Int2 to)
  {
    return Mathf.Abs(to.Y - from.Y) + Mathf.Abs(to.X - from.X);
  }	

  public static int BlockDistance(Int3 from, Int3 to)
  {
    return Mathf.Abs(to.X - from.X) + Mathf.Abs(to.Z - from.Z);
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

  /// <summary>
  /// Hides ending columns of a wall depending on a wall position relative
  /// to neighbouring walls.
  /// </summary>
  public static void CheckAndHideWallColumns(WallWorldObject wallToCheck, LevelBase level)
  { 
    // We should hide appropriate column of a wall if there is parallel wall or door
    // or perpendicular wall or door, also taking into account that they can be placed differently:
    // e.g. wall oriented east looks the same as next block wall oriented west.
    //
    // After columns check is done, we should check for duplicate columns.
    // This happens when you place walls in a cube fashion (all on one block facing 4 sides).
    // In this case both conditions (no parallel wall or one of perpendicular walls) fail which
    // will result in both columns visible for all 4 walls.
    // Thus we need to check neighbouring perpendicular walls and see if they already have 
    // respective column "on", in which case we must hide current wall columns to prevent duplication.
    // 
    // It's awkward and code looks like shit, but I wasn't able to figure out another way. :-(

    // South - X+, East - Z+

    GlobalConstants.Orientation wallOrientation = wallToCheck.ObjectOrientation;
    GlobalConstants.Orientation wallOppositeOrientation = Utils.GetOppositeOrientation(wallOrientation);

    int pol = ((int)wallOrientation - 1) == -1 ? 3 : ((int)wallOrientation - 1);
    int por = ((int)wallOrientation + 1) % 4;

    GlobalConstants.Orientation perpendicularOrientationLeft = (GlobalConstants.Orientation)pol;
    GlobalConstants.Orientation perpendicularOrientationRight = (GlobalConstants.Orientation)por;

    int x = wallToCheck.ArrayCoordinates.X;
    int y = wallToCheck.ArrayCoordinates.Y;
    int z = wallToCheck.ArrayCoordinates.Z;

    Int3 blockCoordinatesLeft = Int3.Zero;
    Int3 blockCoordinatesRight = Int3.Zero;

    BlockEntity currentBlock = level.Level[x, y, z];
    BlockEntity nextBlock = GetNextCellTowardsOrientation(wallToCheck.ArrayCoordinates, wallOrientation, level);

    BlockEntity leftBlock = GetNextCellTowardsOrientation(wallToCheck.ArrayCoordinates, perpendicularOrientationLeft, level);
    BlockEntity leftBlockNext = null;
    if (leftBlock != null)
    {
      blockCoordinatesLeft.Set(leftBlock.ArrayCoordinates);

      leftBlockNext = GetNextCellTowardsOrientation(blockCoordinatesLeft, wallOrientation, level);
    }

    BlockEntity rightBlock = GetNextCellTowardsOrientation(wallToCheck.ArrayCoordinates, perpendicularOrientationRight, level);
    BlockEntity rightBlockNext = null;
    if (rightBlock != null)
    {
      blockCoordinatesRight.Set(rightBlock.ArrayCoordinates);

      rightBlockNext = GetNextCellTowardsOrientation(blockCoordinatesRight, wallOrientation, level);
    }

    // Check parallel walls and doors

    WorldObject doorLeftBlock1 = null;
    WorldObject doorLeftBlock2 = null;
    WorldObject doorLeftBlockNext1 = null;
    WorldObject doorLeftBlockNext2 = null;
    WorldObject doorRightBlock1 = null;
    WorldObject doorRightBlock2 = null;
    WorldObject doorRightBlockNext1 = null;
    WorldObject doorRightBlockNext2 = null;

    if (leftBlock != null)
    {
      doorLeftBlock1 = FindObject(leftBlock.WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);
      doorLeftBlock2 = FindObject(leftBlock.WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
    }

    if (leftBlockNext != null)
    {
      doorLeftBlockNext1 = FindObject(leftBlockNext.WorldObjects, wallOppositeOrientation, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);
      doorLeftBlockNext2 = FindObject(leftBlockNext.WorldObjects, wallOppositeOrientation, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
    }

    if (rightBlock != null)
    {
      doorRightBlock1 = FindObject(rightBlock.WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);
      doorRightBlock2 = FindObject(rightBlock.WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
    }

    if (rightBlockNext != null)
    {
      doorRightBlockNext1 = FindObject(rightBlockNext.WorldObjects, wallOppositeOrientation, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);
      doorRightBlockNext2 = FindObject(rightBlockNext.WorldObjects, wallOppositeOrientation, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
    }

    // Check perpendicular walls and doors.
    //
    // Diagram below is used to name variables - every number represents certain block (current is 5).
    // "Left" and "right" means walls to the left and right relatively to the current wall orientation.
    //  _ _ _
    // |1|2|3|
    // |4|5|6|
    // |7|8|9|
    // 

    WorldObject doorPerpendicular5LeftType1 = FindObject(currentBlock.WorldObjects, perpendicularOrientationLeft, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);
    WorldObject doorPerpendicular5LeftType2 = FindObject(currentBlock.WorldObjects, perpendicularOrientationLeft, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
    WorldObject doorPerpendicular4RightType1 = null;
    WorldObject doorPerpendicular4RightType2 = null;
    WorldObject doorPerpendicular2LeftType1 = null;
    WorldObject doorPerpendicular2LeftType2 = null;
    WorldObject doorPerpendicular1RightType1 = null;
    WorldObject doorPerpendicular1RightType2 = null;

    WorldObject doorPerpendicular5RightType1 = FindObject(currentBlock.WorldObjects, perpendicularOrientationRight, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);
    WorldObject doorPerpendicular5RightType2 = FindObject(currentBlock.WorldObjects, perpendicularOrientationRight, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
    WorldObject doorPerpendicular6LeftType1 = null;
    WorldObject doorPerpendicular6LeftType2 = null;
    WorldObject doorPerpendicular2RightType1 = null;
    WorldObject doorPerpendicular2RightType2 = null;
    WorldObject doorPerpendicular3LeftType1 = null;
    WorldObject doorPerpendicular3LeftType2 = null;

    if (nextBlock != null)
    {
      doorPerpendicular2LeftType1 = FindObject(nextBlock.WorldObjects, perpendicularOrientationLeft, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);
      doorPerpendicular2LeftType2 = FindObject(nextBlock.WorldObjects, perpendicularOrientationLeft, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
      doorPerpendicular2RightType1 = FindObject(nextBlock.WorldObjects, perpendicularOrientationRight, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);
      doorPerpendicular2RightType2 = FindObject(nextBlock.WorldObjects, perpendicularOrientationRight, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
    }

    if (leftBlock != null)
    {
      doorPerpendicular4RightType1 = FindObject(leftBlock.WorldObjects, perpendicularOrientationRight, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);
      doorPerpendicular4RightType2 = FindObject(leftBlock.WorldObjects, perpendicularOrientationRight, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
    }

    if (leftBlockNext != null)
    {
      doorPerpendicular1RightType1 = FindObject(leftBlockNext.WorldObjects, perpendicularOrientationRight, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);
      doorPerpendicular1RightType2 = FindObject(leftBlockNext.WorldObjects, perpendicularOrientationRight, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
    }

    if (rightBlock != null)
    {
      doorPerpendicular6LeftType1 = FindObject(rightBlock.WorldObjects, perpendicularOrientationLeft, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);
      doorPerpendicular6LeftType2 = FindObject(rightBlock.WorldObjects, perpendicularOrientationLeft, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
    }

    if (rightBlockNext != null)
    {
      doorPerpendicular3LeftType1 = FindObject(rightBlockNext.WorldObjects, perpendicularOrientationLeft, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);
      doorPerpendicular3LeftType2 = FindObject(rightBlockNext.WorldObjects, perpendicularOrientationLeft, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
    }

    bool parallelDoorLeft = (doorLeftBlock1 != null || doorLeftBlock2 != null || doorLeftBlockNext1 != null || doorLeftBlockNext2 != null);
    bool parallelDoorRight = (doorRightBlock1 != null || doorRightBlock2 != null || doorRightBlockNext1 != null || doorRightBlockNext2 != null);

    //  _ _ _
    // |1|2|3|
    // |4|5|6|
    // |7|8|9|
    // 

    bool perpendicularDoorLeft = (doorPerpendicular5LeftType1 != null || doorPerpendicular5LeftType2 != null
      || doorPerpendicular2LeftType1 != null || doorPerpendicular2LeftType2 != null
      || doorPerpendicular4RightType1 != null || doorPerpendicular4RightType2 != null
      || doorPerpendicular1RightType1 != null || doorPerpendicular1RightType2 != null);

    bool perpendicularDoorRight = (doorPerpendicular5RightType1 != null || doorPerpendicular5RightType2 != null
      || doorPerpendicular2RightType1 != null || doorPerpendicular2RightType2 != null
      || doorPerpendicular6LeftType1 != null || doorPerpendicular6LeftType2 != null
      || doorPerpendicular3LeftType1 != null || doorPerpendicular3LeftType2 != null);


    bool parallelWallLeft = ((leftBlock != null && leftBlock.WallsByOrientation[wallOrientation] != null)
      || (leftBlockNext != null && leftBlockNext.WallsByOrientation[wallOppositeOrientation] != null) 
      || parallelDoorLeft);

    bool parallelWallRight = ((rightBlock != null && rightBlock.WallsByOrientation[wallOrientation] != null)
      || (rightBlockNext != null && rightBlockNext.WallsByOrientation[wallOppositeOrientation] != null)
      || parallelDoorRight);

    bool perpendicularWallLeft = ((currentBlock.WallsByOrientation[perpendicularOrientationLeft] != null
      || (leftBlock != null && leftBlock.WallsByOrientation[perpendicularOrientationRight] != null))
      && ((nextBlock != null && nextBlock.WallsByOrientation[perpendicularOrientationLeft] != null)
        || (leftBlockNext != null && leftBlockNext.WallsByOrientation[perpendicularOrientationRight] != null)));

    bool perpendicularWallRight = ((currentBlock.WallsByOrientation[perpendicularOrientationRight] != null
      || (rightBlock != null && rightBlock.WallsByOrientation[perpendicularOrientationLeft] != null))
      && ((nextBlock != null && nextBlock.WallsByOrientation[perpendicularOrientationRight] != null)
        || (rightBlockNext != null && rightBlockNext.WallsByOrientation[perpendicularOrientationLeft] != null)));

    //string debug = string.Format("{0} {1} PL: {2} PR: {3} PPL: {4} PPR: {5}", wallToCheck.ArrayCoordinates, wallOrientation, parallelWallLeft, parallelWallRight, perpendicularWallLeft, perpendicularWallRight);
    //Debug.Log(debug);

    bool hideLeftColumn = (parallelWallLeft && (!perpendicularWallLeft || perpendicularDoorLeft)) 
      || (!parallelWallLeft && (perpendicularWallLeft || perpendicularDoorLeft)) 
      || (parallelWallLeft && (perpendicularWallLeft || perpendicularDoorLeft));

    bool hideRightColumn = (parallelWallRight && (!perpendicularWallRight || perpendicularDoorRight)) 
      || (!parallelWallRight && (perpendicularWallRight || perpendicularDoorRight)) 
      || (parallelWallRight && (perpendicularWallRight || perpendicularDoorRight));

    // Remove duplicate wall columns

    if (!hideLeftColumn)
    {
      hideLeftColumn = (currentBlock.WallsByOrientation[perpendicularOrientationLeft] != null && currentBlock.WallsByOrientation[perpendicularOrientationLeft].RightColumnVisible)
        || (leftBlock != null && leftBlock.WallsByOrientation[perpendicularOrientationRight] != null && leftBlock.WallsByOrientation[perpendicularOrientationRight].LeftColumnVisible)
        || (nextBlock != null && nextBlock.WallsByOrientation[perpendicularOrientationLeft] != null && nextBlock.WallsByOrientation[perpendicularOrientationLeft].LeftColumnVisible)
        || (leftBlockNext != null && leftBlockNext.WallsByOrientation[perpendicularOrientationRight] != null && leftBlockNext.WallsByOrientation[perpendicularOrientationRight].RightColumnVisible);
    }

    if (!hideRightColumn)
    {
      hideRightColumn = (currentBlock.WallsByOrientation[perpendicularOrientationRight] != null && currentBlock.WallsByOrientation[perpendicularOrientationRight].LeftColumnVisible)
        || (rightBlock != null && rightBlock.WallsByOrientation[perpendicularOrientationLeft] != null && rightBlock.WallsByOrientation[perpendicularOrientationLeft].RightColumnVisible)
        || (nextBlock != null && nextBlock.WallsByOrientation[perpendicularOrientationRight] != null && nextBlock.WallsByOrientation[perpendicularOrientationRight].RightColumnVisible)
        || (rightBlockNext != null && rightBlockNext.WallsByOrientation[perpendicularOrientationLeft] != null && rightBlockNext.WallsByOrientation[perpendicularOrientationLeft].LeftColumnVisible);
    }

    // Columns are visible (active) by default, so we want "false" to hide game object if condition is true.

    wallToCheck.LeftColumnVisible = !hideLeftColumn;
    wallToCheck.RightColumnVisible = !hideRightColumn;
  }

  /// <summary>
  /// Hides the wall sides if it has adjacent walls to save draw calls and minimize lighting artefacts.
  /// 
  /// If we have neighbouring walls, hide appropriate lateral sides of the current wall.
  /// This is to reduce draw calls and get rid of potential lighting artefacts:
  /// if you have two cubes adjacent to each other, at their "shared" side there will be situations
  /// when lighting can sometimes highlight "shared" side of the adjacent cube, which result in a seam that
  /// clearly shows that two objects are separate.
  /// \
  ///  \
  ///   \ seam is here
  ///    |\ _
  ///    | |_|
  ///    |/
  ///   /
  ///  /
  /// /
  /// </summary>
  public static void HideWallSides(WallWorldObject wall, LevelBase level)
  {     
    // Increment of X is equivalent of going to the south, increment of Z - east.
    // So, we have four cases: neighbouring Xs for east and west walls, and neighbouring Zs for south and north.

    TryHideLeftRightSides(wall, level);
    TryHideTopBottomSides(wall, level);
  }

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
    GlobalConstants.Orientation wallOppositeOrientation = GetOppositeOrientation(wallOrientation);

    // Check parallel walls for current orientation on neighbouring cells and on opposite sides of the next
    // cells towards orientation of the neighbouring (the same as current) cell.

    bool parallelWallCurrentLeft = false;
    bool parallelWallNextLeft = false;
    bool parallelWallCurrentRight = false;
    bool parallelWallNextRight = false;

    // South - X+, East - Z+

    if (wallOrientation == GlobalConstants.Orientation.EAST)
    {
      parallelWallCurrentLeft = (lx >= 0) ? (level.Level[lx, y, z].WallsByOrientation[wallOrientation] != null) : false;
      parallelWallNextLeft = (lx >= 0 && hz < level.MapZ) ? (level.Level[lx, y, hz].WallsByOrientation[wallOppositeOrientation] != null) : false;
      parallelWallCurrentRight = (hx < level.MapX) ? (level.Level[hx, y, z].WallsByOrientation[wallOrientation] != null) : false;
      parallelWallNextRight = (hx < level.MapX && hz < level.MapZ) ? (level.Level[hx, y, hz].WallsByOrientation[wallOppositeOrientation] != null) : false;

      if (parallelWallCurrentLeft || parallelWallNextLeft)
      {
        wall.BWO.LeftQuad.gameObject.SetActive(false);
      }

      if (parallelWallCurrentRight || parallelWallNextRight)
      {
        wall.BWO.RightQuad.gameObject.SetActive(false);
      }
    }
    else if (wallOrientation == GlobalConstants.Orientation.SOUTH)
    {
      parallelWallCurrentLeft = (hz < level.MapZ) ? (level.Level[x, y, hz].WallsByOrientation[wallOrientation] != null) : false;
      parallelWallNextLeft = (hx < level.MapX && hz < level.MapZ) ? (level.Level[hx, y, hz].WallsByOrientation[wallOppositeOrientation] != null) : false;
      parallelWallCurrentRight = (lz >= 0) ? (level.Level[x, y, lz].WallsByOrientation[wallOrientation] != null) : false;
      parallelWallNextRight = (hx < level.MapX && lz >= 0) ? (level.Level[hx, y, lz].WallsByOrientation[wallOppositeOrientation] != null) : false;

      if (parallelWallCurrentLeft || parallelWallNextLeft)
      {
        wall.BWO.LeftQuad.gameObject.SetActive(false);
      }

      if (parallelWallCurrentRight || parallelWallNextRight)
      {
        wall.BWO.RightQuad.gameObject.SetActive(false);
      }
    }
    else if (wallOrientation == GlobalConstants.Orientation.WEST)
    { 
      parallelWallCurrentLeft = (hx < level.MapX) ? (level.Level[hx, y, z].WallsByOrientation[wallOrientation] != null) : false;
      parallelWallNextLeft = (hx < level.MapX && lz >= 0) ? (level.Level[hx, y, lz].WallsByOrientation[wallOppositeOrientation] != null) : false;
      parallelWallCurrentRight = (lx >= 0) ? (level.Level[lx, y, z].WallsByOrientation[wallOrientation] != null) : false;
      parallelWallNextRight = (lx >= 0 && lz >= 0) ? (level.Level[lx, y, lz].WallsByOrientation[wallOppositeOrientation] != null) : false;

      if (parallelWallCurrentLeft || parallelWallNextLeft)
      {
        wall.BWO.LeftQuad.gameObject.SetActive(false);
      }

      if (parallelWallCurrentRight || parallelWallNextRight)
      {
        wall.BWO.RightQuad.gameObject.SetActive(false);
      }
    }
    else if (wallOrientation == GlobalConstants.Orientation.NORTH)
    {
      parallelWallCurrentLeft = (lz >= 0) ? (level.Level[x, y, lz].WallsByOrientation[wallOrientation] != null) : false;
      parallelWallNextLeft = (lx >= 0 && lz >= 0) ? (level.Level[lx, y, lz].WallsByOrientation[wallOppositeOrientation] != null) : false;
      parallelWallCurrentRight = (hz < level.MapZ) ? (level.Level[x, y, hz].WallsByOrientation[wallOrientation] != null) : false;
      parallelWallNextRight = (lx >= 0 && hz < level.MapZ) ? (level.Level[lx, y, hz].WallsByOrientation[wallOppositeOrientation] != null) : false;

      if (parallelWallCurrentLeft || parallelWallNextLeft)
      {
        wall.BWO.LeftQuad.gameObject.SetActive(false);
      }

      if (parallelWallCurrentRight || parallelWallNextRight)
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
    GlobalConstants.Orientation wallOppositeOrientation = GetOppositeOrientation(wallOrientation);

    BlockEntity nextBlockCurrent = GetNextCellTowardsOrientation(wall.ArrayCoordinates, wallOrientation, level);
    BlockEntity nextBlockUpper = GetNextCellTowardsOrientation(new Int3(wall.ArrayCoordinates.X, wall.ArrayCoordinates.Y + 1, wall.ArrayCoordinates.Z), wallOrientation, level);

    bool bottomWallCurrent = (ly >= 0) ? level.Level[x, ly, z].WallsByOrientation[wallOrientation] != null : false;
    bool bottomWallNext = (ly >= 0 && nextBlockCurrent != null) ? nextBlockCurrent.WallsByOrientation[wallOppositeOrientation] != null : false;
    bool upperWallCurrent = (hy < level.MapY) ? level.Level[x, hy, z].WallsByOrientation[wallOrientation] != null : false;
    bool upperWallNext = (hy< level.MapY && nextBlockUpper != null) ? nextBlockUpper.WallsByOrientation[wallOppositeOrientation] != null : false;

    WorldObject bottomDoorCurrent1 = null;
    WorldObject bottomDoorCurrent2 = null;
    WorldObject bottomDoorNext1 = null;
    WorldObject bottomDoorNext2 = null;

    if (ly >= 0)
    {
      bottomDoorCurrent1 = FindObject(level.Level[x, ly, z].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
      bottomDoorCurrent2 = FindObject(level.Level[x, ly, z].WorldObjects, wallOrientation, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);

      if (nextBlockCurrent != null)
      {
        bottomDoorNext1 = FindObject(nextBlockCurrent.WorldObjects, wallOppositeOrientation, GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE);
        bottomDoorNext2 = FindObject(nextBlockCurrent.WorldObjects, wallOppositeOrientation, GlobalConstants.WorldObjectClass.DOOR_OPENABLE);
      }
    }

    // To disable Z fighting of door frame with bottom side of the wall, handle this case specifically.

    if (bottomWallCurrent || bottomWallNext 
     || bottomDoorCurrent1 != null || bottomDoorCurrent2 != null 
     || bottomDoorNext1 != null || bottomDoorNext2 != null)
    {
      wall.BWO.BottomQuad.gameObject.SetActive(false);
    }

    if (upperWallCurrent || upperWallNext)
    {
      wall.BWO.TopQuad.gameObject.SetActive(false);
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
