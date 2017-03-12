using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

/// <summary>
/// Handles instantiation of game objects and assigning different logic to them.
/// </summary>
public class App : MonoBehaviour
{  
  public Terrain Mountains;

  public GameObject TestMob;
  public SunController Sun;

  public ParticleSystem Stars;

  public CloudsController CloudsControllerScript;

  [HideInInspector]
  public List<GameObject> Characters;

  public GameObject ObjectsInstancesTransform;

  [Header("Fog settings")]
  public bool EnableFog = true;
  public FogMode Type = FogMode.ExponentialSquared;
  public Color FogColor = Color.black;
  [Range(0.0f, 1.0f)]
  public float FogDensity = 0.2f;
  [Range(1.0f, 100.0f)]
  public float LinearFogWidth = 1.0f;

  float _fogDensity = 0.0f;

  void OnEnable()
  {
    SceneManager.sceneLoaded += SceneLoadedHandler;
  }

  void OnDisable()
  {
    SceneManager.sceneLoaded -= SceneLoadedHandler;
  }

  ParticleSystemRenderer _starsRenderer;
  void Start()
  {
    _starsRotation = Stars.transform.localEulerAngles;

    _starsRenderer = Stars.GetComponent<ParticleSystemRenderer>();
  }

  float _fogDensityDelta = 0.0f;
  void AdjustFogDensityForTime()
  {
    _fogDensityDelta = FogDensity / (GlobalConstants.DawnEndTime - GlobalConstants.DawnStartTime);

    int igt = DateAndTime.Instance.InGameTime;

    if (igt > GlobalConstants.DawnEndTime && igt < GlobalConstants.DuskStartTime)
    {
      _fogDensity = 0.0f;
    }
    else if (igt > GlobalConstants.DuskEndTime)
    {
      _fogDensity = FogDensity;
    }
    else if (igt >= GlobalConstants.DawnStartTime && igt <= GlobalConstants.DawnEndTime)
    {
      _fogDensity = FogDensity - _fogDensityDelta * igt;
    }
    else if (igt >= GlobalConstants.DuskStartTime && igt <= GlobalConstants.DuskEndTime)
    {
      int normalizedTime = igt - GlobalConstants.DuskStartTime;
      _fogDensity = _fogDensityDelta * normalizedTime;
    }
  }

  Color _fogColor = Color.black;
  void SceneLoadedHandler(Scene scene, LoadSceneMode mode)
  { 
    DateAndTime.Instance.InGameTime = GlobalConstants.DawnEndTime;

    AdjustFogDensityForTime();

    _fogColor = FogColor;

    UnityEngine.RenderSettings.fog = EnableFog;
    UnityEngine.RenderSettings.fogMode = Type;
    UnityEngine.RenderSettings.fogColor = _fogColor;
    UnityEngine.RenderSettings.fogDensity = _fogDensity;
    UnityEngine.RenderSettings.fogStartDistance = Camera.main.farClipPlane - LinearFogWidth * 2;
    UnityEngine.RenderSettings.fogEndDistance = Camera.main.farClipPlane - LinearFogWidth;

    Camera.main.backgroundColor = _fogColor;

    BuildMap();
    SpawnItems();

    // FIXME: clouds are not visible if far clip plane is quite less than 1000
    // CloudsControllerScript.Generate(80);

    ScreenFader.Instance.FadeIn();

    GUIManager.Instance.SetCompassVisibility(true);
    GUIManager.Instance.PlayerForm.ShowForm(true);

    GameData.Instance.PlayerMoveState = GameData.PlayerMoveStateEnum.NORMAL;    
    GameData.Instance.CurrentGameState = GameData.GameState.RUNNING;

    SoundManager.Instance.MapLoadingFinishedHandler();

    GUIManager.Instance.SetupGameForms();

    Vector3 starsPos = new Vector3(LevelLoader.Instance.LevelSize.X * GlobalConstants.WallScaleFactor, 
                                   LevelLoader.Instance.LevelSize.Y * GlobalConstants.WallScaleFactor, 
                                   LevelLoader.Instance.LevelSize.Z * GlobalConstants.WallScaleFactor);
    Stars.transform.localPosition = starsPos;
  }

