using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

public class App : MonoBehaviour
{  
  public Terrain Mountains;

  public InputController InputControllerScript;

  public GameObject TestMob;
  public SunController Sun;

  public ParticleSystem Stars;

  public CloudsController CloudsControllerScript;

  [HideInInspector]
  public List<GameObject> Characters;

  public GameObject ObjectsInstancesTransform;
      
  int[,] _floorSoundTypeByPosition;
  public int[,] FloorSoundTypeByPosition
  {
    get { return _floorSoundTypeByPosition; }
  }

  int _nameSuffix = 0;

  char[,] _mapLayout;
  List<string> _map = new List<string>();

  Dictionary<int, GameObject> _mapObjectsHashTable = new Dictionary<int, GameObject>();
  Dictionary<Vector2, List<int>> _mapObjectsHashListByPosition = new Dictionary<Vector2, List<int>>();

  Vector3 _cameraPos = Vector3.zero;
  public Vector3 CameraPos
  {
    set { _cameraPos = value; }
    get { return _cameraPos; }
  }

  [HideInInspector]
  public int CameraOrientation = -1;

  Vector3 _cameraAngles = Vector3.zero;
  public Vector3 CameraAngles
  {
    get { return _cameraAngles; }
  }

  public GameObject CameraPivot;

  public MapFilename MapFilenameField;

  [Header("Fog settings")]
  public bool EnableFog = true;
  public FogMode Type = FogMode.ExponentialSquared;
  public Color FogColor = Color.black;
  [Range(0.0f, 1.0f)]
  public float FogDensity = 0.2f;
  [Range(1.0f, 100.0f)]
  public float LinearFogWidth = 1.0f;

  float _fogDensity = 0.0f;

  int _generatedMapWidth = 50, _generatedMapHeight = 50;
  public int GeneratedMapWidth
  {
    get { return _generatedMapWidth; }
  }

  public int GeneratedMapHeight
  {
    get { return _generatedMapHeight; }
  }

  GeneratedMap _generatedMap;
  public GeneratedMap GeneratedMap
  {
    get { return _generatedMap; }
  }

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

  LevelBase _newLevelClass;
  public LevelBase NewLevelClass
  {
    get { return _newLevelClass; }
  }

  Vector3 _villageLevelSize = new Vector3(80, 40, 80);

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

    _cameraPos = CameraPivot.transform.position;

    switch (MapFilenameField)
    {
      case MapFilename.ROOMS:
        //LoadMap("Assets/maps/rooms_test_map.xml");
        LoadMapBinary("SerializedMaps/Rooms.map");
        break;
      case MapFilename.ROOMS_CONNECTED:
        LoadMap("Assets/maps/rooms_test_map2.xml");
        break;
      case MapFilename.TEST:
        LoadMap("Assets/maps/test_map.xml");
        break;
      case MapFilename.BINARY_TREE:
        //LoadMap("Assets/maps/binary_tree_map.xml");
        LoadMapBinary("SerializedMaps/BinaryTree.map");
        break;
      case MapFilename.SIDEWINDER:
        LoadMap("Assets/maps/sidewinder_map.xml");
        break;
      case MapFilename.GROWING_TREE:
        LoadMap("Assets/maps/growing_tree_test_map.xml");
        break;
      case MapFilename.TOWN:
        LoadMap("Assets/maps/town.xml");
        break;
      case MapFilename.DUNGEON1:
        LoadMap("Assets/maps/dungeon1.xml");
        break;
      case MapFilename.GEN_VILLAGE:
        _generatedMap = new Village(_generatedMapWidth, _generatedMapHeight);
        break;
      case MapFilename.DARWIN_VILLAGE:
        _newLevelClass = new DarwinVillage((int)_villageLevelSize.x, 
                                           (int)_villageLevelSize.y, 
                                           (int)_villageLevelSize.z);
        _newLevelClass.Generate();
        BuildMapNew();
        CloudsControllerScript.Generate(_villageLevelSize.y * GlobalConstants.WallScaleFactor);
        break;
      default:
        LoadMap("Assets/maps/test_map.xml");
        break;
    }

