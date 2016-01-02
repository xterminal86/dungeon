using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneratedMap
{
  protected const int _maxIdleIterations = 1000;

  protected int _mapWidth, _mapHeight;

  // For storing map objects to instantiate (walls, doors etc.)
  protected GeneratedMapCell[,] _map;
  public GeneratedMapCell[,] Map
  {
    get { return _map; }
  }

  // For storing availiability for traversal
  protected PathfindingCell[,] _pathfindingMap;
  public PathfindingCell[,] PathfindingMap
  {
    get { return _pathfindingMap; }
  }

  protected CameraStartingPos _cameraPos = new CameraStartingPos();
  public CameraStartingPos CameraPos
  {
    get { return _cameraPos; }
  }

  protected List<Int2> _unoccupiedCells = new List<Int2>();
  public List<Int2> UnoccupiedCells
  {
    get { return _unoccupiedCells; }
  }

  protected string _musicTrack = string.Empty;
  public string MusicTrack
  {
    get { return _musicTrack; }
  }

  public GeneratedMap(int width, int height)
  {
    _map = new GeneratedMapCell[width, height];
    _pathfindingMap = new PathfindingCell[width, height];

    _mapWidth = width;
    _mapHeight = height;

    for (int x = 0; x < _mapHeight; x++)
    {
      for (int y = 0; y < _mapWidth; y++)
      {
        _map[x, y] = new GeneratedMapCell();
        _pathfindingMap[x, y] = new PathfindingCell();
      }
    }
  }

  public virtual void Generate()
  {
  }

  public virtual void FillUnoccupiedCells()
  {
    _unoccupiedCells.Clear();

    for (int x = 0; x < _mapHeight; x++)
    {
      for (int y = 0; y < _mapWidth; y++)
      {
        if (_map[x, y].CellType == GeneratedCellType.NONE || _map[x, y].CellType == GeneratedCellType.ROAD)
        //if (_pathfindingMap[x, y].Walkable)
        {
          _unoccupiedCells.Add(new Int2(x, y));
        }
      }
    }
  }

  protected Vector2 GetRandomCellPos()
  {
    int x = Random.Range(1, _mapHeight - 1);
    int y = Random.Range(1, _mapWidth - 1);
    
    return new Vector2(x, y);
  }

  Int2 _unoccupiedCell = new Int2();
  public Int2 GetRandomUnoccupiedCell()
  {
    int index = Random.Range(0, _unoccupiedCells.Count - 1);

    return _unoccupiedCells[index];
  }

  Int2 _pos = new Int2();
  public Int2 GetRoadPosition()
  {
    for (int x = 0; x < _mapHeight; x++)
    {
      for (int y = 0; y < _mapWidth; y++)
      {
        if (_map[x, y].CellType == GeneratedCellType.ROAD)
        {
          _pos.X = x;
          _pos.Y = y;

          return _pos;
        }
      }
    }

    return null;
  }

  public void RemoveBlockAtPosition(int x, int y, int layer)
  {
    for (int i = 0; i < _map[x, y].Blocks.Count; i++)
    {
      if (_map[x, y].Blocks[i].Layer == layer)
      {
        _map[x, y].Blocks.RemoveAt(i);
        break;
      }
    }
  }

  public bool HasBlock(int x, int y, SerializableBlockType blockType)
  {
    foreach (var block in _map[x, y].Blocks)
    {
      if (block.BlockType == blockType)
      {
        return true;
      }
    }

    return false;
  }

  public SerializableBlock GetBlock(int x, int y, int layer)
  {
    foreach (var item in _map[x, y].Blocks)
    {
      if (item.X == x && item.Y == y && item.Layer == layer)
      {
        return item;
      }
    }

    return null;
  }

  public GeneratedMapCell GetMapCellByPosition(int x, int y)
  {
    int clampedX = Mathf.Clamp(x, 0, _mapHeight - 1);
    int clampedY = Mathf.Clamp(y, 0, _mapWidth - 1);

    return _map[clampedX, clampedY];
  }

  public GeneratedMapCell GetMapCellByPosition(Int2 mapPos)
  {
    return GetMapCellByPosition(mapPos.X, mapPos.Y);
  }
}

public class GeneratedMapCell
{
  public GeneratedCellType CellType = GeneratedCellType.NONE;

  public List<SerializableBlock> Blocks = new List<SerializableBlock>();
  public List<SerializableObject> Objects = new List<SerializableObject>();
};

public class PathfindingCell
{
  public bool Walkable = true;

  public Dictionary<GlobalConstants.Orientation, bool> SidesWalkability = new Dictionary<GlobalConstants.Orientation, bool>();

  public PathfindingCell()
  {
    Walkable = true;

    SidesWalkability[GlobalConstants.Orientation.EAST] = true;
    SidesWalkability[GlobalConstants.Orientation.SOUTH] = true;
    SidesWalkability[GlobalConstants.Orientation.WEST] = true;
    SidesWalkability[GlobalConstants.Orientation.NORTH] = true;
  }
};

public enum GeneratedCellType
{
  NONE = 0,
  ROAD,
  ROOM,
  OBSTACLE,
  DECOR
}