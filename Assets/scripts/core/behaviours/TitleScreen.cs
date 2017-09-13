using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;
using System;

public class TitleScreen : MonoBehaviour 
{
  public Transform SceneHolder;

  public GameObject Stairs;
    
  void Awake()
  {    
    RenderSettings.fog = true;
    RenderSettings.fogMode = FogMode.Linear;
    RenderSettings.fogColor = Color.black;
    RenderSettings.fogStartDistance = 0;
    RenderSettings.fogEndDistance = 18;

    InputController.Instance.CameraComponent.GetComponent<Skybox>().enabled = false;
    InputController.Instance.CameraComponent.backgroundColor = Color.black;

    GUIManager.Instance.TitleScreenButtonsHolder.SetActive(true);
  }

  void OnEnable()
  {
    SceneManager.sceneLoaded += SceneLoadedHandler;
  }

  void OnDisable()
  {
    SceneManager.sceneLoaded -= SceneLoadedHandler;
  }

  void SceneLoadedHandler(Scene scene, LoadSceneMode mode)
  {
    LevelLoader.Instance.LoadLevel(ScenesList.TITLE_SCREEN);

    GameData.Instance.PlayerMoveState = GameData.PlayerMoveStateEnum.HOLD_PLAYER;

    ScreenFader.Instance.FadeIn();

    BuildMap();
  }

  void BuildMap()
  {
    int blockChunkNumber = 0;

    GameObject blocksChunkHolder = null;
    GameObject staticObjectsChunkHolder = null;

    string blocksChunkName = string.Empty;
    string staticObjectsChunkName = string.Empty;

    List<GameObject> blockChunks = new List<GameObject>();
    List<GameObject> staticObjectsChunks = new List<GameObject>();

    int chunksNumber = LevelLoader.Instance.LevelMap.MapX / GlobalConstants.BlocksChunkSize;

    for (int cx = 0; cx < chunksNumber; cx++)
    {
      int currentChunkStartX = cx * GlobalConstants.BlocksChunkSize;

      for (int cy = 0; cy < chunksNumber; cy++)
      {
        int currentChunkStartZ = cy * GlobalConstants.BlocksChunkSize;

        blocksChunkName = string.Format("CHUNK_{0}", blockChunkNumber);
        staticObjectsChunkName = string.Format("OBJECTS_{0}", blockChunkNumber);

        blocksChunkHolder = new GameObject(blocksChunkName);
        staticObjectsChunkHolder = new GameObject(staticObjectsChunkName);

        for (int x = currentChunkStartX; x < currentChunkStartX + GlobalConstants.BlocksChunkSize; x++)
        {
          for (int z = currentChunkStartZ; z < currentChunkStartZ + GlobalConstants.BlocksChunkSize; z++)
          {
            for (int y = 0; y < LevelLoader.Instance.LevelMap.MapY; y++)
            {
              if (LevelLoader.Instance.LevelMap.Level[x, y, z].BlockType != GlobalConstants.BlockType.AIR 
                && !LevelLoader.Instance.LevelMap.Level[x, y, z].SkipInstantiation)
              {
                InstantiateBlock(LevelLoader.Instance.LevelMap.Level[x, y, z], blocksChunkHolder.transform);
              }

              if (LevelLoader.Instance.LevelMap.Level[x, y, z].WorldObjects.Count != 0)
              {
                InstantiateObjects(LevelLoader.Instance.LevelMap.Level[x, y, z]);
              }

              if (LevelLoader.Instance.LevelMap.Level[x, y, z].Teleporter != null)
              {
                InstantiateTeleporter(LevelLoader.Instance.LevelMap.Level[x, y, z]);
              }

              PlaceWalls(LevelLoader.Instance.LevelMap.Level[x, y, z], staticObjectsChunkHolder.transform);
            }
          }
        }

        if (blocksChunkHolder.transform.childCount != 0)
        {
          blockChunks.Add(blocksChunkHolder);
        }

        if (staticObjectsChunkHolder.transform.childCount != 0)
        {
          staticObjectsChunks.Add(staticObjectsChunkHolder);
        }

        blockChunkNumber++;
      }
    }

    foreach (var item in blockChunks)
    {      
      item.CombineMeshes();
    }

    foreach (var item in staticObjectsChunks)
    {
      item.CombineMeshes();
    }

    CleanupChunks(blockChunks);
    CleanupChunks(staticObjectsChunks);

    GC.Collect();

    Int3 cameraPos = new Int3(LevelLoader.Instance.LevelMap.PlayerPos.X, LevelLoader.Instance.LevelMap.PlayerPos.Y, LevelLoader.Instance.LevelMap.PlayerPos.Z);
    InputController.Instance.SetupCamera(cameraPos);
  }

