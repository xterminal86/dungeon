using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleScreen : MonoBehaviour 
{
  public GameObject Wall;
  public GameObject Floor;
  public GameObject Portcullis;
  public GameObject Stairs;
  public GameObject Torch;

  void Awake()
  {
    Vector3 cameraPos = Camera.main.transform.position;

    // Walls

    var go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 2, 0, cameraPos.z + 4), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 2, 0, cameraPos.z + 4), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x - 2, 2, cameraPos.z + 4), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x + 2, 2, cameraPos.z + 4), Quaternion.identity);
    go = (GameObject)Instantiate(Wall, new Vector3(cameraPos.x, 2, cameraPos.z + 4), Quaternion.identity);

    // Torches

    go = (GameObject)Instantiate(Torch, new Vector3(cameraPos.x - 1.5f, 0, cameraPos.z + 4), Quaternion.identity);
    go.transform.Rotate(Vector3.up, 180.0f, Space.World);

    go = (GameObject)Instantiate(Torch, new Vector3(cameraPos.x + 1.5f, 0, cameraPos.z + 4), Quaternion.identity);
    go.transform.Rotate(Vector3.up, 180.0f, Space.World);

    // Floor and ceiling

    go = (GameObject)Instantiate(Floor, new Vector3(cameraPos.x, 0, cameraPos.z + 4), Quaternion.identity);

    go = (GameObject)Instantiate(Floor, new Vector3(cameraPos.x, 2, cameraPos.z + 4), Quaternion.identity);
    go.transform.Rotate(Vector3.right, 180.0f, Space.World);

    go = (GameObject)Instantiate(Floor, new Vector3(cameraPos.x - 2, 0, cameraPos.z + 2), Quaternion.identity);
    go = (GameObject)Instantiate(Floor, new Vector3(cameraPos.x, 0, cameraPos.z + 2), Quaternion.identity);
    go = (GameObject)Instantiate(Floor, new Vector3(cameraPos.x + 2, 0, cameraPos.z + 2), Quaternion.identity);

    // Portcullis

    go = (GameObject)Instantiate(Portcullis, new Vector3(cameraPos.x, 0, cameraPos.z + 4), Quaternion.identity);
    go.transform.Rotate(Vector3.up, 180.0f, Space.World);

    // Stairs
  }

  public void NewGameHandler()
  {
    Application.LoadLevel("main");
  }
}
