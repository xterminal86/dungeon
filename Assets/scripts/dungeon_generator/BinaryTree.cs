using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class BinaryTree : GenerationAlgorithmBase
{
  Grid _gridRef;

  /// <summary>
  /// Adaptation of "Binary Tree" algorithm (from e-book "Mazes for Programmers" by Jamis Buck, ISBN-13: 978-1-68050-055-4, p. 6)
  /// 
  /// General description given in the book:
  /// 1) Starting from random unvisited point, decide to carve north or east
  /// 2) Mark current cell as visited
  /// 3) Repeat until all cells are visited
  /// 
  /// Algorithm described in the book was designed for "paper sheet walls" grid (look for comments in GrowingTree.cs), 
  /// so I had to lock walls to get more or less similar behaviour.
  /// This particular implementation of the algorithm carves right or down (i.e. east or south) as opposed to north or east.
  /// 
  /// </summary>
  /// <param name="grid"></param>
  public override void Do(Grid grid)
  {
    _gridRef = grid;

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
          // Lock down
          grid.Map[x + 1, y].Status = CellStatus.LOCKED;
        }
        // Carve down
        else
        {
          grid.Map[x + 1, y].CellType = CellType.FLOOR;          
          // Lock right
          grid.Map[x, y + 1].Status = CellStatus.LOCKED;          
        }
      }
    }
    
    // Erase last column and row before maze boundary to avoid getting unreachable zones in the maze
    for (int i = 1; i < grid.MapHeight - 1; i++)
    {
      grid.Map[i, grid.MapWidth - 2].CellType = CellType.FLOOR;
    }

    for (int i = 1; i < grid.MapWidth - 1; i++)
    {
      grid.Map[grid.MapHeight - 2, i].CellType = CellType.FLOOR;
    }

    FindStartingPos();
    SaveToFile();
  }

  void FindStartingPos()
  {
    for (int x = 0; x < _gridRef.MapHeight; x++)
    {
      for (int y = 0; y < _gridRef.MapWidth; y++)
      {
        if (_gridRef.Map[x, y].CellType == CellType.FLOOR)
        {
          _serializableMap.CameraPos.X = x;
          _serializableMap.CameraPos.Y = y;
          return;
        }
      }
    }
  }

  public override void SaveToFile()
  {
    _serializableMap.MapWdith = _gridRef.MapWidth;
    _serializableMap.MapHeight = _gridRef.MapHeight;

    _binaryFilename = string.Format("SerializedMaps/{0}.map", this.ToString());

    FileStream fs = new FileStream(_binaryFilename, FileMode.Create);
    BinaryFormatter bf = new BinaryFormatter();

    for (int x = 0; x < _gridRef.MapHeight; x++)
    {
      for (int y = 0; y < _gridRef.MapWidth; y++)
      {
        switch (_gridRef.Map[x, y].CellType)
        {
          case CellType.FLOOR:
            PackBlock(x, y, 0, "floor-bricks-small", 0, false);
            PackBlock(x, y, 2, "floor-bricks-big", 0, true);
            break;
          case CellType.WALL:
            PackBlock(x, y, 0, "block-tiles", 0, false);
            PackBlock(x, y, 1, "block-tiles", 0, false);
            break;
        }
      }
    }

    bf.Serialize(fs, _serializableMap);
    fs.Close();
  }
}
