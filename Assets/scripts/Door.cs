using UnityEngine;
using System.Collections;

public class Door : MapObject
{
  public GameObject DoorModel;

  const float _doorClosedY = 1.0f;
  const float _doorOpenY = 2.9f;

  [HideInInspector]
  public bool IsDoorOpen = false;

  public void Setup(Vector3 position, bool status)
  {
    IsDoorOpen = status;
    _doorPosition = DoorModel.transform.position;
  }

  Vector3 _doorPosition = Vector3.zero;
  void Update()
  {
    DoorModel.transform.position = _doorPosition;
  }

  /*
  IEnumerator ToggleDoor()
  {
    float cond = _doorPosition.y;
    if (cond < _doorOpenY)
    {
      while (cond < _doorOpenY)
      {
        cond += Time.smoothDeltaTime * GlobalConstants.DoorOpenSpeed;        
        _doorPosition.y += Time.smoothDeltaTime * GlobalConstants.DoorOpenSpeed;
        yield return null;
      }
    }
    else
    {
      while (cond > _doorClosedY)
      {
        cond -= Time.smoothDeltaTime * GlobalConstants.DoorOpenSpeed;        
        _doorPosition.y -= Time.smoothDeltaTime * GlobalConstants.DoorOpenSpeed;
        yield return null;
      }
    }


    if (cond >= _doorOpenY)
    {
      _doorPosition.y = _doorOpenY;
      IsDoorOpen = true;
    }
    else if (cond <= _doorClosedY)
    {
      _doorPosition.y = _doorClosedY;
      IsDoorOpen = false;
    }
  }
  */
}
