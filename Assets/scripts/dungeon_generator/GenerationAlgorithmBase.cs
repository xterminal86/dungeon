using UnityEngine;
using System.Collections;

public abstract class GenerationAlgorithmBase 
{
  public virtual void Do(Grid grid)
  {
  }

  public virtual void Cleanup(Grid grid)
  {    
    for (int x = 0; x < grid.MapHeight; x++)
    {
      for (int y = 0; y < grid.MapWidth; y++)
      {        
        if (grid.Map[x == 0 ? 0 : x - 1, y].CellType == CellType.FLOOR || grid.Map[x == grid.MapHeight - 1 ? grid.MapHeight - 1 : x + 1, y].CellType == CellType.FLOOR
         || grid.Map[x, y == 0 ? 0 : y - 1].CellType == CellType.FLOOR || grid.Map[x, y == grid.MapWidth - 1 ? grid.MapWidth - 1 : y + 1].CellType == CellType.FLOOR)
        {
          grid.Map[x, y].CellStatusForCleanup = CellType.BOUNDARY;
        }
      }
    }

    for (int x = 0; x < grid.MapHeight; x++)
    {
      for (int y = 0; y < grid.MapWidth; y++)
      {
        if (grid.Map[x, y].CellStatusForCleanup != CellType.BOUNDARY)
        {
          grid.Map[x, y].CellType = CellType.EMPTY;
        }
      }
    }
  }
}
