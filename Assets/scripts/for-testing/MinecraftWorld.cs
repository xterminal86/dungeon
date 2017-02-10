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

          GameObject prefab = PrefabsManager.Instance.FindPrefabByName(GlobalConstants.BlockPrefabByType[_world.Level[x, y, z].BlockType]);

          if (prefab != null)
          {
            GameObject block = (GameObject)Instantiate(prefab, _world.Level[x, y, z].WorldCoordinates, Quaternion.identity);

            block.transform.SetParent(WorldHolder);

            MinecraftBlockAnimated mba = block.GetComponent<MinecraftBlockAnimated>();

            if (mba != null)
            {
              Utils.HideLevelBlockSides(mba, _world.Level[x, y, z].ArrayCoordinates, _world);
            }
            else
            {
              MinecraftBlock mb = block.GetComponent<MinecraftBlock>();
              Utils.HideLevelBlockSides(mb, _world.Level[x, y, z].ArrayCoordinates, _world);
            }
          }
        }
      }
    }
  }
}
