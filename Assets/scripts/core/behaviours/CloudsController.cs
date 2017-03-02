using System.Collections;
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

  // All clouds holder
  public GameObject AllClouds;

  public GameObject CloudBlock;

  public int MaximumNumberOfClouds = 10;

  public Color CloudColor = Color.white;

  // Maximum width and height of the cloud (should be odd)

  [Range(5, 199)]
  public int MaxCloudSize = 29;

  [Range(2, 15)]
  public int MaxCloudThickness = 5;

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
    
  GameObject _allClouds;
  Texture2D _cloudTexture;
  Material _cloudMaterial;
	void Start() 
	{    
    if (MaxCloudSize % 2 == 0)
    {
      MaxCloudSize++;
    }
	}

  float _cloudsHeight = 4.0f;
  public void Generate(float cloudsHeight)
  {
    _cloudsHeight = cloudsHeight;

    _cloud = new int[MaxCloudSize, MaxCloudSize];

    _cloudTexture = new Texture2D(128, 128, TextureFormat.ARGB32, false);

    _allClouds = (GameObject)Instantiate(AllClouds);

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

    GenerateCloudsScaled();

    // Spread clouds across the map
    SpreadClouds();
  }

  List<float> _cloudsSpeeds = new List<float>();
  List<GameObject> _cloudsList = new List<GameObject>();
  void GenerateCloudsScaled()
  {
    for (int i = 0; i < MaximumNumberOfClouds; i++)
    {
      GameObject cloudHolder = (GameObject)Instantiate(CloudHolder, new Vector3(0.0f, _cloudsHeight, 0.0f), Quaternion.identity);

      int cloudSize = Random.Range(5, MaxCloudSize);
      int cloudSizeAddon = Random.Range(1, cloudSize / 2);
      int condition = Random.Range(0, 2);

      GameObject cloud = (GameObject)Instantiate(CloudBlock, new Vector3(0.0f, _cloudsHeight, 0.0f), Quaternion.identity);
      Vector3 scale = cloud.transform.localScale;

      scale.x = cloudSize;
      scale.y = Random.Range(1, MaxCloudThickness);
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

      cloud.transform.SetParent(cloudHolder.transform, false);
      cloudHolder.transform.SetParent(_allClouds.transform, false);

      SetMaterial(cloud);

      float floatSpeed = Random.Range(0.01f, CloudSpeedRange);

      _cloudsSpeeds.Add(floatSpeed);
      _cloudsList.Add(cloudHolder);
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

      pos.Set(CloudsWorldOrigin.x + newX, _cloudsHeight + cloudsHeightOffset, CloudsWorldOrigin.y + newZ);

      cloud.transform.localPosition = pos;
      cloud.transform.localEulerAngles = rotation;
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

    _newCloudPos.Set(CloudsWorldOrigin.x - CloudsSpreadRadius - MaxCloudSize, _cloudsHeight + cloudsHeightOffset, CloudsWorldOrigin.y + newZ);

    _cloudsList[cloudIndex].transform.localPosition = _newCloudPos;
    _cloudsList[cloudIndex].transform.localEulerAngles = _newCloudRotation;

    _cloudsSpeeds[cloudIndex] = Random.Range(0.01f, CloudSpeedRange);
  }
}
