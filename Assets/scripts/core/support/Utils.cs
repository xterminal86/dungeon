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
    int endX = startX + roomHeight - 1;
    int endY = startY + roomWidth - 1;

    //Debug.Log("Rectangle " + startX + " " + startY + " " + endX + " " + endY);

    for (int i = startX; i <= endX; i++)
    {
      arrayToModify[i, startY].CellType = cellTypeToSet;
      arrayToModify[i, endY].CellType = cellTypeToSet;
    }
    
    for (int i = startY + 1; i <= endY - 1; i++)
    {
      arrayToModify[startX, i].CellType = cellTypeToSet;
      arrayToModify[endX, i].CellType = cellTypeToSet;
    }
  }

  /// <summary>
  /// Hide appropriate sides of a block to prevent side doubling and lighting artifacts.
  /// </summary>
  /// <param name="block">Block to operate on</param>
  /// <param name="coordinates">Block coordinates</param>
  /// <param name="level">Level class reference</param>
  public static void HideLevelBlockSides(MinecraftBlock block, Vector3 coordinates, LevelBase level)
  {
    Vector3[] arrayCoordinatesAdds = new Vector3[6] 
    { 
      new Vector3(-1.0f, 0.0f, 0.0f),
      new Vector3(1.0f, 0.0f, 0.0f),
      new Vector3(0.0f, -1.0f, 0.0f),
      new Vector3(0.0f, 1.0f, 0.0f),
      new Vector3(0.0f, 0.0f, -1.0f),
      new Vector3(0.0f, 0.0f, 1.0f),
    };

    int x = (int)coordinates.x;
    int y = (int)coordinates.y;
    int z = (int)coordinates.z;

    for (int side = 0; side < 6; side++)
    { 
      int nx = x + (int)arrayCoordinatesAdds[side][0];
      int ny = y + (int)arrayCoordinatesAdds[side][1];
      int nz = z + (int)arrayCoordinatesAdds[side][2];

      // Check for map size limits
      if (nx != x && nx >= 0 && nx <= level.MapX - 1)
      {
        // Disable shadow receiving if submerged
        if (!level.Level[x, y, z].IsLiquid && level.Level[nx, y, z].IsLiquid)
        {
          if ((int)arrayCoordinatesAdds[side][0] == -1)
          {
            block.LeftQuadRenderer.receiveShadows = false;
          }
          else if ((int)arrayCoordinatesAdds[side][0] == 1)
          {
            block.RightQuadRenderer.receiveShadows = false;
          }
        }

        // Hide corresponding side of the current block if:
        //
        // 1) Current block is solid and neighbouring block is solid
        // 2) Current block is liquid and neighbouring block is liquid
        // 3) Current block is liquid and neighbouring block is solid

        if ((!level.Level[x, y, z].IsLiquid && !level.Level[nx, y, z].IsLiquid && level.Level[nx, y, z].BlockType != GlobalConstants.BlockType.AIR)
          || (level.Level[x, y, z].IsLiquid &&  level.Level[nx, y, z].IsLiquid)
          || (level.Level[x, y, z].IsLiquid && !level.Level[nx, y, z].IsLiquid))
        {
          if ((int)arrayCoordinatesAdds[side][0] == -1)
          {
            block.LeftQuad.gameObject.SetActive(false);
          }
          else if ((int)arrayCoordinatesAdds[side][0] == 1)
          {
            block.RightQuad.gameObject.SetActive(false);
          }
        }
      }

      if (nz != z && nz >= 0 && nz <= level.MapZ - 1)
      {
        // Disable shadow casting if submerged
        if (!level.Level[x, y, z].IsLiquid && level.Level[x, y, nz].IsLiquid)
        {
          if ((int)arrayCoordinatesAdds[side][2] == -1)
          {
            block.ForwardQuadRenderer.receiveShadows = false;
          }
          else if ((int)arrayCoordinatesAdds[side][2] == 1)
          {
            block.BackQuadRenderer.receiveShadows = false;
          }
        }

        if ((!level.Level[x, y, z].IsLiquid && !level.Level[x, y, nz].IsLiquid && level.Level[x, y, nz].BlockType != GlobalConstants.BlockType.AIR)
          || (level.Level[x, y, z].IsLiquid && level.Level[x, y, nz].IsLiquid)
          || (level.Level[x, y, z].IsLiquid && !level.Level[x, y, nz].IsLiquid))
        {
          if ((int)arrayCoordinatesAdds[side][2] == -1)
          {
            block.ForwardQuad.gameObject.SetActive(false);
          }
          else if ((int)arrayCoordinatesAdds[side][2] == 1)
          {
            block.BackQuad.gameObject.SetActive(false);
          }
        }
      }

      if (ny != y && ny >= 0 && ny <= level.MapY - 1)
      {        
        // Disable shadow casting if submerged
        if (!level.Level[x, y, z].IsLiquid && level.Level[x, ny, z].IsLiquid)
        {
          if ((int)arrayCoordinatesAdds[side][1] == -1)
          {
            block.DownQuadRenderer.receiveShadows = false;
          }
          else if ((int)arrayCoordinatesAdds[side][1] == 1)
          {
            block.UpQuadRenderer.receiveShadows = false;
          }
        }

        if ((!level.Level[x, y, z].IsLiquid && !level.Level[x, ny, z].IsLiquid && level.Level[x, ny, z].BlockType != GlobalConstants.BlockType.AIR)
          || (level.Level[x, y, z].IsLiquid && level.Level[x, ny, z].IsLiquid))
        {
          if ((int)arrayCoordinatesAdds[side][1] == -1)
          {            
            block.DownQuad.gameObject.SetActive(false);
          }
          else if ((int)arrayCoordinatesAdds[side][1] == 1)
          {
            block.UpQuad.gameObject.SetActive(false);
          }
        }
      }
    }
  }
}
