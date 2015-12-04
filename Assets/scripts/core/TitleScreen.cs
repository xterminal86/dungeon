﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour 
{
  public GameObject Wall;
  public GameObject Floor;
  public GameObject Stairs;
  public GameObject Torch;

  public Image ScreenFader;

  Color _screenFaderColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
  void Awake()
  {
    RenderSettings.fog = true;
    RenderSettings.fogMode = FogMode.Linear;
    RenderSettings.fogColor = Color.black;
    RenderSettings.fogStartDistance = 0;
    RenderSettings.fogEndDistance = 20;

    SetupScene();
  }

  public void NewGameHandler()
  {
    ScreenFader.gameObject.SetActive(true);
    StartCoroutine(NewGameRoutine());
  }

  public void ExitGameHandler()
  {
    Application.Quit();
  }

  float _fadeSpeed = 1.0f;
  IEnumerator NewGameRoutine()
  {
    float alpha = 0.0f;

    while (alpha < 1.0f)
    {
      _screenFaderColor.a = alpha;

      alpha += Time.smoothDeltaTime * _fadeSpeed;

      ScreenFader.color = _screenFaderColor;

      yield return null;
    }

    Application.LoadLevel("main");
  }

  void SetupScene()
  {
    Vector3 cameraPos = Camera.main.transform.position;
    
    // Walls
    
    var go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 2, 0, cameraPos.z + 4), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 2, 0, cameraPos.z + 4), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 2, 2, cameraPos.z + 4), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 2, 2, cameraPos.z + 4), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 4, 0, cameraPos.z + 4), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 4, 0, cameraPos.z + 4), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 4, 2, cameraPos.z + 4), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 4, 2, cameraPos.z + 4), Quaternion.identity);
    
    // Torches
    
    go = (GameObject)Instantiate(Torch, new Vector3(cameraPos.x - 1.5f, 0, cameraPos.z + 4), Quaternion.identity);
    go.transform.Rotate(Vector3.up, 180.0f, Space.World);
    
    go = (GameObject)Instantiate(Torch, new Vector3(cameraPos.x + 1.5f, 0, cameraPos.z + 4), Quaternion.identity);
    go.transform.Rotate(Vector3.up, 180.0f, Space.World);
    
    // Floor
    
    go = (GameObject)Instantiate(Floor, new Vector3(cameraPos.x - 2, 0, cameraPos.z + 2), Quaternion.identity);
    go = (GameObject)Instantiate(Floor, new Vector3(cameraPos.x, 0, cameraPos.z + 2), Quaternion.identity);
    go = (GameObject)Instantiate(Floor, new Vector3(cameraPos.x + 2, 0, cameraPos.z + 2), Quaternion.identity);
    go = (GameObject)Instantiate(Floor, new Vector3(cameraPos.x - 4, 0, cameraPos.z + 2), Quaternion.identity);
    go = (GameObject)Instantiate(Floor, new Vector3(cameraPos.x + 4, 0, cameraPos.z + 2), Quaternion.identity);
    
    // Stairs
    
    go = (GameObject)Instantiate(Stairs, new Vector3(cameraPos.x, 0, cameraPos.z + 4), Quaternion.identity);
    go.transform.Rotate(Vector3.up, 180.0f, Space.World);
    
    // Stairs hall
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 0, cameraPos.z + 6), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 2, cameraPos.z + 6), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 0, cameraPos.z + 6), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 2, cameraPos.z + 6), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 0, cameraPos.z + 8), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 2, cameraPos.z + 8), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 0, cameraPos.z + 8), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 2, cameraPos.z + 8), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 0, cameraPos.z + 10), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 2, cameraPos.z + 10), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 0, cameraPos.z + 10), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 2, cameraPos.z + 10), Quaternion.identity);
  }
}