  // TODO: In the future move all item names from items-db.xml somewhere
  void SpawnItems()
  {    
    var cc = GameData.Instance.PlayerCharacterVariable.GetCharacterClass;

    var io = SpawnItem(GlobalConstants.WorldItemType.PLACEHOLDER, "Scroll", false, "Personal Notes", GlobalConstants.PersonalNotesByClass[cc]);
    GUIManager.Instance.InventoryForm.AddItemToInventory(io);

    io = SpawnItem(GlobalConstants.WorldItemType.FOOD, "Bread", false, "Loaf of Bread", GlobalConstants.BreadDescription);
    GUIManager.Instance.InventoryForm.AddItemToInventory(io);

    io = SpawnItem(GlobalConstants.WorldItemType.WEAPON_MELEE, "Long Sword", false, "Iron Sword", GlobalConstants.LoremIpsum);
    GUIManager.Instance.InventoryForm.AddItemToInventory(io);

    io = SpawnItem(GlobalConstants.WorldItemType.ARMOR_CHEST, "Cloth Armor", false, "Green Cloth Armor", GlobalConstants.ClothArmorDescription);
    GUIManager.Instance.InventoryForm.AddItemToInventory(io);

    io = SpawnItem(GlobalConstants.WorldItemType.WEAPON_MELEE, "Battleaxe", false, "Battle Axe", GlobalConstants.AxeDescription);
    GUIManager.Instance.InventoryForm.AddItemToInventory(io);
  }
 
  /// <summary>
  /// Constructs a copy of the database item instance and 
  /// uses it to create unique instance of corresponding ItemObject.
  /// </summary>
  /// <returns>Unique instance of corresponding database object</returns>
  /// <param name="type">Database item type</param>
  /// <param name="databaseName">Database item name</param>
  /// <param name="showInWorld">If set to <c>true</c> show 3D model in the world.</param>
  /// <param name="itemName">Item name for description window</param>
  /// <param name="description">Item description for description window</param>
  ItemObject SpawnItem(GlobalConstants.WorldItemType type, string databaseName, bool showInWorld = true, string itemName = "", string description = "")
  {
    SerializableItem copy = null;
    var item = GameData.Instance.GetItem(type, databaseName);
    if (item != null)
    {      
      switch (type)
      {
        case GlobalConstants.WorldItemType.PLACEHOLDER:
          copy = new SerializableItem(item as SerializableItem);
          break;

        case GlobalConstants.WorldItemType.FOOD:
          copy = new SerializableFoodItem(item as SerializableFoodItem);
          break;

        case GlobalConstants.WorldItemType.WEAPON_MELEE:
          copy = new SerializableWeaponItem(item as SerializableWeaponItem);
          break;

        case GlobalConstants.WorldItemType.ARMOR_CHEST:
          copy = new SerializableArmorItem(item as SerializableArmorItem);
          break;

        default:
          break;
      }

      if (copy != null)
      {
        copy.ItemName = string.IsNullOrEmpty(itemName) ? databaseName : itemName;
        copy.ItemDescription = description;

        GameObject go = PrefabsManager.Instance.FindPrefabByName(copy.PrefabName);
        if (go != null)
        {
          GameObject inst = InstantiatePrefab(Int3.Zero, go);
          ItemObject io = CreateItemObject(inst, copy, showInWorld);
          return io;
        }
      }
    }

    return null;
  }

  int _prefabNameSuffix = 0;
  GameObject InstantiatePrefab(Int3 mapPosition, GameObject goToInstantiate, GlobalConstants.Orientation orientation = GlobalConstants.Orientation.EAST)
  {
    GameObject go = (GameObject)Instantiate(goToInstantiate);

    go.name = string.Format("{0}.{1}", go.name, _prefabNameSuffix);

    _prefabNameSuffix++;

    // If transform has no parent, position is the same as localPosition

    Vector3 position = go.transform.position;
    position.x = mapPosition.X * GlobalConstants.WallScaleFactor;
    position.y = mapPosition.Y * GlobalConstants.WallScaleFactor;
    position.z = mapPosition.Z * GlobalConstants.WallScaleFactor;

    Vector3 eulerAngles = go.transform.eulerAngles;
    eulerAngles.y = GlobalConstants.OrientationAngles[orientation];

    go.transform.position = position;
    go.transform.eulerAngles = eulerAngles;

    return go;
  }

