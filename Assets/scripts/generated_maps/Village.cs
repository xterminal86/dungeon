using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Village : GeneratedMap
{
  bool[,] _occupiedCells;
  public Village(int width, int height) : base(width, height)
  {
    _occupiedCells = new bool[width, height];
    ResetOccupiedCells();
  }

  void ResetOccupiedCells()
  {
    for (int i = 0; i < _mapWidth; i++)
    {
      for (int j = 0; j < _mapHeight; j++)
      {
        _occupiedCells[i, j] = false;
      }
    }         
  }

  public override void Generate()
  {
    for (int x = 0; x < _mapHeight; x++)
    {
      for (int y = 0; y < _mapWidth; y++)
      {
        SerializableBlock b = new SerializableBlock();
        b.X = x;
        b.Y = y;
        b.Layer = 0;
        b.PrefabName = GlobalConstants.StaticPrefabsNamesById[GlobalConstants.StaticPrefabsEnum.FLOOR_GRASS];
        b.FootstepSoundType = (int)GlobalConstants.FootstepSoundType.GRASS;

        _map[x, y].Blocks.Add(b);
      }
    }

    FindStartingPos();

    GenerateTrees(20, 2);
  }

  int _maxEmptyLoops = 100;
  void GenerateTrees(int number, int minDistance)
  {
    int counter = 0;
    //while (counter != number)
    {
    }
  }

  void FindStartingPos()
  {
    _cameraPos.X = 1;
    _cameraPos.Y = 1;
    _cameraPos.Facing = 0;

    /*
    for (int x = 0; x < _mapHeight; x++)
    {
      for (int y = 0; y < _mapWidth; y++)
      {
        if (_gridRef.Map[x, y].CellType == CellType.FLOOR)
        {
          _serializableMap.CameraPos.X = x;
          _serializableMap.CameraPos.Y = y;
          return;
        }
      }
    }
    */
  }
}
