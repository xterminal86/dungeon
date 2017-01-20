﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsController : MonoBehaviour 
{
  // Block with top and bottom quads only
  public GameObject CloudInner;

  // Block with wall that should be rotated for corresponding cases (see below)
  public GameObject CloudOuter;

  // Empty object to hold our formed cloud structure for easy manipulation
  public GameObject CloudHolder;

  public GameObject CloudBlock;

  public int MaximumNumberOfClouds = 10;

  public float CloudsHeight = 4.0f;

  public Color CloudColor = Color.white;

  // Maximum width and height of the cloud (should be odd)

  [Range(5, 199)]
  public int MaxCloudSize = 29;

  public Vector2 CloudsWorldOrigin = Vector2.zero;
  public float CloudsSpreadRadius = 10.0f;

  int[,] _cloud;

  // Starting array coordinate (_startIndex, _startIndex)
  int _startIndex = 0;

  [Range(0.01f, 4.0f)]
  public float CloudSpeedRange = 0.1f;

  // Used in cloud generation algorithm to look around block by adding corresponding component of this vector
  List<Vector2> _cloudPositions = new List<Vector2>()
  {
    new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1)
  };
    
  Texture2D _cloudTexture;
  Material _cloudMaterial;
	void Start () 
	{    
    if (MaxCloudSize % 2 == 0)
    {
      MaxCloudSize++;
    }

    _cloud = new int[MaxCloudSize, MaxCloudSize];

    _cloudTexture = new Texture2D(128, 128, TextureFormat.ARGB32, false);

    for (int x = 0; x < _cloudTexture.width; x++)
    {
      for (int y = 0; y < _cloudTexture.height; y++)
      {
        _cloudTexture.SetPixel(x, y, new Color(CloudColor.r, CloudColor.g, CloudColor.b, CloudColor.a));
      }
    }

    _cloudTexture.Apply();

    _cloudMaterial = new Material(Shader.Find("Unlit/Transparent"));

    _cloudMaterial.SetTexture("_MainTex", _cloudTexture);

    //GenerateClouds();
    GenerateCloudsScaled();

    // Spread clouds across the map
    SpreadClouds();
	}

  void GenerateCloudsScaled()
  {
    for (int i = 0; i < MaximumNumberOfClouds; i++)
    {
      _cloudHolder = (GameObject)Instantiate(CloudHolder, new Vector3(0.0f, CloudsHeight, 0.0f), Quaternion.identity);

      int cloudSize = Random.Range(5, MaxCloudSize);
      int cloudSizeAddon = Random.Range(1, cloudSize / 2);
      int condition = Random.Range(0, 2);

      GameObject cloud = (GameObject)Instantiate(CloudBlock, new Vector3(0.0f, CloudsHeight, 0.0f), Quaternion.identity);
      Vector3 scale = cloud.transform.localScale;

      scale.x = cloudSize;
      scale.y = Random.Range(1, 4);
      scale.z = cloudSize;

      if (condition == 0)
      {
        scale.x += cloudSizeAddon;
      }
      else
      {
        scale.z += cloudSizeAddon;
      }

      cloud.transform.localScale = scale;

      cloud.transform.SetParent(_cloudHolder.transform, false);
      SetMaterial(cloud);

      float floatSpeed = Random.Range(0.01f, CloudSpeedRange);

      _cloudsSpeeds.Add(floatSpeed);
      _cloudsList.Add(_cloudHolder);
    }
  }

  void GenerateClouds()
  {
    // Generation starts here

    for (int i = 0; i < MaximumNumberOfClouds; i++)    
    {      
      System.Array.Clear(_cloud, 0, MaxCloudSize * MaxCloudSize);

      _startIndex = (MaxCloudSize + 1) / 2;

      FormCloud(_startIndex, _startIndex);

      // FIXME: single hole is just one case. There might be two, three and more neighbouring holes as well.
      // Either fix it or beat it.

      //CloseHoles();

      //PrintClouds();

      InstantiateCloud();
    }
  }
      
  float[] _cloudRotationAngles = { 0.0f, 90.0f, 180.0f, 270.0f };    
  void SpreadClouds()
  {    
    Vector3 pos = Vector3.zero;
    Vector3 rotation = Vector3.zero;

    foreach (var cloud in _cloudsList)
    {
      pos.x = CloudsWorldOrigin.x;
      pos.z = CloudsWorldOrigin.y;

      rotation = Vector3.zero;

      int rotationIndex = Random.Range(0, _cloudRotationAngles.Length);

      int newX = Random.Range(-(int)CloudsSpreadRadius, (int)CloudsSpreadRadius);
      int newZ = Random.Range(-(int)CloudsSpreadRadius, (int)CloudsSpreadRadius);

      rotation.y = _cloudRotationAngles[rotationIndex];

      float cloudsHeightOffset = Random.Range(-4.0f, 4.0f);

      pos.Set(CloudsWorldOrigin.x + newX, CloudsHeight + cloudsHeightOffset, CloudsWorldOrigin.y + newZ);

      cloud.transform.localPosition = pos;
      cloud.transform.localEulerAngles = rotation;
    }
  }

  void CloseHoles()
  {
    int lx, hx, ly, hy;

    for (int x = 0; x < MaxCloudSize; x++)
    {
      lx = x - 1;
      hx = x + 1;

      if (lx < 0 || hx >= MaxCloudSize)
      {
        continue;
      }

      for (int y = 0; y < MaxCloudSize; y++)
      {        
        ly = y - 1;
        hy = y + 1;

        if (ly < 0 || hy >= MaxCloudSize)
        {
          continue;
        }

        if (_cloud[x, y] == 0)
        {
          // Just a hole block
          if (_cloud[x, ly] == 1 && _cloud[lx, y] == 1 && _cloud[x, hy] == 1 && _cloud[hx, y] == 1)
          {
            _cloud[x, y] = 1;
          }
        }
      }
    }
  }

  int _nextProbability = 100;
  void FormCloud(int posX, int posY)
  {
    if (posX < 0 || posX == MaxCloudSize || posY < 0 || posY == MaxCloudSize || _cloud[posX, posY] == 1)
    {
      return;
    }

    _cloud[posX, posY] = 1;

    for (int i = 0; i < _cloudPositions.Count; i++)
    {
      int success = Random.Range(0, 100);

      if (success < _nextProbability)
      {  
        FormCloud(posX + (int)_cloudPositions[i].x, posY + (int)_cloudPositions[i].y);

        _nextProbability -= 5;

        _nextProbability = Mathf.Clamp(_nextProbability, 50, 100);
      }
    }
  }

  void PrintClouds()
  {
    string output = string.Empty;

    for (int x = 0; x < MaxCloudSize; x++)
    {
      for (int y = 0; y < MaxCloudSize; y++)
      {
        output += _cloud[x, y].ToString();
      }

      output += "\n";
    }

    Debug.Log(output);
  }

  void SetMaterial(GameObject go)
  {
    var renderers = go.GetComponentsInChildren<MeshRenderer>();

    for (int i = 0; i < renderers.Length; i++)
    {
      renderers[i].material = _cloudMaterial;
    }
  }

  List<float> _cloudsSpeeds = new List<float>();
  List<GameObject> _cloudsList = new List<GameObject>();

  GameObject _cloudHolder;
  void InstantiateCloud()
  { 
    _cloudHolder = (GameObject)Instantiate(CloudHolder, new Vector3(CloudsWorldOrigin.x, CloudsHeight, CloudsWorldOrigin.y), Quaternion.identity);

    // Form cloud

    for (int x = 0; x < MaxCloudSize; x++)
    {
      for (int y = 0; y < MaxCloudSize; y++)
      {
        if (_cloud[x, y] == 1)
        {
          GameObject cloudBlock = (GameObject)Instantiate(CloudInner, new Vector3(x, 0.0f, y), Quaternion.identity);
          cloudBlock.transform.SetParent(_cloudHolder.transform, false);

          SetMaterial(cloudBlock);
        }
      }
    }

    // Place walls for cloud block on sides that are facing empty space.
    //
    // Array columns are in positive Z direction, rows - negative X
    // Outer cloud prefab should not be rotated when 0 is to the left of the array element
    // then it is designed to be rotated 90 degrees clockwise for each consequent case: 
    // 0 array element to the up, right and down.

    int ly, hy, lx, hx = 0;

    GameObject cloudOuter;

    for (int x = 0; x < MaxCloudSize; x++)
    {
      lx = x - 1;
      hx = x + 1;

      for (int y = 0; y < MaxCloudSize; y++)
      {
        ly = y - 1;
        hy = y + 1;

        // Do nothing if current array block is empty

        if (_cloud[x, y] == 0)
        {
          continue;
        }

        // Force place walls for a block that is on the edge of the map since we cannot look past the array bounds
        CloseEdgeCloudBlock(x, y);

        // Block to the left

        if (ly >= 0 && _cloud[x, ly] == 0)
        {
          cloudOuter = (GameObject)Instantiate(CloudOuter, new Vector3(x, 0.0f, y), Quaternion.identity);
          cloudOuter.transform.SetParent(_cloudHolder.transform, false);

          SetMaterial(cloudOuter);
        }

        // Up

        if (lx >= 0 && _cloud[lx, y] == 0)
        {
          cloudOuter = (GameObject)Instantiate(CloudOuter, new Vector3(x, 0.0f, y), Quaternion.AngleAxis(90.0f, Vector3.up));
          cloudOuter.transform.SetParent(_cloudHolder.transform, false);

          SetMaterial(cloudOuter);
        }

        // Right

        if (hy < MaxCloudSize && _cloud[x, hy] == 0)
        {
          cloudOuter = (GameObject)Instantiate(CloudOuter, new Vector3(x, 0.0f, y), Quaternion.AngleAxis(180.0f, Vector3.up));
          cloudOuter.transform.SetParent(_cloudHolder.transform, false);

          SetMaterial(cloudOuter);
        }

        // Down

        if (hx < MaxCloudSize && _cloud[hx, y] == 0)
        {
          cloudOuter = (GameObject)Instantiate(CloudOuter, new Vector3(x, 0.0f, y), Quaternion.AngleAxis(270.0f, Vector3.up));
          cloudOuter.transform.SetParent(_cloudHolder.transform, false);

          SetMaterial(cloudOuter);
        }
      }
    }

    float floatSpeed = Random.Range(0.01f, CloudSpeedRange);

    _cloudsSpeeds.Add(floatSpeed);
    _cloudsList.Add(_cloudHolder);
  }

  void CloseEdgeCloudBlock(int x, int y)
  {
    int lx = x - 1;
    int hx = x + 1;
    int ly = y - 1;
    int hy = y + 1;

    GameObject cloudOuter;

    if (ly < 0)
    {
      cloudOuter = (GameObject)Instantiate(CloudOuter, new Vector3(x, 0.0f, y), Quaternion.identity);
      cloudOuter.transform.SetParent(_cloudHolder.transform, false);

      SetMaterial(cloudOuter);
    }

    if (hy >= MaxCloudSize)
    {
      cloudOuter = (GameObject)Instantiate(CloudOuter, new Vector3(x, 0.0f, y), Quaternion.AngleAxis(180.0f, Vector3.up));
      cloudOuter.transform.SetParent(_cloudHolder.transform, false);

      SetMaterial(cloudOuter);
    }

    if (lx < 0)
    {
      cloudOuter = (GameObject)Instantiate(CloudOuter, new Vector3(x, 0.0f, y), Quaternion.AngleAxis(90.0f, Vector3.up));
      cloudOuter.transform.SetParent(_cloudHolder.transform, false);

      SetMaterial(cloudOuter);
    }

    if (hx >= MaxCloudSize)
    {
      cloudOuter = (GameObject)Instantiate(CloudOuter, new Vector3(x, 0.0f, y), Quaternion.AngleAxis(270.0f, Vector3.up));
      cloudOuter.transform.SetParent(_cloudHolder.transform, false);

      SetMaterial(cloudOuter);
    }
  }

  Vector3 _cloudPos = Vector3.zero;
	void Update() 
	{
    for (int i = 0; i < _cloudsList.Count; i++)
    { 
      if (_cloudsList[i].transform.localPosition.x > CloudsWorldOrigin.x + CloudsSpreadRadius)
      {
        ResetCloudPosition(i);
      }

      _cloudPos = _cloudsList[i].transform.localPosition;
      _cloudPos.x += _cloudsSpeeds[i] * Time.deltaTime;
      _cloudsList[i].transform.localPosition = _cloudPos;
    }
	}  

  Vector3 _newCloudPos = Vector3.zero;
  Vector3 _newCloudRotation = Vector3.zero;
  void ResetCloudPosition(int cloudIndex)
  {
    _newCloudPos = Vector3.zero;
    _newCloudRotation = Vector3.zero;

    int rotationIndex = Random.Range(0, _cloudRotationAngles.Length);

    _newCloudRotation.y = _cloudRotationAngles[rotationIndex];

    float cloudsHeightOffset = Random.Range(-4.0f, 4.0f);
    int newZ = Random.Range(-(int)CloudsSpreadRadius, (int)CloudsSpreadRadius);

    _newCloudPos.Set(CloudsWorldOrigin.x - CloudsSpreadRadius - MaxCloudSize, CloudsHeight + cloudsHeightOffset, CloudsWorldOrigin.y + newZ);

    _cloudsList[cloudIndex].transform.localPosition = _newCloudPos;
    _cloudsList[cloudIndex].transform.localEulerAngles = _newCloudRotation;

    _cloudsSpeeds[cloudIndex] = Random.Range(0.01f, CloudSpeedRange);
  }
}