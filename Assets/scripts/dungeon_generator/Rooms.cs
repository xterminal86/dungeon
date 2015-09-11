using UnityEngine;
using System.Collections;

public class Rooms : GenerationAlgorithmBase
{
  Grid _gridRef;

  public override void Do(Grid grid)
  {
    _gridRef = grid;

    MakeRooms();
  }

  int _iterations = 0;
  void MakeRooms()
  {
    while (_iterations < _gridRef.MaxRooms)
    {
      Vector2 cellPos = _gridRef.GetRandomCellPos();
      int roomWidth = Random.Range(2, _gridRef.RoomMaxWidth + 1);
      int roomHeight = Random.Range(2, _gridRef.RoomMaxHeight + 1);

      // Again, remember that map size of 10x5 means x:10, y:5 -> 10 columns, 5 rows in array form.
      // So we need to use height in x calculations and width in y.

      int boundaryX = ((int)cellPos.x + roomHeight);
      int boundaryY = ((int)cellPos.y + roomWidth);

      if (boundaryX > _gridRef.MapHeight - 1)
      {
        cellPos.x -= roomHeight;
      }

      if (boundaryY > _gridRef.MapWidth - 1)
      {
        cellPos.y -= roomWidth;
      }

      //Debug.Log(cellPos + " " + roomWidth + " " + roomHeight);

      for (int i = (int)cellPos.x; i < (int)cellPos.x + roomHeight; i++)
      {
        for (int j = (int)cellPos.y; j < (int)cellPos.y + roomWidth; j++)
        {
          _gridRef.Map[i, j].SetVisualRepresentation(CellVisualization.EMPTY);
        }
      }

      _iterations++;
    }
  }
}
