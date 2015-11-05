﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rooms : GenerationAlgorithmBase
{
  Grid _gridRef;

  List<RoomBounds> _roomsBounds = new List<RoomBounds>();
  List<Int2> _roomsCentralPoints = new List<Int2>();

  bool _noRoomsIntersection = false;
  bool _connectRooms = false;
  int _roomMaxWidth = -1;
  int _roomMaxHeight = -1;
  int _maxRooms = -1;
  int _roomsDistance = -1;

  GrowingTree _maze;

  public Rooms(int roomMaxWidth, int roomMaxHeight, int maxRooms, 
               int roomsDistance, bool noRoomsIntersection, bool connectRooms,
               bool removeDeadEnds, int deadEndsToRemove)
  {
    _roomMaxWidth = roomMaxWidth;
    _roomMaxHeight = roomMaxHeight;
    _maxRooms = maxRooms;
    _roomsDistance = roomsDistance;
    _noRoomsIntersection = noRoomsIntersection;
    _connectRooms = connectRooms;

    _maze = new GrowingTree(DecisionType.NEWEST, removeDeadEnds, deadEndsToRemove);
  }

  public override void Do(Grid grid)
  {
    _gridRef = grid;

    MakeRooms();

    if (_connectRooms)
    {
      //SortRooms();
      //CarvePassages();
      GenerateMaze();
    }
  }

  int _iterations = 0;
  void MakeRooms()
  {
    while (_iterations < _maxRooms)
    {
      Vector2 cellPos = _gridRef.GetRandomCellPos();
      int roomWidth = Random.Range(5, _roomMaxWidth + 1);
      int roomHeight = Random.Range(5, _roomMaxHeight + 1);

      // Again, remember that map size of 10x5 means x:10, y:5 -> 10 columns, 5 rows in array form.
      // So we need to use height in x calculations and width in y.

      cellPos = CheckRoomBounds(cellPos, roomWidth, roomHeight);

      //Debug.Log(cellPos + " " + roomWidth + " " + roomHeight);

      if (_noRoomsIntersection)
      {
        int exit = 0;
        while (true)
        {
          if (exit > _gridRef.MapSize)
          {
            break;
          }

          bool res = IsRegionOccupied(cellPos, roomWidth, roomHeight, _roomsDistance);
          if (res)
          {
            cellPos = _gridRef.GetRandomCellPos();
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
      }
      else
      {
        MakeRoom(cellPos, roomWidth, roomHeight);
        Int2 p1 = new Int2(cellPos.x, cellPos.y);
        Int2 p2 = new Int2((int)cellPos.x + roomHeight - 1, (int)cellPos.y + roomWidth - 1);
        _roomsBounds.Add(new RoomBounds(p1, p2));
      }

      _iterations++;

    } // end while

    int count = 1;
    foreach (var room in _roomsBounds)
    {
      int x = room.FirstPoint.X + ((room.SecondPoint.X - room.FirstPoint.X) / 2);
      int y = room.FirstPoint.Y + ((room.SecondPoint.Y - room.FirstPoint.Y) / 2);

      Int2 cp = new Int2(x, y);

      _roomsCentralPoints.Add(cp);

      Debug.Log(count + ") " + room.FirstPoint + " " + room.SecondPoint + " center: " + cp);
      count++;
    }
  }

  List<Int2> _sortedRoomCenters = new List<Int2>();
  void SortRooms()
  {
    for (int x = 0; x < _gridRef.MapHeight; x++)
    {
      for (int y = 0; y < _gridRef.MapWidth; y++)
      {
        bool res = FindCenterPointInList(x, y);
        if (res)
        {
          _sortedRoomCenters.Add(new Int2(x, y));
        }
      }
    }

    Debug.Log("Sorted rooms:");

    int count = 1;
    foreach (var p in _sortedRoomCenters)
    {
      Debug.Log(count + ") " + p);
      count++;
    }
  }

  bool FindCenterPointInList(int x, int y)
  {
    foreach (var p in _roomsCentralPoints)
    {
      if (p.X == x && p.Y == y)
      {
        return true;
      }      
    }

    return false;
  }

  bool IsRegionOccupied(Vector2 cellPos, int roomWidth, int roomHeight, int roomsDistance)
  {
    int minX = ((int)cellPos.x - roomsDistance) < 0 ? 0 : (int)cellPos.x - roomsDistance;
    int minY = ((int)cellPos.y - roomsDistance) < 0 ? 0 : (int)cellPos.y - roomsDistance;
    int maxX = ((int)cellPos.x + roomHeight + roomsDistance) > _gridRef.MapHeight ? _gridRef.MapHeight : (int)cellPos.x + roomHeight + roomsDistance;
    int maxY = ((int)cellPos.y + roomWidth + roomsDistance) > _gridRef.MapWidth ? _gridRef.MapWidth : (int)cellPos.y + roomWidth + roomsDistance;
    
    for (int i = minX; i < maxX; i++)
    {
      for (int j = minY; j < maxY; j++)
      {
        // Room can "eat" adjacent wall, if it gets into current room floor
        //if (_gridRef.Map[i, j].CellType == CellType.FLOOR)

        if (_gridRef.Map[i, j].CellType == CellType.FLOOR || _gridRef.Map[i, j].CellType == CellType.WALL)
        {
          return true;
        }
      }
    }

    return false;
  }

  Vector2 CheckRoomBounds(Vector2 cellPos, int roomWidth, int roomHeight)
  {
    int boundaryX = ((int)cellPos.x + roomHeight);
    int boundaryY = ((int)cellPos.y + roomWidth);
    
    if (boundaryX > _gridRef.MapHeight - 1)
    {
      cellPos.x -= roomHeight;
    }
    
    if (boundaryY > _gridRef.MapWidth - 1)
    {
      cellPos.y -= roomWidth;
    }

    return new Vector2(cellPos.x, cellPos.y);
  }

  void MakeRoom(Vector2 cellPos, int roomWidth, int roomHeight)
  {
    // Wall everything
    for (int i = (int)cellPos.x; i < (int)cellPos.x + roomHeight; i++)
    {
      for (int j = (int)cellPos.y; j < (int)cellPos.y + roomWidth; j++)
      {
        if (_gridRef.Map[i, j].CellType != CellType.FLOOR)
        {
          _gridRef.Map[i, j].CellType = CellType.WALL;
          _gridRef.Map[i, j].Status = CellStatus.VISITED;
        }
      }
    }

    // Carve room inside
    for (int i = (int)cellPos.x + 1; i < (int)cellPos.x + roomHeight - 1; i++)
    {
      for (int j = (int)cellPos.y + 1; j < (int)cellPos.y + roomWidth - 1; j++)
      {
        _gridRef.Map[i, j].CellType = CellType.FLOOR;
      }
    }
  }

  void CarvePassages()
  {
    if (_sortedRoomCenters.Count == 1) return;

    for (int i = 0; i < _sortedRoomCenters.Count - 1; i++)
    {
      KeyValuePair<Int2, Int2> pair = new KeyValuePair<Int2, Int2>(_sortedRoomCenters[i], _sortedRoomCenters[i + 1]);
      CarvePassage(pair);
    }

    /*
    if (_roomsBounds.Count == 1) return;

    for (int i = 0; i < _roomsBounds.Count - 1; i++)
    {
      var centralPoints = GetCentralPoints(_roomsBounds[i], _roomsBounds[i + 1]);
      CarvePassage(centralPoints);
    }
    */
  }

  void GenerateMaze()
  {
    for (int x = 0; x < _gridRef.MapHeight; x++)
    {
      for (int y = 0; y < _gridRef.MapWidth; y++)
      {
        if (_gridRef.Map[x, y].Status != CellStatus.VISITED)
        {
          _gridRef.Map[x, y].CellType = CellType.WALL;
        }
      }
    }

    _maze.Do(_gridRef);
  }

  KeyValuePair<Int2, Int2> _centralPoints;
  KeyValuePair<Int2, Int2> GetCentralPoints(RoomBounds r1, RoomBounds r2)
  {
    int cx1 = r1.FirstPoint.X + ((r1.SecondPoint.X - r1.FirstPoint.X) / 2); 
    int cy1 = r1.FirstPoint.Y + ((r1.SecondPoint.Y - r1.FirstPoint.Y) / 2);
    
    int cx2 = r2.FirstPoint.X + ((r2.SecondPoint.X - r2.FirstPoint.X) / 2); 
    int cy2 = r2.FirstPoint.Y + ((r2.SecondPoint.Y - r2.FirstPoint.Y) / 2);
    
    Int2 c1 = new Int2(cx1, cy1);
    Int2 c2 = new Int2(cx2, cy2);

    //Debug.Log("Room1 " + r1 + " approx. center " + c1);
    //Debug.Log("Room2 " + r2 + " approx. center " + c2);

    _centralPoints = new KeyValuePair<Int2, Int2>(c1, c2);

    return _centralPoints;
  }

  void CarvePassage(KeyValuePair<Int2, Int2> centralPoints)
  {
    Debug.Log("Carving manhattan passage from " + centralPoints.Key + " to " + centralPoints.Value); 

    Int2 p1 = centralPoints.Key;
    Int2 p2 = centralPoints.Value;

    if (Mathf.Abs(p1.Y - p2.Y) >= Mathf.Abs(p1.X - p2.X))
    {
      int fromY = (p1.Y < p2.Y) ? p1.Y : p2.Y;
      int toY = (p1.Y < p2.Y) ? p2.Y : p1.Y;

      for (int i = fromY; i <= toY; i++)
      {
        _gridRef.Map[p1.X, i].CellType = CellType.FLOOR;
                
        WallPassage(p1.X - 1, p1.X + 1, i - 1, i + 1);
      }

      int fromX = (p1.X < p2.X) ? p1.X : p2.X;
      int toX = (p1.X < p2.X) ? p2.X : p1.X;

      for (int i = fromX; i <= toX; i++)
      {
        _gridRef.Map[i, p2.Y].CellType = CellType.FLOOR;

        WallPassage(i - 1, i + 1, p2.Y - 1, p2.Y + 1);        
      }
    }
    else
    {
      int fromX = (p1.X < p2.X) ? p1.X : p2.X;
      int toX = (p1.X < p2.X) ? p2.X : p1.X;
      
      for (int i = fromX; i <= toX; i++)
      {
        _gridRef.Map[i, p1.Y].CellType = CellType.FLOOR;

        WallPassage(i - 1, i + 1, p1.Y - 1, p1.Y + 1);
      }
      
      int fromY = (p1.Y < p2.Y) ? p1.Y : p2.Y;
      int toY = (p1.Y < p2.Y) ? p2.Y : p1.Y;
      
      for (int i = fromY; i <= toY; i++)
      {
        _gridRef.Map[p2.X, i].CellType = CellType.FLOOR;

        WallPassage(p2.X - 1, p2.X + 1, i - 1, i + 1);
      }
    }
  }

  void WallPassage(int minX, int maxX, int minY, int maxY)
  {
    for (int i = minX; i <= maxX; i++)
    {
      for (int j = minY; j <= maxY; j++)
      {
        if (i == (minX + 1) && j == (minY + 1)) continue;

        if (_gridRef.Map[i, j].CellType == CellType.EMPTY)
        {
          _gridRef.Map[i, j].CellType = CellType.WALL;
        }
      }
    }
  }
}
