using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cell 
{
  public Dictionary<CellDirections, Cell> Neighbours = new Dictionary<CellDirections, Cell>();

  public CellStatus Status;

  Vector2 _coordinates = Vector2.zero;
  public Vector2 Coordinates
  {
    get { return _coordinates; }
  }

  List<Cell> _links = new List<Cell>();
  public List<Cell> Links
  {
    get { return _links; }
  }

  Dictionary<CellType, char> _visualRepresentation = new Dictionary<CellType, char>()
  {
    { CellType.EMPTY, ' ' },
    { CellType.FLOOR, '.' },
    { CellType.WALL, '#' },
    { CellType.BOUNDARY, '!' }
  };

  public char VisualRepresentation
  {
    get { return _visualRepresentation[CellType]; }
  }

  public CellType CellType = CellType.EMPTY;

  public Cell(Vector2 coords, CellType defaultCell)
  {
    _coordinates.Set(coords.x, coords.y);
    _links.Clear();
    CellType = defaultCell;
    Status = CellStatus.UNVISITED;
  }

  public void Link(Cell c, bool bidirectional = true)
  {
    if (_links.Contains(c)) return;

    _links.Add(c);

    if (bidirectional)
    {
      c.Links.Add(this);
      c.CellType = CellType.EMPTY;
    }
  }

  public void Reset()
  {
    _links.Clear();
    CellType = CellType.EMPTY;
  }

  public void Unlink(Cell c, bool bidirectional = true)
  {
    if (!_links.Contains(c)) return;

    _links.Remove(c);

    if (bidirectional)
    {
      c.Links.Remove(this);
      c.CellType = CellType.WALL;
    }
  }

  public bool IsLinkedTo(Cell c)
  {
    return _links.Contains(c);
  }

  public bool IsLinked()
  {
    return (_links.Count != 0);
  }
}

public enum CellType
{
  EMPTY = 0,
  FLOOR,
  WALL,
  BOUNDARY
}

public enum CellStatus
{
  UNVISITED = 0,
  VISITED,
  LOCKED
}

public enum CellDirections
{
  NORTH = 0,
  EAST,
  SOUTH,
  WEST
}
