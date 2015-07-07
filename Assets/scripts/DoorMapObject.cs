using UnityEngine;
using System.Collections;

public class DoorMapObject : MapObject
{
  public bool DoorIsOpen = false;

  const float _doorMinY = 1.0f;
  const float _doorMaxY = 2.9f;
  const float _doorOpenSpeed = 1.0f;

  Job _job;
  public override void ActionCompleteHandler (object sender)
  {
    MapObject mo = sender as MapObject;
    if (mo != null)
    {
      Debug.Log(Name + ": I was toggled by " + mo.Name);
      if (_job != null)
      {
        _job.KillJob();
      }

      _job = new Job(DoorToggleRoutine());
    }
  }

  DoorMovingState _doorMovingState = DoorMovingState.STILL;
  IEnumerator DoorToggleRoutine()
  {
    Vector3 position = GameObjectToControl.transform.localPosition;

    if (!DoorIsOpen && _doorMovingState == DoorMovingState.STILL)
    {
      _doorMovingState = DoorMovingState.OPENING;

      while (position.y < _doorMaxY)
      {
        position.y += Time.smoothDeltaTime * _doorOpenSpeed;

        GameObjectToControl.transform.localPosition = position;

        yield return null;
      }

      _doorMovingState = DoorMovingState.STILL;
      DoorIsOpen = true;

      position.y = _doorMaxY;
      GameObjectToControl.transform.localPosition = position;
    }
    else if (!DoorIsOpen && _doorMovingState == DoorMovingState.OPENING)
    {
      _doorMovingState = DoorMovingState.CLOSING;
      DoorIsOpen = false;

      while (position.y > _doorMinY)
      {
        position.y -= Time.smoothDeltaTime * _doorOpenSpeed;
        
        GameObjectToControl.transform.localPosition = position;
        
        yield return null;
      }

      _doorMovingState = DoorMovingState.STILL;

      position.y = _doorMinY;
      GameObjectToControl.transform.localPosition = position;
    }
    else if (DoorIsOpen && _doorMovingState == DoorMovingState.STILL)
    {
      _doorMovingState = DoorMovingState.CLOSING;
      DoorIsOpen = false;

      while (position.y > _doorMinY)
      {
        position.y -= Time.smoothDeltaTime * _doorOpenSpeed;
        
        GameObjectToControl.transform.localPosition = position;
        
        yield return null;
      }

      _doorMovingState = DoorMovingState.STILL;
      
      position.y = _doorMinY;
      GameObjectToControl.transform.localPosition = position;
    }
    else if (!DoorIsOpen && _doorMovingState == DoorMovingState.CLOSING)
    {
      _doorMovingState = DoorMovingState.OPENING;     
      DoorIsOpen = false;

      while (position.y < _doorMaxY)
      {
        position.y += Time.smoothDeltaTime * _doorOpenSpeed;
        
        GameObjectToControl.transform.localPosition = position;
        
        yield return null;
      }
      
      _doorMovingState = DoorMovingState.STILL;
      DoorIsOpen = true;
      
      position.y = _doorMaxY;
      GameObjectToControl.transform.localPosition = position;
    }

    Debug.Log (Name + " open status: " + DoorIsOpen);
  }
}

enum DoorMovingState
{
  OPENING = 0,
  CLOSING,
  STILL
}
