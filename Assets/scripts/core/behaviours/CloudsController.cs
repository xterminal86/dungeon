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

  public int MaximumNumberOfClouds = 10;

  // Maximum width and height of the cloud
  const int _size = 15;

  int[,] _cloud = new int[_size, _size];

  int _startIndex = 0;

  List<Vector2> _cloudPositions = new List<Vector2>()
  {
    new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1)
  };
    
	void Start () 
	{
    _startIndex = (_size + 1) / 2;

    FormCloud(_startIndex, _startIndex);

    CloseHoles();

    //PrintClouds();
    InstantiateClouds();
	}

  void CloseHoles()
  {
    int lx, hx, ly, hy;

    for (int x = 0; x < _size; x++)
    {
      lx = x - 1;
      hx = x + 1;

      if (lx < 0 || hx >= _size)
      {
        continue;
      }

      for (int y = 0; y < _size; y++)
      {        
        ly = y - 1;
        hy = y + 1;

        if (ly < 0 || hy >= _size)
        {
          continue;
        }

        if (_cloud[x, y] == 0 && _cloud[x, ly] == 1 && _cloud[lx, y] == 1 && _cloud[x, hy] == 1 && _cloud[hx, y] == 1)
        {
          _cloud[x, y] = 1;
        }
      }
    }
  }

  int _nextProbability = 100;
  void FormCloud(int posX, int posY)
  {
    if (posX < 0 || posX == _size || posY < 0 || posY == _size || _cloud[posX, posY] == 1)
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

    for (int x = 0; x < _size; x++)
    {
      for (int y = 0; y < _size; y++)
      {
        output += _cloud[x, y].ToString();
      }

      output += "\n";
    }

    Debug.Log(output);
  }

  GameObject _cloudHolder;
  void InstantiateClouds()
  {    
    _cloudHolder = (GameObject)Instantiate(CloudHolder);

    // Form cloud

    for (int x = 0; x < _size; x++)
    {
      for (int y = 0; y < _size; y++)
      {
        if (_cloud[x, y] == 1)
        {
          GameObject cloudBlock = (GameObject)Instantiate(CloudInner, new Vector3(x, 0.0f, y), Quaternion.identity);
          cloudBlock.transform.SetParent(_cloudHolder.transform, false);
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

    for (int x = 0; x < _size; x++)
    {
      lx = x - 1;
      hx = x + 1;

      for (int y = 0; y < _size; y++)
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
        }

        // Up

        if (lx >= 0 && _cloud[lx, y] == 0)
        {
          cloudOuter = (GameObject)Instantiate(CloudOuter, new Vector3(x, 0.0f, y), Quaternion.AngleAxis(90.0f, Vector3.up));
          cloudOuter.transform.SetParent(_cloudHolder.transform, false);
        }

        // Right

        if (hy < _size && _cloud[x, hy] == 0)
        {
          cloudOuter = (GameObject)Instantiate(CloudOuter, new Vector3(x, 0.0f, y), Quaternion.AngleAxis(180.0f, Vector3.up));
          cloudOuter.transform.SetParent(_cloudHolder.transform, false);
        }

        // Down

        if (hx < _size && _cloud[hx, y] == 0)
        {
          cloudOuter = (GameObject)Instantiate(CloudOuter, new Vector3(x, 0.0f, y), Quaternion.AngleAxis(270.0f, Vector3.up));
          cloudOuter.transform.SetParent(_cloudHolder.transform, false);
        }
      }
    }
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
    }

    if (hy >= _size)
    {
      cloudOuter = (GameObject)Instantiate(CloudOuter, new Vector3(x, 0.0f, y), Quaternion.AngleAxis(180.0f, Vector3.up));
      cloudOuter.transform.SetParent(_cloudHolder.transform, false);
    }

    if (lx < 0)
    {
      cloudOuter = (GameObject)Instantiate(CloudOuter, new Vector3(x, 0.0f, y), Quaternion.AngleAxis(90.0f, Vector3.up));
      cloudOuter.transform.SetParent(_cloudHolder.transform, false);
    }

    if (hx >= _size)
    {
      cloudOuter = (GameObject)Instantiate(CloudOuter, new Vector3(x, 0.0f, y), Quaternion.AngleAxis(270.0f, Vector3.up));
      cloudOuter.transform.SetParent(_cloudHolder.transform, false);
    }
  }

	void Update () 
	{
		
	}
}
