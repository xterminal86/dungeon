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

    FillUnoccupiedCells();

    FindStartingPos();

    _musicTrack = "amb-forest";
  }

  int _iterations = 0;
  int _maxBuildings = 10;
  int _roomsDistance = 5;
  int _roomMinWidth = 5, _roomMinHeight = 5, _roomMaxWidth = 7, _roomMaxHeight = 7;
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
    if (_roadMarks.Count > 1)
    {
      RoadBuilder rb = new RoadBuilder(_map, _mapWidth, _mapHeight);

      SerializableBlock b = CreateBlock(_roadMarks[0].X, _roadMarks[0].Y, 0, GlobalConstants.StaticPrefabsEnum.FLOOR_COBBLESTONE_BRICKS);
      b.FootstepSoundType = (int)GlobalConstants.FootstepSoundType.STONE;

      _map[_roadMarks[0].X, _roadMarks[0].Y].Blocks.Add(b);
      _map[_roadMarks[0].X, _roadMarks[0].Y].CellType = GeneratedCellType.ROAD;          

      for (int i = 0; i < _roadMarks.Count - 1; i++)
      {
        var road = rb.BuildRoad(_roadMarks[i], _roadMarks[i + 1]);

        foreach (var item in road)
        { 
          if (!FindBlock(item.Coordinate, _map[item.Coordinate.X, item.Coordinate.Y].Blocks))          
          {
            b = CreateBlock(item.Coordinate.X, item.Coordinate.Y, 0, GlobalConstants.StaticPrefabsEnum.FLOOR_COBBLESTONE_BRICKS);
            b.FootstepSoundType = (int)GlobalConstants.FootstepSoundType.STONE;          

            _map[item.Coordinate.X, item.Coordinate.Y].Blocks.Add(b);
            _map[item.Coordinate.X, item.Coordinate.Y].CellType = GeneratedCellType.ROAD;          
          }          
        }
      }
    }    
  }

  bool FindBlock(Int2 coords, List<SerializableBlock> listToSearch)
  {
    foreach (var item in listToSearch)
    {
      if (item.X == coords.X && item.Y == coords.Y)
      {
        return true;
      }
    }

    return false;
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

        _map[x, y].CellType = GeneratedCellType.OBSTACLE;
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
    MakeWindows(cellPos, roomWidth, roomHeight);
    PlaceFloor(cellPos, roomWidth, roomHeight);
    PlaceRoof(cellPos, roomWidth, roomHeight);
  }

  int _windowDistance = 4;
  void MakeWindows(Vector2 cellPos, int roomWidth, int roomHeight)
  {
    int startX = (int)cellPos.x;
    int endX = (int)cellPos.x + roomHeight - 1;
    int startY = (int)cellPos.y;
    int endY = (int)cellPos.y + roomWidth - 1;
            
    FindWindowOnColumn(startX, endX, startY, _windowDistance);
    FindWindowOnColumn(startX, endX, endY, _windowDistance);
    FindWindowOnRow(startY, endY, startX, _windowDistance);
    FindWindowOnRow(startY, endY, endX, _windowDistance);    
  }

  void PlaceWalls(Vector2 cellPos, int roomWidth, int roomHeight)
  {
    int startX = (int)cellPos.x;
    int endX = (int)cellPos.x + roomHeight - 1;
    int startY = (int)cellPos.y;
    int endY = (int)cellPos.y + roomWidth - 1;

    SerializableBlock blockLeft, blockRight;

    // Left and right walls
    for (int i = startX; i <= endX; i++) 
    {
      for (int layer = 0; layer < 2; layer++)
      { 
        if (i == startX || i == endX)
        {
          blockLeft = CreateBlock(i, startY, layer, GlobalConstants.StaticPrefabsEnum.BLOCK_WOODEN_LOG);        
          blockRight = CreateBlock(i, endY, layer, GlobalConstants.StaticPrefabsEnum.BLOCK_WOODEN_LOG);
        }
        else
        {                
          blockLeft = CreateBlock(i, startY, layer, GlobalConstants.StaticPrefabsEnum.WALL_THIN_WOODEN, (int)GlobalConstants.Orientation.WEST);
          blockRight = CreateBlock(i, endY, layer, GlobalConstants.StaticPrefabsEnum.WALL_THIN_WOODEN, (int)GlobalConstants.Orientation.EAST);
        }

        _map[blockLeft.X, blockLeft.Y].Blocks.Add(blockLeft);
        _map[blockRight.X, blockRight.Y].Blocks.Add(blockRight);
      }
    }

    // Up and down walls
    for (int i = startY + 1; i <= endY - 1; i++) 
    {
      for (int layer = 0; layer < 2; layer++)
      { 
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
        block.PrefabName = GlobalConstants.StaticPrefabsNamesById[GlobalConstants.StaticPrefabsEnum.FLOOR_WOODEN_BLACK];
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

    /*
    // Roofs with overhang
    
    int startX = (int)cellPos.x - 1;
    int endX = (int)cellPos.x + roomHeight;
    int startY = (int)cellPos.y - 1;
    int endY = (int)cellPos.y + roomWidth;
    */

    int layer = 2;

    int stepsX = endX - startX + 1;
    int stepsY = endY - startY + 1;

    int counter = 0;
    while ((endX - startX >= 1) && (endY - startY) >= 1)
    {
      MakePerimeter(startX, endX, startY, endY, layer);

      startX++;
      startY++;
      endX--;
      endY--;

      layer++;

      counter++;
    }

    CloseRoofHoles(startX, startY, stepsX, stepsY, layer);
  }

  /// <summary>
  /// If room size is NxN and N is odd, then our roof will have one hole in the center.
  /// If room size is WxH and W > H (i.e. has more columns than rows) then it closes itself only
  /// if width is even.
  /// If W < H then room closes itself only if H is even.
  /// If width and height are both even, room closes itself, so we don't have to do anything.
  /// 
  /// Again, don't forget that room of size e.g. 7x6 has 7 rows and 6 columns.
  /// 
  /// </summary>
  void CloseRoofHoles(int startX, int startY, int stepsX, int stepsY, int layer)
  {
    if (stepsX > stepsY && stepsY % 2 != 0)
    {
      for (int i = 0; i < stepsX - stepsY + 1; i++)
      {
        SerializableBlock b = CreateBlock(startX + i, startY, layer, GlobalConstants.StaticPrefabsEnum.ROOF_COBBLESTONE_CLOSING, (int)GlobalConstants.Orientation.EAST);
        _map[startX + i, startY].Blocks.Add(b);
      }
    }
    else if (stepsX < stepsY && stepsX % 2 != 0)
    {
      for (int i = 0; i < stepsY - stepsX + 1; i++)
      {
        SerializableBlock b = CreateBlock(startX, startY + i, layer, GlobalConstants.StaticPrefabsEnum.ROOF_COBBLESTONE_CLOSING, (int)GlobalConstants.Orientation.SOUTH);
        _map[startX, startY + i].Blocks.Add(b);
      }
    }
    else if (stepsX == stepsY && stepsX % 2 != 0 && stepsY % 2 != 0)
    {
      SerializableBlock b = CreateBlock(startX, startY, layer, GlobalConstants.StaticPrefabsEnum.ROOF_COBBLESTONE_CLOSING, (int)GlobalConstants.Orientation.SOUTH);
      _map[startX, startY].Blocks.Add(b);
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
        
    for (int i = startX + 1; i < endX - 1; i++)
    {
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

    for (int i = startY + 1; i < endY - 1; i++)
    {
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
        RemoveBlockAtPosition(p.X, p.Y, layer);

        SerializableObject obj = CreateDoor(p.X, p.Y, layer, GlobalConstants.ObjectPrefabsEnum.DOOR_WOODEN, (int)item.Value, "door-openable");
        _map[p.X, p.Y].Objects.Add(obj);

        MarkRoadSpot(p, item.Value);

        break;
      }
    }
  }

  List<Int2> _roadMarks = new List<Int2>();
  void MarkRoadSpot(Int2 point, GlobalConstants.Orientation doorFacing)
  {
    Int2 marker = new Int2();
    if (doorFacing == GlobalConstants.Orientation.EAST)
    {
      marker.X = point.X;
      marker.Y = point.Y + 1;
    }
    else if (doorFacing == GlobalConstants.Orientation.SOUTH)
    {
      marker.X = point.X + 1;
      marker.Y = point.Y;
    }
    else if (doorFacing == GlobalConstants.Orientation.WEST)
    {
      marker.X = point.X;
      marker.Y = point.Y - 1;
    }
    else if (doorFacing == GlobalConstants.Orientation.NORTH)
    {
      marker.X = point.X - 1;
      marker.Y = point.Y;
    }

    _roadMarks.Add(marker);
  }

  void FindWindowOnColumn(int startX, int endX, int columnY, int windowDistance)
  {
    int middle = (_windowDistance - 1) / 2;

    int counter = 0;
    int facing = 0;

    Int2 windowPosition = new Int2();

    for (int i = startX + 1; i < endX - 1; i++)
    {
      counter = 0;

      for (int d = 0; d < _windowDistance; d++)
      {
        if (_map[i + d, columnY].Objects.Count != 0 || HasBlock(i + d, columnY, SerializableBlockType.WINDOW))
        {
          counter = 0;
          break;
        }

        if (counter == middle)
        {
          windowPosition.X = i + d;
          windowPosition.Y = columnY;          
        }

        counter++;
      }

      if (counter != 0)
      {
        SerializableBlock b = GetBlock(windowPosition.X, windowPosition.Y, 0);
        
        if (b != null)
        {          
          facing = b.Facing;
        }

        RemoveBlockAtPosition(windowPosition.X, windowPosition.Y, 0);

        SerializableBlock window = CreateBlock(windowPosition.X, windowPosition.Y, 0, GlobalConstants.StaticPrefabsEnum.WALL_THIN_WOODEN_WINDOW, facing);
        window.BlockType = SerializableBlockType.WINDOW;

        _map[windowPosition.X, windowPosition.Y].Blocks.Add(window);        
      }
    }
  }

  void FindWindowOnRow(int startY, int endY, int rowX, int windowDistance)
  {
    int middle = (_windowDistance - 1) / 2;

    int counter = 0;
    int facing = 0;

    Int2 windowPosition = new Int2();

    for (int i = startY + 1; i < endY - 1; i++)
    {
      counter = 0;

      for (int d = 0; d < _windowDistance; d++)
      {
        if (_map[rowX, i + d].Objects.Count != 0 || HasBlock(rowX, i + d, SerializableBlockType.WINDOW))
        {
          counter = 0;
          break;
        }

        if (counter == middle)
        {
          windowPosition.X = rowX;
          windowPosition.Y = i + d;
        }

        counter++;
      }

      if (counter != 0)
      {
        SerializableBlock b = GetBlock(windowPosition.X, windowPosition.Y, 0);

        if (b != null)
        {
          facing = b.Facing;
        }

        RemoveBlockAtPosition(windowPosition.X, windowPosition.Y, 0);

        SerializableBlock window = CreateBlock(windowPosition.X, windowPosition.Y, 0, GlobalConstants.StaticPrefabsEnum.WALL_THIN_WOODEN_WINDOW, facing);
        window.BlockType = SerializableBlockType.WINDOW;

        _map[windowPosition.X, windowPosition.Y].Blocks.Add(window);
      }
    }
  }

  void MakePerimeter(int startX, int endX, int startY, int endY, int layer)
  {
    //Debug.Log("Size: " + (endX - startX) + " " + (endY - startY) + " | " + startX + " " + endX + " " + startY + " " + endY + " " + layer);

    SerializableBlock roof1;
    SerializableBlock roof2;

    for (int i = startX; i <= endX; i++)
    {
      if (i == startX)
      {
        roof1 = CreateBlock(i, startY, layer, GlobalConstants.StaticPrefabsEnum.ROOF_COBBLESTONE_CORNER, (int)GlobalConstants.Orientation.NORTH);
        roof2 = CreateBlock(i, endY, layer, GlobalConstants.StaticPrefabsEnum.ROOF_COBBLESTONE_CORNER, (int)GlobalConstants.Orientation.EAST);
      }
      else if (i == endX)
      {
        roof1 = CreateBlock(i, startY, layer, GlobalConstants.StaticPrefabsEnum.ROOF_COBBLESTONE_CORNER, (int)GlobalConstants.Orientation.WEST);
        roof2 = CreateBlock(i, endY, layer, GlobalConstants.StaticPrefabsEnum.ROOF_COBBLESTONE_CORNER, (int)GlobalConstants.Orientation.SOUTH);
      }
      else
      {
        roof1 = CreateBlock(i, startY, layer, GlobalConstants.StaticPrefabsEnum.ROOF_COBBLESTONE_LINE, (int)GlobalConstants.Orientation.WEST);
        roof2 = CreateBlock(i, endY, layer, GlobalConstants.StaticPrefabsEnum.ROOF_COBBLESTONE_LINE, (int)GlobalConstants.Orientation.EAST);
      }

      _map[i, startY].Blocks.Add(roof1);
      _map[i, endY].Blocks.Add(roof2);
    }

    for (int i = startY + 1; i <= endY - 1; i++)
    {
      roof1 = CreateBlock(startX, i, layer, GlobalConstants.StaticPrefabsEnum.ROOF_COBBLESTONE_LINE, (int)GlobalConstants.Orientation.NORTH);
      roof2 = CreateBlock(endX, i, layer, GlobalConstants.StaticPrefabsEnum.ROOF_COBBLESTONE_LINE, (int)GlobalConstants.Orientation.SOUTH);

      _map[i, startY].Blocks.Add(roof1);
      _map[i, endY].Blocks.Add(roof2);
    }
  }

  SerializableBlock CreateBlock(int x, int y, int layer, GlobalConstants.StaticPrefabsEnum type, int facing = 0, bool flip = false)
  {
    SerializableBlock b = new SerializableBlock();

    b.X = x;
    b.Y = y;
    b.Layer = layer;
    b.Facing = facing;
    b.FlipFlag = flip;
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
    for (int x = 0; x < _mapHeight; x++)
    {
      for (int y = 0; y < _mapWidth; y++)
      {
        if (_map[x, y].CellType != GeneratedCellType.OBSTACLE)
        {
          _cameraPos.X = x;
          _cameraPos.Y = y;
          _cameraPos.Facing = (int)GlobalConstants.Orientation.SOUTH;
          return;
        }
      }
    }
  }
}
