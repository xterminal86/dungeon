using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneratedMap
{
  protected const int _maxIdleIterations = 1000;

  protected int _mapWidth, _mapHeight;
  
  protected GeneratedMapCell[,] _map;
  public GeneratedMapCell[,] Map
  {
    get { return _map; }
  }

  protected CameraStartingPos _cameraPos = new CameraStartingPos();
  public CameraStartingPos CameraPos
  {
    get { return _cameraPos; }
  }

  protected string _musicTrack = string.Empty;
  public string MusicTrack
  {
    get { return _musicTrack; }
  }

  public GeneratedMap(int width, int height)
  {
    _map = new GeneratedMapCell[width, height];
    _mapWidth = width;
    _mapHeight = height;

    for (int x = 0; x < _mapHeight; x++)
    {
      for (int y = 0; y < _mapWidth; y++)
      {
        _map[x, y] = new GeneratedMapCell();
      }
    }
  }

  public virtual void Generate()
  {
  }

  protected Vector2 GetRandomCellPos()
  {
    int x = Random.Range(1, _mapHeight - 1);
    int y = Random.Range(1, _mapWidth - 1);
    
    return new Vector2(x, y);
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
}

public class GeneratedMapCell
{
  public GeneratedCellType CellType = GeneratedCellType.NONE;

  public List<SerializableBlock> Blocks = new List<SerializableBlock>();
  public List<SerializableObject> Objects = new List<SerializableObject>();
};

public enum GeneratedCellType
{
  NONE = 0,
  ROAD,
  ROOM,
  OBSTACLE
}