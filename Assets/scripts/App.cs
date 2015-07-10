using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

public class App : MonoSingleton<App>
{
  public GameObject DummyObjectPrefab;
  public GameObject WallPrefab;
  public GameObject FloorPrefab;
  public GameObject CeilingPrefab;
  public GameObject DoorPrefab;
  public GameObject ButtonPrefab;
  public GameObject ObjectsInstancesTransform;

  char[,] _mapLayout;
  List<string> _map = new System.Collections.Generic.List<string>();

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

  [Header("Fog settings")]
  public Color FogColor = Color.black;
  public float FogDensity = 0.2f;
  public bool EnableFog = true;

  void Awake()
  {
    UnityEngine.RenderSettings.fog = EnableFog;
    UnityEngine.RenderSettings.fogColor = FogColor;
    UnityEngine.RenderSettings.fogDensity = FogDensity;

    MapLoadingFinished += InputController.Instance.MapLoadingFinishedHandler;
    MapLoadingFinished += SoundManager.Instance.MapLoadingFinishedHandler;

    _cameraPos = CameraPivot.transform.position;

    LoadMap("Assets/maps/test_map.xml");
    //LoadMap("Assets/maps/binary_tree_map.xml");

    if (MapLoadingFinished != null)
      MapLoadingFinished();
  }

  protected override void Init()
  {
    base.Init();
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
          CameraOrientation = int.Parse(node.Attributes["facing"].InnerText);
          GlobalConstants.Orientation o = GlobalConstants.OrientationsMap[CameraOrientation];
          _cameraPos.x = x * GlobalConstants.WallScaleFactor;
          _cameraPos.z = y * GlobalConstants.WallScaleFactor;
          CameraPivot.transform.position = _cameraPos;
          CameraPivot.transform.Rotate(Vector3.up, GlobalConstants.OrientationAngles[o]);
          _cameraAngles = CameraPivot.transform.eulerAngles;
          break;
      case "LAYOUT":
          ParseLayout(node);
          break;
      case "OBJECT":
          SpawnObject(node);
          break;
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

    GameObject go;
    Vector3 goPosition = Vector3.zero;
    int x = 0, y = 0;
    foreach (var line in _map)
    {
      for (int i = 0; i < line.Length; i++)
      {
        _mapLayout[x, y] = line[i];

        if (line[i] == '#')
        {
          go = (GameObject)Instantiate(WallPrefab);
          goPosition = go.transform.position;
          goPosition.x = x * GlobalConstants.WallScaleFactor;
          goPosition.z = y * GlobalConstants.WallScaleFactor;
          go.transform.position = goPosition;
          go.transform.parent = ObjectsInstancesTransform.transform;
        }
        else
        {
          go = (GameObject)Instantiate(FloorPrefab);
          goPosition = go.transform.position;
          goPosition.x = x * GlobalConstants.WallScaleFactor;
          goPosition.z = y * GlobalConstants.WallScaleFactor;
          go.transform.position = goPosition;
          go.transform.parent = ObjectsInstancesTransform.transform;

          go = (GameObject)Instantiate(CeilingPrefab);
          goPosition = go.transform.position;
          goPosition.x = x * GlobalConstants.WallScaleFactor;
          goPosition.z = y * GlobalConstants.WallScaleFactor;
          go.transform.position = goPosition;
          go.transform.parent = ObjectsInstancesTransform.transform;
        }
        y++;        
      }
      
      y = 0;
      x++;
    }
  }

  Vector2 _dictionaryKey = Vector2.zero;
  void SpawnObject(XmlNode node)
  {
    GameObject go = null;
    BehaviourMapObject bmo = null;
    int x = int.Parse(node.Attributes["x"].InnerText);
    int y = int.Parse(node.Attributes["y"].InnerText);    
    string objectType = node.Attributes["type"].InnerText;
    string objectName = node.Attributes["name"].InnerText;
    int orientation = int.Parse(node.Attributes["facing"].InnerText);
    _dictionaryKey.Set(x, y);
    switch(objectType)
    {
      case "door":
        go = (GameObject)Instantiate(DoorPrefab);
        bmo = go.GetComponent<BehaviourMapObject>();
        if (bmo != null)
        {
          bmo.MapObjectInstance = new DoorMapObject();
          (bmo.MapObjectInstance as DoorMapObject).DoorIsOpen = bool.Parse(node.Attributes["open"].InnerText);
        }
        break;
      case "button":
        go = (GameObject)Instantiate(ButtonPrefab);
        bmo = go.GetComponent<BehaviourMapObject>();
        if (bmo != null)
        {
          bmo.MapObjectInstance = new ButtonMapObject();          
          MapObject mo = GetMapObjectByName(node.Attributes["connect"].InnerText);
          bmo.MapObjectInstance.ActionCallback += bmo.MapObjectInstance.ActionHandler;
          bmo.MapObjectInstance.ActionCompleteCallback += mo.ActionCompleteHandler;
        }
        break;
      case "dummy":
        go = (GameObject)Instantiate(DummyObjectPrefab);
        break;
      default:
        break;
    }
    
    if (go != null)
    {
      if (bmo != null)
      { 
        bmo.MapObjectInstance.Name = objectName;
        bmo.MapObjectInstance.HashCode = objectName.GetHashCode();
        bmo.MapObjectInstance.Facing = int.Parse(node.Attributes["facing"].InnerText);
        bmo.MapObjectInstance.BMO = bmo;
        bmo.MapObjectInstance.GameObjectToControl = bmo.Model;
      }

      GlobalConstants.Orientation o = GlobalConstants.OrientationsMap[orientation];
      Vector3 position = new Vector3(y * GlobalConstants.WallScaleFactor,
                                     go.transform.position.y,
                                     x * GlobalConstants.WallScaleFactor);
      go.transform.position = position;
      go.transform.Rotate(Vector3.up, GlobalConstants.OrientationAngles[o]);
      go.transform.parent = ObjectsInstancesTransform.transform;
      _mapObjectsHashTable[objectName.GetHashCode()] = go;

      if (_mapObjectsHashListByPosition.ContainsKey(_dictionaryKey))
      {
        _mapObjectsHashListByPosition[_dictionaryKey].Add(objectName.GetHashCode());
      }
      else
      {
        _mapObjectsHashListByPosition.Add(_dictionaryKey, new List<int>(){ objectName.GetHashCode() });
      }
    }

  }    

  // ********************** HELPER FUNCTIONS ********************** //

  public char GetMapLayoutPoint(int x, int y)
  {
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

  public MapObject GetMapObjectByName(string name)
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
}
