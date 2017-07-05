﻿using System.Collections;
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

    _playerPos.Set(26, posY, 24);

    CreateGround();

    MakeHillLayered(GlobalConstants.BlockType.STONE, new Int3(_mapX / 2, posY, _mapZ / 2), 3);

    PlaceTeleporter(new Int3(2, posY, 1), new Int3(_mapX / 2, posY + 3, _mapZ / 2));
    PlaceTeleporter(new Int3(_mapX / 2, posY + 3, _mapZ / 2), new Int3(1, posY, 1));
    PlaceShrine(new Int3(_mapX - 1, posY, 0), GlobalConstants.WorldObjectPrefabType.SHRINE_MIGHT, GlobalConstants.Orientation.SOUTH);
    PlaceShrine(new Int3(_mapX - 1, posY, _mapZ - 1), GlobalConstants.WorldObjectPrefabType.SHRINE_SPIRIT, GlobalConstants.Orientation.SOUTH);

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

