﻿using UnityEngine;
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

    Cell c = grid.GetRandomCell();
  
    Int2 pos = new Int2((int)c.Coordinates.x, (int)c.Coordinates.y);
    _visitedCells.Add(pos);

    MakeWall(pos);
    Int2 neighbour = ChooseNeighbour(pos);
    _visitedCells.Add(neighbour);

    while (_visitedCells.Count != 0)
    {
      if (_safeguard > grid.MapSize) break;

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
        }
      }
      else
      {
        pos = _visitedCells[_visitedCells.Count - 1];

        neighbour = ChooseNeighbour(pos);
        if (neighbour == null)
        {
          RemoveCell(pos);
        }
        else
        {
          MakeWall(neighbour);
          _visitedCells.Add(neighbour);
        }
      }

      _safeguard++;
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
    int xMin = center.X - 1;
    int xMax = center.X + 1;
    int yMin = center.Y - 1;
    int yMax = center.Y + 1;

    xMin = Mathf.Clamp(xMin, 0, _gridRef.MapHeight);
    xMax = Mathf.Clamp(xMax, 0, _gridRef.MapHeight);
    yMin = Mathf.Clamp(yMin, 0, _gridRef.MapWidth);
    yMax = Mathf.Clamp(yMax, 0, _gridRef.MapWidth);

    for (int i = xMin; i <= xMax; i++)
    {
      for (int j = yMin; j <= yMax; j++)
      {
        if (_gridRef.Map[i, j].CellType != CellType.EMPTY) continue;

        if (i == center.X && j == center.Y)
        {
          _gridRef.Map[i, j].CellType = CellType.FLOOR;
        }

        _gridRef.Map[i, j].CellType = CellType.WALL;
      }
    }

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
      return null;
    }

    int index = Random.Range(0, _neighbours.Count);

    return _neighbours[index];
  }

  bool IsCellValid(Int2 pos)
  {
    int x = pos.X;
    int y = pos.Y;

    if (x == 0 || x == _gridRef.MapHeight || y == 0 || y == _gridRef.MapWidth)
    {
      return false;
    }

    foreach (var item in _untouchableWalls)
    {
      if (item.X == x && item.Y == y)
      {
        return false;
      }
    }

    foreach (var item in _visitedCells)
    {
      if (item.X == x && item.Y == y)
      {
        return false;
      }
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