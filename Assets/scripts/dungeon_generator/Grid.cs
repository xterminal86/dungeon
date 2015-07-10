using UnityEngine;
using System.Collections;

public class Grid 
{
  int _mapWidth = -1, _mapHeight = -1;
  public int MapSize
  {
    get { return _mapWidth * _mapHeight; }
  }

  public int MapWidth
  {
    get { return _mapWidth; }
  }

  public int MapHeight
  {
    get { return _mapHeight; }
  }

  Cell[,] _map;
  public Cell[,] Map
  {
    get { return _map; }
  }

  public Grid (int mapWidth, int mapHeight)
  {
    _mapWidth = mapWidth;
    _mapHeight = mapHeight;
    PrepareGrid();
    ConfigureCells();
  }

  protected virtual void PrepareGrid()
  {
    // Remember, that 10x5 means 10 columns and 5 rows
    _map = new Cell[_mapHeight, _mapWidth];

    Vector2 coords = Vector2.zero;
    for (int x = 0; x < _mapHeight; x++)
    {
      for (int y = 0; y < _mapWidth; y++)
      {
        coords.Set(x, y);
        _map[x, y] = new Cell(coords);
      }
    }
  }

  protected virtual void ConfigureCells()
  {
    int nextRow = -1, nextColumn = -1, prevRow = -1, prevColumn = -1;
    for (int x = 0; x < _mapHeight; x++)
    {
      nextRow = x + 1 > _mapHeight - 1 ? -1 : x + 1;
      prevRow = x - 1 < 0 ? -1 : x - 1;

      for (int y = 0; y < _mapWidth; y++)
      {
        nextColumn = y + 1 > _mapWidth - 1 ? -1 : y + 1;
        prevColumn = y - 1 < 0 ? -1 : y - 1;

        _map[x, y].Neighbours[CellDirections.NORTH] = (prevRow == -1) ? null : _map[prevRow, y];
        _map[x, y].Neighbours[CellDirections.EAST] = (nextColumn == -1) ? null : _map[x, nextColumn];
        _map[x, y].Neighbours[CellDirections.SOUTH] = (nextRow == -1) ? null : _map[nextRow, y];
        _map[x, y].Neighbours[CellDirections.WEST] = (prevColumn == -1) ? null : _map[x, prevColumn];
      }
    }
  }

  public virtual Cell GetCell(int x, int y)
  {
    if (x < 0 || x > _mapHeight - 1 || y < 0 || y > _mapWidth - 1)
    {
      return null;
    }

    return _map[x, y];
  }

  public Cell GetRandomCell()
  {
    int x = Random.Range(0, _mapHeight);
    int y = Random.Range(0, _mapWidth);

    return _map[x, y];
  }
}

public enum CellType
{
  EMPTY = '.',
  WALL = '#'
}
