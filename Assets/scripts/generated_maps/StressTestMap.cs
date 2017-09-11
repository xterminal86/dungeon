using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressTestMap : LevelBase 
{
  public StressTestMap(int x, int y, int z) : base(x, y, z)
  {    
  }

  public override void GenerateLevel()
  {    
    _playerPos.Set(0, 1, 0);

    GenerateRandomTerrain();
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
}
