using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinecraftWorld : MonoBehaviour 
{
  public Transform WorldHolder;

  BlockEntity[,,] _world;

  int _worldSize = 60;
  	
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
          _world[x, y, z].BlockType = GlobalConstants.BlockType.AIR;
          _world[x, y, z].ArrayCoordinates.Set(x, y, z);
          _world[x, y, z].WorldCoordinates.Set(x * GlobalConstants.WallScaleFactor, y * GlobalConstants.WallScaleFactor, z * GlobalConstants.WallScaleFactor);
        }
      }
    }

    CreateHills();
    //CreateWorld();
    InstantiateWorld();
	}

  // Should be odd
  int _maxHillsHeight = 21;
  void CreateHills()
  {    
    int[] heights = new int[(_maxHillsHeight - 1) / 2 + 1];

    int h = 1;
    for (int i = 0; i < heights.Length; i++)
    {
      heights[i] = h;
      h += 2;
    }

    for (int i = 0; i < 20; i++)
    {
      int ind = Random.Range(0, heights.Length);

      h = heights[ind];

      int x = Random.Range(10, _worldSize - 10);
      int z = Random.Range(10, _worldSize - 10);

      MakeHillLayered(x, z, h);

      /*
      int choice = Random.Range(0, 2);

      if (choice == 0)
      {
        MakeHillQbert(x, z, h);
      }
      else
      {
        MakeHillLayered(x, z, h);
      }
      */
    }

    DiscardHiddenBlocks(1, _worldSize - 1, 1, _worldSize - 1);
  }

  void MakeHillQbert(int x, int y, int height)
  {
    int lx = x - 1;
    int ly = y - 1;
    int hx = x + 1;
    int hy = y + 1;

    if (height < 0 || lx < 0 || ly < 0 || hx >= _worldSize - 1 || hy >= _worldSize - 1)
    {
      return;
    }

    if (_world[x, height, y].BlockType == GlobalConstants.BlockType.AIR)
    {
      _world[x, height, y].BlockType = GlobalConstants.BlockType.GRASS;
    }
      
    if (_world[lx, height, y].BlockType == GlobalConstants.BlockType.AIR)
    {
      _world[lx, height, y].BlockType = GlobalConstants.BlockType.GRASS;
    }

    if (_world[x, height, ly].BlockType == GlobalConstants.BlockType.AIR)
    {
      _world[x, height, ly].BlockType = GlobalConstants.BlockType.GRASS;
    }

    if (_world[hx, height, y].BlockType == GlobalConstants.BlockType.AIR)
    {
      _world[hx, height, y].BlockType = GlobalConstants.BlockType.GRASS;
    }

    if (_world[x, height, hy].BlockType == GlobalConstants.BlockType.AIR)
    {
      _world[x, height, hy].BlockType = GlobalConstants.BlockType.GRASS;
    }

    MakeHillQbert(lx, y, height - 1);
    MakeHillQbert(hx, y, height - 1);
    MakeHillQbert(x, ly, height - 1);
    MakeHillQbert(x, hy, height - 1);
  }

  void MakeHillLayered(int x, int y, int height)
  {
    int lx = x - height;
    int ly = y - height;
    int hx = x + height;
    int hy = y + height;

    lx = Mathf.Clamp(lx, 0, _worldSize - 1);
    ly = Mathf.Clamp(ly, 0, _worldSize - 1);
    hx = Mathf.Clamp(hx, 0, _worldSize - 1);
    hy = Mathf.Clamp(hy, 0, _worldSize - 1);

    for (int h = 0; h < height; h++)
    {
      for (int ax = lx + h; ax <= hx - h; ax++)
      {
        for (int ay = ly + h; ay <= hy - h; ay++)
        {
          _world[ax, h, ay].BlockType = GlobalConstants.BlockType.GRASS;

          /*
          if (_world[ax, h, ay].BlockId == 0)
          {
            if (h > 5 && h <= 10)
            {
              _world[ax, h, ay].BlockId = 4;
            }
            else if (h > 10)
            {
              _world[ax, h, ay].BlockId = 3;
            }
            else
            {
              _world[ax, h, ay].BlockId = 1;
            }
          }
          */
        }
      }
    }
  }

  void CreateWorld()
  {
    for (int x = 0; x < _worldSize; x++)
    {
      for (int z = 0; z < _worldSize; z++)
      {
        int id = Random.Range(1, GlobalConstants.BlockPrefabById.Count);

        if (id == 2)
        {
          _world[x, 0, z].IsLiquid = true;
        }

        _world[x, 0, z].BlockType = (GlobalConstants.BlockType)id;
        _world[x, 0, z].WorldCoordinates.Set(x * GlobalConstants.WallScaleFactor, 0.0f, z * GlobalConstants.WallScaleFactor);

        id = Random.Range(1, 3);

        if (id == 2)
        {
          _world[x, 1, z].IsLiquid = true;
        }

        _world[x, 1, z].BlockType = (GlobalConstants.BlockType)id;
        _world[x, 1, z].WorldCoordinates.Set(x * GlobalConstants.WallScaleFactor, GlobalConstants.WallScaleFactor, z * GlobalConstants.WallScaleFactor);
      }
    }
  }

  /// <summary>
  /// Sets no-instantiate flag on blocks that are surrounded on all 6 sides.
  /// Works for all map height on a given area.
  /// </summary>
  /// <param name="areaStartX">Area start x.</param>
  /// <param name="areaEndX">Area end x.</param>
  /// <param name="areaStartZ">Area start z.</param>
  /// <param name="areaEndZ">Area end z.</param>
  void DiscardHiddenBlocks(int areaStartX, int areaEndX, int areaStartZ, int areaEndZ)
  {
    areaStartX = Mathf.Clamp(areaStartX, 0, _worldSize - 1);
    areaEndX = Mathf.Clamp(areaEndX, 0, _worldSize - 1);
    areaStartZ = Mathf.Clamp(areaStartZ, 0, _worldSize - 1);
    areaEndZ = Mathf.Clamp(areaEndZ, 0, _worldSize - 1);

    int lx, ly, lz, hx, hy, hz = 0;

    for (int y = 1; y < _worldSize - 1; y++)
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
          if (_world[x, y, z].BlockType == GlobalConstants.BlockType.AIR)
          {
            continue;
          }

          lz = z - 1;
          hz = z + 1;

          // We cannot replace BlockId directly, since then on next loop iteration
          // the condition will fail.
          if (_world[lx, y, z].BlockType != GlobalConstants.BlockType.AIR && !_world[lx, y, z].IsLiquid 
           && _world[hx, y, z].BlockType != GlobalConstants.BlockType.AIR && !_world[hx, y, z].IsLiquid
           && _world[x, ly, z].BlockType != GlobalConstants.BlockType.AIR && !_world[x, ly, z].IsLiquid
           && _world[x, hy, z].BlockType != GlobalConstants.BlockType.AIR && !_world[x, hy, z].IsLiquid
           && _world[x, y, lz].BlockType != GlobalConstants.BlockType.AIR && !_world[x, y, lz].IsLiquid 
           && _world[x, y, hz].BlockType != GlobalConstants.BlockType.AIR && !_world[x, y, hz].IsLiquid)
          {
            _world[x, y, z].SkipInstantiation = true;
          }
        }
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
          if (_world[x, y, z].BlockType == GlobalConstants.BlockType.AIR || _world[x, y, z].SkipInstantiation)
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

          GameObject prefab = PrefabsManager.Instance.FindPrefabByName(GlobalConstants.BlockPrefabById[_world[x, y, z].BlockType]);

          if (prefab != null)
          {
            GameObject block = (GameObject)Instantiate(prefab, _world[x, y, z].WorldCoordinates, Quaternion.identity);

            block.transform.SetParent(WorldHolder);

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

        if ((!_world[x, y, z].IsLiquid && !_world[nx, y, z].IsLiquid && _world[nx, y, z].BlockType != GlobalConstants.BlockType.AIR)
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

        if ((!_world[x, y, z].IsLiquid && !_world[x, y, nz].IsLiquid && _world[x, y, nz].BlockType != GlobalConstants.BlockType.AIR)
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

        if ((!_world[x, y, z].IsLiquid && !_world[x, ny, z].IsLiquid && _world[x, ny, z].BlockType != GlobalConstants.BlockType.AIR)
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
