using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utils
{
  public static int BlockDistance(Int2 from, Int2 to)
  {
    return Mathf.Abs(to.Y - from.Y) + Mathf.Abs(to.X - from.X);
  }	

  public static void MarkRectangle(Vector2 cellPos, int roomWidth, int roomHeight, GeneratedCellType cellTypeToSet, GeneratedMapCell[,] arrayToModify)
  {
    int startX = (int)cellPos.x;
    int startY = (int)cellPos.y;
    int endX = startX + roomHeight;
    int endY = startY + roomWidth;

    //Debug.Log("Rectangle " + startX + " " + startY + " " + endX + " " + endY);

    for (int i = startX; i < endX; i++)
    {
      arrayToModify[i, startY].CellType = cellTypeToSet;
      arrayToModify[i, endY].CellType = cellTypeToSet;
    }
    
    for (int i = startY + 1; i <= endY - 1; i++)
    {
      arrayToModify[i, startY].CellType = cellTypeToSet;
      arrayToModify[i, endY].CellType = cellTypeToSet;
    }
  }
}
