using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinecraftWorld : MonoBehaviour 
{
  public Transform WorldHolder;

  LevelBase _world;

  int _worldSize = 60;
  	
	void Start() 
	{
    PrefabsManager.Instance.Initialize();

    _world = new TestLevel(_worldSize, _worldSize, _worldSize);

    _world.Generate();

    InstantiateWorld();
	}

  void InstantiateWorld()
  { 
    for (int y = 0; y < _worldSize; y++)
    {
      for (int x = 0; x < _worldSize; x++)
      {
        for (int z = 0; z < _worldSize; z++)
        {
          if (_world.Level[x, y, z].BlockType == GlobalConstants.BlockType.AIR || _world.Level[x, y, z].SkipInstantiation)
          {
            continue;
          }

          float wx = _world.Level[x, y, z].WorldCoordinates.x;
          float wy = _world.Level[x, y, z].WorldCoordinates.y;
          float wz = _world.Level[x, y, z].WorldCoordinates.z;

          if (_world.Level[x, y, z].IsLiquid)
          {
            _world.Level[x, y, z].WorldCoordinates.Set(wx, wy - 0.3f, wz);
          }

          GameObject prefab = PrefabsManager.Instance.FindPrefabByName(GlobalConstants.BlockPrefabById[_world.Level[x, y, z].BlockType]);

          if (prefab != null)
          {
            GameObject block = (GameObject)Instantiate(prefab, _world.Level[x, y, z].WorldCoordinates, Quaternion.identity);

            block.transform.SetParent(WorldHolder);

            MinecraftBlock blockScript = block.GetComponent<MinecraftBlock>();

            Utils.HideLevelBlockSides(blockScript, _world.Level[x, y, z].ArrayCoordinates, _world);
          }
        }
      }
    }
  }
}
