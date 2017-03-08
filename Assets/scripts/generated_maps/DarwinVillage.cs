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
    int posY = _mapY / 2;

    _playerPos.Set(0, posY + 1, 0);

    CreateGround();

    PlaceWorldObject(new Int3(0, posY + 1, 1), GlobalConstants.WorldObjectClass.WALL, GlobalConstants.WorldObjectPrefabType.WALL_BORDER_STONE_BRICKS, GlobalConstants.Orientation.EAST);
    PlaceWorldObject(new Int3(1, posY + 1, 1), GlobalConstants.WorldObjectClass.DOOR_OPENABLE, GlobalConstants.WorldObjectPrefabType.DOOR_WOODEN_SWING, GlobalConstants.Orientation.EAST);
    PlaceWorldObject(new Int3(2, posY + 1, 1), GlobalConstants.WorldObjectClass.DOOR_OPENABLE, GlobalConstants.WorldObjectPrefabType.DOOR_IRON_SWING, GlobalConstants.Orientation.EAST);
    var wo = PlaceWorldObject(new Int3(3, posY + 1, 1), GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE, GlobalConstants.WorldObjectPrefabType.DOOR_IRON_SWING, GlobalConstants.Orientation.EAST);
    PlaceWorldObject(new Int3(4, posY + 1, 2), GlobalConstants.WorldObjectClass.LEVER, GlobalConstants.WorldObjectPrefabType.LEVER, GlobalConstants.Orientation.EAST, wo);

    MakeHillLayered(GlobalConstants.BlockType.STONE, new Int3(_mapX / 2, posY, _mapZ / 2), 3);

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

