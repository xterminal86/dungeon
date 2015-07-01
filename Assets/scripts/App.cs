﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

public class App : MonoSingleton<App>
{
  public GameObject WallPrefab;
  public GameObject FloorPrefab;
  public GameObject CeilingPrefab;
  public GameObject DummyObjectPrefab;
  public GameObject ObjectsInstancesTransform;

  List<string> _map = new System.Collections.Generic.List<string>();

  List<GameObject> _instances = new List<GameObject>();
  Vector3 _cameraPos = Vector3.zero;
  public Vector3 CameraPos
  {
    set { _cameraPos = value; }
    get { return _cameraPos; }
  }

  public int CameraOrientation;

  Vector3 _cameraAngles = Vector3.zero;
  public Vector3 CameraAngles
  {
    get { return _cameraAngles; }
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
    doc.Load("Assets/maps/test_map.xml");
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

    GameObject go;
    Vector3 goPosition = Vector3.zero;
    int x = 0, y = 0;
    foreach (var line in _map)
    {
      for (int i = 0; i < line.Length; i++)
      {
        if (line[i] == '#')
        {
          go = (GameObject)Instantiate(WallPrefab);
          goPosition = go.transform.position;
          goPosition.x = x * GlobalConstants.WallScaleFactor;
          goPosition.z = y * GlobalConstants.WallScaleFactor;
          go.transform.position = goPosition;
          go.transform.parent = ObjectsInstancesTransform.transform;
          _instances.Add(go);
        }
        else
        {
          go = (GameObject)Instantiate(FloorPrefab);
          goPosition = go.transform.position;
          goPosition.x = x * GlobalConstants.WallScaleFactor;
          goPosition.z = y * GlobalConstants.WallScaleFactor;
          go.transform.position = goPosition;
          go.transform.parent = ObjectsInstancesTransform.transform;
          _instances.Add(go);

          go = (GameObject)Instantiate(CeilingPrefab);
          goPosition = go.transform.position;
          goPosition.x = x * GlobalConstants.WallScaleFactor;
          goPosition.z = y * GlobalConstants.WallScaleFactor;
          go.transform.position = goPosition;
          go.transform.parent = ObjectsInstancesTransform.transform;
          _instances.Add(go);
        }
        y++;        
      }
      
      y = 0;
      x++;
    }
  }
}
