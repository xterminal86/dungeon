using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// A Star pathfinder
/// </summary>
public class Pathfinder
{
  Int2 _start = new Int2();
  Int2 _end = new Int2();
    
  List<PathNode> _path = new List<PathNode>();

  List<PathNode> _openList = new List<PathNode>();
  List<PathNode> _closedList = new List<PathNode>();

  GeneratedMapCell[,] _map;

  int _hvCost = 10;
  int _diagonalCost = 15;

  int _mapWidth = 0, _mapHeight = 0;
  public Pathfinder(GeneratedMapCell[,] map, int width, int height)
  {
    _map = map;

    _mapWidth = width;
    _mapHeight = height;

    //PrintMap();
  }

  /// <summary>
  /// Найти первую свободную клетку вокруг данной, ближайшую по направлению к точке назначения
  /// </summary>
  /// <param name="aroundThis">Точка, вокруг которой ищем</param>
  /// <returns>Координаты найденной точки или null</returns>
  Int2 FindEmptyCell(Int2 aroundThis)
  {
    Int2 pointFound = new Int2();

    int lx = aroundThis.X - 1;
    int ly = aroundThis.Y - 1;
    int hx = aroundThis.X + 1;
    int hy = aroundThis.Y + 1;

    lx = Mathf.Clamp(lx, 0, _mapHeight);
    ly = Mathf.Clamp(ly, 0, _mapWidth);
    hx = Mathf.Clamp(hx, 0, _mapHeight);
    hy = Mathf.Clamp(hy, 0, _mapWidth);
        
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
  /// В случае, когда точка назначения попала на препятствие, то пробуем найти ближайшую точку рядом с ней.
  /// Если нашли, то точка назначения становится этой точкой, иначе никуда не идём вообще.  
  /// </summary>
  /// <param name="pos">Точка назначения</param>
  void FindNearestNode(Int2 pos)
  {
    Int2 pointFound = FindEmptyCell(pos);

    if (pointFound != null)
    {
      _end = pointFound;
      //Debug.Log(string.Format("Found: going to {0} instead", _end));
    }
    else
    {
      //Debug.Log("Seems like we can't get there... :(");
      _end = _start;
    }
  }

  /// <summary>
  /// Возвращает стоимость перемещения между двумя точками
  /// </summary>
  /// <param name="point">Точка 1</param>
  /// <param name="goal">Точка 2</param>
  /// <returns>Стоимость перемещения</returns>
  int TraverseCost(Int2 point, Int2 goal)
  {
    if (point.X == goal.X || point.Y == goal.Y)
    {
      return _hvCost;
    }

    return _diagonalCost;
  }

  /// <summary>
  /// Эвристическая функция оценки расстояния от данной точки до точки назначения (для A* алгоритма)
  /// </summary>
  /// <param name="point">Данная точка</param>
  /// <param name="end">Точка назначения</param>
  /// <returns>"Стоимость" до точки назначения</returns>
  int ManhattanDistance(Int2 point, Int2 end)
  {
    int cost = Mathf.Abs( (Mathf.Abs(end.Y) - Mathf.Abs(point.Y)) + 
                          (Mathf.Abs(end.X) - Mathf.Abs(point.X)) ) * _hvCost;

    //Debug.Log(string.Format("Manhattan distance remaining from {0} to {1}: {2}", point.ToString(), end.ToString(), cost));

    return cost;
  }

  /// <summary>
  /// Метод ищет элемент с самой низкой итоговой стоимостью из списка узлов пути
  /// </summary>
  /// <param name="list"></param>
  /// <returns></returns>
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

    return index;
  }

  /// <summary>
  /// Возвращает элемент из заданного списка с заданными координатами
  /// </summary>
  /// <param name="nodeCoordinate">Координаты карты</param>
  /// <param name="listToLookIn">Список, в котором производится поиск</param>
  /// <returns>Элемент из заданного списка с заданными координатами или null</returns>
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

