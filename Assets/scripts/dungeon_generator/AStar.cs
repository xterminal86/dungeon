using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// A* pathfinder (no diagonal movement)
/// </summary>
public class AStar
{
  Int2 _start = new Int2();
  Int2 _end = new Int2();
    
  List<PathNode> _path = new List<PathNode>();

  List<PathNode> _openList = new List<PathNode>();
  List<PathNode> _closedList = new List<PathNode>();

  GeneratedMapCell[,] _map;

  int _hvCost = 10;
  int _diagonalCost = 1500;

  int _mapWidth = 0, _mapHeight = 0;
  public AStar(GeneratedMapCell[,] map, int width, int height)
  {
    _map = map;

    _mapWidth = width;
    _mapHeight = height;

    //PrintMap();
  }

  /// <summary>
  /// Find first empty cell around current, closest to the destination point
  /// </summary>
  /// <param name="aroundThis">Point around which we search</param>
  /// <returns>Its coordinates if found or null otherwise</returns>
  Int2 FindEmptyCell(Int2 aroundThis)
  {
    Int2 pointFound = new Int2();

    int lx = aroundThis.X - 1;
    int ly = aroundThis.Y - 1;
    int hx = aroundThis.X + 1;
    int hy = aroundThis.Y + 1;

    lx = Mathf.Clamp(lx, 0, _mapHeight - 1);
    ly = Mathf.Clamp(ly, 0, _mapWidth - 1);
    hx = Mathf.Clamp(hx, 0, _mapHeight - 1);
    hy = Mathf.Clamp(hy, 0, _mapWidth - 1);

    /*
    bool found = false;
    int cost = -1;
    int minCost = int.MaxValue;

    cost = ManhattanDistance(new Int2(lx, aroundThis.Y), _start);
    if (cost < minCost)
    {
      found = true;
      minCost = cost;
      pointFound.X = lx;
      pointFound.Y = aroundThis.Y;
    }

    cost = ManhattanDistance(new Int2(hx, aroundThis.Y), _start);
    if (cost < minCost)
    {
      found = true;
      minCost = cost;
      pointFound.X = hx;
      pointFound.Y = aroundThis.Y;
    }

    cost = ManhattanDistance(new Int2(aroundThis.X, ly), _start);
    if (cost < minCost)
    {
      found = true;
      minCost = cost;
      pointFound.X = aroundThis.X;
      pointFound.Y = ly;
    }

    cost = ManhattanDistance(new Int2(aroundThis.X, hy), _start);
    if (cost < minCost)
    {
      found = true;
      minCost = cost;
      pointFound.X = aroundThis.X;
      pointFound.Y = hy;
    }
    */

    int minCost = int.MaxValue;
    bool found = false;
    for (int x = lx; x <= hx; x++)
    {
      for (int y = ly; y <= hy; y++)
      {
        if (_map[x, y].CellType == GeneratedCellType.OBSTACLE ||
            _map[x, y].CellType == GeneratedCellType.ROOM ||
           (x == aroundThis.X && y == aroundThis.Y))
        {
          continue;
        }

        int cost = ManhattanDistance(new Int2(x, y), _start);

        if (cost < minCost)
        {
          found = true;
          minCost = cost;
          pointFound.X = x;
          pointFound.Y = y;
        }
      }
    }

    return found ? pointFound : null;
  }

  /// <summary>
  /// Returns traversal cost between two points
  /// </summary>
  /// <param name="point">Point 1</param>
  /// <param name="goal">Point 2</param>
  /// <returns>Cost of the traversal</returns>
  int TraverseCost(Int2 point, Int2 goal)
  {
    if (point.X == goal.X || point.Y == goal.Y)
    {    
      return _hvCost;
    }

    return _diagonalCost;
  }

  /// <summary>
  /// Heuristic
  /// </summary>
  int ManhattanDistance(Int2 point, Int2 end)
  {
    int cost = ( Mathf.Abs(end.Y - point.Y) + Mathf.Abs(end.X - point.X) ) * _hvCost;

    //Debug.Log(string.Format("Manhattan distance remaining from {0} to {1}: {2}", point.ToString(), end.ToString(), cost));

    return cost;
  }

  /// <summary>
  /// Searches for the element with lowest total cost
  /// </summary>
  int FindCheapestElement(List<PathNode> list)
  {
    int f = int.MaxValue;
    int index = -1;
    int count = 0;

    foreach (var item in list)
    {
      if (item.CostF < f)
      {
        f = item.CostF;
        index = count;
      }

      count++;      
    }

    //Debug.Log("Cheapest element " + list[index].Coordinate + " " + list[index].CostF);

    return index;
  }

  PathNode FindNode(Int2 nodeCoordinate, List<PathNode> listToLookIn)
  {
    foreach (var item in listToLookIn)
    {
      if (nodeCoordinate.X == item.Coordinate.X &&
          nodeCoordinate.Y == item.Coordinate.Y)
      {
        return item;
      }
    }

    return null;
  }

  bool IsNodePresent(Int2 nodeCoordinate, List<PathNode> listToLookIn)
  {
    foreach (var item in listToLookIn)
    {
      if (nodeCoordinate.X == item.Coordinate.X &&
          nodeCoordinate.Y == item.Coordinate.Y)
      {
        return true;
      }
    }

    return false;
  }

