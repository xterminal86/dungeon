using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevel : LevelBase 
{
  public TestLevel(int x, int y, int z) : base(x, y, z)
  {    
  }
  
  // Should be odd
  int _maxHillsHeight = 21;
  public override void Generate()
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

      MakeHillLayered(x, 0, z, h);
    }

    DiscardHiddenBlocks(1, _mapX - 1, 1, _mapY - 1, 1, _mapZ - 1);
  }
}
