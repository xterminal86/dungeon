using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrowingTree : GenerationAlgorithmBase
{
  Grid _gridRef;

  List<Int2> _visitedCells = new List<Int2>();
  
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
    pos.X = 8;
    pos.Y = 3;
    grid.Map[pos.X, pos.Y].Status = CellStatus.VISITED;
    _visitedCells.Add(pos);

    //Debug.Log("Starting point: " + pos);

    MakeWall(pos);

    Int2 neighbour = new Int2();
    while (_visitedCells.Count != 0)
    {
      if (_safeguard > 100000)
      {
        Debug.LogWarning("Terminated by safeguard on " + pos);
        break;
      }

      if (_gtRandomFlag)
      {
        int val = Random.Range(0, _visitedCells.Count);
        pos = _visitedCells[val];
      }
      else
      {
        pos = _visitedCells[_visitedCells.Count - 1];
      }

      //Debug.Log("Current cell " + pos);

      neighbour = ChooseNeighbour(pos);            
      if (neighbour == null)
      {
        RemoveCell(pos);
      }
      else
      {        
        MakeWall(neighbour);
        _gridRef.Map[neighbour.X, neighbour.Y].Status = CellStatus.VISITED;
        _visitedCells.Add(neighbour);
        LockPreviousCellWalls(pos, neighbour);
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
      _gridRef.Map[0, i].Status = CellStatus.LOCKED;

      cell.X = _gridRef.MapHeight - 1;
      cell.Y = i;
      _gridRef.Map[_gridRef.MapHeight - 1, i].CellType = CellType.WALL;
      _gridRef.Map[_gridRef.MapHeight - 1, i].Status = CellStatus.LOCKED;
    }

    for (int i = 1; i < _gridRef.MapHeight; i++)
    {
      cell.X = i;
      cell.Y = 0;
      _gridRef.Map[i, 0].CellType = CellType.WALL;
      _gridRef.Map[i, 0].Status = CellStatus.LOCKED;

      cell.X = i;
      cell.Y = _gridRef.MapWidth - 1;
      _gridRef.Map[i, _gridRef.MapWidth - 1].CellType = CellType.WALL;
      _gridRef.Map[i, _gridRef.MapWidth - 1].Status = CellStatus.LOCKED;      
    }
  }

  void LockPreviousCellWalls(Int2 prevPos, Int2 neighbour)
  {
    bool leftPassage = (prevPos.X - neighbour.X) > 0;
    bool downPassage = (prevPos.Y - neighbour.Y) < 0;

    if (leftPassage)
    {
      // ##XX#
      // #..X#
      // ##XX#

      if (_gridRef.Map[prevPos.X - 1, prevPos.Y].CellType == CellType.WALL)     { _gridRef.Map[prevPos.X - 1, prevPos.Y].Status = CellStatus.LOCKED; }
      if (_gridRef.Map[prevPos.X + 1, prevPos.Y].CellType == CellType.WALL)     { _gridRef.Map[prevPos.X + 1, prevPos.Y].Status = CellStatus.LOCKED; }
      //if (_gridRef.Map[prevPos.X - 1, prevPos.Y + 1].CellType == CellType.WALL) { _gridRef.Map[prevPos.X - 1, prevPos.Y + 1].Status = CellStatus.LOCKED; }
      if (_gridRef.Map[prevPos.X, prevPos.Y + 1].CellType == CellType.WALL) { _gridRef.Map[prevPos.X, prevPos.Y + 1].Status = CellStatus.LOCKED; }
      //if (_gridRef.Map[prevPos.X + 1, prevPos.Y + 1].CellType == CellType.WALL) { _gridRef.Map[prevPos.X + 1, prevPos.Y + 1].Status = CellStatus.LOCKED; }
    }
    else if (!leftPassage)
    {
      // #XX##
      // #X..#
      // #XX##

      if (_gridRef.Map[prevPos.X - 1, prevPos.Y].CellType == CellType.WALL)     { _gridRef.Map[prevPos.X - 1, prevPos.Y].Status = CellStatus.LOCKED; }
      if (_gridRef.Map[prevPos.X + 1, prevPos.Y].CellType == CellType.WALL)     { _gridRef.Map[prevPos.X + 1, prevPos.Y].Status = CellStatus.LOCKED; }
      //if (_gridRef.Map[prevPos.X - 1, prevPos.Y - 1].CellType == CellType.WALL) { _gridRef.Map[prevPos.X - 1, prevPos.Y - 1].Status = CellStatus.LOCKED; }
      if (_gridRef.Map[prevPos.X, prevPos.Y - 1].CellType == CellType.WALL) { _gridRef.Map[prevPos.X, prevPos.Y - 1].Status = CellStatus.LOCKED; }
      //if (_gridRef.Map[prevPos.X + 1, prevPos.Y - 1].CellType == CellType.WALL) { _gridRef.Map[prevPos.X + 1, prevPos.Y - 1].Status = CellStatus.LOCKED; }
    }
    else if (downPassage)
    {
      // XXX
      // X.X
      // #.#
      // ###

      //if (_gridRef.Map[prevPos.X - 1, prevPos.Y - 1].CellType == CellType.WALL) { _gridRef.Map[prevPos.X - 1, prevPos.Y - 1].Status = CellStatus.LOCKED; }
      if (_gridRef.Map[prevPos.X - 1, prevPos.Y].CellType == CellType.WALL) { _gridRef.Map[prevPos.X - 1, prevPos.Y].Status = CellStatus.LOCKED; }
      //if (_gridRef.Map[prevPos.X - 1, prevPos.Y + 1].CellType == CellType.WALL) { _gridRef.Map[prevPos.X - 1, prevPos.Y + 1].Status = CellStatus.LOCKED; }
      if (_gridRef.Map[prevPos.X, prevPos.Y - 1].CellType == CellType.WALL)     { _gridRef.Map[prevPos.X, prevPos.Y - 1].Status = CellStatus.LOCKED; }
      if (_gridRef.Map[prevPos.X, prevPos.Y + 1].CellType == CellType.WALL)     { _gridRef.Map[prevPos.X, prevPos.Y + 1].Status = CellStatus.LOCKED; }
    }
    else if (!downPassage)
    {
      // ###
      // #.#
      // X.X
      // XXX

      //if (_gridRef.Map[prevPos.X + 1, prevPos.Y - 1].CellType == CellType.WALL) { _gridRef.Map[prevPos.X + 1, prevPos.Y - 1].Status = CellStatus.LOCKED; }
      if (_gridRef.Map[prevPos.X + 1, prevPos.Y].CellType == CellType.WALL) { _gridRef.Map[prevPos.X + 1, prevPos.Y].Status = CellStatus.LOCKED; }
      //if (_gridRef.Map[prevPos.X + 1, prevPos.Y + 1].CellType == CellType.WALL) { _gridRef.Map[prevPos.X + 1, prevPos.Y + 1].Status = CellStatus.LOCKED; }
      if (_gridRef.Map[prevPos.X, prevPos.Y - 1].CellType == CellType.WALL)     { _gridRef.Map[prevPos.X, prevPos.Y - 1].Status = CellStatus.LOCKED; }
      if (_gridRef.Map[prevPos.X, prevPos.Y + 1].CellType == CellType.WALL)     { _gridRef.Map[prevPos.X, prevPos.Y + 1].Status = CellStatus.LOCKED; }
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

    if (x <= 1 || x >= _gridRef.MapHeight - 1 || y <= 1 || y >= _gridRef.MapWidth - 1)
    {
      return false;
    }

    if (_gridRef.Map[x, y].Status == CellStatus.VISITED || _gridRef.Map[x, y].Status == CellStatus.LOCKED)
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