  /// <summary>
  /// Проверяет есть ли в данном списке элемент с данными координатами
  /// </summary>
  /// <param name="nodeCoordinate">Координаты точки</param>
  /// <param name="listToLookIn">Список, в котором производится поиск</param>
  /// <returns>true, если элемент есть, иначе false</returns>
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
  /// Создаёт следующие узлы пути для работы алгоритма вокруг данной точки
  /// </summary>
  /// <param name="node"></param>
  void LookAround(PathNode node)
  {    
    int lowerX = node.Coordinate.X - 1;
    int lowerY = node.Coordinate.Y - 1;
    int higherX = node.Coordinate.X + 1;
    int higherY = node.Coordinate.Y + 1;

    lowerX = Mathf.Clamp(lowerX, 0, _mapHeight);
    lowerY = Mathf.Clamp(lowerY, 0, _mapWidth);
    higherX = Mathf.Clamp(higherX, 0, _mapHeight);
    higherY = Mathf.Clamp(higherY, 0, _mapWidth);
        
    Int2 coordinate = new Int2();
    for (int x = lowerX; x <= higherX; x++)
    {
      for (int y = lowerY; y <= higherY; y++)
      {
        // Закомментировать условие, если допускается срезать углы рядом с препятствием

        if (_map[higherX, node.Coordinate.Y] != -1 && (x == higherX && (y == higherY || y == lowerY)) ||
            _map[lowerX, node.Coordinate.Y] != -1 && (x == lowerX && (y == higherY || y == lowerY)) ||
            _map[node.Coordinate.X, lowerY] != -1 && (y == lowerY && (x == lowerX || x == higherX)) ||
            _map[node.Coordinate.X, higherY] != -1 && (y == higherY && (x == lowerX || x == higherX)))
        {
          continue;
        }
        
        coordinate.X = x;
        coordinate.Y = y;

        bool isInClosedList = IsNodePresent(coordinate, _closedList);
        
        if (_map[x, y] == -1 && !isInClosedList)
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
  
  /// <summary>
  /// Тест на завершение работы алгоритма A*
  /// </summary>
  /// <returns></returns>
  bool ExitCondition()
  {
    return (_openList.Count == 0 || IsNodePresent(_end, _closedList));    
  }

  /// <summary>
  /// Метод пытается построить путь между заданными точками по алгоритму A*,
  /// и возвращает его в виде списка узлов, которые нужно пройти, чтобы добраться до точки назначения.
  /// </summary>
  /// <param name="start">Исходная точка</param>
  /// <param name="end">Точка назначения</param>
  /// <returns>Список узлов пути в порядке следования кординат до цели</returns>
  public List<PathNode> FindPath(Int2 start, Int2 end)
  {
    _start = start;
    _end = end;

    if (_map[_end.X, _end.Y] != -1)
    {
      //Debug.Log("Goal is on the obstacle! Trying to find nearest cell...");
      FindNearestNode(_end);
    }

    _path.Clear();
    _openList.Clear();
    _closedList.Clear();

    // Алгоритм A* начинается здесь

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

  /// <summary>
  /// Метод создания итогового пути до цели
  /// </summary>
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

    //PrintPath();
  }
  
  void PrintPath()
  {
    StringBuilder sb = new StringBuilder();

    sb.Append(string.Format("Path from {0} to {1} :", _start.ToString(), _end.ToString()));

    foreach (var item in _path)
    {
      sb.Append(string.Format("[{0};{1}] => ", item.Coordinate.X, item.Coordinate.Y));
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
  /// Класс узла пути
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

    // Координата карты этого узла
    public Int2 Coordinate = new Int2();
    // Ссылка на родительский узел
    public PathNode ParentNode = null;

    // Итоговая стоимость
    public int CostF = 0;
    // Стоимость перехода в данную точку из стартовой, с учётом уже пройденного пути
    public int CostG = 0;
    // Эвристическая стоимость
    public int CostH = 0;
  }
}