  /// <summary>
  /// Attaches specific ItemObject to prefab (BehaviourItemObject)
  /// </summary>
  ItemObject CreateItemObject(GameObject go, SerializableItem item, bool showInWorld = true)
  {    
    BehaviourItemObject bio = go.GetComponent<BehaviourItemObject>();
    if (bio == null)
    {
      Debug.LogWarning("Could not get BIO component from " + item.ItemName);
      return null;
    }   

    switch (item.ItemType)
    {
      case GlobalConstants.WorldItemType.PLACEHOLDER:
        bio.ItemObjectInstance = new PlaceholderItemObject(item.ItemName, item.ItemDescription, item.AtlasIconIndex, bio);
        break;

      case GlobalConstants.WorldItemType.FOOD:
        var sfi = item as SerializableFoodItem;
        bio.ItemObjectInstance = new FoodItemObject(item.ItemName, item.ItemDescription, item.AtlasIconIndex, bio, sfi.Saturation);
        break;

      case GlobalConstants.WorldItemType.WEAPON_MELEE:
        var swi = item as SerializableWeaponItem;
        bio.ItemObjectInstance = new WeaponItemObject(item.ItemName, item.ItemDescription, item.AtlasIconIndex, bio,
          swi.MinimumDamage, swi.MaximumDamage, swi.Cooldown);
        break;

      case GlobalConstants.WorldItemType.ARMOR_CHEST:
        var sai = item as SerializableArmorItem;
        bio.ItemObjectInstance = new ArmorItemObject(item.ItemName, item.ItemDescription, item.AtlasIconIndex, bio, sai.ArmorClassModifier);          
        break;

      default:
        break;
    }

    if (bio.ItemObjectInstance != null)
    {
      bio.ItemObjectInstance.ItemType = item.ItemType;
      bio.ItemObjectInstance.LMBAction += bio.ItemObjectInstance.LMBHandler;
      bio.ItemObjectInstance.RMBAction += bio.ItemObjectInstance.RMBHandler;
    }

    bio.gameObject.SetActive(showInWorld);

    return bio.ItemObjectInstance;
  }

  void SetupMobs()
  {
    var go = (GameObject)Instantiate(TestMob, Vector3.zero, Quaternion.identity);
    var mm = go.GetComponent<ModelMover>();
    if (mm != null)
    {
      if (!SoundManager.Instance.LastPlayedSoundOfChar.ContainsKey(go.name.GetHashCode())) 
      {
        SoundManager.Instance.LastPlayedSoundOfChar.Add(go.name.GetHashCode(), 0);
      }
      
      mm.Actor = new EnemyActor(mm);
    }
  }
    
  int _terrainAddedWidth = 10;
  void MakeMountains()
  {    
    Vector3 terrainSize = new Vector3((LevelLoader.Instance.LevelSize.X + _terrainAddedWidth) * GlobalConstants.WallScaleFactor, 
                                      GlobalConstants.DefaultVillageMountainsSize.y,
                                      (LevelLoader.Instance.LevelSize.Z + _terrainAddedWidth) * GlobalConstants.WallScaleFactor);
    
    Mountains.terrainData.size = terrainSize;

    Vector3 terrainPosition = new Vector3(-1 - _terrainAddedWidth, 0, -terrainSize.z - 1);
    Instantiate(Mountains, terrainPosition, Quaternion.identity);
    
    terrainPosition = new Vector3(-terrainSize.x - 1, 0, -1 - _terrainAddedWidth);
    Instantiate(Mountains, terrainPosition, Quaternion.identity);
    
    terrainPosition = new Vector3(-1 - _terrainAddedWidth, 0, terrainSize.z - (_terrainAddedWidth * 2 + 1));
    Instantiate(Mountains, terrainPosition, Quaternion.identity);
    
    terrainPosition = new Vector3(terrainSize.x - (_terrainAddedWidth * 2 + 1), 0, -1);
    Instantiate(Mountains, terrainPosition, Quaternion.identity);

    terrainPosition = new Vector3(terrainSize.x - (_terrainAddedWidth + 1), 0, -terrainSize.z);
    Instantiate(Mountains, terrainPosition, Quaternion.identity);    
  }

