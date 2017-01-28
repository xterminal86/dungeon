﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBase
{
  protected BlockEntity[,,] _level;
  public BlockEntity[,,] Level
  {
    get { return _level; }
  }

  protected int _mapX, _mapY, _mapZ;

  public int MapX
  {
    get { return _mapX; }
  }

  public int MapY
  {
    get { return _mapY; }
  }

  public int MapZ
  {
    get { return _mapZ; }
  }

  // Position of player (might be starting pos and saved for subsequent loading)
  protected Int3 _playerPos = new Int3();
  public Int3 PlayerPos
  {
    get { return _playerPos; }
  }

  public LevelBase(int x, int y, int z)
  {
    _mapX = x;
    _mapY = y;
    _mapZ = z;

    _level = new BlockEntity[x, y, z];

    InitArray();
  }

  public virtual void Generate()
  {
  }

  void InitArray()
  {
    for (int y = 0; y < _mapY; y++)
    {
      for (int x = 0; x < _mapX; x++)
      {
        for (int z = 0; z < _mapZ; z++)
        {
          _level[x, y, z] = new BlockEntity();
          _level[x, y, z].BlockType = GlobalConstants.BlockType.AIR;
          _level[x, y, z].ArrayCoordinates.Set(x, y, z);
          _level[x, y, z].WorldCoordinates.Set(x * GlobalConstants.WallScaleFactor, y * GlobalConstants.WallScaleFactor, z * GlobalConstants.WallScaleFactor);
        }
      }
    }
  }

  // ************************ HELPER FUNCTIONS ************************ //

  /// <summary>
  /// Sets no-instantiate flag on blocks that are surrounded on all 6 sides.
  /// Works for all map height on a given area.
  /// </summary>
  /// <param name="areaStartX">Area start x.</param>
  /// <param name="areaEndX">Area end x.</param>
  /// <param name="areaStartZ">Area start z.</param>
  /// <param name="areaEndZ">Area end z.</param>
  protected void DiscardHiddenBlocks(int areaStartX, int areaEndX, 
                                     int areaStartY, int areaEndY, 
                                     int areaStartZ, int areaEndZ)
  {
    areaStartX = Mathf.Clamp(areaStartX, 0, _mapX - 1);
    areaEndX = Mathf.Clamp(areaEndX, 0, _mapX - 1);
    areaStartY = Mathf.Clamp(areaStartY, 0, _mapY - 1);
    areaEndY = Mathf.Clamp(areaEndY, 0, _mapY - 1);
    areaStartZ = Mathf.Clamp(areaStartZ, 0, _mapZ - 1);
    areaEndZ = Mathf.Clamp(areaEndZ, 0, _mapZ - 1);

    int lx, ly, lz, hx, hy, hz = 0;

    for (int y = 1; y < _mapY - 1; y++)
    {
      ly = y - 1;
      hy = y + 1;

      for (int x = areaStartX; x < areaEndX; x++)
      {
        lx = x - 1;
        hx = x + 1;

        for (int z = areaStartZ; z < areaEndZ; z++)
        {          
          // Skip if current block is air block
          if (_level[x, y, z].BlockType == GlobalConstants.BlockType.AIR)
          {
            continue;
          }

          lz = z - 1;
          hz = z + 1;

          // We cannot replace BlockId directly, since then on next loop iteration
          // the condition will fail.
          if (_level[lx, y, z].BlockType != GlobalConstants.BlockType.AIR &&  !_level[lx, y, z].IsLiquid 
            && _level[hx, y, z].BlockType != GlobalConstants.BlockType.AIR && !_level[hx, y, z].IsLiquid
            && _level[x, ly, z].BlockType != GlobalConstants.BlockType.AIR && !_level[x, ly, z].IsLiquid
            && _level[x, hy, z].BlockType != GlobalConstants.BlockType.AIR && !_level[x, hy, z].IsLiquid
            && _level[x, y, lz].BlockType != GlobalConstants.BlockType.AIR && !_level[x, y, lz].IsLiquid 
            && _level[x, y, hz].BlockType != GlobalConstants.BlockType.AIR && !_level[x, y, hz].IsLiquid)
          {
            _level[x, y, z].SkipInstantiation = true;
          }
        }
      }
    }
  }

  /// <summary>
  /// Sets blocks of given cube are to air.
  /// </summary>
  /// <param name="areaStartX">Area start x.</param>
  /// <param name="areaEndX">Area end x.</param>
  /// <param name="areaStartY">Area start y.</param>
  /// <param name="areaEndY">Area end y.</param>
  /// <param name="areaStartZ">Area start z.</param>
  /// <param name="areaEndZ">Area end z.</param>
  protected void ClearArea(int areaStartX, int areaEndX, 
                           int areaStartY, int areaEndY, 
                           int areaStartZ, int areaEndZ)
  {
    areaStartX = Mathf.Clamp(areaStartX, 0, _mapX - 1);
    areaEndX = Mathf.Clamp(areaEndX, 0, _mapX - 1);
    areaStartY = Mathf.Clamp(areaStartY, 0, _mapY - 1);
    areaEndY = Mathf.Clamp(areaEndY, 0, _mapY - 1);
    areaStartZ = Mathf.Clamp(areaStartZ, 0, _mapZ - 1);
    areaEndZ = Mathf.Clamp(areaEndZ, 0, _mapZ - 1);

    for (int y = areaStartY; y < areaEndY; y++)
    {
      for (int x = areaStartX; x < areaEndX; x++)
      {
        for (int z = areaStartZ; z < areaEndZ; z++)
        {
          // Air blocks are checked first during instantiation and skipped.
          //
          // IsLiquid and SkipInstantiation flags are not touched. 
          // Set them accordingly when you change block type from air to other.

          _level[x, y, z].BlockType = GlobalConstants.BlockType.AIR;
        }
      }
    }

  }

  // ************************ WORLD GENERATION ************************ //

  // Should be odd
  int _maxHillsHeight = 15;
  protected void MakeHillLayered(int x, int y, int z, int height)
  {
    int lx = x - height;
    int lz = z - height;
    int hx = x + height;
    int hz = z + height;

    lx = Mathf.Clamp(lx, 0, _mapX - 1);
    lz = Mathf.Clamp(lz, 0, _mapZ - 1);
    hx = Mathf.Clamp(hx, 0, _mapX - 1);
    hz = Mathf.Clamp(hz, 0, _mapZ - 1);

    int hy = 0;

    int choice = Random.Range(0, 2);

    for (int h = 0; h < height; h++)
    {
      hy = h + y;
      hy = Mathf.Clamp(hy, 0, _mapY - 1);

      for (int ax = lx + h; ax <= hx - h; ax++)
      {
        for (int ay = lz + h; ay <= hz - h; ay++)
        {               
          _level[ax, hy, ay].BlockType = (choice == 0) ? GlobalConstants.BlockType.GRASS : GlobalConstants.BlockType.STONE;
        }
      }
    }
  }

  protected void MakeHillQbert(int x, int z, int height)
  {
    int lx = x - 1;
    int lz = z - 1;
    int hx = x + 1;
    int hz = z + 1;

    if (height < 0 || lx < 0 || lz < 0 || hx >= _mapX - 1 || hz >= _mapZ - 1)
    {
      return;
    }

    if (_level[x, height, z].BlockType == GlobalConstants.BlockType.AIR)
    {
      _level[x, height, z].BlockType = GlobalConstants.BlockType.GRASS;
    }

    if (_level[lx, height, z].BlockType == GlobalConstants.BlockType.AIR)
    {
      _level[lx, height, z].BlockType = GlobalConstants.BlockType.GRASS;
    }

    if (_level[x, height, lz].BlockType == GlobalConstants.BlockType.AIR)
    {
      _level[x, height, lz].BlockType = GlobalConstants.BlockType.GRASS;
    }

    if (_level[hx, height, z].BlockType == GlobalConstants.BlockType.AIR)
    {
      _level[hx, height, z].BlockType = GlobalConstants.BlockType.GRASS;
    }

    if (_level[x, height, hz].BlockType == GlobalConstants.BlockType.AIR)
    {
      _level[x, height, hz].BlockType = GlobalConstants.BlockType.GRASS;
    }

    MakeHillQbert(lx, z, height - 1);
    MakeHillQbert(hx, z, height - 1);
    MakeHillQbert(x, lz, height - 1);
    MakeHillQbert(x, hz, height - 1);
  }
}
