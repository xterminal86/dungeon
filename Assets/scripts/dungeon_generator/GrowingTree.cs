using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrowingTree : GenerationAlgorithmBase
{
  Grid _gridRef;

  List<Int2> _visitedCells = new List<Int2>();
  List<Int2> _untouchableWalls = new List<Int2>();

  int _safeguard = 0;

  bool _gtRandomFlag = false;
  public GrowingTree(bool gtRandomFlag)
  {
    _gtRandomFlag = gtRandomFlag;
  }

  public override void Do(Grid grid)
  {
    _gridRef = grid;

    //CreateBounds();

    Cell c = grid.GetRandomCell();
  
    Int2 pos = new Int2((int)c.Coordinates.x, (int)c.Coordinates.y);
    _visitedCells.Add(pos);

    MakeWall(pos);

    Int2 neighbour = new Int2();

    //Int2 neighbour = ChooseNeighbour(pos);
    //_visitedCells.Add(neighbour);
    //InvalidatePreviousCell(pos, neighbour);

    string debugString = string.Empty;

    while (_visitedCells.Count != 0)
    {
      if (_safeguard > 100000)
      {
        Debug.LogWarning("Terminated by safeguard");
        break;
      }

      if (_gtRandomFlag)
      {
        int index = Random.Range(0, _visitedCells.Count);
        pos = _visitedCells[index];

        neighbour = ChooseNeighbour(pos);
        if (neighbour == null)
        {
          RemoveCell(pos);
        }
        else
        {
          MakeWall(neighbour);
          _visitedCells.Add(neighbour);
          InvalidatePreviousCell(pos, neighbour);
        }
      }
      else
      {        
        pos = _visitedCells[_visitedCells.Count - 1];

        //Debug.Log("Current cell " + pos);

        neighbour = ChooseNeighbour(pos);
        if (neighbour == null)
        {          
          RemoveCell(pos);
        }
        else
        {
          MakeWall(neighbour);          
          _visitedCells.Add(neighbour);
          InvalidatePreviousCell(pos, neighbour);
        }

        /*
        debugString = string.Empty;

        debugString += string.Format("Visited cells ({0}) => ", _visitedCells.Count);

        foreach (var item in _visitedCells)
        {
          debugString += string.Format("{0}", item);
        }

        Debug.Log(debugString);
        */

        //Debug.Log(pos + " " + neighbour);
      }

      _safeguard++;
    }    
  }

  void CreateBounds()
  {
    Int2 cell = new Int2();
    for (int i = 0; i < _gridRef.MapWidth; i++)
    {
      cell.X = 0;
      cell.Y = i;
      _gridRef.Map[0, i].CellType = CellType.WALL;
      _untouchableWalls.Add(cell);

      cell.X = _gridRef.MapHeight - 1;
      cell.Y = i;
      _gridRef.Map[_gridRef.MapHeight - 1, i].CellType = CellType.WALL;
      _untouchableWalls.Add(cell);
    }

    for (int i = 1; i < _gridRef.MapHeight; i++)
    {
      cell.X = i;
      cell.Y = 0;
      _gridRef.Map[i, 0].CellType = CellType.WALL;
      _untouchableWalls.Add(cell);

      cell.X = i;
      cell.Y = _gridRef.MapWidth - 1;
      _gridRef.Map[i, _gridRef.MapWidth - 1].CellType = CellType.WALL;
      _untouchableWalls.Add(cell);
    }
  }

  void InvalidatePreviousCell(Int2 cell, Int2 neighbour)
  {
    int xMin = cell.X - 1;
    int xMax = cell.X + 1;
    int yMin = cell.Y - 1;
    int yMax = cell.Y + 1;

    xMin = Mathf.Clamp(xMin, 0, _gridRef.MapHeight - 1);
    xMax = Mathf.Clamp(xMax, 0, _gridRef.MapHeight - 1);
    yMin = Mathf.Clamp(yMin, 0, _gridRef.MapWidth - 1);
    yMax = Mathf.Clamp(yMax, 0, _gridRef.MapWidth - 1);

    int nx = neighbour.X;
    int ny = neighbour.Y;

    Int2 tmp = new Int2();
    for (int x = xMin; x <= xMax; x++)
    {
      for (int y = yMin; y <= yMax; y++)
      {
        tmp.X = x;
        tmp.Y = y;

        if ((x == cell.X && y == cell.Y) || (x == tmp.X && y == tmp.Y)) continue;

        if (!DoesCellExist(tmp, _visitedCells))
        {
          _visitedCells.Add(tmp);
        }
      }
    }
  }

  void RemoveCell(Int2 pos)
  {
    int x = pos.X;
    int y = pos.Y;

    for (int i = 0; i < _visitedCells.Count; i++)
    {
      if (_visitedCells[i].X == x && _visitedCells[i].Y == y)
      {
        _visitedCells.RemoveAt(i);
        break;
      }
    }
  }

  void MakeWall(Int2 center)
  {
    //Debug.Log("Making wall around " + center);

    int xMin = center.X - 1;
    int xMax = center.X + 1;
    int yMin = center.Y - 1;
    int yMax = center.Y + 1;
    
    xMin = Mathf.Clamp(xMin, 0, _gridRef.MapHeight - 1);
    xMax = Mathf.Clamp(xMax, 0, _gridRef.MapHeight - 1);
    yMin = Mathf.Clamp(yMin, 0, _gridRef.MapWidth - 1);
    yMax = Mathf.Clamp(yMax, 0, _gridRef.MapWidth - 1);

    //Debug.Log(string.Format("xMin {0} yMin {1} xMax {2} yMax {3}", xMin, yMin, xMax, yMax));

    for (int i = xMin; i <= xMax; i++)
    {
      for (int j = yMin; j <= yMax; j++)
      {
        if (_gridRef.Map[i, j].CellType == CellType.FLOOR) continue;

        if (i == center.X && j == center.Y)
        {          
          _gridRef.Map[i, j].CellType = CellType.FLOOR;
        }
        else
        {
          _gridRef.Map[i, j].CellType = CellType.WALL;
        }
      }
    }

    /*
    Int2 ul = new Int2(xMin, yMin);
    Int2 ur = new Int2(xMax, yMin);
    Int2 bl = new Int2(xMin, yMax);
    Int2 br = new Int2(xMax, yMax);

    if (!DoesCellExist(ul, _untouchableWalls))
    {
      _untouchableWalls.Add(ul);
    }

    if (!DoesCellExist(ur, _untouchableWalls))
    {
      _untouchableWalls.Add(ur);
    }

    if (!DoesCellExist(bl, _untouchableWalls))
    {
      _untouchableWalls.Add(bl);
    }

    if (!DoesCellExist(br, _untouchableWalls))
    {
      _untouchableWalls.Add(br);
    }
    */
  }

  List<Int2> _neighbours = new List<Int2>();
  Int2 ChooseNeighbour(Int2 center)
  {
    _neighbours.Clear();

    bool res = false;

    Int2 up = new Int2(center.X - 1, center.Y);
    Int2 down = new Int2(center.X + 1, center.Y);
    Int2 left = new Int2(center.X, center.Y - 1);
    Int2 right = new Int2(center.X, center.Y + 1);

    res = IsCellValid(up);
    if (res) _neighbours.Add(up);

    res = IsCellValid(down);
    if (res) _neighbours.Add(down);

    res = IsCellValid(left);
    if (res) _neighbours.Add(left);

    res = IsCellValid(right);
    if (res) _neighbours.Add(right);

    if (_neighbours.Count == 0)
    {
      //Debug.Log("No neighbours found!");

      return null;
    }

    int index = Random.Range(0, _neighbours.Count);

    //Debug.Log("Choosed " + _neighbours[index]);

    return _neighbours[index];
  }

  bool IsCellValid(Int2 pos)
  {
    int x = pos.X;
    int y = pos.Y;

    if (x == 0 || x == _gridRef.MapHeight - 1 || y == 0 || y == _gridRef.MapWidth - 1)
    {
      return false;
    }

    /*
    foreach (var item in _untouchableWalls)
    {
      if (item.X == x && item.Y == y)
      {
        return false;
      }
    }
    */

    if (DoesCellExist(pos, _visitedCells))
    {
      return false;
    }

    return true;
  }

  bool DoesCellExist(Int2 pos, List<Int2> listToSearch)
  {
    int x = pos.X;
    int y = pos.Y;

    foreach (var item in listToSearch)
    {
      if (item.X == x && item.Y == y)
      {
        return true;
      }
    }

    return false;
  }
}
