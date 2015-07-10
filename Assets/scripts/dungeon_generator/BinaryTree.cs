using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BinaryTree : GenerationAlgorithmBase
{
  public override void Do(Grid grid)
  {
    for (int x = 0; x < grid.MapHeight; x++)
    {
      for (int y = 0; y < grid.MapWidth; y++)
      {
        int val = Random.Range(0, 2);

        Cell c = (val == 0) ? grid.Map[x, y].Neighbours[CellDirections.EAST] : grid.Map[x, y].Neighbours[CellDirections.SOUTH];

        if (c != null && !c.IsLinked())
        {
          grid.Map[x, y].Link(c);
        }
      }
    }
  }
}
