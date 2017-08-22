using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevel : LevelBase 
{
  public TestLevel(int x, int y, int z) : base(x, y, z)
  {    
  }

  public override void GenerateLevel()
  {    
    CreateSuperflatTerrain();
    //GenerateRandomTerrain();
    //GenerateHills();

    _playerPos.Set(7, 1, 5);

    MakePerimeter();

    WorldObject wo = null;

    PlaceWall(new Int3(5, 1, 6), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);

    // Sign

    PlaceWall(new Int3(0, 1, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(0, 2, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceSign(new Int3(0, 1, 1), GlobalConstants.WorldObjectPrefabType.SIGN_PLAQUE_METAL, GlobalConstants.Orientation.EAST, "\n\nWelcome to the\nTest Map!");

    // Doors

    PlaceWall(new Int3(1, 2, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceDoor(new Int3(1, 1, 1), GlobalConstants.WorldObjectPrefabType.DOOR_WOODEN_SWING, GlobalConstants.Orientation.EAST, true, false, 3.0f, 4.0f);
    PlaceWall(new Int3(2, 2, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceDoor(new Int3(2, 1, 1), GlobalConstants.WorldObjectPrefabType.DOOR_IRON_SWING, GlobalConstants.Orientation.EAST, true, false, 3.0f, 4.0f);
    PlaceWall(new Int3(3, 2, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    wo = PlaceDoor(new Int3(3, 1, 1), GlobalConstants.WorldObjectPrefabType.DOOR_IRON_SWING, GlobalConstants.Orientation.EAST, false, false, 3.0f, 4.0f);
    PlaceWall(new Int3(4, 2, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(4, 1, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceControl(new Int3(4, 1, 1), GlobalConstants.WorldObjectClass.LEVER, GlobalConstants.WorldObjectPrefabType.LEVER, GlobalConstants.Orientation.EAST, wo);
    PlaceControl(new Int3(4, 1, 2), GlobalConstants.WorldObjectClass.LEVER, GlobalConstants.WorldObjectPrefabType.LEVER, GlobalConstants.Orientation.WEST, wo);

    // Secret door

    wo = PlaceDoor(new Int3(5, 1, 1), GlobalConstants.WorldObjectPrefabType.DOOR_STONE_BRICKS_SLIDING, GlobalConstants.Orientation.EAST, false, true, 0.5f, 0.5f);
    PlaceWall(new Int3(5, 2, 1), GlobalConstants.WorldObjectPrefabType.NOTHING, GlobalConstants.Orientation.EAST);

    PlaceWall(new Int3(6, 1, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(6, 2, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceControl(new Int3(6, 1, 1), GlobalConstants.WorldObjectClass.BUTTON, GlobalConstants.WorldObjectPrefabType.BUTTON, GlobalConstants.Orientation.EAST, wo);
    PlaceControl(new Int3(6, 1, 2), GlobalConstants.WorldObjectClass.BUTTON, GlobalConstants.WorldObjectPrefabType.BUTTON, GlobalConstants.Orientation.WEST, wo);

    // Portcullis

    PlaceWall(new Int3(7, 2, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    wo = PlaceDoor(new Int3(7, 1, 1), GlobalConstants.WorldObjectPrefabType.DOOR_PORTCULLIS, GlobalConstants.Orientation.EAST, false, true, 0.5f, 2.0f);

    PlaceWall(new Int3(8, 1, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(8, 2, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceControl(new Int3(8, 1, 1), GlobalConstants.WorldObjectClass.LEVER, GlobalConstants.WorldObjectPrefabType.LEVER, GlobalConstants.Orientation.EAST, wo);
    PlaceControl(new Int3(8, 1, 2), GlobalConstants.WorldObjectClass.LEVER, GlobalConstants.WorldObjectPrefabType.LEVER, GlobalConstants.Orientation.WEST, wo);

    PlaceWall(new Int3(9, 2, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    wo = PlaceDoor(new Int3(9, 1, 1), GlobalConstants.WorldObjectPrefabType.DOOR_PORTCULLIS, GlobalConstants.Orientation.EAST, false, true, 0.5f, 2.0f);

    PlaceWall(new Int3(10, 1, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(10, 2, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceControl(new Int3(10, 1, 1), GlobalConstants.WorldObjectClass.BUTTON, GlobalConstants.WorldObjectPrefabType.BUTTON, GlobalConstants.Orientation.EAST, wo);
    PlaceControl(new Int3(10, 1, 2), GlobalConstants.WorldObjectClass.BUTTON, GlobalConstants.WorldObjectPrefabType.BUTTON, GlobalConstants.Orientation.WEST, wo);

    // Control multiple objects

    PlaceWall(new Int3(1, 1, 5), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(5, 1, 5), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);

    WorldObject o1 = PlaceDoor(new Int3(2, 1, 5), GlobalConstants.WorldObjectPrefabType.DOOR_PORTCULLIS, GlobalConstants.Orientation.EAST, false, true, 0.5f, 2.0f);
    WorldObject o2 = PlaceDoor(new Int3(3, 1, 5), GlobalConstants.WorldObjectPrefabType.DOOR_IRON_SWING, GlobalConstants.Orientation.EAST, false, false, 3.0f, 4.0f);
    WorldObject o3 = PlaceDoor(new Int3(4, 1, 5), GlobalConstants.WorldObjectPrefabType.DOOR_WOODEN_SWING, GlobalConstants.Orientation.EAST, true, false, 3.0f, 4.0f);

    PlaceControl(new Int3(1, 1, 5), GlobalConstants.WorldObjectClass.BUTTON, GlobalConstants.WorldObjectPrefabType.BUTTON, GlobalConstants.Orientation.EAST, o1, o2, o3);
    PlaceControl(new Int3(5, 1, 5), GlobalConstants.WorldObjectClass.LEVER, GlobalConstants.WorldObjectPrefabType.LEVER, GlobalConstants.Orientation.EAST, o1, o2, o3);
    PlaceControl(new Int3(5, 1, 6), GlobalConstants.WorldObjectClass.LEVER, GlobalConstants.WorldObjectPrefabType.LEVER, GlobalConstants.Orientation.WEST, o1, o2, o3);

    // Teleporter

    PlaceWall(new Int3(11, 1, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(11, 2, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(11, 1, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS_WINDOW, GlobalConstants.Orientation.SOUTH);
    PlaceWall(new Int3(11, 2, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.SOUTH);
    PlaceWall(new Int3(11, 1, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.WEST);
    PlaceWall(new Int3(11, 2, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.WEST);

    PlaceWall(new Int3(10, 1, 0), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.SOUTH);
    PlaceWall(new Int3(10, 2, 0), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.SOUTH);
    PlaceSign(new Int3(10, 1, 0), GlobalConstants.WorldObjectPrefabType.SIGN_PLAQUE_METAL, GlobalConstants.Orientation.SOUTH, "\n\n\nKing of the Hill");
    PlaceTeleporter(new Int3(11, 1, 1), new Int3(5, 21, 21));

    // Upper level

    MakeHillCubed(GlobalConstants.BlockType.GRASS, new Int3(5, 1, 20), 5, 20);

    PlaceTeleporter(new Int3(5, 21, 20), new Int3(0, 1, 1));

    // Climbing test

    _level[8, 1, 10].BlockType = GlobalConstants.BlockType.DIRT;
    _level[9, 2, 10].BlockType = GlobalConstants.BlockType.DIRT;
    _level[8, 2, 11].BlockType = GlobalConstants.BlockType.DIRT;
    _level[9, 3, 11].BlockType = GlobalConstants.BlockType.DIRT;
    _level[10, 3, 11].BlockType = GlobalConstants.BlockType.DIRT;
    _level[11, 3, 11].BlockType = GlobalConstants.BlockType.DIRT;
    _level[11, 2, 11].BlockType = GlobalConstants.BlockType.DIRT;
    _level[11, 1, 11].BlockType = GlobalConstants.BlockType.DIRT;

    _level[7, 1, 10].SidesWalkability[GlobalConstants.Orientation.EAST] = false;

    PlaceSign(new Int3(7, 1, 10), GlobalConstants.WorldObjectPrefabType.SIGN_POST_WOODEN, GlobalConstants.Orientation.EAST, "Press F\nto climb on the block\nin front of you");
    PlaceStairs(new Int3(8, 1, 9), GlobalConstants.WorldObjectPrefabType.STAIRS_STONE_BRICKS, GlobalConstants.Orientation.EAST);

    // Objects

    PlaceObject(new Int3(0, 1, 0), GlobalConstants.WorldObjectPrefabType.TORCH, GlobalConstants.Orientation.WEST);
    PlaceObject(new Int3(4, 1, 0), GlobalConstants.WorldObjectPrefabType.TORCH, GlobalConstants.Orientation.WEST);
    PlaceObject(new Int3(10, 1, 0), GlobalConstants.WorldObjectPrefabType.TORCH, GlobalConstants.Orientation.WEST);

    PlaceShrine(new Int3(10, 21, 25), GlobalConstants.WorldObjectPrefabType.SHRINE_MIGHT, GlobalConstants.Orientation.SOUTH);
    PlaceShrine(new Int3(10, 21, 15), GlobalConstants.WorldObjectPrefabType.SHRINE_SPIRIT, GlobalConstants.Orientation.SOUTH);
    //PlaceShrine(new Int3(1, 1, 0), GlobalConstants.WorldObjectPrefabType.SHRINE_SPIRIT, GlobalConstants.Orientation.SOUTH);

    // Cube

    PlaceWall(new Int3(20, 1, 5), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(20, 1, 5), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.SOUTH);
    PlaceWall(new Int3(20, 1, 5), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.WEST);
    PlaceWall(new Int3(20, 1, 5), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.NORTH);

    PlaceWall(new Int3(20, 1, 8), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(21, 1, 8), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.NORTH);
    PlaceWall(new Int3(20, 1, 7), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(19, 1, 8), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.SOUTH);

    // Side hiding test

    PlaceWall(new Int3(22, 1, 5), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(23, 2, 6), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.WEST);
    PlaceWall(new Int3(23, 1, 6), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.WEST);

    PlaceWall(new Int3(24, 1, 5), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.SOUTH);
    PlaceWall(new Int3(25, 1, 4), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.NORTH);

    // Columns test

    PlaceWall(new Int3(28, 1, 4), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.NORTH);
    PlaceWall(new Int3(28, 1, 5), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.NORTH);
    PlaceDoor(new Int3(28, 1, 5), GlobalConstants.WorldObjectPrefabType.DOOR_WOODEN_SWING, GlobalConstants.Orientation.EAST, true, false, 3.0f, 4.0f);
    PlaceWall(new Int3(28, 1, 6), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.NORTH);

    PlaceWall(new Int3(30, 1, 4), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(31, 1, 5), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.WEST);
    PlaceWall(new Int3(31, 1, 4), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.SOUTH);
    PlaceDoor(new Int3(32, 1, 4), GlobalConstants.WorldObjectPrefabType.DOOR_WOODEN_SWING, GlobalConstants.Orientation.EAST, true, false, 3.0f, 4.0f);

    // States test

    PlaceWall(new Int3(30, 1, 20), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(30, 1, 20), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.SOUTH);
    PlaceWall(new Int3(30, 1, 20), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.NORTH);
    PlaceWall(new Int3(30, 1, 19), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.SOUTH);
    PlaceWall(new Int3(30, 1, 19), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.NORTH);
    PlaceWall(new Int3(30, 1, 18), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.SOUTH);
    PlaceWall(new Int3(30, 1, 18), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.NORTH);
    PlaceWall(new Int3(31, 1, 17), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    wo = PlaceDoor(new Int3(30, 1, 18), GlobalConstants.WorldObjectPrefabType.DOOR_PORTCULLIS, GlobalConstants.Orientation.WEST, false, true, 0.5f, 2.0f);
    PlaceControl(new Int3(31, 1, 17), GlobalConstants.WorldObjectClass.BUTTON, GlobalConstants.WorldObjectPrefabType.BUTTON, GlobalConstants.Orientation.EAST, wo);

    PlaceActor("char-steve", new Int3(30, 1, 20), GlobalConstants.Orientation.EAST, GlobalConstants.ActorRole.DUMMY);

    DiscardHiddenBlocks(1, _mapX - 1, 1, _mapY - 1, 1, _mapZ - 1);

    Pathfinder p = new Pathfinder(this);
    p.BuildPath(new Int3(0, 1, 0), new Int3(0, 1, 8), true);
  }

  void MakePerimeter()
  {
    Int3 pos = new Int3();

    for (int y = 1; y < 3; y++)
    {
      for (int z = 0; z < _mapZ; z++)
      {        
        pos.X = 0;
        pos.Y = y;
        pos.Z = z;

        PlaceWall(pos, GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.NORTH);

        pos.X = _mapX - 1;
        pos.Y = y;
        pos.Z = z;

        PlaceWall(pos, GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.SOUTH);
      }

      for (int x = 0; x < _mapX; x++)
      {
        pos.X = x;
        pos.Y = y;
        pos.Z = 0;

        PlaceWall(pos, GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.WEST);

        pos.X = x;
        pos.Y = y;
        pos.Z = _mapZ - 1;

        PlaceWall(pos, GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
      }
    }
  }

  void CreateSuperflatTerrain()
  {
    for (int x = 0; x < _mapX; x++)
    {
      for (int z = 0; z < _mapZ; z++)
      {
        _level[x, 0, z].BlockType = GlobalConstants.BlockType.GRASS;
      }
    }
  }

  void GenerateRandomTerrain()
  {
    for (int x = 0; x < _mapX; x++)
    {
      for (int z = 0; z < _mapZ; z++)
      {
        int choice = Random.Range(1, GlobalConstants.BlockPrefabByType.Count + 1);
        GlobalConstants.BlockType block = (GlobalConstants.BlockType)choice;

        _level[x, 0, z].BlockType = block;

        if (block == GlobalConstants.BlockType.WATER)
        {
          _level[x, 0, z].IsLiquid = true;
        }
      }
    }
  }

  // Should be odd
  int _maxHillsHeight = 21;
  void GenerateHills()
  {
    int[] heights = new int[(_maxHillsHeight - 1) / 2 + 1];

    int h = 1;
    for (int i = 0; i < heights.Length; i++)
    {
      heights[i] = h;
      h += 2;
    }

    for (int i = 0; i < 20; i++)
    {
      int ind = Random.Range(0, heights.Length);

      h = heights[ind];

      int x = Random.Range(10, _mapX - 10);
      int z = Random.Range(10, _mapZ - 10);

      MakeHillLayered(GlobalConstants.BlockType.GRASS, new Int3(x, 0, z), h);
    }
  }
}
