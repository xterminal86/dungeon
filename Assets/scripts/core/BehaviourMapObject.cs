using UnityEngine;
using System.Collections;

public class BehaviourMapObject : MonoBehaviour
{
  public AudioSource StartSound;
  public AudioSource EndSound;
  public AudioSource LoopingSound;

  public GameObject Model;

  public MapObject MapObjectInstance;

  Collider _collider;
  void Start()
  {
    _collider = GetComponentInChildren<BoxCollider>();
    if (LoopingSound != null)
    {
      LoopingSound.Play();
    }
  }

  void Update()
  {
    if (Input.GetMouseButtonDown(1)) 
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      RaycastHit hit;      
      if (_collider != null && _collider.Raycast(ray, out hit, GlobalConstants.WallScaleFactor)) 
      {
        //int facing = Mathf.Abs(App.Instance.CameraOrientation - MapObjectInstance.Facing);
        //if (facing == 2 && MapObjectInstance.ActionCallback != null)
        if (MapObjectInstance.ActionCallback != null)
        {
          MapObjectInstance.ActionCallback(MapObjectInstance);
        }
      }
    }
  }
}
