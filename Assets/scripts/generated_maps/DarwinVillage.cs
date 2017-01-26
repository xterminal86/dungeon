using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarwinVillage
{
  BlockEntity[,,] _level;
  public BlockEntity[,,] Level
  {
    get { return _level; }
  }

  int _mapX, _mapY, _mapZ;

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

  // Position of camera (might be starting pos and saved for subsequent loading)
  Vector3 _cameraPos = Vector3.zero;
  public Vector3 CameraPos
  {
    get { return _cameraPos; }
  }

  public DarwinVillage(int x, int y, int z)
  {    
    _mapX = x;
    _mapY = y;
    _mapZ = z;

    _level = new BlockEntity[x, y, z];

    InitArray();
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

  public void Generate()
  {
    int posY = _mapY / 2;

    _cameraPos.Set(0, posY + 1, 0);

    for (int x = 0; x < _mapX; x++)
    {
      for (int z = 0; z < _mapZ; z++)
      {
        _level[x, posY, z].BlockType = GlobalConstants.BlockType.GRASS;
        _level[x, posY, z].WorldCoordinates.Set(x * GlobalConstants.WallScaleFactor, 
                                             posY * GlobalConstants.WallScaleFactor, 
                                                z * GlobalConstants.WallScaleFactor);
        
        _level[x, posY, z].FootstepSound = GlobalConstants.FootstepSoundType.GRASS;
      }
    }
  }
}

