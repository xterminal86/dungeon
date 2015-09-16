﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rooms : GenerationAlgorithmBase
{
  Grid _gridRef;

  List<RoomBounds> _roomsBounds = new List<RoomBounds>();

  bool _noRoomsIntersection = false;
  public Rooms(bool noRoomsIntersection)
  {
    _noRoomsIntersection = noRoomsIntersection;
  }

  public override void Do(Grid grid)
  {
    _gridRef = grid;

    MakeRooms();
  }

  int _iterations = 0;
  void MakeRooms()
  {
    while (_iterations < _gridRef.MaxRooms)
    {
      Vector2 cellPos = _gridRef.GetRandomCellPos();
      int roomWidth = Random.Range(5, _gridRef.RoomMaxWidth + 1);
      int roomHeight = Random.Range(5, _gridRef.RoomMaxHeight + 1);

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

          bool res = IsRegionValid(cellPos, roomWidth, roomHeight);
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

    foreach (var room in _roomsBounds)
    {
      Debug.Log(room.FirstPoint + " " + room.SecondPoint);
    }

  }

  bool IsRegionValid(Vector2 cellPos, int roomWidth, int roomHeight)
  {
    for (int i = (int)cellPos.x; i < (int)cellPos.x + roomHeight; i++)
    {
      for (int j = (int)cellPos.y; j < (int)cellPos.y + roomWidth; j++)
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
        _gridRef.Map[i, j].CellType = CellType.WALL;
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
}
