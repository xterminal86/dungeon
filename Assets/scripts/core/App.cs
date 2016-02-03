using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

public class App : MonoSingleton<App>
{  
  public Terrain Mountains;

  public GameObject TestMob;

  [HideInInspector]
  public List<GameObject> Characters;

  public GameObject ObjectsInstancesTransform;

  public PlayerMoveStateEnum PlayerMoveState;
  public GameState CurrentGameState;
    
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

  [HideInInspector]
  public Callback MapLoadingFinished;

  public MapFilename MapFilenameField;

  [Header("Fog settings")]
  public bool EnableFog = true;
  public FogMode Type = FogMode.ExponentialSquared;
  public Color FogColor = Color.black;
  [Range(0.0f, 1.0f)]
  public float FogDensity = 0.2f;
  [Range(1.0f, 100.0f)]
  public float LinearFogWidth = 1.0f;

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

  void OnLevelWasLoaded(int level)
  {    
    UnityEngine.RenderSettings.fog = EnableFog;
    UnityEngine.RenderSettings.fogMode = Type;
    UnityEngine.RenderSettings.fogColor = FogColor;
    UnityEngine.RenderSettings.fogDensity = FogDensity;
    UnityEngine.RenderSettings.fogStartDistance = Camera.main.farClipPlane - LinearFogWidth * 2;
    UnityEngine.RenderSettings.fogEndDistance = Camera.main.farClipPlane - LinearFogWidth;

    Camera.main.backgroundColor = FogColor;

    MapLoadingFinished += InputController.Instance.MapLoadingFinishedHandler;
    MapLoadingFinished += SoundManager.Instance.MapLoadingFinishedHandler;

    _cameraPos = CameraPivot.transform.position;

    MakeMountains();

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
      default:
        LoadMap("Assets/maps/test_map.xml");
        break;
    }

    if (_generatedMap != null)
    {
      _generatedMap.Generate();
      BuildMap();
    }

    SpawnItems();
    SetupCharacters();
    SetupMobs();

    ScreenFader.Instance.FadeIn();

    GUIManager.Instance.SetCompassVisibility(true);
    GUIManager.Instance.PlayerForm.ShowForm(true);

    PlayerMoveState = PlayerMoveStateEnum.NORMAL;    
    CurrentGameState = GameState.RUNNING;

