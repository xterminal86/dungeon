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

  public int RoomsDistance = 1;
  public bool NoRoomsIntersection = false;
  public bool ConnectRooms = false;

  [Header("Growing Tree only")]
  public DecisionType PassageType = DecisionType.NEWEST;

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
      _map = new Grid(MapWidth, MapHeight, RoomMaxWidth, RoomMaxHeight, MaxRooms, RoomsDistance);
    }
    else if (MazeGenerationMethod == (int)GenerationMethods.GROWING_TREE)
    {
      _map = new Grid(MapWidth, MapHeight, CellType.EMPTY);
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
        alg = new Rooms(NoRoomsIntersection, ConnectRooms);
        alg.Do(_map);
        break;
      case (int)GenerationMethods.GROWING_TREE:
        alg = new GrowingTree(PassageType);
        alg.Do(_map);
        break;
      default:
        break;
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

  public enum GenerationMethods
  {
    BINARY_TREE = 0,
    SIDEWINDER,
    ROOMS,
    GROWING_TREE
  }
}
