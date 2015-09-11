using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sidewinder : GenerationAlgorithmBase
{
  public override void Do(Grid grid)
  {
    Vector2 coords = Vector2.zero;
    List<Vector2> run = new List<Vector2>();

    for (int x = 0; x < grid.MapHeight; x++)
    {
      for (int y = 0; y < grid.MapWidth; y++)
      {
        coords.Set(x, y);
        run.Add(coords);

        int val = Random.Range(0, 2);

        if (val == 0)
        {
          grid.Map[x, y].VisualRepresentation = (char)CellVisualization.EMPTY;
        }
        else
        {
          int cellIndex = Random.Range(0, run.Count);
          Cell c = grid.GetCell((int)run[cellIndex].x, (int)run[cellIndex].y);
          Cell down = grid.GetCell((int)run[cellIndex].x, (int)run[cellIndex].y + 1);
          if (c != null && down != null)
          {
            c.Link(down);
            run.Clear();
          }
        }
      }
    }
  }
}