  void CleanupChunks(List<GameObject> chunks)
  {
    foreach (var chunk in chunks)
    {
      foreach (Transform child in chunk.transform)
      {
        Destroy(child.gameObject);
      }
    }
  }

  Int3 _blockCoordinates = new Int3();
  void PlaceWalls(BlockEntity block, Transform parent)
  {
    _blockCoordinates.Set(block.ArrayCoordinates);

    // Instantiate and hide invisible sides (and duplicate columns)

    foreach (var obj in block.WallsByOrientation)
    {     
      if (obj.Value != null)
      {        
        // Shared walls have string.Empty as prefabName

        if (obj.Value.PrefabName != string.Empty)
        {
          Utils.CheckAndHideWallColumns(obj.Value, LevelLoader.Instance.LevelMap);

          GameObject prefab = PrefabsManager.Instance.FindPrefabByName(obj.Value.PrefabName);

          GameObject go = (GameObject)Instantiate(prefab, block.WorldCoordinates, Quaternion.identity);

          Vector3 eulerAngles = go.transform.eulerAngles;
          eulerAngles.y = GlobalConstants.OrientationAngles[obj.Value.ObjectOrientation];

          go.transform.eulerAngles = eulerAngles;
          go.transform.parent = parent; // ObjectsInstancesTransform.transform;

          BehaviourWorldObject bwo = go.GetComponent<BehaviourWorldObject>();
          bwo.WallColumnLeft.gameObject.SetActive(obj.Value.LeftColumnVisible);
          bwo.WallColumnRight.gameObject.SetActive(obj.Value.RightColumnVisible);
          bwo.WorldObjectInstance = obj.Value;

          obj.Value.BWO = bwo;

          Utils.HideWallSides(obj.Value, LevelLoader.Instance.LevelMap);
        }
      }
    }
  }

  void InstantiateTeleporter(BlockEntity block)
  {    
    GameObject prefab = PrefabsManager.Instance.FindPrefabByName(block.Teleporter.PrefabName);

    GameObject go = (GameObject)Instantiate(prefab, block.WorldCoordinates, Quaternion.identity);

    Vector3 eulerAngles = go.transform.eulerAngles;
    eulerAngles.y = GlobalConstants.OrientationAngles[block.Teleporter.ObjectOrientation];

    go.transform.eulerAngles = eulerAngles;
    go.transform.parent = SceneHolder.transform;

    BehaviourWorldObject bwo = go.GetComponent<BehaviourWorldObject>();
    bwo.AmbientSound.Play();
  }

  void InstantiateBlock(BlockEntity blockEntity, Transform parent)
  {
    GameObject prefab = PrefabsManager.Instance.FindPrefabByName(GlobalConstants.BlockPrefabByType[blockEntity.BlockType]);

    if (prefab != null)
    {
      GameObject block = (GameObject)Instantiate(prefab, blockEntity.WorldCoordinates, Quaternion.identity);
      block.transform.parent = parent; //ObjectsInstancesTransform.transform;
      MinecraftBlockAnimated blockAnimated = block.GetComponent<MinecraftBlockAnimated>();
      if (blockAnimated != null)
      {
        Utils.HideLevelBlockSides(blockAnimated, blockEntity.ArrayCoordinates, LevelLoader.Instance.LevelMap);
      }
      else
      {
        MinecraftBlock blockScript = block.GetComponent<MinecraftBlock>();
        Utils.HideLevelBlockSides(blockScript, blockEntity.ArrayCoordinates, LevelLoader.Instance.LevelMap);
      }
    }
  }

