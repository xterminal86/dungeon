using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rooms : GenerationAlgorithmBase
{
  Grid _gridRef;

  List<RoomBounds> _roomsBounds = new List<RoomBounds>();

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
      GenerateMaze();
      MakeDoorways();
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

  List<Int2> _doorwayCandidates = new List<Int2>();
  void MakeDoorways()
  {
    for (int i = 0; i < _roomsBounds.Count; i++)
    {
      _doorwayCandidates.Clear();

      for (int y = _roomsBounds[i].FirstPoint.Y + 1; y <= _roomsBounds[i].SecondPoint.Y - 1; y++)
      {
        int x1 = _roomsBounds[i].FirstPoint.X + 1;
        int x2 = _roomsBounds[i].SecondPoint.X - 1;

        int yUp = (y - 2) < 0 ? 0 : y - 2;
        int yDown = (y + 2) > _gridRef.MapHeight - 1 ? _gridRef.MapHeight - 1 : y + 2;

        CellStatus up = _gridRef.Map[x1, yUp].Status;
        CellStatus down = _gridRef.Map[x2, yDown].Status;
        CellType upType = _gridRef.Map[x1, yUp].CellType;
        CellType downType = _gridRef.Map[x2, yDown].CellType;

        if (up == CellStatus.VISITED && upType == CellType.FLOOR)
        {
          _doorwayCandidates.Add(new Int2(x1, _roomsBounds[i].FirstPoint.Y));
        }

        if (down == CellStatus.VISITED && downType == CellType.FLOOR)
        {
          _doorwayCandidates.Add(new Int2(x2, _roomsBounds[i].FirstPoint.Y));
        }
      }

      for (int x = _roomsBounds[i].FirstPoint.X + 1; x <= _roomsBounds[i].SecondPoint.X - 1; x++)
      {
        int y1 = _roomsBounds[i].FirstPoint.Y + 1;
        int y2 = _roomsBounds[i].SecondPoint.Y - 1;
        
        int xLeft = (x - 2) < 0 ? 0 : x - 2;
        int xRight = (x + 2) > _gridRef.MapWidth - 1 ? _gridRef.MapWidth - 1 : x + 2;
        
        CellStatus left = _gridRef.Map[xLeft, y1].Status;
        CellStatus right = _gridRef.Map[xRight, y2].Status;
        CellType leftType = _gridRef.Map[xLeft, y1].CellType;
        CellType rightType = _gridRef.Map[xRight, y2].CellType;

        if (left == CellStatus.VISITED && leftType == CellType.FLOOR)
        {
          _doorwayCandidates.Add(new Int2(_roomsBounds[i].FirstPoint.X, y1));
        }
        
        if (right == CellStatus.VISITED && rightType == CellType.FLOOR)
        {
          _doorwayCandidates.Add(new Int2(_roomsBounds[i].SecondPoint.X, y2));
        }
      }

      if (_doorwayCandidates.Count == 0)
      {
        Debug.Log("Removing orphaned room: " + _roomsBounds[i].FirstPoint + " " + _roomsBounds[i].SecondPoint);
        _roomsBounds.RemoveAt(i);
      }
      else
      {
        int doorwayIndex = Random.Range(0, _doorwayCandidates.Count);
        Int2 pos = _doorwayCandidates[doorwayIndex];
        Debug.Log("Making doorway at " + pos);
        _gridRef.Map[pos.X, pos.Y].CellType = CellType.FLOOR;
      }
    }
  }
}
