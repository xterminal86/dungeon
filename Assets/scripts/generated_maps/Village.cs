using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Village : GeneratedMap
{
  List<RoomBounds> _roomsBounds = new List<RoomBounds>();

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
    GenerateBuildings();
    ConnectBuildings();
    //GenerateTrees(80, 3);
    //GenerateGrass();

    FindStartingPos();

    _musicTrack = "amb-forest";
  }

  int _iterations = 0;
  int _maxBuildings = 5;
  int _roomsDistance = 2;
  int _roomMinWidth = 5, _roomMinHeight = 5, _roomMaxWidth = 11, _roomMaxHeight = 11;
  void GenerateBuildings()
  {
    while (_iterations < _maxBuildings)
    {
      Vector2 cellPos = GetRandomCellPos();
      int roomWidth = Random.Range(_roomMinWidth, _roomMaxWidth + 1);
      int roomHeight = Random.Range(_roomMinHeight, _roomMaxHeight + 1);
      
      // Again, remember that map size of 10x5 means x:10, y:5 -> 10 columns, 5 rows in array form.
      // So we need to use height in x calculations and width in y.
      
      cellPos = CheckRoomBounds(cellPos, roomWidth, roomHeight);
      
      //Debug.Log(cellPos + " " + roomWidth + " " + roomHeight);
      
      int exit = 0;
      while (true)
      {
        if (exit > _maxIdleIterations)
        {
          break;
        }
        
        bool res = IsRegionOccupied(cellPos, roomWidth, roomHeight, _roomsDistance);
        if (res)
        {
          cellPos = GetRandomCellPos();
          cellPos = CheckRoomBounds(cellPos, roomWidth, roomHeight);
        }
        else
        {
          MakeRoom(cellPos, roomWidth, roomHeight);
          Int2 p1 = new Int2(cellPos.x, cellPos.y);
          
          // For loop in MakeRoom() does not include (x + roomHeight), so we subtract 1 to get second boundary point
          Int2 p2 = new Int2((int)cellPos.x + roomHeight - 1, (int)cellPos.y + roomWidth - 1);
          
          _roomsBounds.Add(new RoomBounds(p1, p2));
          
          break;
        }
        
        exit++;
      }
      
      _iterations++;
      
    } // end while
  }

  void ConnectBuildings()
  {
  }

  void GenerateGrass()
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

  // ********************* HELPER FUNCTIONS ********************* //

  Vector2 CheckRoomBounds(Vector2 cellPos, int roomWidth, int roomHeight)
  {
    int boundaryX = ((int)cellPos.x + roomHeight);
    int boundaryY = ((int)cellPos.y + roomWidth);

    //Debug.Log("CheckRoomBounds: " + cellPos + " " + boundaryX + " " + boundaryY);

    if (boundaryX > _mapHeight - 1)
    {
      cellPos.x -= roomHeight;
    }
    
    if (boundaryY > _mapWidth - 1)
    {
      cellPos.y -= roomWidth;
    }

    if (cellPos.x < 0) cellPos.x = 1;
    if (cellPos.y < 0) cellPos.y = 1;

    return new Vector2(cellPos.x, cellPos.y);
  }
  
  bool IsRegionOccupied(Vector2 cellPos, int roomWidth, int roomHeight, int roomsDistance)
  {
    int minX = ((int)cellPos.x - roomsDistance) < 0 ? 0 : (int)cellPos.x - roomsDistance;
    int minY = ((int)cellPos.y - roomsDistance) < 0 ? 0 : (int)cellPos.y - roomsDistance;
    int maxX = ((int)cellPos.x + roomHeight + roomsDistance) > _mapHeight ? _mapHeight : (int)cellPos.x + roomHeight + roomsDistance;
    int maxY = ((int)cellPos.y + roomWidth + roomsDistance) > _mapWidth ? _mapWidth : (int)cellPos.y + roomWidth + roomsDistance;
    
    for (int i = minX; i < maxX; i++)
    {
      for (int j = minY; j < maxY; j++)
      {
        if (_map[i, j].CellType == GeneratedCellType.ROOM)
        {
          return true;
        }
      }
    }
    
    return false;
  }

  void MakeRoom(Vector2 cellPos, int roomWidth, int roomHeight)
  {
    for (int i = (int)cellPos.x; i < (int)cellPos.x + roomHeight; i++)
    {
      for (int j = (int)cellPos.y; j < (int)cellPos.y + roomWidth; j++)
      {
        _map[i, j].CellType = GeneratedCellType.ROOM;
      }
    }

    PlaceWalls(cellPos, roomWidth, roomHeight);
    PlaceFloor(cellPos, roomWidth, roomHeight);
  }

  void PlaceWalls(Vector2 cellPos, int roomWidth, int roomHeight)
  {
    // Left and right walls
    for (int i = (int)cellPos.x; i < (int)cellPos.x + roomHeight; i++) 
    {
      SerializableBlock blockLeft = new SerializableBlock();
      blockLeft.X = i;
      blockLeft.Y = (int)cellPos.y;
      blockLeft.Layer = 0;
      blockLeft.PrefabName = GlobalConstants.StaticPrefabsNamesById[GlobalConstants.StaticPrefabsEnum.BLOCK_BRICKS_RED];

      SerializableBlock blockRight = new SerializableBlock();
      blockRight.X = i;
      blockRight.Y = (int)cellPos.y + roomWidth - 1;
      blockRight.Layer = 0;
      blockRight.PrefabName = GlobalConstants.StaticPrefabsNamesById[GlobalConstants.StaticPrefabsEnum.BLOCK_BRICKS_RED];

      _map[blockLeft.X, blockLeft.Y].Blocks.Add(blockLeft);
      _map[blockRight.X, blockRight.Y].Blocks.Add(blockRight);
    }

    // Up and down walls
    for (int i = (int)cellPos.y + 1; i < (int)cellPos.y + roomWidth - 1; i++) 
    {
      SerializableBlock blockUp = new SerializableBlock();
      blockUp.X = (int)cellPos.x;
      blockUp.Y = i;
      blockUp.Layer = 0;
      blockUp.PrefabName = GlobalConstants.StaticPrefabsNamesById[GlobalConstants.StaticPrefabsEnum.BLOCK_BRICKS_RED];

      SerializableBlock blockDown = new SerializableBlock();
      blockDown.X = (int)cellPos.x + roomHeight - 1;
      blockDown.Y = i;
      blockDown.Layer = 0;
      blockDown.PrefabName = GlobalConstants.StaticPrefabsNamesById[GlobalConstants.StaticPrefabsEnum.BLOCK_BRICKS_RED];

      _map[blockUp.X, blockUp.Y].Blocks.Add(blockUp);
      _map[blockDown.X, blockDown.Y].Blocks.Add(blockDown);
    }
  }

  void PlaceFloor(Vector2 cellPos, int roomWidth, int roomHeight)
  {
    for (int i = (int)cellPos.x + 1; i < (int)cellPos.x + roomHeight - 1; i++)
    {
      for (int j = (int)cellPos.y + 1; j < (int)cellPos.y + roomWidth - 1; j++)
      {
        SerializableBlock block = new SerializableBlock();
        block.X = i;
        block.Y = j;
        block.Layer = 0;
        block.PrefabName = GlobalConstants.StaticPrefabsNamesById[GlobalConstants.StaticPrefabsEnum.FLOOR_WOODEN];
        block.FootstepSoundType = (int)GlobalConstants.FootstepSoundType.WOOD;

        _map[i, j].Blocks.Add(block);
      }
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
