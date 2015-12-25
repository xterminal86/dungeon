using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class DungeonGenerator : MonoBehaviour 
{
  public Text TextArea;

  [Range(2, 100)]
  public int MapWidth;
  [Range(2, 100)]
  public int MapHeight;

  public int MazeGenerationMethod;

  [Header("Rooms only")]
  [Range(2, 10)]
  public int RoomMaxWidth = 3;
  [Range(2, 10)]
  public int RoomMaxHeight = 3;
  [Range(1, 100)]
  public int MaxRooms = 1;
  public bool RoomsRemoveDeadEnds = false;
  public int RoomsDeadEndsToRemove = 1;

  public int RoomsDistance = 1;
  public bool NoRoomsIntersection = false;
  public bool ConnectRooms = false;

  [Header("Growing Tree only")]
  public DecisionType PassageType = DecisionType.NEWEST;
  public bool RemoveDeadEnds = false;
  public int DeadEndsToRemove = 1000;

  public bool DoCleanup = true;

  StringBuilder _result;

  Grid _map;
  GenerationAlgorithmBase alg;
  void Start () 
  {
    // NxN for the map and additional N for the newlines
    int bufferSize = MapWidth * MapHeight + MapHeight;

    _result = new StringBuilder(bufferSize);

    if (MazeGenerationMethod == (int)GenerationMethods.ROOMS)
    {
      _map = new Grid(MapWidth, MapHeight);
    }
    else
    {
      _map = new Grid(MapWidth, MapHeight, CellType.WALL);
    }

    switch (MazeGenerationMethod)
    {
      case (int)GenerationMethods.BINARY_TREE:
        alg = new BinaryTree();
        alg.Do(_map);
        break;
      case (int)GenerationMethods.SIDEWINDER:
        alg = new Sidewinder();
        alg.Do(_map);
        break;
      case (int)GenerationMethods.ROOMS:
        alg = new Rooms(RoomMaxWidth, RoomMaxHeight, MaxRooms, RoomsDistance, NoRoomsIntersection, ConnectRooms,
                        RoomsRemoveDeadEnds, RoomsDeadEndsToRemove);
        alg.Do(_map);
        break;
      case (int)GenerationMethods.GROWING_TREE:
        alg = new GrowingTree(PassageType, RemoveDeadEnds, DeadEndsToRemove);
        alg.Do(_map);
        break;
      case (int)GenerationMethods.ROAD_BUILDER_TEST:
        BuildRoad();
        break;
      default:
        break;
    }

    if (alg != null && DoCleanup)
    {
      alg.Cleanup(_map);
    }

    TextArea.text = GetOutput();
    Debug.Log (TextArea.text);
	}

  string GetOutput()
  {
    _result.Remove(0, _result.Length);
    _result.Capacity = 0;

    for (int i = 0; i < MapHeight; i++)
    {
      for (int j = 0; j < MapWidth; j++)
      {
        _result.Append(_map.Map[i, j].VisualRepresentation);
      }
      _result.Append('\n');
    }

    return _result.ToString();
  }

  GeneratedMapCell[,] _testMap;
  void BuildRoad()
  {
    _testMap = new GeneratedMapCell[MapHeight, MapWidth];

    for (int x = 0; x < MapHeight; x++)
    {
      for (int y = 0; y < MapWidth; y++)
      {
        _map.Map[x, y].CellType = CellType.FLOOR;

        _testMap[x, y] = new GeneratedMapCell();
        _testMap[x, y].CellType = GeneratedCellType.NONE;
      }
    }

    Int2 tl = new Int2(10, 10);
    Int2 br = new Int2(20, 20);

    BuildRoom(tl, br);

    for (int x = 0; x < MapHeight; x++)
    {
      for (int y = 0; y < MapWidth; y++)
      {
        if (_testMap[x, y].CellType == GeneratedCellType.OBSTACLE)
        {
          _map.Map[x, y].CellType = CellType.WALL;
        }
        else
        {
          _map.Map[x, y].CellType = CellType.FLOOR;
        }
      }
    }

    Int2 start = new Int2(20, 20);
    Int2 end = new Int2(15, 15);

    var rb = new RoadBuilder(_testMap, MapWidth, MapHeight);

    rb.BuildRoadAsync(start, end, true);

    Job j = new Job(BuildPathRoutine(rb));
  }

  IEnumerator BuildPathRoutine(RoadBuilder rb)
  {
    List<RoadBuilder.PathNode> road;

    while ((road = rb.GetResult()) == null)
    {
      int x = rb.CurrentNode.Coordinate.X;
      int y = rb.CurrentNode.Coordinate.Y;
      
      _map.Map[x, y].CellType = CellType.TEST_MARK;

      TextArea.text = GetOutput();

      yield return null;
    }

    if (road.Count == 0)
    {
      Debug.Log("Could not find path!");
    }
    else
    {
      foreach (var item in road)
      {
        int x = item.Coordinate.X;
        int y = item.Coordinate.Y;
        
        _map.Map[x, y].CellType = CellType.TEST_MARK;
      }
    }

    TextArea.text = GetOutput();   

    yield return null;
  }

  void BuildRoom(Int2 tl, Int2 br)
  {
    for (int x = tl.X; x < br.X; x++)
    {
      for (int y = tl.Y; y < br.Y; y++)
      {
        //_map.Map[x, y].CellType = CellType.WALL;
        _testMap[x, y].CellType = GeneratedCellType.OBSTACLE;
      }
    }

    for (int x = tl.X + 1; x < br.X - 1; x++)
    {
      for (int y = tl.Y + 1; y < br.Y - 1; y++)
      {
        //_map.Map[x, y].CellType = CellType.FLOOR;
        _testMap[x, y].CellType = GeneratedCellType.ROAD;
      }
    }

    //_testMap[tl.X + 1, tl.Y].CellType = GeneratedCellType.ROAD;
  }

  public enum GenerationMethods
  {
    BINARY_TREE = 0,
    SIDEWINDER,
    ROOMS,
    GROWING_TREE,
    ROAD_BUILDER_TEST
  }
}
