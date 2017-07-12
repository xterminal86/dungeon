using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarwinVillage : LevelBase
{
  public DarwinVillage(int x, int y, int z) : base(x, y, z)
  { 
  }

  public override void GenerateLevel()
  {
    int posY = _mapY / 2 + 1;

    //_playerPos.Set(26, posY, 24);
    _playerPos.Set(0, posY, 0);

    CreateGround();

    var blockType = GlobalConstants.BlockType.DIRT;

    _level[1, posY, 1].BlockType = blockType;
    _level[2, posY, 1].BlockType = blockType;
    _level[2, posY + 1, 1].BlockType = blockType;
    _level[3, posY + 2, 1].BlockType = blockType;
    _level[4, posY + 3, 1].BlockType = blockType;
    _level[4, posY + 2, 1].BlockType = blockType;
    _level[4, posY + 1, 1].BlockType = blockType;
    _level[4, posY, 1].BlockType = blockType;

    PlaceWall(new Int3(5, posY, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(5, posY + 1, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(6, posY, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(6, posY + 1, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(7, posY, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(7, posY + 1, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(8, posY, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWall(new Int3(8, posY + 1, 1), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.EAST);

    WorldObject wo = PlaceDoor(new Int3(9, posY, 1), GlobalConstants.WorldObjectPrefabType.DOOR_STONE_BRICKS_SLIDING, GlobalConstants.Orientation.EAST, false, true, 0.5f, 0.5f);
    PlaceWall(new Int3(9, posY + 1, 1), GlobalConstants.WorldObjectPrefabType.NOTHING, GlobalConstants.Orientation.EAST);
    PlaceControl(new Int3(8, posY, 1), GlobalConstants.WorldObjectClass.BUTTON, GlobalConstants.WorldObjectPrefabType.BUTTON, GlobalConstants.Orientation.EAST, wo);

    PlaceObject(new Int3(7, posY, 1), GlobalConstants.WorldObjectPrefabType.TORCH, GlobalConstants.Orientation.EAST);

    PlaceWall(new Int3(10, posY, 2), GlobalConstants.WorldObjectPrefabType.WALL_STONE_BRICKS, GlobalConstants.Orientation.WEST);
    PlaceControl(new Int3(8, posY, 2), GlobalConstants.WorldObjectClass.BUTTON, GlobalConstants.WorldObjectPrefabType.BUTTON, GlobalConstants.Orientation.WEST, wo);
    PlaceControl(new Int3(10, posY, 1), GlobalConstants.WorldObjectClass.LEVER, GlobalConstants.WorldObjectPrefabType.LEVER, GlobalConstants.Orientation.EAST, wo);

    //PlaceTeleporter(new Int3(1, posY, 2), new Int3(4, posY + 3, 2));
    //PlaceTeleporter(new Int3(4, posY + 2, 0), new Int3(0, posY, 0));
    //PlaceShrine(new Int3(_mapX - 1, posY, 0), GlobalConstants.WorldObjectPrefabType.SHRINE_MIGHT, GlobalConstants.Orientation.SOUTH);
    //PlaceShrine(new Int3(_mapX - 1, posY, _mapZ - 1), GlobalConstants.WorldObjectPrefabType.SHRINE_SPIRIT, GlobalConstants.Orientation.SOUTH);

    //PlaceShrine(new Int3(1, posY, 3), GlobalConstants.WorldObjectPrefabType.SHRINE_MIGHT, GlobalConstants.Orientation.SOUTH);

    DiscardHiddenBlocks(1, _mapX - 1, 1, _mapY - 1, 1, _mapZ - 1);
  }

  void CreateGround()
  {
    int posY = _mapY / 2;

    for (int x = 0; x < _mapX; x++)
    {
      for (int z = 0; z < _mapZ; z++)
      {
        _level[x, posY, z].BlockType = GlobalConstants.BlockType.GRASS;
        _level[x, posY, z].WorldCoordinates.Set(x * GlobalConstants.WallScaleFactor, 
                                                posY * GlobalConstants.WallScaleFactor, 
                                                z * GlobalConstants.WallScaleFactor);
      }
    }
  }
}

