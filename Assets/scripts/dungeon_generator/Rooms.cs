using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rooms : GenerationAlgorithmBase
{
  Grid _gridRef;

  List<RoomBounds> _roomsBounds = new List<RoomBounds>();
  List<Int2> _roomsCentralPoints = new List<Int2>();

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

    SortRooms();
    CarvePassages();
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

  /*
  KeyValuePair<Int2, Int2> _doorways;
  KeyValuePair<Int2, Int2> GetDoorways(RoomBounds room1, RoomBounds room2)
  {
    int x1, y1, x2, y2 = 0;
    Int2 d1 = new Int2();
    Int2 d2 = new Int2();

    // First room is below second
    if (room1.FirstPoint.X > room2.SecondPoint.X) 
    {
      // Both rooms are approximately on the same vertical line
      if (room1.FirstPoint.Y >= room2.FirstPoint.Y && room1.FirstPoint.Y <= room2.SecondPoint.Y ||
          room1.SecondPoint.Y >= room2.FirstPoint.Y && room1.SecondPoint.Y <= room2.SecondPoint.Y)
      {
        // Get random point between left and right sides (exclusive) on upper side
        y1 = Random.Range(room1.FirstPoint.Y + 1, room1.SecondPoint.Y);
        x1 = room1.FirstPoint.X;

        // Same thing for the room above
        y2 = Random.Range(room2.FirstPoint.Y + 1, room2.SecondPoint.Y);
        x2 = room2.SecondPoint.X;

        d1.X = x1;
        d1.Y = y1;

        d2.X = x2;
        d2.Y = y2;
      }
      // if second room if to the left
      else if (room1.FirstPoint.Y >= room2.SecondPoint.Y)
      {
        // Randomly choose at which side we create a doorway
        int choice = Random.Range(0, 2);
        // Left side
        if (choice == 0)
        {
          y1 = room1.FirstPoint.Y;
          x1 = Random.Range(room1.FirstPoint.X + 1, room1.SecondPoint.X);
        }
        // Right side
        else
        {
          y1 = Random.Range(room1.FirstPoint.Y + 1, room1.SecondPoint.Y);
          x1 = room1.FirstPoint.X;
        }

        // Do the same thing for the second room
        choice = Random.Range(0, 2);
        // Bottom side
        if (choice == 0)
        {
          y2 = room2.SecondPoint.Y;
          x2 = Random.Range(room2.FirstPoint.X + 1, room2.SecondPoint.X);
        }
        // Right side
        else
        {
          y2 = Random.Range(room2.FirstPoint.Y + 1, room2.SecondPoint.Y);
          x2 = room2.SecondPoint.X;
        }

        d1.X = x1;
        d1.Y = y1;

        d2.X = x2;
        d2.Y = y2;
      }
      // if second room is to the right
      else if (room1.SecondPoint.Y <= room2.FirstPoint.Y)
      {
      }
    }

    _doorways = new KeyValuePair<Int2, Int2>(d1, d2);

    return _doorways;
  }

  void CarvePassage(KeyValuePair<Int2, Int2> doorways)
  {
    Debug.Log("Carving passage from " + doorways.Key + " to " + doorways.Value);
  }
  */
}
