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

          if (_world[x, y, z].IsLiquid)
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
        // Disable shadow receiving if submerged
        if (!_world[x, y, z].IsLiquid && _world[nx, y, z].IsLiquid)
        {
          if ((int)_arrayCoordinatesAdds[side][0] == -1)
          {
            block.LeftQuadRenderer.receiveShadows = false;
          }
          else if ((int)_arrayCoordinatesAdds[side][0] == 1)
          {
            block.RightQuadRenderer.receiveShadows = false;
          }
        }
        
        // Hide corresponding side of the current block if:
        //
        // 1) Current block is solid and neighbouring block is solid
        // 2) Current block is liquid and neighbouring block is liquid
        // 3) Current block is liquid and neighbouring block is solid

        if ((!_world[x, y, z].IsLiquid && !_world[nx, y, z].IsLiquid && _world[nx, y, z].BlockId != 0)
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
        // Disable shadow casting if submerged
        if (!_world[x, y, z].IsLiquid && _world[x, y, nz].IsLiquid)
        {
          if ((int)_arrayCoordinatesAdds[side][2] == -1)
          {
            block.ForwardQuadRenderer.receiveShadows = false;
          }
          else if ((int)_arrayCoordinatesAdds[side][2] == 1)
          {
            block.BackQuadRenderer.receiveShadows = false;
          }
        }

        if ((!_world[x, y, z].IsLiquid && !_world[x, y, nz].IsLiquid && _world[x, y, nz].BlockId !=0)
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
        // Disable shadow casting if submerged
        if (!_world[x, y, z].IsLiquid && _world[x, ny, z].IsLiquid)
        {
          if ((int)_arrayCoordinatesAdds[side][1] == -1)
          {
            block.DownQuadRenderer.receiveShadows = false;
          }
          else if ((int)_arrayCoordinatesAdds[side][1] == 1)
          {
            block.UpQuadRenderer.receiveShadows = false;
          }
        }

        if ((!_world[x, y, z].IsLiquid && !_world[x, ny, z].IsLiquid && _world[x, ny, z].BlockId != 0)
          || (_world[x, y, z].IsLiquid && _world[x, ny, z].IsLiquid))
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
  }
}
