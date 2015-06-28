using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class App : MonoSingleton<App>
{
  public GameObject WallPrefab;
  public GameObject FloorPrefab;
  public GameObject CeilingPrefab;
  public GameObject ColumnPrefab;
  public GameObject ObjectsInstancesTransform;

  //char[,] _map;
  List<MapCell> _map = new List<MapCell>();

  List<GameObject> _instances = new List<GameObject>();
  Vector3 _cameraPos = Vector3.zero;
  public Vector3 CameraPos
  {
    set { _cameraPos = value; }
    get { return _cameraPos; }
  }

  public GameObject CameraPivot;

  public Callback MapLoadingFinished;
  void Awake()
  {
    UnityEngine.RenderSettings.fog = true;
    UnityEngine.RenderSettings.fogColor = GlobalConstants.FogColor;
    UnityEngine.RenderSettings.fogDensity = GlobalConstants.FogDensity;

    MapLoadingFinished += InputController.Instance.MapLoadingFinishedHandler;

    _cameraPos = CameraPivot.transform.position;

    LoadMap("Assets/maps/test.txt");
    ParseMap();

    if (MapLoadingFinished != null)
      MapLoadingFinished();
  }

  protected override void Init()
  {
    base.Init();
  }

  int _mapColumns = 0, _mapRows = 0;
  void LoadMap(string filename)
  {
    /*
    string[] lines = File.ReadAllLines(filename);
    _mapColumns = lines[0].Length;
    _mapRows = lines.Length;
    _map = new char[_mapRows, _mapColumns];
    int x = 0, y = 0;
    foreach (var line in lines)
    {
      for (int i = 0; i < line.Length; i++)
      {
        _map[x, y] = line[i];
        y++;        
      }
      
      y = 0;
      x++;
    }
    */

    XmlDocument doc = new XmlDocument();
    doc.Load("Assets/maps/test_map.xml");
    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
    {
      if (node.Name == "START")
      {
        int x = int.Parse(node.Attributes["x"].InnerText);
        int y = int.Parse(node.Attributes["y"].InnerText);
        _cameraPos.x = x * GlobalConstants.WallScaleFactor;
        _cameraPos.z = y * GlobalConstants.WallScaleFactor;
        CameraPivot.transform.position = _cameraPos;
      }

      MapCell mapCell = new MapCell();
      foreach (var item in GlobalConstants.MapAttributesDictionary)
      {
        var attribute = node.Attributes[item.Value];
        if (attribute == null) continue;
        int attrValue = int.Parse(attribute.InnerText);
        switch (item.Key)
        {
          case GlobalConstants.MapAttributes.Floor:
            mapCell.FloorId = attrValue;
            break;
          case GlobalConstants.MapAttributes.Ceiling:
            mapCell.CeilingId = attrValue;
            break;
          case GlobalConstants.MapAttributes.CoordX:
            mapCell.CoordX = attrValue;
            break;
          case GlobalConstants.MapAttributes.CoordY:
            mapCell.CoordY = attrValue;
            break;
          case GlobalConstants.MapAttributes.Wall:
            mapCell.WallId = attrValue;
            break;
          default:
            break;

        }

        //Debug.Log(item.Value + " -> " + attrValue);
      }    

      _map.Add(mapCell);
    }
  }

  void ParseMap()
  {
    Vector3 goPosition = Vector3.zero;
    foreach (var cell in _map)
    {
      int x = cell.CoordX;
      int y = cell.CoordY;

      GameObject go = null;

      if (cell.FloorId != -1)
      {
        go = (GameObject)Instantiate(FloorPrefab);
        goPosition = go.transform.position;
        goPosition.x = x * GlobalConstants.WallScaleFactor;
        goPosition.z = y * GlobalConstants.WallScaleFactor;
        go.transform.position = goPosition;
        go.transform.parent = ObjectsInstancesTransform.transform;
        _instances.Add(go);
      }

      if (cell.WallId != -1)
      {
        go = (GameObject)Instantiate(WallPrefab);
        goPosition = go.transform.position;
        goPosition.x = x * GlobalConstants.WallScaleFactor;
        goPosition.z = y * GlobalConstants.WallScaleFactor;
        go.transform.position = goPosition;
        go.transform.parent = ObjectsInstancesTransform.transform;
        _instances.Add(go);
      }

      if (cell.CeilingId != -1)
      {
        go = (GameObject)Instantiate(CeilingPrefab);
        goPosition = go.transform.position;
        goPosition.x = x * GlobalConstants.WallScaleFactor;
        goPosition.z = y * GlobalConstants.WallScaleFactor;
        go.transform.position = goPosition;
        go.transform.parent = ObjectsInstancesTransform.transform;
        _instances.Add(go);
      }      
    }

    /*
    GameObject go;
    Vector3 goPosition = Vector3.zero;
    for (int i = 0; i < _mapRows; i++)
    {
      for (int j = 0; j < _mapColumns; j++)
      {
        //if (_map[i, j] == '0') continue;

        char type = _map[i, j];

        switch(type)
        {            
          case '1':
            go = (GameObject)Instantiate(WallPrefab);
            goPosition = go.transform.position;
            goPosition.x = i * GlobalConstants.WallScaleFactor;
            goPosition.z = j * GlobalConstants.WallScaleFactor;
            go.transform.position = goPosition;
            go.transform.parent = ObjectsInstancesTransform.transform;
            _instances.Add(go);
            break;
          case 'X':
            _cameraPos.x = i * GlobalConstants.WallScaleFactor;
            _cameraPos.z = j * GlobalConstants.WallScaleFactor;            
            CameraPivot.transform.position = _cameraPos;
            break;
          case '2':
            go = (GameObject)Instantiate(ColumnPrefab);
            goPosition = go.transform.position;
            goPosition.x = i * GlobalConstants.WallScaleFactor;
            goPosition.z = j * GlobalConstants.WallScaleFactor;
            go.transform.position = goPosition;
            go.transform.parent = ObjectsInstancesTransform.transform;
            _instances.Add(go);
            break;
          default:
            go = (GameObject)Instantiate(FloorPrefab);
            goPosition = go.transform.position;
            goPosition.x = i * GlobalConstants.WallScaleFactor;
            goPosition.z = j * GlobalConstants.WallScaleFactor;
            go.transform.position = goPosition;
            go.transform.parent = ObjectsInstancesTransform.transform;
            _instances.Add(go);            
            break;
        }
      }
    }
    */
  }
}
