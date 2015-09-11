using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cell 
{
  public char VisualRepresentation = ' ';

  public Dictionary<CellDirections, Cell> Neighbours = new Dictionary<CellDirections, Cell>();

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

  public Cell(Vector2 coords)
  {
    _coordinates.Set(coords.x, coords.y);
    _links.Clear();
    VisualRepresentation = (char)CellVisualization.WALL;
  }

  public void Link(Cell c, bool bidirectional = true)
  {
    if (_links.Contains(c)) return;

    _links.Add(c);

    if (bidirectional)
    {
      c.Links.Add(this);
      c.VisualRepresentation = (char)CellVisualization.EMPTY;
    }
  }

  public void Reset()
  {
    _links.Clear();
    VisualRepresentation = (char)CellVisualization.WALL;
  }

  public void SetVisualRepresentation(CellVisualization type)
  {
    VisualRepresentation = (char)type;
  }

  public void Unlink(Cell c, bool bidirectional = true)
  {
    if (!_links.Contains(c)) return;

    _links.Remove(c);

    if (bidirectional)
    {
      c.Links.Remove(this);
      c.VisualRepresentation = (char)CellVisualization.WALL;
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
  WALL,
  ROOM,
  CORRIDOR
}

public enum CellDirections
{
  NORTH = 0,
  EAST,
  SOUTH,
  WEST
}