    if (MapLoadingFinished != null)
      MapLoadingFinished();    
  }

  protected override void Init()
  {
    base.Init();
  }

  void SpawnItems()
  {
    SerializableObject so = new SerializableObject();

    so.X = 49;
    so.Y = 1;
    so.Layer = 0;
    so.AtlasIcon = "atlas_248".GetHashCode();
    so.PrefabName = "mc-scroll";
    so.ObjectClassName = "item-placeholder";
    so.ObjectName = "Scroll of Placeholding";
    so.TextField = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\n";
    //so.TextField = "0123456789A123456789B123456789C123456789D123456789E123456789F";

    GameObject go = PrefabsManager.Instance.FindPrefabByName("mc-scroll");
    
    if (go == null)
    {
      Debug.LogWarning("Couldn't find prefab: mc-scroll");
      return;
    }
        
    GameObject inst = InstantiatePrefab(so.X, so.Layer, so.Y, go);
    CreateItemObject(inst, so);

    so.ObjectName = "Greetings!";
    so.TextField = "Welcome to Dungeon: There and Back Again!\n\nThis game is one-man work in progress, so expect a lot of bugs and not very active development. If you possess any related (especially artistic) skills, your contribution would be great.\n\nxterminal86\n";
    inst = InstantiatePrefab(so.X, so.Layer, so.Y, go);
    var io = CreateItemObject(inst, so, false);
    GUIManager.Instance.InventoryForm.AddItemToInventory(io);

    go = PrefabsManager.Instance.FindPrefabByName("mc-axe");

    so.AtlasIcon = "atlas_422".GetHashCode();
    so.PrefabName = "mc-axe";
    so.ObjectName = "Axe of Placeholding";
    so.TextField = "This is an axe.";

    inst = InstantiatePrefab(so.X, so.Layer, so.Y, go);
    CreateItemObject(inst, so);
  }

  ItemObject CreateItemObject(GameObject go, SerializableObject so, bool showInWorld = true)
  {
    BehaviourItemObject bio = go.GetComponent<BehaviourItemObject>();
    if (bio == null)
    {
      Debug.LogWarning("Could not get BIO component from " + so.PrefabName);
      return null;
    }   

    bio.CalculateMapPosition();

    switch (so.ObjectClassName)
    {
      case "item-placeholder":
        bio.ItemObjectInstance = new PlaceholderItemObject(so.ObjectName, so.TextField, so.AtlasIcon, bio);
        break;

      default:
        break;
    }

    if (bio.ItemObjectInstance != null)
    {
      bio.ItemObjectInstance.LMBAction += bio.ItemObjectInstance.LMBHandler;
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
      SoundManager.Instance.LastPlayedSoundOfChar.Add(go.name.GetHashCode(), 0);
      
      mm.Actor = new EnemyActor(mm);
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
    Terrain t = (Terrain)Instantiate(Mountains, terrainPosition, Quaternion.identity);
    
    terrainPosition = new Vector3(-terrainSize.x - 1, 0, -1 - _terrainAddedWidth);
    t = (Terrain)Instantiate(Mountains, terrainPosition, Quaternion.identity);
    
    terrainPosition = new Vector3(-1 - _terrainAddedWidth, 0, terrainSize.z - (_terrainAddedWidth * 2 + 1));
    t = (Terrain)Instantiate(Mountains, terrainPosition, Quaternion.identity);
    
    terrainPosition = new Vector3(terrainSize.x - (_terrainAddedWidth * 2 + 1), 0, -1);
    t = (Terrain)Instantiate(Mountains, terrainPosition, Quaternion.identity);

    terrainPosition = new Vector3(terrainSize.x - (_terrainAddedWidth + 1), 0, -terrainSize.z);
    t = (Terrain)Instantiate(Mountains, terrainPosition, Quaternion.identity);    
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
    
    SetupCamera(_generatedMap.CameraPos.X, _generatedMap.CameraPos.Y, _generatedMap.CameraPos.Facing);

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
          SoundManager.Instance.LastPlayedSoundOfChar.Add(go.name.GetHashCode(), 0);

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
          SetupCamera(x, y, facing);
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

    SetupCamera(sc.CameraPos.X, sc.CameraPos.Y, sc.CameraPos.Facing);

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
        bmo.MapObjectInstance = new DoorMapObject(so.ObjectClassName, so.PrefabName, bmo);
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
        bmo.MapObjectInstance = new DoorMapObject(so.ObjectClassName, so.PrefabName, bmo);
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
        bmo.MapObjectInstance = new ButtonMapObject(so.ObjectClassName, so.PrefabName, bmo);
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

  void SetupCamera(int x, int y, int facing)
  {
    CameraOrientation = facing;
    GlobalConstants.Orientation o = GlobalConstants.OrientationsMap[CameraOrientation];
    _cameraPos.x = x * GlobalConstants.WallScaleFactor;
    _cameraPos.z = y * GlobalConstants.WallScaleFactor;
    CameraPivot.transform.position = _cameraPos;
    CameraPivot.transform.Rotate(Vector3.up, GlobalConstants.OrientationAngles[o]);
    _cameraAngles = CameraPivot.transform.eulerAngles;

    InputController.Instance.PlayerMapPos.X = x;
    InputController.Instance.PlayerMapPos.Y = y;

    InputController.Instance.PlayerPreviousMapPos.X = x;
    InputController.Instance.PlayerPreviousMapPos.Y = y;
  }

  float _hungerTimer = 0.0f;
  void Update()
  {
    if (CurrentGameState != GameState.RUNNING)
    {
      return;
    }

    if (GameData.Instance.PlayerCharacterVariable.HitPoints == 0)
    {
      SoundManager.Instance.StopAllSounds();
      SoundManager.Instance.PlaySound(GlobalConstants.SFXPlayerDeath);

      CurrentGameState = GameState.PAUSED;

      ScreenFader.Instance.FadeCompleteCallback += GameOverHandler;
      ScreenFader.Instance.FadeOut();
    }

    _hungerTimer += Time.smoothDeltaTime * GameData.Instance.PlayerCharacterVariable.HungerDecreaseMultiplier;

    if (_hungerTimer > GameData.Instance.PlayerCharacterVariable.HungerTick)
    {
      _hungerTimer = 0.0f;
      GameData.Instance.PlayerCharacterVariable.AddHunger(-1);
    }
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

    var objects = FindObjectsOfType<GameObject>();
    foreach (var item in objects)
    {
      Destroy(item.gameObject);
    }

    ScreenFader.Instance.FadeCompleteCallback -= GameOverHandler;
    Application.LoadLevel("entry");
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
    GEN_VILLAGE
  }

  public enum PlayerMoveStateEnum
  {
    NORMAL = 0,
    HOLD_PLAYER
  }

  public enum GameState
  {
    RUNNING = 0,
    PAUSED
  }
}
