using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneratedMap
{
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
}

public class GeneratedMapCell
{
  public List<SerializableBlock> Blocks = new List<SerializableBlock>();
  public List<SerializableObject> Objects = new List<SerializableObject>();
};