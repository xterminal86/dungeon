using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// A* pathfinder (no diagonal movement)
/// </summary>
public class RoadBuilder
{
  // FIXME: rewrite for new map organization

  Int2 _start = new Int2();
  Int2 _end = new Int2();
    
  List<PathNode> _path = new List<PathNode>();

  List<PathNode> _openList = new List<PathNode>();
  List<PathNode> _closedList = new List<PathNode>();

  //GeneratedMapCell[,] _map;
  //PathfindingCell[,] _pathfindingMap;

  int _hvCost = 10;
  int _diagonalCost = 20;

  int _mapWidth = 0, _mapHeight = 0;
  public RoadBuilder(int width, int height)
  {
    //_map = map;

    _mapWidth = width;
    _mapHeight = height;

    _resultReady = false;
    _abortThread = false;

    //PrintMap();
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

  void LookAround4(PathNode node, bool avoidObstacles)
  {
    sbyte[,] direction = new sbyte[4, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };

    List<GlobalConstants.Orientation> orientations = new List<GlobalConstants.Orientation>()
    { 
      GlobalConstants.Orientation.WEST, GlobalConstants.Orientation.SOUTH, 
      GlobalConstants.Orientation.EAST, GlobalConstants.Orientation.NORTH 
    };

    List<GlobalConstants.Orientation> oppositeOrientations = new List<GlobalConstants.Orientation>()
    { 
      GlobalConstants.Orientation.EAST, GlobalConstants.Orientation.NORTH, 
      GlobalConstants.Orientation.WEST, GlobalConstants.Orientation.SOUTH 
    };

    Int2 coordinate = new Int2();
    for (int i = 0; i < 4; i++)
    {
      coordinate.X = node.Coordinate.X + direction[i, 0];
      coordinate.Y = node.Coordinate.Y + direction[i, 1];

      coordinate.X = Mathf.Clamp(coordinate.X, 0, _mapHeight - 1);
      coordinate.Y = Mathf.Clamp(coordinate.Y, 0, _mapWidth - 1);

      bool isInClosedList = IsNodePresent(coordinate, _closedList);

      //Debug.Log("Current cell " + node.Coordinate + " Next cell " + coordinate);

      bool condition = false;

      /*
      // Cell is valid if next cell is marked as passable and current cell does not have thin wall 
      // along current orientation, and next cell does not have thin wall along opposite orientation.
      bool condition = avoidObstacles ? 
                       (_pathfindingMap[coordinate.X, coordinate.Y].Walkable 
                     && _pathfindingMap[node.Coordinate.X, node.Coordinate.Y].SidesWalkability[orientations[i]] == true
                     && _pathfindingMap[coordinate.X, coordinate.Y].SidesWalkability[oppositeOrientations[i]] == true 
                     && !isInClosedList) 
                     : !isInClosedList;
      */

      /*
      bool condition = (avoidObstacles ? 
                       (_map[coordinate.X, coordinate.Y].CellType != GeneratedCellType.ROOM &&
                        _map[coordinate.X, coordinate.Y].CellType != GeneratedCellType.OBSTACLE && !isInClosedList) :
                       (_map[coordinate.X, coordinate.Y].CellType != GeneratedCellType.ROOM && !isInClosedList) );
      
      GeneratedCellType ct = _map[coordinate.X, coordinate.Y].CellType;

      bool condition = ( avoidObstacles ? 
                       ((ct != GeneratedCellType.OBSTACLE && ct != GeneratedCellType.DECOR) && !isInClosedList) :
                       !isInClosedList );
      */

      if (condition)
      {
        bool isInOpenList = IsNodePresent(coordinate, _openList);

        if (!isInOpenList)
        {
          PathNode newNode = new PathNode(new Int2(coordinate.X, coordinate.Y), node);
          newNode.CostG = node.CostG + TraverseCost(node.Coordinate, newNode.Coordinate);
          newNode.CostH = ManhattanDistance(newNode.Coordinate, _end);
          newNode.CostF = newNode.CostG + newNode.CostH;

          _openList.Add(newNode);
        }
      }      
    }    
  }

  /// <summary>
  /// Creates next nodes for algorithm
  /// </summary>
  void LookAround9(PathNode node)
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
        coordinate.X = x;
        coordinate.Y = y;

        bool isInClosedList = IsNodePresent(coordinate, _closedList);

        /*
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
        */
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
  public List<PathNode> BuildRoad(Int2 start, Int2 end, bool avoidObstacles = false, bool printPath = false)
  {
    _start = start;
    _end = end;

    /*
    //if (_map[_end.X, _end.Y].CellType != GeneratedCellType.NONE && !avoidObstacles)
    if (!_pathfindingMap[_end.X, _end.Y].Walkable)
    {
      Debug.Log(end + " - Goal is on the obstacle! : " + _map[_end.X, _end.Y].CellType + " (this is not an error)");
    }
    */

    _path.Clear();
    _openList.Clear();
    _closedList.Clear();

    // A* starts here

    PathNode node = new PathNode(start);
    node.CostH = ManhattanDistance(start, end);
    node.CostF = node.CostG + node.CostH;

    _openList.Add(node);
        
    //string debugPrint = string.Empty;

    bool exit = false;
    while (!exit)
    {
      int index = FindCheapestElement(_openList);

      var closedNode = _openList[index];
      _closedList.Add(closedNode);

      //debugPrint += string.Format("{0} ", closedNode.Coordinate);

      _openList.RemoveAt(index);

      LookAround4(closedNode, avoidObstacles);

      exit = ExitCondition();
    }
    
    ConstructPath(printPath);

    //Debug.Log("Total closed nodes:\n" + debugPrint);

    return _path;
  }

  bool _threadIsWorking = false;
  public bool IsThreadWorking
  {
    get { return _threadIsWorking; }
  }

  // Async version of above
  public void BuildRoadAsync(Int2 start, Int2 end, bool avoidObstacles = false)
  {    
    _threadIsWorking = true;

    // When we build road to player in async mode if we are just caching variables,
    // we make references, so if in the process of building road player position changes,
    // it fucks up algorithm working, because we change _end during pathfinding loop.
    // So, to prevent this, we copy by value.
    _start = new Int2(start.X, start.Y);
    _end = new Int2(end.X, end.Y);

    /*
    //if (_map[_end.X, _end.Y].CellType != GeneratedCellType.NONE && !avoidObstacles)
    if (!_pathfindingMap[_end.X, _end.Y].Walkable)
    {
      Debug.Log(end + " - Goal is on the obstacle! : " + _map[_end.X, _end.Y].CellType + " (this is not an error)");
    }
    */

    _path.Clear();
    _openList.Clear();
    _closedList.Clear();

    // A* starts here

    PathNode node = new PathNode(start);
    node.CostH = ManhattanDistance(start, end);
    node.CostF = node.CostG + node.CostH;

    _openList.Add(node);

    _resultReady = false;

    JobManager.Instance.CreateThreadB(BuildRoadThreadFunction, avoidObstacles);
  }

  volatile bool _abortThread = false;
  void BuildRoadThreadFunction(object arg)
  {
    bool avoidObstacles = (bool)arg;

    _abortThread = false;

    bool exit = false;
    while (!exit)
    {
      if (ThreadWatcher.Instance.StopAllThreads || _abortThread)
      {
        return;
      }
            
      int index = FindCheapestElement(_openList);

      var closedNode = _openList[index];
      _closedList.Add(closedNode);

      _currentNode = closedNode;

      //Debug.Log(closedNode);

      _openList.RemoveAt(index);

      LookAround4(closedNode, avoidObstacles);

      exit = ExitCondition();      
    }

    //Debug.Log("building road done!");

    _resultReady = true;

    _threadIsWorking = false;
  }

  public void AbortThread()
  {
    _abortThread = true;
  }

  // Constantly check the result via this property in a coroutine  
  public List<PathNode> GetResult(bool ignoreAsync = false)
  {
    if (_resultReady || ignoreAsync)
    {
      ConstructPath();

      return _path;
    }
    
    return null;
  }

  PathNode _currentNode;
  public PathNode CurrentNode
  {
    get { return _currentNode; }
  }

  bool _resultReady = false;
  public bool ResultReady
  {
    get { return _resultReady; }
  }

  void ConstructPath(bool printPath = false)
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

    if (printPath)
    {
      PrintPath();
    }
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
        //sb.Append(string.Format("({0};{1}) => {2} | ", x, y, _map[x, y]));
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
