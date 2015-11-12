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
      if (_collider != null && _collider.Raycast(ray, out hit, GlobalConstants.WallScaleFactor + 1)) 
      {
        // TODO: Disable interaction when not facing the object

        /*
        Vector3 rayTmp = ray.origin;
        rayTmp.y = 0.0f;
        Vector3 hitTmp = hit.point;
        hitTmp.y = 0.0f;
        Vector3 v1 = hitTmp - rayTmp;
        //Vector3 v1 = rayTmp - hitTmp;
        Vector3 v2 = InputController.Instance.GetCameraForwardVector();
        float angle = Vector3.Angle(v1, v2);
        float dot = Vector3.Dot(v1, v2);

        //Debug.DrawLine(hit.point, ray.origin, Color.yellow, 100.0f);
        Debug.Log(v1 + " " + v2 + " " + angle + " " + dot + " " + hit.distance);

        if (angle < 28.0f && hit.distance < GlobalConstants.WallScaleFactor)
        */
        {
          if (MapObjectInstance.ActionCallback != null)
            MapObjectInstance.ActionCallback(MapObjectInstance);
        }
      }
    }
  }
}
