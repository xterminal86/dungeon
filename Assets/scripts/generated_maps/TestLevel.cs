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

    _playerPos.Set(0, 1, 0);

    WorldObject wo = null;

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

    // Tiles
    wo = PlaceDoor(new Int3(5, 1, 1), GlobalConstants.WorldObjectPrefabType.DOOR_STONE_BRICKS_SLIDING, GlobalConstants.Orientation.EAST, false, true, 0.5f, 0.5f);

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

    WorldObject o1 = PlaceDoor(new Int3(2, 1, 5), GlobalConstants.WorldObjectPrefabType.DOOR_PORTCULLIS, GlobalConstants.Orientation.EAST, false, true, 0.5f, 2.0f);
    WorldObject o2 = PlaceDoor(new Int3(3, 1, 5), GlobalConstants.WorldObjectPrefabType.DOOR_PORTCULLIS, GlobalConstants.Orientation.EAST, false, true, 0.5f, 2.0f);
    WorldObject o3 = PlaceDoor(new Int3(4, 1, 5), GlobalConstants.WorldObjectPrefabType.DOOR_PORTCULLIS, GlobalConstants.Orientation.EAST, false, true, 0.5f, 2.0f);

    PlaceControl(new Int3(1, 1, 5), GlobalConstants.WorldObjectClass.BUTTON, GlobalConstants.WorldObjectPrefabType.BUTTON, GlobalConstants.Orientation.EAST, o1, o2, o3);

    // Teleporter

    PlaceWall(new Int3(11, 1, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(11, 2, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(11, 1, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.SOUTH);
    PlaceWall(new Int3(11, 2, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.SOUTH);
    PlaceWall(new Int3(11, 1, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.WEST);
    PlaceWall(new Int3(11, 2, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.WEST);
    PlaceTeleporter(new Int3(11, 1, 1), new Int3(0, 1, 1));

    DiscardHiddenBlocks(1, _mapX - 1, 1, _mapY - 1, 1, _mapZ - 1);
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