  void InstantiateObjects(BlockEntity blockEnity)
  {
    foreach (var worldObject in blockEnity.WorldObjects)    
    {
      GameObject prefab = PrefabsManager.Instance.FindPrefabByName(worldObject.PrefabName);

      if (prefab != null)
      {
        GameObject go = (GameObject)Instantiate(prefab, blockEnity.WorldCoordinates, Quaternion.identity);

        Vector3 eulerAngles = go.transform.eulerAngles;
        eulerAngles.y = GlobalConstants.OrientationAngles[worldObject.ObjectOrientation];

        go.transform.eulerAngles = eulerAngles;
        go.transform.parent = SceneHolder.transform;

        BehaviourWorldObject bwo = go.GetComponent<BehaviourWorldObject>();
        bwo.WorldObjectInstance = worldObject;

        worldObject.BWO = bwo;

        switch (worldObject.ObjectClass)
        {          
          case GlobalConstants.WorldObjectClass.DOOR_OPENABLE:
          case GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE:
            (worldObject as DoorWorldObject).InitBWO();
            break;

          case GlobalConstants.WorldObjectClass.LEVER:
            (worldObject as LeverWorldObject).InitBWO();
            break;

          case GlobalConstants.WorldObjectClass.BUTTON:
            (worldObject as ButtonWorldObject).InitBWO();
            break;

          case GlobalConstants.WorldObjectClass.SIGN:
            (worldObject as SignWorldObject).InitBWO();
            break;          

            /*
          case GlobalConstants.WorldObjectClass.WALL:
            Utils.HideWallSides((worldObject as WallWorldObject), LevelLoader.Instance.LevelMap);
            Utils.SetWallColumns((worldObject as WallWorldObject), LevelLoader.Instance.LevelMap);
            break;
            */

          case GlobalConstants.WorldObjectClass.SHRINE:
            (worldObject as ShrineWorldObject).InitBWO();
            break;          
        }
      }
    }
  }

  /*    
  void SetupScene()
  {    
    Vector3 cameraPos = Camera.main.transform.position;

    GameObject wallPrefab = PrefabsManager.Instance.FindPrefabByName(GlobalConstants.BlockPrefabByType[GlobalConstants.BlockType.STONE_BRICKS_OLD]);
    GameObject torchPrefab = PrefabsManager.Instance.FindPrefabByName(GlobalConstants.WorldObjectPrefabByType[GlobalConstants.WorldObjectPrefabType.TORCH]);

    // Walls
    
    var go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 2, 0, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 2, 0, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 2, 2, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 2, 2, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 4, 0, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 4, 0, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 4, 2, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 4, 2, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    // Torches
    
    go = (GameObject)Instantiate(torchPrefab, new Vector3(cameraPos.x - 2.0f, -0.125f, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;
    //go.transform.Rotate(Vector3.up, 180.0f, Space.World);
    
    go = (GameObject)Instantiate(torchPrefab, new Vector3(cameraPos.x + 2.0f, -0.125f, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;
    //go.transform.Rotate(Vector3.up, 180.0f, Space.World);
    
    // Floor
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 2, 0, cameraPos.z + 2), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x, 0, cameraPos.z + 2), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 2, 0, cameraPos.z + 2), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 4, 0, cameraPos.z + 2), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 4, 0, cameraPos.z + 2), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    // Stairs
    
    go = (GameObject)Instantiate(Stairs, new Vector3(cameraPos.x, 0, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;
    //go.transform.Rotate(Vector3.up, 180.0f, Space.World);

    go = (GameObject)Instantiate(Stairs, new Vector3(cameraPos.x, 4, cameraPos.z + 15), Quaternion.identity);
    go.transform.parent = SceneHolder;
    //go.transform.Rotate(Vector3.up, 180.0f, Space.World);
    
    // Stairs hall
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 1.8f, 0, cameraPos.z + 6), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 1.8f, 2, cameraPos.z + 6), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 1.8f, 0, cameraPos.z + 6), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 1.8f, 2, cameraPos.z + 6), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 1.8f, 0, cameraPos.z + 8), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 1.8f, 2, cameraPos.z + 8), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 1.8f, 0, cameraPos.z + 8), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 1.8f, 2, cameraPos.z + 8), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 1.8f, 0, cameraPos.z + 10), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 1.8f, 2, cameraPos.z + 10), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 1.8f, 0, cameraPos.z + 10), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 1.8f, 2, cameraPos.z + 10), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 1.8f, 4, cameraPos.z + 10), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 1.8f, 4, cameraPos.z + 10), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 1.8f, 2, cameraPos.z + 12), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 1.8f, 2, cameraPos.z + 12), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 1.8f, 4, cameraPos.z + 12), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 1.8f, 4, cameraPos.z + 12), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 1.8f, 2, cameraPos.z + 14), Quaternion.identity);
    go.transform.parent = SceneHolder;
        
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 1.8f, 2, cameraPos.z + 14), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x - 1.8f, 4, cameraPos.z + 14), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(wallPrefab, new Vector3(cameraPos.x + 1.8f, 4, cameraPos.z + 14), Quaternion.identity);
    go.transform.parent = SceneHolder;
  }
  */
}
