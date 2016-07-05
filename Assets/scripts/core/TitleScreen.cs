using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;

public class TitleScreen : MonoBehaviour 
{
  public AudioSource TorchSound;
  public Transform SceneHolder;

  public GameObject Wall;
  public GameObject Floor;
  public GameObject Stairs;
  public GameObject Torch;
    
  void Awake()
  {    
    RenderSettings.fog = true;
    RenderSettings.fogMode = FogMode.Linear;
    RenderSettings.fogColor = Color.black;
    RenderSettings.fogStartDistance = 0;
    RenderSettings.fogEndDistance = 18;

    SetupScene();
  }

  void OnLevelWasLoaded(int level)
  {
    ScreenFader.Instance.FadeIn();
    TorchSound.Play();
  }

  public void NewGameHandler()
  {
    GUIManager.Instance.SetupNewGameForm();
    //ScreenFader.Instance.FadeCompleteCallback += FadeCompleteHandler;
    //ScreenFader.Instance.FadeOut();
  }

  public void StatisticsHandler()
  {    
  }

  public void OptionsHandler()
  {    
  }

  public void ExitGameHandler()
  {
    ScreenFader.Instance.FadeCompleteCallback += ExitGame;
    ScreenFader.Instance.FadeOut();    
  }
    
  void FadeCompleteHandler()
  {
    ScreenFader.Instance.FadeCompleteCallback -= FadeCompleteHandler;
    SceneManager.LoadScene("main");
  }

  void ExitGame()
  {
    ScreenFader.Instance.FadeCompleteCallback -= ExitGame;
    Application.Quit();
  }

  void SetupScene()
  {
    Vector3 cameraPos = Camera.main.transform.position;
    
    // Walls
    
    var go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 2, 0, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 2, 0, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 2, 2, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 2, 2, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 4, 0, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 4, 0, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 4, 2, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 4, 2, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    // Torches
    
    go = (GameObject)Instantiate(Torch, new Vector3(cameraPos.x - 2.0f, -0.125f, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;
    go.transform.Rotate(Vector3.up, 180.0f, Space.World);
    
    go = (GameObject)Instantiate(Torch, new Vector3(cameraPos.x + 2.0f, -0.125f, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;
    go.transform.Rotate(Vector3.up, 180.0f, Space.World);
    
    // Floor
    
    go = (GameObject)Instantiate(Floor, new Vector3(cameraPos.x - 2, 0, cameraPos.z + 2), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Floor, new Vector3(cameraPos.x, 0, cameraPos.z + 2), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Floor, new Vector3(cameraPos.x + 2, 0, cameraPos.z + 2), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Floor, new Vector3(cameraPos.x - 4, 0, cameraPos.z + 2), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Floor, new Vector3(cameraPos.x + 4, 0, cameraPos.z + 2), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    // Stairs
    
    go = (GameObject)Instantiate(Stairs, new Vector3(cameraPos.x, 0, cameraPos.z + 4), Quaternion.identity);
    go.transform.parent = SceneHolder;
    go.transform.Rotate(Vector3.up, 180.0f, Space.World);

    go = (GameObject)Instantiate(Stairs, new Vector3(cameraPos.x, 4, cameraPos.z + 15), Quaternion.identity);
    go.transform.parent = SceneHolder;
    go.transform.Rotate(Vector3.up, 180.0f, Space.World);
    
    // Stairs hall
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 0, cameraPos.z + 6), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 2, cameraPos.z + 6), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 0, cameraPos.z + 6), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 2, cameraPos.z + 6), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 0, cameraPos.z + 8), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 2, cameraPos.z + 8), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 0, cameraPos.z + 8), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 2, cameraPos.z + 8), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 0, cameraPos.z + 10), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 2, cameraPos.z + 10), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 0, cameraPos.z + 10), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 2, cameraPos.z + 10), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 4, cameraPos.z + 10), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 4, cameraPos.z + 10), Quaternion.identity);
    go.transform.parent = SceneHolder;

    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 2, cameraPos.z + 12), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 2, cameraPos.z + 12), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 4, cameraPos.z + 12), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 4, cameraPos.z + 12), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 2, cameraPos.z + 14), Quaternion.identity);
    go.transform.parent = SceneHolder;
        
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 2, cameraPos.z + 14), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 1.8f, 4, cameraPos.z + 14), Quaternion.identity);
    go.transform.parent = SceneHolder;
    
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 1.8f, 4, cameraPos.z + 14), Quaternion.identity);
    go.transform.parent = SceneHolder;
  }
}