  /// <summary>
  /// Creates next nodes for algorithm
  /// </summary>
  void LookAround(PathNode node)
  {    
    int lowerX = node.Coordinate.X - 1;
    int lowerY = node.Coordinate.Y - 1;
    int higherX = node.Coordinate.X + 1;
    int higherY = node.Coordinate.Y + 1;

    lowerX = Mathf.Clamp(lowerX, 0, _mapHeight - 1);
    lowerY = Mathf.Clamp(lowerY, 0, _mapWidth - 1);
    higherX = Mathf.Clamp(higherX, 0, _mapHeight - 1);
    higherY = Mathf.Clamp(higherY, 0, _mapWidth - 1);

    Int2 coordinate = new Int2();
    for (int x = lowerX; x <= higherX; x++)
    {
      for (int y = lowerY; y <= higherY; y++)
      {
        // Comment out the following condition if diagonal cutting of obstacles is allowed

        if ( (_map[higherX, node.Coordinate.Y].CellType == GeneratedCellType.ROOM || 
              _map[higherX, node.Coordinate.Y].CellType == GeneratedCellType.ROAD ) && 
              (x == higherX && (y == higherY || y == lowerY)) ||
             (_map[lowerX, node.Coordinate.Y].CellType == GeneratedCellType.ROOM ||
              _map[lowerX, node.Coordinate.Y].CellType == GeneratedCellType.ROAD) && 
              (x == lowerX && (y == higherY || y == lowerY)) ||
             (_map[node.Coordinate.X, lowerY].CellType == GeneratedCellType.ROOM ||
              _map[node.Coordinate.X, lowerY].CellType == GeneratedCellType.ROAD) && 
              (y == lowerY && (x == lowerX || x == higherX)) ||
             (_map[node.Coordinate.X, higherY].CellType == GeneratedCellType.ROOM ||
              _map[node.Coordinate.X, higherY].CellType == GeneratedCellType.ROAD) && 
              (y == higherY && (x == lowerX || x == higherX)))
        {
          continue;
        }

        coordinate.X = x;
        coordinate.Y = y;

        bool isInClosedList = IsNodePresent(coordinate, _closedList);
        
        if (_map[x, y].CellType != GeneratedCellType.ROOM && !isInClosedList)
        {
          bool isInOpenList = IsNodePresent(coordinate, _openList);

          if (!isInOpenList)
          {
            PathNode newNode = new PathNode(new Int2(x, y), node);
            newNode.CostG = node.CostG + TraverseCost(node.Coordinate, newNode.Coordinate);
            newNode.CostH = ManhattanDistance(newNode.Coordinate, _end);
            newNode.CostF = newNode.CostG + newNode.CostH;

            _openList.Add(newNode);
          }
        }
      }
    }
  }

  bool ExitCondition()
  {
    return (_openList.Count == 0 || IsNodePresent(_end, _closedList));    
  }

  /// <summary>
  /// Method tries to build a path by A* algorithm and returns it as list of nodes
  /// to traverse from start to end
  /// </summary>
  /// <param name="start">Starting point</param>
  /// <param name="end">Destination point</param>
  /// <returns>List of nodes from start to end</returns>
  public List<PathNode> FindPath(Int2 start, Int2 end)
  {
    _start = start;
    _end = end;

    if (_map[_end.X, _end.Y].CellType != GeneratedCellType.NONE)
    {
      Debug.Log("Goal is on the obstacle!");
    }

    _path.Clear();
    _openList.Clear();
    _closedList.Clear();

    // A* starts here

    PathNode node = new PathNode(start);
    node.CostH = ManhattanDistance(start, end);
    node.CostF = node.CostG + node.CostH;

    _openList.Add(node);

    bool exit = false;
    while (!exit)
    {
      int index = FindCheapestElement(_openList);

      var closedNode = _openList[index];
      _closedList.Add(closedNode);

      _openList.RemoveAt(index);

      LookAround(closedNode);

      exit = ExitCondition();
    }

    ConstructPath();
    
    return _path;
  }

  void ConstructPath()
  {
    var node = FindNode(_end, _closedList);

    while (node != null)
    {
      _path.Add(node);
      node = node.ParentNode;
    }

    if (_path.Count != 0)
    {
      _path.Reverse();
      _path.RemoveAt(0);
    }

    PrintPath();
  }
  
  void PrintPath()
  {
    StringBuilder sb = new StringBuilder();

    sb.Append(string.Format("Path from {0} to {1} :", _start.ToString(), _end.ToString()));

    foreach (var item in _path)
    {
      sb.Append(string.Format("[{0};{1} costF: {2}] => ", item.Coordinate.X, item.Coordinate.Y, item.CostF));
    }

    sb.Append("Done!");

    Debug.Log(sb.ToString());
  }

  void PrintMap()
  {
    StringBuilder sb = new StringBuilder();

    for (int x = 0; x < _mapHeight; x++)
    {
      for (int y = 0; y < _mapWidth; y++)
      {
        sb.Append(string.Format("({0};{1}) => {2} | ", x, y, _map[x, y]));
      }
    }

    Debug.Log("Map array: " + sb.ToString());
  }

  /// <summary>
  /// Helper class of path node
  /// </summary>
  public class PathNode
  {
    public PathNode(Int2 coord)
    {
      Coordinate.X = coord.X;
      Coordinate.Y = coord.Y;           
    }

    public PathNode(PathNode rhs)
    {
      Coordinate = rhs.Coordinate;
      ParentNode = rhs.ParentNode;
      CostF = rhs.CostF;
      CostG = rhs.CostG;
      CostH = rhs.CostH;
    }

    public PathNode(Int2 coord, PathNode parent)
    {
      Coordinate.X = coord.X;
      Coordinate.Y = coord.Y;
      ParentNode = parent;
    }

    public override string ToString()
    {
      return Coordinate.ToString();
    }

    // Map coordinate of this node
    public Int2 Coordinate = new Int2();
    // Reference to parent node
    public PathNode ParentNode = null;

    // Total cost
    public int CostF = 0;
    // Cost of traversal here from the starting point with regard of already traversed path
    public int CostG = 0;
    // Heuristic cost
    public int CostH = 0;
  }
}
