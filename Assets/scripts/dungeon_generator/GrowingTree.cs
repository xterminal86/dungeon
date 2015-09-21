using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrowingTree : GenerationAlgorithmBase
{
  Grid _gridRef;

  List<Int2> _visitedCells = new List<Int2>();
  
  int _safeguard = 0;

  DecisionType _decisionType;
  public GrowingTree(DecisionType decisionType)
  {
    _decisionType = decisionType;
  }

  /// <summary>
  /// General description:
  /// 1) Choose random cell, add it to visited cells list
  /// 2) Choose random neighbour (up, down, left, right)
  /// 3) If neighbour can be chosen, add it to visited cells
  /// 4) Depending on the option, choose next cell and repeat from 2 while visited cells list is not empty
  /// 
  /// Option consists of choosing next cell either as the last added to the visited cells list or randomly from it.  
  /// 
  /// This particular implementation uses passage carving approach.
  /// Resulting maze is perfect, i.e. there are no loops, 2x2 floors etc.
  /// 
  /// </summary>
  /// <param name="grid"></param>
  public override void Do(Grid grid)
  {
    _gridRef = grid;
    
    Cell c = grid.GetRandomCell();
    
    Int2 pos = new Int2((int)c.Coordinates.x, (int)c.Coordinates.y);
    grid.Map[pos.X, pos.Y].CellType = CellType.FLOOR;
    grid.Map[pos.X, pos.Y].Status = CellStatus.VISITED;
    _visitedCells.Add(pos);

    Debug.Log("Starting point: " + pos);

    Int2 neighbour = new Int2();
    while (_visitedCells.Count != 0)
    {
      if (_safeguard > 1000000)
      {
        Debug.LogWarning("Terminated by safeguard on " + pos);
        break;
      }

      if (_decisionType == DecisionType.RANDOM)
      {
        int val = Random.Range(0, _visitedCells.Count);
        pos = _visitedCells[val];
      }
      else if (_decisionType == DecisionType.NEWEST)
      {
        pos = _visitedCells[_visitedCells.Count - 1];
      }
      else if (_decisionType == DecisionType.OLDEST)
      {
        pos = _visitedCells[0];
      }

      neighbour = ChooseNeighbour(pos);
      if (neighbour == null)
      {
        //Debug.Log("Couldn't find neighbours of " + pos + " - removing it from the list");

        RemoveCell(pos);
      }
      else
      { 
        //Debug.Log(pos + " - found neighbour " + neighbour);

        _gridRef.Map[neighbour.X, neighbour.Y].CellType = CellType.FLOOR;
        _gridRef.Map[neighbour.X, neighbour.Y].Status = CellStatus.VISITED;
        _visitedCells.Add(neighbour);
      }

      _safeguard++;
    }
  }

  void CreateBounds()
  {
    for (int i = 0; i < _gridRef.MapWidth; i++)
    {
      _gridRef.Map[0, i].CellType = CellType.WALL;
      _gridRef.Map[0, i].Status = CellStatus.LOCKED;

      _gridRef.Map[_gridRef.MapHeight - 1, i].CellType = CellType.WALL;
      _gridRef.Map[_gridRef.MapHeight - 1, i].Status = CellStatus.LOCKED;
    }

    for (int i = 1; i < _gridRef.MapHeight; i++)
    {
      _gridRef.Map[i, 0].CellType = CellType.WALL;
      _gridRef.Map[i, 0].Status = CellStatus.LOCKED;

      _gridRef.Map[i, _gridRef.MapWidth - 1].CellType = CellType.WALL;
      _gridRef.Map[i, _gridRef.MapWidth - 1].Status = CellStatus.LOCKED;      
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
    if (res)
    {
      _neighbours.Add(up);
    }

    res = IsCellValid(down);
    if (res)
    {
      _neighbours.Add(down);
    }

    res = IsCellValid(left);
    if (res)
    {
      _neighbours.Add(left);
    }

    res = IsCellValid(right);
    if (res)
    {
      _neighbours.Add(right);
    }

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
    
    if (x <= 0 || x >= _gridRef.MapHeight - 1 || y <= 0 || y >= _gridRef.MapWidth - 1 || _gridRef.Map[x, y].Status == CellStatus.VISITED)
    {
      return false;
    }

    int count = 0;

    if (_gridRef.Map[x - 1, y].CellType == CellType.FLOOR) { count++; }
    if (_gridRef.Map[x + 1, y].CellType == CellType.FLOOR) { count++; }
    if (_gridRef.Map[x, y - 1].CellType == CellType.FLOOR) { count++; }
    if (_gridRef.Map[x, y + 1].CellType == CellType.FLOOR) { count++; }

    return (count == 1);
  }
}

public enum DecisionType
{
  NEWEST = 0,
  RANDOM,
  OLDEST
}
