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

    // Doors
    PlaceWorldObject(new Int3(1, 1, 1), GlobalConstants.WorldObjectClass.DOOR_OPENABLE, GlobalConstants.WorldObjectPrefabType.DOOR_WOODEN_SWING, GlobalConstants.Orientation.EAST);
    PlaceWorldObject(new Int3(2, 1, 1), GlobalConstants.WorldObjectClass.DOOR_OPENABLE, GlobalConstants.WorldObjectPrefabType.DOOR_IRON_SWING, GlobalConstants.Orientation.EAST);
    wo = PlaceWorldObject(new Int3(3, 1, 1), GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE, GlobalConstants.WorldObjectPrefabType.DOOR_IRON_SWING, GlobalConstants.Orientation.EAST);
    PlaceWorldObject(new Int3(4, 1, 1), GlobalConstants.WorldObjectClass.WALL, GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWorldObject(new Int3(4, 1, 1), GlobalConstants.WorldObjectClass.LEVER, GlobalConstants.WorldObjectPrefabType.LEVER, GlobalConstants.Orientation.EAST, wo);
    PlaceWorldObject(new Int3(4, 1, 2), GlobalConstants.WorldObjectClass.LEVER, GlobalConstants.WorldObjectPrefabType.LEVER, GlobalConstants.Orientation.WEST, wo);

    // Tiles
    PlaceWorldObject(new Int3(5, 2, 1), GlobalConstants.WorldObjectClass.WALL, GlobalConstants.WorldObjectPrefabType.WALL_TILES, GlobalConstants.Orientation.EAST);
    wo = PlaceWorldObject(new Int3(5, 1, 1), GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE, GlobalConstants.WorldObjectPrefabType.DOOR_TILE_SLIDING, GlobalConstants.Orientation.EAST);

    (wo as DoorWorldObject).IsSliding = true;
    (wo as DoorWorldObject).AnimationOpenSpeed = 1.0f;
    (wo as DoorWorldObject).AnimationCloseSpeed = 1.0f;

    PlaceWorldObject(new Int3(6, 1, 1), GlobalConstants.WorldObjectClass.WALL, GlobalConstants.WorldObjectPrefabType.WALL_TILES, GlobalConstants.Orientation.EAST);
    PlaceWorldObject(new Int3(6, 1, 1), GlobalConstants.WorldObjectClass.BUTTON, GlobalConstants.WorldObjectPrefabType.BUTTON, GlobalConstants.Orientation.EAST, wo);
    PlaceWorldObject(new Int3(6, 1, 2), GlobalConstants.WorldObjectClass.BUTTON, GlobalConstants.WorldObjectPrefabType.BUTTON, GlobalConstants.Orientation.WEST, wo);

    // Portcullis

    wo = PlaceWorldObject(new Int3(7, 1, 1), GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE, GlobalConstants.WorldObjectPrefabType.DOOR_PORTCULLIS, GlobalConstants.Orientation.EAST);

    (wo as DoorWorldObject).IsSliding = true;
    (wo as DoorWorldObject).AnimationOpenSpeed = 0.5f;
    (wo as DoorWorldObject).AnimationCloseSpeed = 2.0f;

    PlaceWorldObject(new Int3(8, 1, 1), GlobalConstants.WorldObjectClass.WALL, GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWorldObject(new Int3(8, 1, 1), GlobalConstants.WorldObjectClass.LEVER, GlobalConstants.WorldObjectPrefabType.LEVER, GlobalConstants.Orientation.EAST, wo);
    PlaceWorldObject(new Int3(8, 1, 2), GlobalConstants.WorldObjectClass.LEVER, GlobalConstants.WorldObjectPrefabType.LEVER, GlobalConstants.Orientation.WEST, wo);

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