  void BuildMap()
  {
    for (int y = 0; y < LevelLoader.Instance.LevelMap.MapY; y++)
    {
      for (int x = 0; x < LevelLoader.Instance.LevelMap.MapX; x++)
      {
        for (int z = 0; z < LevelLoader.Instance.LevelMap.MapZ; z++)
        {
          if (LevelLoader.Instance.LevelMap.Level[x, y, z].BlockType != GlobalConstants.BlockType.AIR && !LevelLoader.Instance.LevelMap.Level[x, y, z].SkipInstantiation)
          {
            InstantiateBlock(LevelLoader.Instance.LevelMap.Level[x, y, z]);
          }

          if (LevelLoader.Instance.LevelMap.Level[x, y, z].WorldObjects.Count != 0)
          {
            InstantiateObject(LevelLoader.Instance.LevelMap.Level[x, y, z]);
          }
        }
      }
    }

    Int3 cameraPos = new Int3(LevelLoader.Instance.LevelMap.PlayerPos.X, LevelLoader.Instance.LevelMap.PlayerPos.Y, LevelLoader.Instance.LevelMap.PlayerPos.Z);
    InputController.Instance.SetupCamera(cameraPos);
  }

  GameObject FindCharacterPrefabByName(string name)
  {
    foreach (var item in Characters)
    {
      if (item.name == name)
      {
        return item;
      }
    }

    return null;
  }

  void SpawnCharacter(GameObject model, Int2 pos, string actorClass)
  {
    GameObject go = (GameObject)Instantiate(model, 
                                            new Vector3(pos.X * GlobalConstants.WallScaleFactor, 0, pos.Y * GlobalConstants.WallScaleFactor), 
                                            Quaternion.identity);
    switch (actorClass)
    {
      case GlobalConstants.ActorVillagerClass:
        var mm = go.GetComponent<ModelMover>();
        if (mm != null)
        {
          if (!SoundManager.Instance.LastPlayedSoundOfChar.ContainsKey(go.name.GetHashCode()))
          {
            SoundManager.Instance.LastPlayedSoundOfChar.Add(go.name.GetHashCode(), 0);
          }

          mm.ModelPos.X = pos.X;
          mm.ModelPos.Y = pos.Y;

          mm.Actor = new VillagerActor(mm);
          mm.Actor.ActorName = mm.ActorName;
        }
        break;

      default:
        break;
    }
  }

  // ********************** HELPER FUNCTIONS ********************** //

  void GameOverHandler()
  {
    JobManager.Instance.StartCoroutine(DelayRoutine());       
  }

  IEnumerator DelayRoutine()
  {  
    float timer = 0.0f;

    while (timer < 3.0f)
    {
      timer += Time.smoothDeltaTime;

      yield return null;
    }

    ScreenFader.Instance.FadeIn(() => { SceneManager.LoadScene("title"); });
  }

