using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapCell
{
  public int CoordX = -1;
  public int CoordY = -1;
  public int FloorId = -1;
  public int WallId = -1;
  public int CeilingId = -1;
}

public class Int2
{
  int _x = 0;
  int _y = 0;
  
  public Int2()
  {
    _x = 0;
    _y = 0;
  }
  
  public Int2(int x, int y)
  {
    _x = x;
    _y = y;
  }
  
  public Int2(float x, float y)
  {
    _x = (int)x;
    _y = (int)y;
  }
  
  public Int2(Vector2 v2)
  {
    _x = (int)v2.x;
    _y = (int)v2.y;
  }
  
  public int X
  {
    set { _x = value; }
    get { return _x; }
  }
  
  public int Y
  {
    set { _y = value; }
    get { return _y; }
  }
  
  public override string ToString()
  {
    return string.Format("[Int2: X={0}, Y={1}]", X, Y);
  }
}

public class RoomBounds
{
  public Int2 FirstPoint = new Int2();
  public Int2 SecondPoint = new Int2();
  
  public RoomBounds(Int2 p1, Int2 p2)
  {
    FirstPoint.X = p1.X;
    FirstPoint.Y = p1.Y;
    
    SecondPoint.X = p2.X;
    SecondPoint.Y = p2.Y;
  }
  
  public override string ToString()
  {
    return string.Format("[RoomBounds] -> [" + FirstPoint + " " + SecondPoint + "]");
  }
}