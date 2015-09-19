using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BinaryTree : GenerationAlgorithmBase
{  
  public override void Do(Grid grid)
  {
    Int2 tmp = new Int2();
    for (int x = 1; x < grid.MapHeight - 2; x++)
    {
      for (int y = 1; y < grid.MapWidth - 2; y++)
      {
        if (grid.Map[x, y].Status == CellStatus.VISITED || grid.Map[x, y].Status == CellStatus.LOCKED) continue;

        grid.Map[x, y].CellType = CellType.FLOOR;
        grid.Map[x, y].Status = CellStatus.VISITED;

        tmp.X = x;
        tmp.Y = y;

        int val = Random.Range(0, 2);

        // Carve to the right
        if (val == 0)
        {
          grid.Map[x, y + 1].CellType = CellType.FLOOR;          

          grid.Map[x - 1, y - 1].Status = CellStatus.LOCKED;
          grid.Map[x - 1, y].Status = CellStatus.LOCKED;
          grid.Map[x - 1, y + 1].Status = CellStatus.LOCKED;
          grid.Map[x, y - 1].Status = CellStatus.LOCKED;
          grid.Map[x + 1, y - 1].Status = CellStatus.LOCKED;
          grid.Map[x + 1, y].Status = CellStatus.LOCKED;
        }
        else
        {
          grid.Map[x + 1, y].CellType = CellType.FLOOR;          

          grid.Map[x - 1, y - 1].Status = CellStatus.LOCKED;
          grid.Map[x - 1, y].Status = CellStatus.LOCKED;
          grid.Map[x - 1, y + 1].Status = CellStatus.LOCKED;
          grid.Map[x, y - 1].Status = CellStatus.LOCKED;
          grid.Map[x, y + 1].Status = CellStatus.LOCKED;
          grid.Map[x + 1, y - 1].Status = CellStatus.LOCKED;          
        }
      }
    }    
  }  
}
