using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sidewinder : GenerationAlgorithmBase
{
  Grid _gridRef;

  /// <summary>
  /// Adaptation of "Sidewinder" algorithm (from e-book "Mazes for Programmers" by Jamis Buck, ISBN-13: 978-1-68050-055-4, p. 6)
  /// 
  /// General description:
  /// 1) Start from lower left corner of the grid
  /// 2) Randomly carve east or north
  /// 3) If decided to carve east, add previous cell to the list (so called "run")
  /// 4) If decided to carve north, close the run (randomly choose cell from the list and carve north from there)
  /// 5) Repeat until all cells are visited
  /// 
  /// In this implementation we carve right or down.
  /// Sometimes there can be inaccessible areas, due to locking of opposite walls,
  /// i.e. right, when we carve down, and down, when we carve right.
  /// 
  /// </summary>
  /// <param name="grid">Grid.</param>
  public override void Do(Grid grid)
  {
    _gridRef = grid;

    CreateBounds();

    Int2 tmp = new Int2();

    List<Int2> run = new List<Int2>();

    for (int x = 1; x <= grid.MapHeight - 2; x++)
    {
      for (int y = 1; y <= grid.MapWidth - 2; y++)
      {
        if (grid.Map[x, y].Status == CellStatus.VISITED || grid.Map[x, y].Status == CellStatus.LOCKED) continue;

        grid.Map[x, y].CellType = CellType.FLOOR;
        grid.Map[x, y].Status = CellStatus.VISITED;

        if (x == grid.MapHeight - 2)
        {
          if (grid.Map[x, y + 1].Status != CellStatus.LOCKED)
          {
            grid.Map[x, y + 1].CellType = CellType.FLOOR;
          }
        }
        else if (y == grid.MapWidth - 2)
        {
          if (grid.Map[x + 1, y].Status != CellStatus.LOCKED)
          {
            grid.Map[x + 1, y].CellType = CellType.FLOOR;
          }
        }
        else
        {
          int val = Random.Range(0, 2);

          // Carve right
          if (val == 0)
          {
            grid.Map[x, y + 1].CellType = CellType.FLOOR;
            grid.Map[x + 1, y].Status = CellStatus.LOCKED;
            tmp.X = x;
            tmp.Y = y + 1;
            run.Add(tmp);
          }
          // Carve down
          else
          {
            if (run.Count != 0)
            {
              int index = Random.Range(0, run.Count);
              Int2 pos = run[index];
              grid.Map[pos.X + 1, pos.Y].CellType = CellType.FLOOR;
              if (grid.Map[pos.X, pos.Y + 1].CellType == CellType.WALL)
              {
                grid.Map[pos.X, pos.Y + 1].Status = CellStatus.LOCKED;
              }
              run.Clear();
            }
            else
            {
              grid.Map[x + 1, y].CellType = CellType.FLOOR;
              if (grid.Map[x, y + 1].CellType == CellType.WALL)
              {
                grid.Map[x, y + 1].Status = CellStatus.LOCKED;
              }
            }
          }
        }
      }
    }
  }

  void CreateBounds()
  {
    for (int i = 0; i < _gridRef.MapWidth; i++)
    {
      _gridRef.Map[0, i].Status = CellStatus.LOCKED;
      _gridRef.Map[_gridRef.MapHeight - 1, i].Status = CellStatus.LOCKED;
    }
    
    for (int i = 1; i < _gridRef.MapHeight; i++)
    {
      _gridRef.Map[i, 0].Status = CellStatus.LOCKED;
      _gridRef.Map[i, _gridRef.MapWidth - 1].Status = CellStatus.LOCKED;      
    }
  }
}
