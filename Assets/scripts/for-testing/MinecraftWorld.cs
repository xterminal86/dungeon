using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinecraftWorld : MonoBehaviour 
{
  BlockEntity[,,] _world;

  int _worldSize = 50;

	// Use this for initialization
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
          _world[x, y, z].Coordinates.Set(x * GlobalConstants.WallScaleFactor, y * GlobalConstants.WallScaleFactor, z * GlobalConstants.WallScaleFactor);
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

        _world[x, 0, z].BlockId = id;
        _world[x, 0, z].Coordinates.Set(x * GlobalConstants.WallScaleFactor, 0.0f, z * GlobalConstants.WallScaleFactor);
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

          GameObject prefab = PrefabsManager.Instance.FindPrefabByName(GlobalConstants.BlockPrefabById[_world[x, y, z].BlockId]);

          if (prefab != null)
          {
            GameObject block = (GameObject)Instantiate(prefab, new Vector3(x * GlobalConstants.WallScaleFactor, y * GlobalConstants.WallScaleFactor, z * GlobalConstants.WallScaleFactor), Quaternion.identity);

            MinecraftBlock blockScript = block.GetComponent<MinecraftBlock>();

            HideSides(blockScript, _world[x, y, z].Coordinates);
          }
        }
      }
    }
  }

  void HideSides(MinecraftBlock block, Vector3 coordinates)
  {
    int x = (int)coordinates.x;
    int y = (int)coordinates.y;
    int z = (int)coordinates.z;

    int hx = x + 1;
    int lx = x - 1;
    int hy = y + 1;
    int ly = y - 1;
    int hz = z + 1;
    int lz = z - 1;

    if (lx >= 0 && hx <= _worldSize - 1)
    {
      if (_world[lx, y, z].BlockId != 0)
      {
        block.LeftQuad.gameObject.SetActive(false);
      }

      if (_world[hx, y, z].BlockId != 0)
      {
        block.RightQuad.gameObject.SetActive(false);
      }
    }

    if (lz >= 0 && hz <= _worldSize - 1)
    {
      if (_world[x, y, lz].BlockId != 0)
      {
        block.ForwardQuad.gameObject.SetActive(false);
      }

      if (_world[x, y, hz].BlockId != 0)
      {
        block.BackQuad.gameObject.SetActive(false);
      }
    }
  }

	// Update is called once per frame
	void Update () 
	{
		
	}
}
