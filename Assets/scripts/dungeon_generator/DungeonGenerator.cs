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

  public GenerationMethods MazeGenerationMethod;

  StringBuilder _result;

  Grid _map;
  GenerationAlgorithmBase alg;
  void Start () 
  {
    // NxN for the map and additional N for the newlines
    int bufferSize = MapWidth * MapHeight + MapHeight;

    _result = new StringBuilder(bufferSize);
    _map = new Grid(MapWidth, MapHeight);

    switch (MazeGenerationMethod)
    {
      case GenerationMethods.BINARY_TREE:
        alg = new BinaryTree();
        alg.Do(_map);
        break;
      case GenerationMethods.SIDEWINDER:
        alg = new Sidewinder();
        alg.Do(_map);
        break;
      default:
        break;
    }

    TextArea.text = GetOutput();
    //Debug.Log (TextArea.text);
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
    SIDEWINDER
  }
}
