using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenLevel : LevelBase 
{
  public TitleScreenLevel(int x, int y, int z) : base(x, y, z)
  {    
  }

  public override void GenerateLevel()
  {
    _playerPos.Set(4, 1, 0);

    for (int i = 0; i < 8; i++)
    {
      if (i == 4)
      {
        continue;
      }
      
      _level[i, 0, 0].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;
      _level[i, 0, 1].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;
      _level[i, 0, 2].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;

      _level[i, 1, 2].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;
      _level[i, 2, 2].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;
    }

    PlaceObject(new Int3(3, 1, 1), GlobalConstants.WorldObjectPrefabType.TORCH, GlobalConstants.Orientation.NORTH);
    PlaceObject(new Int3(5, 1, 1), GlobalConstants.WorldObjectPrefabType.TORCH, GlobalConstants.Orientation.SOUTH);
    PlaceObject(new Int3(5, 1, 1), GlobalConstants.WorldObjectPrefabType.CHEST_WOODEN, GlobalConstants.Orientation.EAST);

    _level[4, 0, 0].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;
    _level[4, 0, 1].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;
    _level[4, 0, 2].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;

    _level[2, 1, 0].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;
    _level[2, 1, 1].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;
    _level[2, 2, 1].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;

    _level[6, 1, 0].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;
    _level[6, 1, 1].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;
    _level[6, 2, 1].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;

    _level[4, 2, 2].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;

    PlaceDoor(new Int3(4, 1, 1), GlobalConstants.WorldObjectPrefabType.DOOR_PORTCULLIS, GlobalConstants.Orientation.EAST, false, true, 0.5f, 2.0f);
    PlaceControl(new Int3(3, 1, 1), GlobalConstants.WorldObjectClass.LEVER, GlobalConstants.WorldObjectPrefabType.LEVER, GlobalConstants.Orientation.EAST);

    for (int i = 3; i < 16; i++)
    {
      _level[3, 1, i].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;
      _level[3, 2, i].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;
      _level[4, 0, i].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;
      _level[5, 1, i].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;
      _level[5, 2, i].BlockType = GlobalConstants.BlockType.STONE_BRICKS_OLD;
    }
  }
}
