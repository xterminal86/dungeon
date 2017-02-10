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
    GenerateRandomTerrain();
    //GenerateHills();

    DiscardHiddenBlocks(1, _mapX - 1, 1, _mapY - 1, 1, _mapZ - 1);
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