    if (_generatedMap != null)
    {
      _generatedMap.Generate();
      BuildMap();
      SpawnItems();

      if (MapFilenameField == MapFilename.GEN_VILLAGE)
      {
        MakeMountains();
        SetupCharacters();
        SetupMobs();
      }
    }

    /*
    string output = string.Empty;
    for (int x = 0; x < GeneratedMapWidth; x++)
    {
      for (int y = 0; y < GeneratedMapHeight; y++)
      {
        output += string.Format("[ {0} ; {1} ] -> ", x, y);

        foreach (var item in _generatedMap.PathfindingMap[x, y].SidesWalkability)
        {
          output += string.Format(" | {0} {1} | ", item.Key, item.Value);
        }

        output += string.Format("walkable: {0}", _generatedMap.PathfindingMap[x, y].Walkable);

        output += "\n";
      }
    }

    Debug.Log("Map:\n" + output);
    */

    ScreenFader.Instance.FadeIn();

    GUIManager.Instance.SetCompassVisibility(true);
    GUIManager.Instance.PlayerForm.ShowForm(true);

    GameData.Instance.PlayerMoveState = GameData.PlayerMoveStateEnum.NORMAL;    
    GameData.Instance.CurrentGameState = GameData.GameState.RUNNING;

    InputControllerScript.MapLoadingFinishedHandler();
    SoundManager.Instance.MapLoadingFinishedHandler();

    GUIManager.Instance.SetupGameForms();

