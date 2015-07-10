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

  [Header("Binary Tree Settings")]
  public bool CheckVisitedCells;
  public bool ContinuousDigging;

  //char[,] _map;
  //int [,] _visitedCells;

  StringBuilder _result;

  Grid _map;
  void Start () 
  {
    // NxN for the map and additional N for the newlines
    int bufferSize = MapWidth * MapHeight + MapHeight;

    _result = new StringBuilder(bufferSize);
    _map = new Grid(MapWidth, MapHeight);

    BinaryTree bt = new BinaryTree();
    bt.Do(_map);

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
    BINARY_TREE = 0
  }
}
