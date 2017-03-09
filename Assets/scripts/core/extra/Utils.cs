﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utils
{
  public static int BlockDistance(Int2 from, Int2 to)
  {
    return Mathf.Abs(to.Y - from.Y) + Mathf.Abs(to.X - from.X);
  }	

  /// <summary>
  /// Hide appropriate sides of a block to prevent side doubling and lighting artifacts.
  /// </summary>
  /// <param name="blockSidesList">Block to operate on</param>
  /// <param name="coordinates">Current block coordinates</param>
  /// <param name="level">LevelBase based class</param>
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

      // arrayCoordinatesAdds[side][1] == -1 means that ny will be less that current block y,
      // thus below

      if (ny != y && ny >= 0 && ny <= level.MapY - 1)
      {        
        // Disable shadow casting if submerged
        if (!level.Level[x, y, z].IsLiquid && level.Level[x, ny, z].IsLiquid)
        {
          // If current block is above liquid block
          if ((int)arrayCoordinatesAdds[side][1] == -1)
          {
            // Disable shadow receiving of down quad
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
          // If ny is lower block
          if ((int)arrayCoordinatesAdds[side][1] == -1)
          {            
            // Hide down side of current block
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