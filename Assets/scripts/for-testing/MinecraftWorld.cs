using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinecraftWorld : MonoBehaviour 
{
  BlockEntity[,,] _world;

  int _worldSize = 10;
  	
	void Start () 
	{
    PrefabsManager.Instance.Initialize();

    _world = new BlockEntity[_worldSize, _worldSize, _worldSize];

    for (int y = 0; y < _worldSize; y++)
    {
      for (int x = 0; x < _worldSize; x++)
      {
        for (int z = 0; z < _worldSize; z++)
        {
          _world[x, y, z] = new BlockEntity();
          _world[x, y, z].BlockId = 0;
          _world[x, y, z].ArrayCoordinates.Set(x, y, z);
          _world[x, y, z].WorldCoordinates.Set(x * GlobalConstants.WallScaleFactor, y * GlobalConstants.WallScaleFactor, z * GlobalConstants.WallScaleFactor);
        }
      }
    }

    CreateWorld();
    InstantiateWorld();
	}

  void CreateWorld()
  {
    for (int x = 0; x < _worldSize; x++)
    {
      for (int z = 0; z < _worldSize; z++)
      {
        int id = Random.Range(1, 3);

        if (id == 2)
        {
          _world[x, 0, z].IsLiquid = true;
        }

        _world[x, 0, z].BlockId = id;
        _world[x, 0, z].WorldCoordinates.Set(x * GlobalConstants.WallScaleFactor, 0.0f, z * GlobalConstants.WallScaleFactor);

        id = Random.Range(1, 3);

        if (id == 2)
        {
          _world[x, 1, z].IsLiquid = true;
        }

        _world[x, 1, z].BlockId = id;
        _world[x, 1, z].WorldCoordinates.Set(x * GlobalConstants.WallScaleFactor, GlobalConstants.WallScaleFactor, z * GlobalConstants.WallScaleFactor);
      }
    }
  }

  void InstantiateWorld()
  { 
    for (int y = 0; y < _worldSize; y++)
    {
      for (int x = 0; x < _worldSize; x++)
      {
        for (int z = 0; z < _worldSize; z++)
        {
          if (_world[x, y, z].BlockId == 0)
          {
            continue;
          }

          float wx = _world[x, y, z].WorldCoordinates.x;
          float wy = _world[x, y, z].WorldCoordinates.y;
          float wz = _world[x, y, z].WorldCoordinates.z;

          if (y + 1 <= _worldSize - 1 && _world[x, y, z].IsLiquid && (!_world[x, y + 1, z].IsLiquid || _world[x, y + 1, z].BlockId == 0))
          {            
            _world[x, y, z].WorldCoordinates.Set(wx, wy - 0.3f, wz);
          }

          GameObject prefab = PrefabsManager.Instance.FindPrefabByName(GlobalConstants.BlockPrefabById[_world[x, y, z].BlockId]);

          if (prefab != null)
          {
            GameObject block = (GameObject)Instantiate(prefab,_world[x, y, z].WorldCoordinates, Quaternion.identity);

            MinecraftBlock blockScript = block.GetComponent<MinecraftBlock>();

            HideSides(blockScript, _world[x, y, z].ArrayCoordinates);
          }
        }
      }
    }
  }

  Vector3[] _arrayCoordinatesAdds = new Vector3[6] 
  { 
    new Vector3(-1.0f, 0.0f, 0.0f),
    new Vector3(1.0f, 0.0f, 0.0f),
    new Vector3(0.0f, -1.0f, 0.0f),
    new Vector3(0.0f, 1.0f, 0.0f),
    new Vector3(0.0f, 0.0f, -1.0f),
    new Vector3(0.0f, 0.0f, 1.0f),
  };

  void HideSides(MinecraftBlock block, Vector3 coordinates)
  {
    int x = (int)coordinates.x;
    int y = (int)coordinates.y;
    int z = (int)coordinates.z;

    for (int side = 0; side < 6; side++)
    { 
      int nx = x + (int)_arrayCoordinatesAdds[side][0];
      int ny = y + (int)_arrayCoordinatesAdds[side][1];
      int nz = z + (int)_arrayCoordinatesAdds[side][2];

      // Check for map size limits
      if (nx != x && nx >= 0 && nx <= _worldSize - 1)
      {
        // Hide corresponding side of the current block if:
        //
        // 1) Current block is solid and neighbouring block is solid
        // 2) Current block is liquid and neighbouring block is liquid
        // 3) Current block is liquid and neighbouring block is solid

        if ((!_world[x, y, z].IsLiquid && !_world[nx, y, z].IsLiquid)
          || (_world[x, y, z].IsLiquid &&  _world[nx, y, z].IsLiquid)
          || (_world[x, y, z].IsLiquid && !_world[nx, y, z].IsLiquid))
        {
          if ((int)_arrayCoordinatesAdds[side][0] == -1)
          {
            block.LeftQuad.gameObject.SetActive(false);
          }
          else if ((int)_arrayCoordinatesAdds[side][0] == 1)
          {
            block.RightQuad.gameObject.SetActive(false);
          }
        }
      }

      if (nz != z && nz >= 0 && nz <= _worldSize - 1)
      {
        if ((!_world[x, y, z].IsLiquid && !_world[x, y, nz].IsLiquid)
          || (_world[x, y, z].IsLiquid && _world[x, y, nz].IsLiquid)
          || (_world[x, y, z].IsLiquid && !_world[x, y, nz].IsLiquid))
        {
          if ((int)_arrayCoordinatesAdds[side][2] == -1)
          {
            block.ForwardQuad.gameObject.SetActive(false);
          }
          else if ((int)_arrayCoordinatesAdds[side][2] == 1)
          {
            block.BackQuad.gameObject.SetActive(false);
          }
        }
      }

      if (ny != y && ny >= 0 && ny <= _worldSize - 1)
      {
        if ((!_world[x, y, z].IsLiquid && !_world[x, ny, z].IsLiquid && _world[x, ny, z].BlockId != 0)
          || (_world[x, y, z].IsLiquid && _world[x, ny, z].IsLiquid)
          || (_world[x, y, z].IsLiquid && !_world[x, ny, z].IsLiquid && _world[x, ny, z].BlockId != 0))
        {
          if ((int)_arrayCoordinatesAdds[side][1] == -1)
          {
            block.DownQuad.gameObject.SetActive(false);
          }
          else if ((int)_arrayCoordinatesAdds[side][1] == 1)
          {
            block.UpQuad.gameObject.SetActive(false);
          }
        }
      }
    }

    /*
    int x = (int)coordinates.x;
    int y = (int)coordinates.y;
    int z = (int)coordinates.z;

    int hx = x + 1;
    int lx = x - 1;
    int hy = y + 1;
    int ly = y - 1;
    int hz = z + 1;
    int lz = z - 1;

    // Hide corresponding block quad if it is obstructed by another block
    // Don't do it if obstructing block is a liquid one since it's transparent.

    if (lx >= 0 && lx <= _worldSize - 1)
    {      
      if (_world[lx, y, z].BlockId != 0 && (_world[x, y, z].IsLiquid && (_world[lx, y, z].IsLiquid || !_world[lx, y, z].IsLiquid)))
      {
        block.LeftQuad.gameObject.SetActive(false);
      }
    }

    if (hx >= 0 && hx <= _worldSize - 1)
    {      
      if (_world[hx, y, z].BlockId != 0 && (_world[x, y, z].IsLiquid && (_world[hx, y, z].IsLiquid || !_world[hx, y, z].IsLiquid)))
      {
        block.RightQuad.gameObject.SetActive(false);
      }
    }

    if (lz >= 0 && lz <= _worldSize - 1)
    {
      if (_world[x, y, lz].BlockId != 0 && (_world[x, y, z].IsLiquid && (_world[x, y, lz].IsLiquid || !_world[x, y, lz].IsLiquid)))
      {
        block.ForwardQuad.gameObject.SetActive(false);
      }
    }

    if (hz >= 0 && hz <= _worldSize - 1)
    {
      if (_world[x, y, hz].BlockId != 0 && (_world[x, y, z].IsLiquid && (_world[x, y, hz].IsLiquid || !_world[x, y, hz].IsLiquid)))
      {
        block.BackQuad.gameObject.SetActive(false);
      }
    }

    if (ly >= 0 && ly <= _worldSize - 1)
    {      
      if (_world[x, ly, z].BlockId != 0 && (_world[x, y, z].IsLiquid && (_world[x, ly, z].IsLiquid || !_world[x, ly, z].IsLiquid)))
      {
        block.UpQuad.gameObject.SetActive(false);
      }
    }

    if (hy >= 0 && hy <= _worldSize - 1)
    {      
      if (_world[x, hy, z].BlockId != 0 && (_world[x, y, z].IsLiquid && (_world[x, hy, z].IsLiquid || !_world[x, hy, z].IsLiquid)))
      {
        block.DownQuad.gameObject.SetActive(false);
      }
    }
    */
  }
}
