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

    GenerateTrees(80, 3);
  }

  void GenerateTrees(int number, int minDistance)
  {
    int counter = 0, idleCounter = 0;
    int x = 0, y = 0;
    while (counter != number)
    {
      if (idleCounter == _maxIdleIterations)
      {
        Debug.LogWarning("Terminated by safeguard: trees created " + counter + " out of " + number);
        break;
      }

      x = Random.Range(0, _mapHeight - 1);
      y = Random.Range(0, _mapWidth - 1);

      bool res = IsValid(x, y, minDistance);
      if (res)
      {
        SerializableBlock b = new SerializableBlock();
        b.X = x;
        b.Y = y;
        b.Layer = 0;
        b.PrefabName = GlobalConstants.StaticPrefabsNamesById[GlobalConstants.StaticPrefabsEnum.TREE_BIRCH];

        _map[x, y].Blocks.Add(b);

        counter++;
      }
      else
      {
        idleCounter++;
      }
    }

    Debug.Log("Successfully created " + counter + " trees");
  }

  bool IsValid(int x, int y, int minDistance)
  {
    if (!_occupiedCells[x, y])
    {
      int xStart = Mathf.Clamp(x - minDistance, 0, _mapHeight - 1);
      int xEnd = Mathf.Clamp(x + minDistance, 0, _mapHeight - 1);
      int yStart = Mathf.Clamp(y - minDistance, 0, _mapWidth - 1);
      int yEnd = Mathf.Clamp(y + minDistance, 0, _mapWidth - 1);

      for (int i = xStart; i <= xEnd; i++)
      {
        for (int j = yStart; j <= yEnd; j++)
        {
          if (i == x && j == y)
          {
            continue;
          }

          if (_occupiedCells[i, j])
          {
            return false;
          }
        }
      }
            
      _occupiedCells[x, y] = true;
      return true;
    }
    
    return false;
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