  void InstantiateBlock(BlockEntity blockEntity)
  {
    GameObject prefab = PrefabsManager.Instance.FindPrefabByName(GlobalConstants.BlockPrefabByType[blockEntity.BlockType]);

    if (prefab != null)
    {
      GameObject block = (GameObject)Instantiate(prefab, blockEntity.WorldCoordinates, Quaternion.identity);
      block.transform.parent = ObjectsInstancesTransform.transform;
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

  void InstantiateObject(BlockEntity blockEnity)
  {
    foreach (var item in blockEnity.WorldObjects)    
    {
      GameObject prefab = PrefabsManager.Instance.FindPrefabByName(item.PrefabName);

      if (prefab != null)
      {
        GameObject go = (GameObject)Instantiate(prefab, blockEnity.WorldCoordinates, Quaternion.identity);

        Vector3 eulerAngles = go.transform.eulerAngles;
        eulerAngles.y = GlobalConstants.OrientationAngles[item.ObjectOrientation];

        go.transform.eulerAngles = eulerAngles;
        go.transform.parent = ObjectsInstancesTransform.transform;

        BehaviourWorldObject bwo = go.GetComponent<BehaviourWorldObject>();
        bwo.WorldObjectInstance = item;

        item.BWO = bwo;

        switch (item.ObjectClass)
        {          
          case GlobalConstants.WorldObjectClass.DOOR_OPENABLE:
          case GlobalConstants.WorldObjectClass.DOOR_CONTROLLABLE:
            (item as DoorWorldObject).InitBWO();
            break;

          case GlobalConstants.WorldObjectClass.LEVER:
            (item as LeverWorldObject).InitBWO();
            break;

          case GlobalConstants.WorldObjectClass.BUTTON:
            (item as ButtonWorldObject).InitBWO();
            break;

          case GlobalConstants.WorldObjectClass.SIGN:
            (item as SignWorldObject).InitBWO();
            break;
        }
      }
    }
  }

  Color _starsColor = Color.white;
  Vector3 _starsRotation = Vector3.zero;
  void Update()
  {
    if (DateAndTime.Instance.WasTick)
    {
      if (DateAndTime.Instance.InGameTime >= GlobalConstants.DuskStartTime && DateAndTime.Instance.InGameTime <= GlobalConstants.DuskEndTime)
      {
        _starsColor.r += Sun.DayNightFadeDelta;
        _starsColor.g += Sun.DayNightFadeDelta;
        _starsColor.b += Sun.DayNightFadeDelta;
        _starsColor.a += Sun.DayNightFadeDelta;

        _fogDensity += _fogDensityDelta;

        _fogColor.r -= Sun.DayNightFadeDelta;
        _fogColor.g -= Sun.DayNightFadeDelta;
        _fogColor.b -= Sun.DayNightFadeDelta;
      }
      else if (DateAndTime.Instance.InGameTime >= GlobalConstants.DawnStartTime && DateAndTime.Instance.InGameTime <= GlobalConstants.DawnEndTime)
      {
        _starsColor.r -= Sun.DayNightFadeDelta;
        _starsColor.g -= Sun.DayNightFadeDelta;
        _starsColor.b -= Sun.DayNightFadeDelta;
        _starsColor.a -= Sun.DayNightFadeDelta;

        _fogDensity -= _fogDensityDelta;

        _fogColor.r += Sun.DayNightFadeDelta;
        _fogColor.g += Sun.DayNightFadeDelta;
        _fogColor.b += Sun.DayNightFadeDelta;
      }

      //_starsRotation.x += (Sun.SunMoveDelta / 4.0f);
      //_starsRotation.y += (Sun.SunMoveDelta / 4.0f);
      _starsRotation.z += (Sun.SunMoveDelta / 4.0f);

      Stars.transform.localEulerAngles = _starsRotation;
    }

    _fogColor.r = Mathf.Clamp(_fogColor.r, 0.0f, FogColor.r);
    _fogColor.g = Mathf.Clamp(_fogColor.g, 0.0f, FogColor.g);
    _fogColor.b = Mathf.Clamp(_fogColor.b, 0.0f, FogColor.b);

    _fogDensity = Mathf.Clamp(_fogDensity, 0.0f, FogDensity);

    _starsColor.r = Mathf.Clamp(_starsColor.r, 0.0f, 1.0f);
    _starsColor.g = Mathf.Clamp(_starsColor.g, 0.0f, 1.0f);
    _starsColor.b = Mathf.Clamp(_starsColor.b, 0.0f, 1.0f);
    _starsColor.a = Mathf.Clamp(_starsColor.a, 0.0f, 1.0f);

    _starsRenderer.material.SetColor("_TintColor", _starsColor);
    //var psMain = Stars.main;
    //psMain.startColor = _starsColor;

    UnityEngine.RenderSettings.fogDensity = _fogDensity;

    //UnityEngine.RenderSettings.fogColor = _fogColor;

    if (DateAndTime.Instance.DaytimeString == "Dusk" && !Stars.gameObject.activeSelf)
    {
      Stars.gameObject.SetActive(true);
    }
    else if (DateAndTime.Instance.DaytimeString == "Daytime" && Stars.gameObject.activeSelf)
    {
      Stars.gameObject.SetActive(false);
    }
  }
}