    //Vector3 starsPos = new Vector3((GeneratedMapWidth / 2) * GlobalConstants.WallScaleFactor, 0.0f, (GeneratedMapHeight / 2) * GlobalConstants.WallScaleFactor);
    //Stars.transform.localPosition = starsPos;
  }

  // TODO: In the future move all item names from items-db.xml somewhere
  void SpawnItems()
  {    
    var io = SpawnItem(GlobalConstants.WorldItemType.PLACEHOLDER, "Scroll", false, "Scroll of Welcoming", GlobalConstants.PlayerGreeting);
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
          GameObject inst = InstantiatePrefab(0, 0, 0, go);
          var io = CreateItemObject(inst, copy, showInWorld);
          return io;
        }
      }
    }

    return null;
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

    bio.CalculateMapPosition();

    switch (item.ItemType)
    {
      case GlobalConstants.WorldItemType.PLACEHOLDER:
        bio.ItemObjectInstance = new PlaceholderItemObject(item.ItemName, item.ItemDescription, item.AtlasIconIndex, bio, InputControllerScript);
        break;

      case GlobalConstants.WorldItemType.FOOD:
        var sfi = item as SerializableFoodItem;
        bio.ItemObjectInstance = new FoodItemObject(item.ItemName, item.ItemDescription, item.AtlasIconIndex, bio, sfi.Saturation, InputControllerScript);
        break;

      case GlobalConstants.WorldItemType.WEAPON_MELEE:
        var swi = item as SerializableWeaponItem;
        bio.ItemObjectInstance = new WeaponItemObject(item.ItemName, item.ItemDescription, item.AtlasIconIndex, bio,
          swi.MinimumDamage, swi.MaximumDamage, swi.Cooldown, InputControllerScript);
        break;

      case GlobalConstants.WorldItemType.ARMOR_CHEST:
        var sai = item as SerializableArmorItem;
        bio.ItemObjectInstance = new ArmorItemObject(item.ItemName, item.ItemDescription, item.AtlasIconIndex, bio, sai.ArmorClassModifier, InputControllerScript);          
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
      
      mm.Actor = new EnemyActor(mm, this, InputControllerScript);
    }
  }
    
  int _terrainAddedWidth = 10;
  void MakeMountains()
  {    
    Vector3 terrainSize = new Vector3((_generatedMapWidth + _terrainAddedWidth) * GlobalConstants.WallScaleFactor, 
                                      GlobalConstants.DefaultVillageMountainsSize.y,
                                      (_generatedMapHeight + _terrainAddedWidth) * GlobalConstants.WallScaleFactor);
    
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

  void BuildMapNew()
  {
    for (int y = 0; y < _newLevelClass.MapY; y++)
    {
      for (int x = 0; x < _newLevelClass.MapX; x++)
      {
        for (int z = 0; z < _newLevelClass.MapZ; z++)
        {
          if (_newLevelClass.Level[x, y, z].BlockType == GlobalConstants.BlockType.AIR || _newLevelClass.Level[x, y, z].SkipInstantiation)
          {
            continue;
          }

          GameObject prefab = PrefabsManager.Instance.FindPrefabByName(GlobalConstants.BlockPrefabById[_newLevelClass.Level[x, y, z].BlockType]);

          if (prefab != null)
          {
            GameObject block = (GameObject)Instantiate(prefab, _newLevelClass.Level[x, y, z].WorldCoordinates, Quaternion.identity);
            block.transform.parent = ObjectsInstancesTransform.transform;

            MinecraftBlockAnimated blockAnimated = block.GetComponent<MinecraftBlockAnimated>();
            if (blockAnimated != null)
            {
              Utils.HideLevelBlockSides(blockAnimated, _newLevelClass.Level[x, y, z].ArrayCoordinates, _newLevelClass);
            }
            else
            {              
              MinecraftBlock blockScript = block.GetComponent<MinecraftBlock>();
              Utils.HideLevelBlockSides(blockScript, _newLevelClass.Level[x, y, z].ArrayCoordinates, _newLevelClass);
            }
          }
        }
      }
    }

    SetupCamera((int)_newLevelClass.PlayerPos.X, (int)_newLevelClass.PlayerPos.Y, (int)_newLevelClass.PlayerPos.Z, 2);
  }

  void BuildMap()
  {
    _mapColumns = _generatedMapWidth;
    _mapRows = _generatedMapHeight;

    _floorSoundTypeByPosition = new int[_mapRows, _mapColumns];

    for (int x = 0; x < _generatedMapHeight; x++)
    {
      for (int y = 0; y < _generatedMapWidth; y++)
      {
        foreach (var block in _generatedMap.Map[x, y].Blocks)
        {
          if (block.FootstepSoundType != -1)
          {
            _floorSoundTypeByPosition[x, y] = block.FootstepSoundType;
          }

          SpawnBlock(block);
        }

        foreach (var obj in _generatedMap.Map[x, y].Objects)
        {
          SpawnObject(obj);
        }
      }
    }
    
    SetupCamera(_generatedMap.CameraPos.X, 0, _generatedMap.CameraPos.Y, _generatedMap.CameraPos.Facing);

    SoundManager.Instance.PlayMusicTrack(_generatedMap.MusicTrack);    
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

          mm.Actor = new VillagerActor(mm, this, InputControllerScript);
          mm.Actor.ActorName = mm.ActorName;
        }
        break;

      default:
        break;
    }
  }

  int _mapColumns = 0;
  public int MapColumns
  {
    get { return _mapColumns; }
  }

  int _mapRows = 0;
  public int MapRows
  {
    get { return _mapRows; }
  }

  void LoadMap(string filename)
  {
    XmlDocument doc = new XmlDocument();
    doc.Load(filename);
    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
    {      
      switch(node.Name)
      {
        case "START":
          int x = int.Parse(node.Attributes["x"].InnerText);
          int y = int.Parse(node.Attributes["y"].InnerText);
          int facing = int.Parse(node.Attributes["facing"].InnerText);
          SetupCamera(x, 0, y, facing);
          break;
      case "FOG":
          float density = float.Parse(node.Attributes["density"].InnerText);
          UnityEngine.RenderSettings.fogDensity = density;
          break;
      case "LAYOUT":
          ParseLayout(node);
          break;
      case "MUSIC":
          string trackName = node.Attributes["track"].InnerText;
          SoundManager.Instance.PlayMusicTrack(trackName);
          break;
      case "OBJECT":
          SpawnObject(node);
          break;
        case "OBJECTRANGE":
          SpawnObjects(node);
          break;
        case "BLOCKRANGE":
          SpawnBlocks(node);
          break;
        case "BLOCK":
          SpawnBlock(node);
          break;
      }
    }
  }

  void LoadMapBinary(string fileName)
  {
    FileStream fs = new FileStream(fileName, FileMode.Open);
    BinaryFormatter bf = new BinaryFormatter();

    SerializableMap sc = new SerializableMap();

    sc = (SerializableMap)bf.Deserialize(fs);

    fs.Close();

    _mapColumns = sc.MapWidth;
    _mapRows = sc.MapHeight;

    _floorSoundTypeByPosition = new int[_mapRows, _mapColumns];
    _mapLayout = new char[_mapRows, _mapColumns];

    SetupCamera(sc.CameraPos.X, 0, sc.CameraPos.Y, sc.CameraPos.Facing);

    string musicTrack = sc.MusicTrack;

    if (!string.IsNullOrEmpty(musicTrack))
    {
      SoundManager.Instance.PlayMusicTrack(musicTrack);
    }

    foreach (var item in sc.SerializableBlocksList)
    {
      SpawnBlock(item);

      if (item.FootstepSoundType != -1)
      {
        _floorSoundTypeByPosition[item.X, item.Y] = item.FootstepSoundType;
      }

      //Debug.Log(item.ToString());
    }

    foreach (var item in sc.SerializableObjectsList)
    {
      SpawnObject(item);
    }
  }

  void SpawnBlock(XmlNode node)
  {
    int x = int.Parse(node.Attributes["x"].InnerText);
    int y = int.Parse(node.Attributes["y"].InnerText);
    int layer = int.Parse(node.Attributes["layer"].InnerText);
    string prefabName = node.Attributes["prefab"].InnerText;
    int facing = 0;
    bool flip = false;

    if (node.Attributes.GetNamedItem("facing") != null)
    {
      facing = int.Parse(node.Attributes["facing"].InnerText);
    }

    if (node.Attributes.GetNamedItem("flip") != null)
    {
      flip = bool.Parse(node.Attributes["flip"].InnerText);
    }

    GameObject go = PrefabsManager.Instance.FindPrefabByName(prefabName);
    
    if (go == null)
    {
      Debug.LogWarning("Couldn't find prefab: " + prefabName);
      return;
    }

    InstantiatePrefab(x, layer, y, go, facing, flip);
  }

  void SpawnBlock(SerializableBlock cellInfo)
  {
    int x = cellInfo.X;
    int y = cellInfo.Y;
    int layer = cellInfo.Layer;
    string prefabName = cellInfo.PrefabName;
    int facing = cellInfo.Facing;
    bool flip = cellInfo.FlipFlag;

    GameObject go = PrefabsManager.Instance.FindPrefabByName(prefabName);
    
    if (go == null)
    {
      Debug.LogWarning("Couldn't find prefab: " + prefabName);
      return;
    }
    
    InstantiatePrefab(x, layer, y, go, facing, flip);
  }

  void SpawnBlocks(XmlNode node)
  {
    int xStart = int.Parse(node.Attributes["xStart"].InnerText);
    int yStart = int.Parse(node.Attributes["yStart"].InnerText);    
    int xEnd = int.Parse(node.Attributes["xEnd"].InnerText);
    int yEnd = int.Parse(node.Attributes["yEnd"].InnerText);
    int layer = int.Parse(node.Attributes["layer"].InnerText);
    string prefabName = node.Attributes["prefab"].InnerText;
    bool flip = false;

    GameObject go = PrefabsManager.Instance.FindPrefabByName(prefabName);

    if (go == null)
    {
      Debug.LogWarning("Couldn't find prefab: " + prefabName);
      return;
    }

    if (node.Attributes.GetNamedItem("flip") != null)
    {
      flip = bool.Parse(node.Attributes["flip"].InnerText);
    }

    for (int x = xStart; x <= xEnd; x++)
    {
      for (int y = yStart; y <= yEnd; y++)
      {
        InstantiatePrefab(x, layer, y, go, 0, flip);
      }
    }
  }

  void ParseLayout(XmlNode node)
  {
    var mapLayout = Regex.Split(node.InnerText, "\r\n");
    for (int i = 0; i < mapLayout.Length; i++)
    {
      if (mapLayout[i] != string.Empty) 
      {
        _map.Add(mapLayout[i]);
      }
    }
    
    _mapColumns = _map[0].Length;
    _mapRows = _map.Count;
    
    _mapLayout = new char[_mapRows, _mapColumns];
    _floorSoundTypeByPosition = new int[_mapRows, _mapColumns];
    for (int i = 0; i < _mapRows; i++)
    {
      for (int j = 0; j < _mapColumns; j++)
      {
        _floorSoundTypeByPosition[i, j] = -1;
      }
    }


    int x = 0, y = 0;
    foreach (var line in _map)
    {
      for (int i = 0; i < line.Length; i++)
      {
        _mapLayout[x, y] = line[i];
        y++;
      }

      y = 0;
      x++;
    }
  }

  /*
  void InstantiatePrefab(int x, int z, GameObject goToInstantiate)
  {
    Vector3 goPosition = Vector3.zero;

    GameObject go = (GameObject)Instantiate(goToInstantiate);

    goPosition = go.transform.position;
    goPosition.x = x * GlobalConstants.WallScaleFactor;
    goPosition.z = z * GlobalConstants.WallScaleFactor;
    go.transform.position = goPosition;
    go.transform.parent = ObjectsInstancesTransform.transform;
  }
  */

  GameObject InstantiatePrefab(int x, int yOffset, int z, GameObject goToInstantiate, int facing = 0, bool flip = false)
  {
    GlobalConstants.Orientation o = GlobalConstants.OrientationsMap[facing];

    Vector3 goPosition = Vector3.zero;
    
    GameObject go = (GameObject)Instantiate(goToInstantiate);

    go.name = string.Format("{0}_{1}", go.name, _nameSuffix);

    goPosition = go.transform.position;
    goPosition.x = x * GlobalConstants.WallScaleFactor;
    goPosition.z = z * GlobalConstants.WallScaleFactor;
    goPosition.y = (yOffset == 0) ? go.transform.position.y : yOffset * GlobalConstants.WallScaleFactor;
    go.transform.position = goPosition;
    go.transform.Rotate(Vector3.up, GlobalConstants.OrientationAngles[o], Space.World);
    if (flip)
    {
      go.transform.Rotate(Vector3.right, 180.0f, Space.World);
    }
    go.transform.parent = ObjectsInstancesTransform.transform;

    _nameSuffix++;

    return go;
  }

  void SpawnObject(XmlNode node)
  {
    int x = int.Parse(node.Attributes["x"].InnerText);
    int y = int.Parse(node.Attributes["y"].InnerText);
    int layer = int.Parse(node.Attributes["layer"].InnerText);
    string moClass = node.Attributes["class"].InnerText;
    int facing = int.Parse(node.Attributes["facing"].InnerText);
    string prefabName = node.Attributes["prefab"].InnerText;

    string id = string.Empty;
    string objectToControlId = string.Empty;
    string doorSoundType = string.Empty;

    float animationOpenSpeed = 1.0f;
    float animationCloseSpeed = 1.0f;

    Vector2 animationSpeeds = Vector2.one;

    if (node.Attributes.GetNamedItem("id") != null)
    {
      id = node.Attributes["id"].InnerText;
    }

    if (node.Attributes.GetNamedItem("control") != null)
    {
      objectToControlId = node.Attributes["control"].InnerText;
    }

    if (node.Attributes.GetNamedItem("doorSound") != null)
    {
      doorSoundType = node.Attributes["doorSound"].InnerText;
    }

    if (node.Attributes.GetNamedItem("openSpeed") != null)
    {
      animationOpenSpeed = float.Parse(node.Attributes["openSpeed"].InnerText);
    }

    if (node.Attributes.GetNamedItem("closeSpeed") != null)
    {
      animationCloseSpeed = float.Parse(node.Attributes["closeSpeed"].InnerText);
    }

    animationSpeeds.Set(animationOpenSpeed, animationCloseSpeed);

    GameObject go = PrefabsManager.Instance.FindPrefabByName(prefabName);
    
    if (go == null)
    {
      Debug.LogWarning("Couldn't find prefab: " + prefabName);
      return;
    }

    GameObject sceneObject = InstantiatePrefab(x, layer, y, go, facing);

    if (id != string.Empty)
    {
      _mapObjectsHashTable.Add(id.GetHashCode(), sceneObject);
    }

    SerializableObject so = new SerializableObject();

    so.X = x;
    so.Y = y;
    so.Layer = layer;
    so.ObjectClassName = moClass;
    so.PrefabName = prefabName;
    so.Facing = facing;
    so.ObjectId = id;
    so.ObjectToControlId = objectToControlId;
    so.DoorSoundType = doorSoundType;
    so.AnimationOpenSpeed = animationOpenSpeed;
    so.AnimationCloseSpeed = animationCloseSpeed;

    CreateMapObject(sceneObject, so);
  }

  void SpawnObject(SerializableObject so)
  {
    GameObject go = PrefabsManager.Instance.FindPrefabByName(so.PrefabName);
    
    if (go == null)
    {
      Debug.LogWarning("Couldn't find prefab: " + so.PrefabName);
      return;
    }

    GameObject sceneObject = InstantiatePrefab(so.X, so.Layer, so.Y, go, so.Facing);
    
    if (so.ObjectId != string.Empty)
    {
      _mapObjectsHashTable.Add(so.ObjectId.GetHashCode(), sceneObject);
    }

    CreateMapObject(sceneObject, so);
  }

  void SpawnObjects(XmlNode node)
  {
    int xStart = int.Parse(node.Attributes["xStart"].InnerText);
    int yStart = int.Parse(node.Attributes["yStart"].InnerText);    
    int xEnd = int.Parse(node.Attributes["xEnd"].InnerText);
    int yEnd = int.Parse(node.Attributes["yEnd"].InnerText);
    int layer = int.Parse(node.Attributes["layer"].InnerText);
    string moClass = node.Attributes["class"].InnerText;
    int facing = int.Parse(node.Attributes["facing"].InnerText);
    string prefabName = node.Attributes["prefab"].InnerText;
    
    GameObject go = PrefabsManager.Instance.FindPrefabByName(prefabName);
    
    if (go == null)
    {
      Debug.LogWarning("Couldn't find prefab: " + prefabName);
      return;
    }

    for (int x = xStart; x <= xEnd; x++)
    {
      for (int y = yStart; y <= yEnd; y++)
      {
        GameObject sceneObject = InstantiatePrefab(x, layer, y, go, facing);

        SerializableObject so = new SerializableObject();
        
        so.X = x;
        so.Y = y;
        so.Layer = layer;
        so.ObjectClassName = moClass;
        so.PrefabName = prefabName;
        so.Facing = facing;

        CreateMapObject(sceneObject, so);
      }
    }
  }

  Dictionary<int, VillagerInfo> _villagersInfo = new Dictionary<int, VillagerInfo>();
  public Dictionary<int, VillagerInfo> VillagersInfo
  {
    get { return _villagersInfo; }
  }

  void SetupCharacters()
  {
    //var resource = Resources.Load("text/OneVillager");
    //var resource = Resources.Load("text/NoVillagers");
    var resource = Resources.Load("text/Villagers");
    
    if (resource == null) return;

    TextAsset ta = resource as TextAsset;

    XmlDocument doc = new XmlDocument();
    doc.LoadXml(ta.text);
    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
    {      
      switch (node.Name)
      {
        case "CHARACTER":
          if (node.Attributes.GetNamedItem("prefab") == null)
          {
            continue;
          }

          VillagerInfo vi = new VillagerInfo();
          int hash = node.Attributes["name"].InnerText.GetHashCode();

          string prefabName = node.Attributes["prefab"].InnerText;
          string actorClass = node.Attributes["class"].InnerText;

          var prefab = FindCharacterPrefabByName(prefabName);
          if (prefab != null)
          {
            Int2 pos = _generatedMap.GetRandomUnoccupiedCell();
            SpawnCharacter(prefab, pos, actorClass);
          }

          vi.HailString = node.Attributes["hailString"].InnerText;
          vi.PortraitName = node.Attributes["portraitName"].InnerText;

          foreach (XmlNode tag in node.ChildNodes)
          {
            switch (tag.Name)
            {
              case "NAME":
                vi.VillagerName = tag.Attributes["text"].InnerText;
                break;
              case "JOB":
                vi.VillagerJob = tag.Attributes["text"].InnerText;
                break;
              case "GOSSIP":
                foreach (XmlNode line in tag.ChildNodes)
                {
                  switch (line.Name)
                  {
                    case "LINE":
                      vi.VillagerGossipLines.Add(line.Attributes["text"].InnerText);
                      break;
                    default:
                      break;
                  }
                }
                break;
              default:
                break;
            }
          }

          _villagersInfo.Add(hash, vi);

          break;
        default:
          break;
      }
    }
  }

  // ********************** HELPER FUNCTIONS ********************** //

  public char GetMapLayoutPoint(int x, int y)
  {
    if (_mapLayout == null) return '#';

    int lx = Mathf.Clamp(x, 0, _mapRows);
    int ly = Mathf.Clamp(y, 0, _mapColumns);

    return _mapLayout[lx, ly];
  }

  public GameObject GetGameObjectByName(string name)
  {
    int hash = name.GetHashCode();
    if (_mapObjectsHashTable.ContainsKey(hash))
    {
      return _mapObjectsHashTable[hash];
    }

    return null;
  }

  public MapObject GetMapObjectById(string name)
  {
    int hash = name.GetHashCode();
    if (_mapObjectsHashTable.ContainsKey(hash))
    {
      BehaviourMapObject bmo = _mapObjectsHashTable[hash].GetComponent<BehaviourMapObject>();
      if (bmo != null)
      {
        return bmo.MapObjectInstance;
      }
    }
    
    return null;
  }

  List<MapObject> _searchResult = new List<MapObject>();
  Vector2 _searchKey = Vector2.zero;
  public List<MapObject> GetMapObjectsByPosition(int x, int y)
  {
    _searchKey.Set(x, y);
    if (!_mapObjectsHashListByPosition.ContainsKey(_searchKey)) return null;

    _searchResult.Clear();

    foreach (var hash in _mapObjectsHashListByPosition[_searchKey])
    {
      if (_mapObjectsHashTable.ContainsKey(hash))
      {
        GameObject go = _mapObjectsHashTable[hash];
        BehaviourMapObject bmo = go.GetComponent<BehaviourMapObject>();
        if (bmo != null)
        {
          _searchResult.Add(bmo.MapObjectInstance);
        }
      }
    }

    return _searchResult;
  }

  void CreateMapObject(GameObject go, SerializableObject so)
  {
    BehaviourMapObject bmo = go.GetComponent<BehaviourMapObject>();
    if (bmo == null)
    {
      //Debug.LogWarning("Could not get BMO component from " + prefabName);
      return;
    }

    bmo.CalculateMapPosition();

    switch (so.ObjectClassName)
    {
      case "wall":
        bmo.MapObjectInstance = new WallMapObject(so.ObjectClassName, so.PrefabName, bmo);
        (bmo.MapObjectInstance as WallMapObject).ActionCallback += (bmo.MapObjectInstance as WallMapObject).ActionHandler;
        break;

      case "door-openable":
        bmo.MapObjectInstance = new DoorMapObject(so.ObjectClassName, so.PrefabName, bmo, this);
        if (so.DoorSoundType != string.Empty)
        {
          (bmo.MapObjectInstance as DoorMapObject).DoorSoundType = so.DoorSoundType;
        }
        (bmo.MapObjectInstance as DoorMapObject).AnimationOpenSpeed = so.AnimationOpenSpeed;
        (bmo.MapObjectInstance as DoorMapObject).AnimationCloseSpeed = so.AnimationCloseSpeed;
        (bmo.MapObjectInstance as DoorMapObject).ActionCallback += (bmo.MapObjectInstance as DoorMapObject).ActionHandler;
        (bmo.MapObjectInstance as DoorMapObject).ActionCompleteCallback += (bmo.MapObjectInstance as DoorMapObject).ActionCompleteHandler;
        break;

      case "door-controllable":
        bmo.MapObjectInstance = new DoorMapObject(so.ObjectClassName, so.PrefabName, bmo, this);
        if (so.DoorSoundType != string.Empty)
        {
          (bmo.MapObjectInstance as DoorMapObject).DoorSoundType = so.DoorSoundType;
        }
        (bmo.MapObjectInstance as DoorMapObject).AnimationOpenSpeed = so.AnimationOpenSpeed;
        (bmo.MapObjectInstance as DoorMapObject).AnimationCloseSpeed = so.AnimationCloseSpeed;
        (bmo.MapObjectInstance as DoorMapObject).ControlCallback += (bmo.MapObjectInstance as DoorMapObject).ActionHandler;
        break;

      case "lever":
        bmo.MapObjectInstance = new LeverMapObject(so.ObjectClassName, so.PrefabName, bmo);
        (bmo.MapObjectInstance as LeverMapObject).ActionCallback += (bmo.MapObjectInstance as LeverMapObject).ActionHandler;
          
        if (so.ObjectToControlId != string.Empty)
        {
          MapObject mo = GetMapObjectById(so.ObjectToControlId);
          if (mo != null)
          {
            (bmo.MapObjectInstance as LeverMapObject).ControlledObject = mo;
          }
        }

        break;
      
      case "button":
        bmo.MapObjectInstance = new ButtonMapObject(so.ObjectClassName, so.PrefabName, bmo, this);
        (bmo.MapObjectInstance as ButtonMapObject).ActionCallback += (bmo.MapObjectInstance as ButtonMapObject).ActionHandler;

        if (so.ObjectToControlId != string.Empty)
        {
          MapObject mo = GetMapObjectById(so.ObjectToControlId);
          if (mo != null)
          {
            (bmo.MapObjectInstance as ButtonMapObject).ControlledObject = mo;
          }
        }

        break;

      case "sign":
        bmo.MapObjectInstance = new SignMapObject(so.ObjectClassName, so.PrefabName, bmo, so.TextField);
        break;      

      default:
        break;
    }

    bmo.MapObjectInstance.Facing = so.Facing;
  }

  void SetupCamera(int x, int y, int z, int facing)
  {
    CameraOrientation = facing;
    GlobalConstants.Orientation o = GlobalConstants.OrientationsMap[CameraOrientation];
    _cameraPos.x = x * GlobalConstants.WallScaleFactor;
    _cameraPos.y = y * GlobalConstants.WallScaleFactor;
    _cameraPos.z = z * GlobalConstants.WallScaleFactor;
    CameraPivot.transform.position = _cameraPos;
    CameraPivot.transform.Rotate(Vector3.up, GlobalConstants.OrientationAngles[o]);
    _cameraAngles = CameraPivot.transform.eulerAngles;

    InputControllerScript.PlayerMapPos.X = x;
    InputControllerScript.PlayerMapPos.Y = y;
    InputControllerScript.PlayerMapPos.Z = z;

    InputControllerScript.PlayerPreviousMapPos.X = x;
    InputControllerScript.PlayerPreviousMapPos.Y = y;
    InputControllerScript.PlayerPreviousMapPos.Z = z;
  }

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

  public enum MapFilename
  {
    ROOMS = 0,
    ROOMS_CONNECTED,
    TEST,
    BINARY_TREE,
    SIDEWINDER,
    GROWING_TREE,
    TOWN,
    DUNGEON1,
    GEN_VILLAGE,
    DARWIN_VILLAGE
  }
}
