using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadBuilder
{
  GeneratedMapCell[,] _map;

  int _mapWidth = -1;
  int _mapHeight = -1;

  int _hvCost = 10;

  Int2 _startintPoint = new Int2();
  Int2 _endingPoint = new Int2();

  public RoadBuilder(GeneratedMapCell[,] map, int mapWidth, int mapHeight)
  {
    _map = map;
    _mapWidth = mapWidth;
    _mapHeight = mapHeight;
  }

  int _safeguard = 1000;
  int _idleCounter = 0;

  List<Node> _closedList = new List<Node>();
  List<Node> _openList = new List<Node>();

  List<Int2> _road = new List<Int2>();
  public List<Int2> BuildRoad(Int2 start, Int2 end)
  {
    _startintPoint = start;
    _endingPoint = end;

    _openList.Clear();
    _closedList.Clear();

    Node startingNode = new Node(start);
    startingNode.Cost = ManhattanDistance(start, end);

    _openList.Add(startingNode);

    bool exit = false;

    _idleCounter = 0;

    while (!exit)
    {
      if (_idleCounter > _safeguard)
      {
        Debug.LogWarning("Terminated by safeguard");
        break;
      }

      int index = FindCheapestNode(_openList);

      _closedList.Add(_openList[index]);

      LookAround(_openList[index]);

      if (IsNodePresent(end, _closedList) || index == -1)
      {
        exit = true;
      }

      _idleCounter++;
    }

    ConstructPath();

    return _road;
  }

  void ConstructPath()
  {
    _road.Clear();

    foreach (var item in _closedList)
    {
      _road.Add(item.Coordinate);
    }
  }

  List<Int2> _around = new List<Int2>();
  void LookAround(Node node)
  {
    _around.Clear();

    int x = node.Coordinate.X;
    int y = node.Coordinate.Y;

    int xUp = node.Coordinate.X - 1;
    int xDown = node.Coordinate.X + 1;
    int yLeft = node.Coordinate.Y - 1;
    int yRight = node.Coordinate.Y + 1;

    xUp = Mathf.Clamp(xUp, 0, _mapHeight - 1);
    xDown = Mathf.Clamp(xDown, 0, _mapHeight - 1);
    yLeft = Mathf.Clamp(yLeft, 0, _mapWidth - 1);
    yRight = Mathf.Clamp(yRight, 0, _mapWidth - 1);

    Int2 up = new Int2(xUp, y);
    Int2 down = new Int2(xDown, y);
    Int2 left = new Int2(x, yLeft);
    Int2 right = new Int2(x, yRight);

    _around.Add(up);
    _around.Add(down);
    _around.Add(left);
    _around.Add(right);

    foreach (var item in _around)
    {
      if (!IsNodePresent(item, _openList) && _map[item.X, item.Y].CellType != GeneratedCellType.ROOM)
      {
        Node n = new Node(item);
        n.Cost = ManhattanDistance(item, _endingPoint);

        _openList.Add(n);
      }
    }
  }

  int FindCheapestNode(List<Node> list)
  {
    int index = -1;
    int cost = int.MaxValue;

    for (int i = 0; i < list.Count; i++)
    {
      if (list[i].Cost < cost)
      {
        cost = list[i].Cost;
        index = i;
      }
    }

    return index;
  }

  bool IsNodePresent(Int2 nodeCoordinate, List<Node> listToLookIn)
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

  int ManhattanDistance(Int2 point, Int2 end)
  {
    int cost = ( Mathf.Abs(end.Y - point.Y) + Mathf.Abs(end.X - point.X) ) * _hvCost;
    
    //Debug.Log(string.Format("Manhattan distance remaining from {0} to {1}: {2}", point.ToString(), end.ToString(), cost));
    
    return cost;
  }

  public class Node
  {
    public Node(Int2 coord)
    {
      Coordinate.X = coord.X;
      Coordinate.Y = coord.Y;
    }

    public override string ToString()
    {
      return string.Format("[{0};{1} cost: {2}]", Coordinate.X, Coordinate.Y, Cost);
    }

    public Int2 Coordinate = new Int2();
    public int Cost = int.MaxValue;
  };
}
