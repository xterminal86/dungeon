using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Village : GeneratedMap
{
  List<RoomBounds> _roomsBounds = new List<RoomBounds>();

  public Village(int width, int height) : base(width, height)
  {    
  }
    
  public override void Generate()
  {
    GenerateBuildings();
    ConnectBuildings();
    GenerateTrees(80, 2);
    GenerateGrass();

    FindStartingPos();

    _musicTrack = "amb-forest";
  }

  int _iterations = 0;
  int _maxBuildings = 5;
  int _roomsDistance = 5;
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
        if (_map[x, y].CellType == GeneratedCellType.NONE || _map[x, y].CellType == GeneratedCellType.OBSTACLE)
        {
          SerializableBlock b = CreateBlock(x, y, 0, GlobalConstants.StaticPrefabsEnum.FLOOR_GRASS);
          b.FootstepSoundType = (int)GlobalConstants.FootstepSoundType.GRASS;

          _map[x, y].Blocks.Add(b);
        }
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
        SerializableBlock b = CreateBlock(x, y, 0, GlobalConstants.StaticPrefabsEnum.TREE_BIRCH);
        
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
    if (_map[x, y].CellType == GeneratedCellType.NONE)
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

          if (_map[i, j].CellType != GeneratedCellType.NONE)
          {
            return false;
          }
        }
      }

      _map[x, y].CellType = GeneratedCellType.OBSTACLE;

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
    MakeDoorway(cellPos, roomWidth, roomHeight);
    PlaceFloor(cellPos, roomWidth, roomHeight);
    PlaceRoof(cellPos, roomWidth, roomHeight);    
  }

  void PlaceWalls(Vector2 cellPos, int roomWidth, int roomHeight)
  {
    int startX = (int)cellPos.x;
    int endX = (int)cellPos.x + roomHeight - 1;
    int startY = (int)cellPos.y;
    int endY = (int)cellPos.y + roomWidth - 1;

    // Left and right walls
    for (int i = (int)cellPos.x; i < (int)cellPos.x + roomHeight; i++) 
    {
      for (int layer = 0; layer < 2; layer++)
      {        
        //SerializableBlock blockLeft = CreateBlock(i, (int)cellPos.y, layer, GlobalConstants.StaticPrefabsEnum.BLOCK_BRICKS_RED);        
        //SerializableBlock blockRight = CreateBlock(i, (int)cellPos.y + roomWidth - 1, layer, GlobalConstants.StaticPrefabsEnum.BLOCK_BRICKS_RED);
                
        SerializableBlock blockLeft = CreateBlock(i, startY, layer, GlobalConstants.StaticPrefabsEnum.WALL_THIN_WOODEN, (int)GlobalConstants.Orientation.WEST);
        SerializableBlock blockRight = CreateBlock(i, endY, layer, GlobalConstants.StaticPrefabsEnum.WALL_THIN_WOODEN, (int)GlobalConstants.Orientation.EAST);

        _map[blockLeft.X, blockLeft.Y].Blocks.Add(blockLeft);
        _map[blockRight.X, blockRight.Y].Blocks.Add(blockRight);
      }
    }

    // Up and down walls
    for (int i = (int)cellPos.y; i < (int)cellPos.y + roomWidth; i++) 
    {
      for (int layer = 0; layer < 2; layer++)
      {        
        //SerializableBlock blockUp = CreateBlock((int)cellPos.x, i, layer, GlobalConstants.StaticPrefabsEnum.BLOCK_BRICKS_RED);
        //SerializableBlock blockDown = CreateBlock((int)cellPos.x + roomHeight - 1, i, layer, GlobalConstants.StaticPrefabsEnum.BLOCK_BRICKS_RED);

        SerializableBlock blockUp = CreateBlock(startX, i, layer, GlobalConstants.StaticPrefabsEnum.WALL_THIN_WOODEN, (int)GlobalConstants.Orientation.NORTH);
        SerializableBlock blockDown = CreateBlock(endX, i, layer, GlobalConstants.StaticPrefabsEnum.WALL_THIN_WOODEN, (int)GlobalConstants.Orientation.SOUTH);

        _map[blockUp.X, blockUp.Y].Blocks.Add(blockUp);
        _map[blockDown.X, blockDown.Y].Blocks.Add(blockDown);
      }
    }
  }

  void PlaceFloor(Vector2 cellPos, int roomWidth, int roomHeight)
  {
    for (int i = (int)cellPos.x; i < (int)cellPos.x + roomHeight; i++)
    {
      for (int j = (int)cellPos.y; j < (int)cellPos.y + roomWidth; j++)
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

  void PlaceRoof(Vector2 cellPos, int roomWidth, int roomHeight)
  {
    int startX = (int)cellPos.x;
    int endX = (int)cellPos.x + roomHeight - 1;
    int startY = (int)cellPos.y;
    int endY = (int)cellPos.y + roomWidth - 1;
    int layer = 2;

    /*
    
    // Roofs with overhang
    
    int startX = (int)cellPos.x - 1;
    int endX = (int)cellPos.x + roomHeight;
    int startY = (int)cellPos.y - 1;
    int endY = (int)cellPos.y + roomWidth;
    
    */

    while ((endX - startX >= 1) && (endY - startY) >= 1)
    {
      MakePerimeter(startX, endX, startY, endY, layer);

      startX++;
      startY++;
      endX--;
      endY--;

      layer++;
    }
  }

  Dictionary<Int2, GlobalConstants.Orientation> _doorwayCandidates = new Dictionary<Int2, GlobalConstants.Orientation>();
  List<Int2> _doorwayPositions = new List<Int2>();
  void MakeDoorway(Vector2 cellPos, int roomWidth, int roomHeight)
  {
    _doorwayCandidates.Clear();
    _doorwayPositions.Clear();

    int startX = (int)cellPos.x;
    int endX = (int)cellPos.x + roomHeight - 1;
    int startY = (int)cellPos.y;
    int endY = (int)cellPos.y + roomWidth - 1;
    int layer = 0;
        
    for (int i = startX; i < endX; i++)
    {
      if (i == startX || i == endX)
      {
        continue;
      }

      Int2 pos1 = new Int2();

      pos1.X = i;
      pos1.Y = startY;

      _doorwayCandidates.Add(pos1, GlobalConstants.Orientation.WEST);
      _doorwayPositions.Add(pos1);
      
      Int2 pos2 = new Int2();

      pos2.X = i;
      pos2.Y = endY;

      _doorwayCandidates.Add(pos2, GlobalConstants.Orientation.EAST);
      _doorwayPositions.Add(pos2);
    }

    for (int i = startY; i < endY; i++)
    {
      if (i == startY || i == endY)
      {
        continue;
      }

      Int2 pos1 = new Int2();

      pos1.X = startX;
      pos1.Y = i;

      _doorwayCandidates.Add(pos1, GlobalConstants.Orientation.NORTH);
      _doorwayPositions.Add(pos1);

      Int2 pos2 = new Int2();

      pos2.X = endX;
      pos2.Y = i;

      _doorwayCandidates.Add(pos2, GlobalConstants.Orientation.SOUTH);
      _doorwayPositions.Add(pos2);
    }

    int index = Random.Range(0, _doorwayPositions.Count);
    Int2 p = _doorwayPositions[index];

    foreach (var item in _doorwayCandidates)
    {
      if (item.Key.X == p.X && item.Key.Y == p.Y)
      {
        for (int i = 0; i < _map[p.X, p.Y].Blocks.Count; i++)
        {
          if (_map[p.X, p.Y].Blocks[i].Layer == layer)
          {
            _map[p.X, p.Y].Blocks.RemoveAt(i);
            SerializableObject obj = CreateDoor(p.X, p.Y, layer, GlobalConstants.ObjectPrefabsEnum.DOOR_WOODEN, (int)item.Value, "door-openable");
            _map[p.X, p.Y].Objects.Add(obj);
            return;
          }
        }
      }
    }
  }

  void MakePerimeter(int startX, int endX, int startY, int endY, int layer)
  {
    SerializableBlock roof1;
    SerializableBlock roof2;

    for (int i = startX; i <= endX; i++)
    {
      if (i == startX)
      {
        roof1 = CreateBlock(i, startY, layer, GlobalConstants.StaticPrefabsEnum.ROOF_WOODEN_CORNER, (int)GlobalConstants.Orientation.NORTH);
        roof2 = CreateBlock(i, endY, layer, GlobalConstants.StaticPrefabsEnum.ROOF_WOODEN_CORNER, (int)GlobalConstants.Orientation.EAST);
      }
      else if (i == endX)
      {
        roof1 = CreateBlock(i, startY, layer, GlobalConstants.StaticPrefabsEnum.ROOF_WOODEN_CORNER, (int)GlobalConstants.Orientation.WEST);
        roof2 = CreateBlock(i, endY, layer, GlobalConstants.StaticPrefabsEnum.ROOF_WOODEN_CORNER, (int)GlobalConstants.Orientation.SOUTH);
      }
      else
      {
        roof1 = CreateBlock(i, startY, layer, GlobalConstants.StaticPrefabsEnum.ROOF_WOODEN_LINE, (int)GlobalConstants.Orientation.WEST);
        roof2 = CreateBlock(i, endY, layer, GlobalConstants.StaticPrefabsEnum.ROOF_WOODEN_LINE, (int)GlobalConstants.Orientation.EAST);
      }

      _map[i, startY].Blocks.Add(roof1);
      _map[i, endY].Blocks.Add(roof2);
    }

    for (int i = startY + 1; i <= endY - 1; i++)
    {
      roof1 = CreateBlock(startX, i, layer, GlobalConstants.StaticPrefabsEnum.ROOF_WOODEN_LINE, (int)GlobalConstants.Orientation.NORTH);
      roof2 = CreateBlock(endX, i, layer, GlobalConstants.StaticPrefabsEnum.ROOF_WOODEN_LINE, (int)GlobalConstants.Orientation.SOUTH);

      _map[i, startY].Blocks.Add(roof1);
      _map[i, endY].Blocks.Add(roof2);
    }
  }

  SerializableBlock CreateBlock(int x, int y, int layer, GlobalConstants.StaticPrefabsEnum type, int facing = 0)
  {
    SerializableBlock b = new SerializableBlock();

    b.X = x;
    b.Y = y;
    b.Layer = layer;
    b.Facing = facing;
    b.PrefabName = GlobalConstants.StaticPrefabsNamesById[type];

    return b;
  }
  
  SerializableObject CreateObject(int x, int y, int layer, GlobalConstants.ObjectPrefabsEnum type, int facing, string objectClassName)
  {
    SerializableObject obj = new SerializableObject();

    obj.X = x;
    obj.Y = y;
    obj.Layer = layer;
    obj.Facing = facing;
    obj.PrefabName = GlobalConstants.ObjectPrefabsNamesById[type];
    obj.ObjectClassName = objectClassName;
    
    return obj;
  }

  SerializableObject CreateDoor(int x, int y, int layer, GlobalConstants.ObjectPrefabsEnum type, int facing, string objectClassName)
  {
    SerializableObject obj = new SerializableObject();

    obj.X = x;
    obj.Y = y;
    obj.Layer = layer;
    obj.Facing = facing;
    obj.PrefabName = GlobalConstants.ObjectPrefabsNamesById[type];
    obj.ObjectClassName = objectClassName;
    obj.DoorSoundType = "openable";
    obj.AnimationOpenSpeed = 2.0f;
    obj.AnimationCloseSpeed = 3.0f;

    return obj;
  }

  void FindStartingPos()
  {
    _cameraPos.X = 0;
    _cameraPos.Y = 0;
    _cameraPos.Facing = 2;

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
