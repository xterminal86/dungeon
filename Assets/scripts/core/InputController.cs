using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputController : MonoSingleton<InputController> 
{
  public Transform RaycastPoint;

  public Int2 PlayerMapPos = new Int2();

  CameraTurnArgument _cameraTurnArgument = new CameraTurnArgument();
  CameraMoveArgument _cameraMoveArgument = new CameraMoveArgument();
  void Awake () 
  {	
    _cameraTurnArgument.Speed = GlobalConstants.CameraTurnSpeed;
    _cameraMoveArgument.Speed = GlobalConstants.CameraMoveSpeed;
	}
	
  public void MapLoadingFinishedHandler()
  {
    _cameraAngles = App.Instance.CameraAngles;
    _cameraPos = App.Instance.CameraPos;
  }

  float _cameraBob = 0.0f;
  bool _isProcessing = false;
  Vector3 _cameraAngles = Vector3.zero;
  Vector3 _cameraPos = Vector3.zero;
	void Update () 
  {
    if (!_isProcessing)
    {
      ProcessKeyboard();
      ProcessMouse();
    }

    _cameraPos.y += _cameraBob;

    App.Instance.CameraPivot.transform.eulerAngles = _cameraAngles;
    App.Instance.CameraPos = _cameraPos;
    App.Instance.CameraPivot.transform.position = App.Instance.CameraPos;
	}

  bool _doMove = false;
  void ProcessKeyboard ()
  {
    _doMove = false;

    if (Input.GetKey(KeyCode.E)) 
    {      
      TurnCamera(App.Instance.CameraOrientation, App.Instance.CameraOrientation + 1, true);
    }
    else if (Input.GetKey(KeyCode.Q)) 
    {
      TurnCamera(App.Instance.CameraOrientation, App.Instance.CameraOrientation - 1, false);
    }
    else if (Input.GetKey(KeyCode.W)) 
    {
      _cameraMoveArgument.MoveType = CameraMoveType.FORWARD;
      _doMove = true;
    }
    else if (Input.GetKey(KeyCode.S)) 
    {
      _cameraMoveArgument.MoveType = CameraMoveType.BACKWARD;
      _doMove = true;
    }
    else if (Input.GetKey(KeyCode.A)) 
    {
      _cameraMoveArgument.MoveType = CameraMoveType.STRAFE_LEFT;
      _doMove = true;
    }
    else if (Input.GetKey(KeyCode.D)) 
    {
      _cameraMoveArgument.MoveType = CameraMoveType.STRAFE_RIGHT;
      _doMove = true;
    }
    else if (Input.GetKeyDown(KeyCode.Space))
    {
      //Debug.Log(App.Instance.GetMapObjectsByPosition(1, 3)[0]);
      //Debug.Log (App.Instance.GetMapObjectByName("door_1"));
      //Debug.Log (App.Instance.GetGameObjectByName("door_1"));
      //Debug.Log (App.Instance.GetMapObjectByName("test"));
    }

    if (_doMove)
    {
      int posX = (int)App.Instance.CameraPos.x;
      int posZ = (int)App.Instance.CameraPos.z;
      _cameraMoveArgument.From = new Vector2(posX, posZ);
      _cameraMoveArgument.Speed = GlobalConstants.CameraMoveSpeed;
      bool res = CanMove(posX, posZ, _cameraMoveArgument.MoveType);
      if (res)
      {
        StartCoroutine("CameraMoveRoutine", _cameraMoveArgument);
      }
      else
      {
        _cameraMoveArgument.Speed = GlobalConstants.CameraCannotMoveSpeed;
        StartCoroutine("CameraCannotMoveRoutine", _cameraMoveArgument);
      }
    }
  }

  RaycastHit _raycastHit;      
  void ProcessMouse()
  {
    if (Input.GetMouseButtonDown(0))
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      if (Physics.Raycast(ray.origin, ray.direction, out _raycastHit, GlobalConstants.WallScaleFactor + 1))
      {          
        if (_raycastHit.collider != null)
        {
          BehaviourMapObject bmo = _raycastHit.collider.gameObject.GetComponentInParent<BehaviourMapObject>();
          if (bmo != null)
          {            
            float d = Vector3.Distance(App.Instance.CameraPos, bmo.transform.position);
            int facing = Mathf.Abs(bmo.MapObjectInstance.Facing - App.Instance.CameraOrientation);

            float dCond = d - float.Epsilon;

            //Debug.Log(_raycastHit.distance + " " + d);

            //if (dCond <= GlobalConstants.WallScaleFactor && (facing == 2 || facing == 0))
            if ( (facing == 2 && dCond <= GlobalConstants.WallScaleFactor) || 
                 (facing == 0 && dCond <= 0.0f) )
            {
              if (bmo.MapObjectInstance.ActionCallback != null)
                bmo.MapObjectInstance.ActionCallback(bmo.MapObjectInstance);
            }
          }
        }
      }
    }
  }

  protected override void Init()
  {
    base.Init ();
  }

  bool CanMove(int posX, int posZ, CameraMoveType moveType)
  {
    int newX = (int)_cameraPos.x / GlobalConstants.WallScaleFactor;
    int newZ = (int)_cameraPos.z / GlobalConstants.WallScaleFactor;
    
    int xComponent = Mathf.RoundToInt (Mathf.Sin (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
    int zComponent = Mathf.RoundToInt (Mathf.Cos (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
    
    if (moveType == CameraMoveType.FORWARD)
    {
      if (xComponent != 0) newX += xComponent;
      if (zComponent != 0) newZ += zComponent;
    }
    else if (moveType == CameraMoveType.BACKWARD)
    {
      if (xComponent != 0) newX -= xComponent;
      if (zComponent != 0) newZ -= zComponent;
    }
    else if (moveType == CameraMoveType.STRAFE_LEFT)
    {
      if (xComponent != 0) newZ += xComponent;
      if (zComponent != 0) newX -= zComponent;
    }
    else if (moveType == CameraMoveType.STRAFE_RIGHT)
    {
      if (xComponent != 0) newZ -= xComponent;
      if (zComponent != 0) newX += zComponent;
    }

    // Check bounds
    if (newX < 0 || newX > App.Instance.MapRows - 1 || newZ < 0 || newZ > App.Instance.MapColumns - 1)
    {
      return false;
    }

    char emptyCell = App.Instance.GetMapLayoutPoint(newX, newZ);
    bool obstacleAhead = false;
        
    Ray ray = new Ray(RaycastPoint.position, new Vector3(xComponent, 0.0f, zComponent));

    Vector3 tmp = ray.direction;

    if (moveType == CameraMoveType.BACKWARD)
    {
      if (xComponent != 0) tmp.x = -ray.direction.x;
      if (zComponent != 0) tmp.z = -ray.direction.z;
      ray.direction = tmp;
    }
    else if (moveType == CameraMoveType.STRAFE_LEFT)
    {
      tmp = Vector3.zero;
      if (xComponent != 0) tmp.z = ray.direction.x;
      if (zComponent != 0) tmp.x = -ray.direction.z;
      ray.direction = tmp;
    }
    else if (moveType == CameraMoveType.STRAFE_RIGHT)
    {
      tmp = Vector3.zero;
      if (xComponent != 0) tmp.z = -ray.direction.x;
      if (zComponent != 0) tmp.x = ray.direction.z;
      ray.direction = tmp;
    }

    RaycastHit hit;      
    if (Physics.Raycast(ray, out hit, GlobalConstants.WallScaleFactor))
    {
      if (hit.collider != null)
      {
        obstacleAhead = true;
      }
    }

    //return (emptyCell == '.' && !obstacleAhead);
    return !obstacleAhead;
  }

  void TurnCamera(int from, int to, bool turnRight)
  {    
    if (from < 0) from = GlobalConstants.OrientationsMap.Count - 1;
    if (from == GlobalConstants.OrientationsMap.Count) from = 0;
    if (to < 0) to = GlobalConstants.OrientationsMap.Count - 1;
    if (to == GlobalConstants.OrientationsMap.Count) to = 0;

    _cameraTurnArgument.From = from;
    _cameraTurnArgument.To = to;
    _cameraTurnArgument.TurnRight = turnRight;
    StartCoroutine("CameraTurnRoutine", _cameraTurnArgument);
  }

  // Due to unknown (or, more precise, not researched) accumulation of error,
  // I had to hardcode resulting angle values after the turn.
  // Previously, it was possible to rotate more than 90 degress by pressing the same button
  // just after coroutine is finished (i.e. when rotation is finished).
  // This resulted in excess degree of about 9 during next rotation. 
  // Repeating the same process fucks up everything in the end.
  IEnumerator CameraTurnRoutine(object arg)
  {
    CameraTurnArgument ca = arg as CameraTurnArgument;
    if (ca == null) yield return null;
    _isProcessing = true;
    float cond = 0.0f;
    float toAngle = GlobalConstants.OrientationAngles[GlobalConstants.OrientationsMap[ca.To]];
    while (Mathf.Abs(cond) < 90.0f)
    {
      cond += Time.deltaTime * ca.Speed;

      if (cond + Time.deltaTime * ca.Speed > 90.0f)
      {
        break;
      }

      if (ca.TurnRight) _cameraAngles.y += Time.deltaTime * ca.Speed;
      else _cameraAngles.y -= Time.deltaTime * ca.Speed;
      yield return null;
    }

    _cameraAngles.y = toAngle;

    App.Instance.CameraOrientation = ca.To;

    _isProcessing = false;   
  }

  // Same thing as commented above here, but instead we get error in position.
  IEnumerator CameraMoveRoutine(object arg)
  {
    CameraMoveArgument ca = arg as CameraMoveArgument;
    if (ca == null) yield return null;
   
    _isProcessing = true;

    int newX = (int)_cameraPos.x;
    int newZ = (int)_cameraPos.z;

    int xComponent = Mathf.RoundToInt (Mathf.Sin (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
    int zComponent = Mathf.RoundToInt (Mathf.Cos (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));

    if (ca.MoveType == CameraMoveType.FORWARD)
    {
      if (xComponent != 0) newX += xComponent * GlobalConstants.WallScaleFactor;
      if (zComponent != 0) newZ += zComponent * GlobalConstants.WallScaleFactor;
    }
    else if (ca.MoveType == CameraMoveType.BACKWARD)
    {
      if (xComponent != 0) newX -= xComponent * GlobalConstants.WallScaleFactor;
      if (zComponent != 0) newZ -= zComponent * GlobalConstants.WallScaleFactor;
    }
    else if (ca.MoveType == CameraMoveType.STRAFE_LEFT)
    {
      if (xComponent != 0) newZ += xComponent * GlobalConstants.WallScaleFactor;
      if (zComponent != 0) newX -= zComponent * GlobalConstants.WallScaleFactor;
    }
    else if (ca.MoveType == CameraMoveType.STRAFE_RIGHT)
    {
      if (xComponent != 0) newZ -= xComponent * GlobalConstants.WallScaleFactor;
      if (zComponent != 0) newX += zComponent * GlobalConstants.WallScaleFactor;
    }

    int dx = (newX - (int)_cameraPos.x) / GlobalConstants.WallScaleFactor;
    int dz = (newZ - (int)_cameraPos.z) / GlobalConstants.WallScaleFactor;

    _cameraBob = 0.0f;

    bool _bobFlag = false;
    float cond = 0.0f;
    float half = GlobalConstants.WallScaleFactor / 2;
    while (cond < GlobalConstants.WallScaleFactor)
    {
      cond += Time.deltaTime * ca.Speed;

      if (cond > half)
      {
        _bobFlag = true;
      }

      if (!_bobFlag)
      {
        _cameraBob = Time.smoothDeltaTime * GlobalConstants.CameraBobSpeed;
      }
      else
      {
        _cameraBob = -Time.smoothDeltaTime * GlobalConstants.CameraBobSpeed;
      }

      /*
      if (cond + Time.smoothDeltaTime * ca.Speed > GlobalConstants.WallScaleFactor)
      {
        break;
      }
      */

      if (ca.MoveType == CameraMoveType.FORWARD)
      {
        if (xComponent != 0) _cameraPos.x += xComponent * Time.deltaTime * ca.Speed;
        else if (zComponent != 0) _cameraPos.z += zComponent * Time.deltaTime * ca.Speed;
      }
      else if (ca.MoveType == CameraMoveType.BACKWARD)
      {
        if (xComponent != 0) _cameraPos.x -= xComponent * Time.deltaTime * ca.Speed;
        else if (zComponent != 0) _cameraPos.z -= zComponent * Time.deltaTime * ca.Speed;
      }
      else if (ca.MoveType == CameraMoveType.STRAFE_LEFT)
      {
        if (xComponent != 0) _cameraPos.z += xComponent * Time.deltaTime * ca.Speed;
        else if (zComponent != 0) _cameraPos.x -= zComponent * Time.deltaTime * ca.Speed;
      }
      else if (ca.MoveType == CameraMoveType.STRAFE_RIGHT)
      {
        if (xComponent != 0) _cameraPos.z -= xComponent * Time.deltaTime * ca.Speed;
        else if (zComponent != 0) _cameraPos.x += zComponent * Time.deltaTime * ca.Speed;
      }

      yield return null;
    }

    _cameraBob = 0.0f;

    _cameraPos.x = newX;
    // Hardcoded camera pivot Y position - OK, since the actual camera is child of pivot.
    // If we want to change camera height, we should do this by changing the camera inside the pivot GameObject.
    _cameraPos.y = 0.0f;  
    _cameraPos.z = newZ;
         
    PlayerMapPos.X += dx;
    PlayerMapPos.Y += dz;

    if (App.Instance.FloorSoundTypeByPosition[PlayerMapPos.X, PlayerMapPos.Y] != -1)
    {
      SoundManager.Instance.PlayFootstepSound((GlobalConstants.FootstepSoundType)App.Instance.FloorSoundTypeByPosition[PlayerMapPos.X, PlayerMapPos.Y]);
    }

    _isProcessing = false;
  }

  IEnumerator CameraCannotMoveRoutine(object arg)
  {
    CameraMoveArgument ca = arg as CameraMoveArgument;
    if (ca == null) yield return null;

    SoundManager.Instance.PlaySound("player-cannot-move");

    _isProcessing = true;
    
    int newX = (int)_cameraPos.x;
    int newZ = (int)_cameraPos.z;
    
    int xComponent = Mathf.RoundToInt (Mathf.Sin (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
    int zComponent = Mathf.RoundToInt (Mathf.Cos (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));

    float cond = 0.0f;
    float nudge = (float)GlobalConstants.WallScaleFactor / 4.0f;
    float half = nudge / 2.0f;
    while (cond < nudge)
    {
      cond += Time.deltaTime * ca.Speed;

      /*
      if (cond + Time.deltaTime * ca.Speed > nudge)
      {
        break;
      }
      */

      if (ca.MoveType == CameraMoveType.FORWARD)
      {
        if (xComponent != 0)
        {
          if (cond < half) _cameraPos.x += xComponent * Time.deltaTime * ca.Speed;
          else _cameraPos.x -= xComponent * Time.deltaTime * ca.Speed;
        }
        else if (zComponent != 0) 
        {
          if (cond < half) _cameraPos.z += zComponent * Time.deltaTime * ca.Speed;
          else _cameraPos.z -= zComponent * Time.deltaTime * ca.Speed;
        }
      }
      else if (ca.MoveType == CameraMoveType.BACKWARD)
      {        
        if (xComponent != 0)
        {
          if (cond < half) _cameraPos.x -= xComponent * Time.deltaTime * ca.Speed;
          else _cameraPos.x += xComponent * Time.deltaTime * ca.Speed;
        }
        else if (zComponent != 0)
        {
          if (cond < half) _cameraPos.z -= zComponent * Time.deltaTime * ca.Speed;
          else _cameraPos.z += zComponent * Time.deltaTime * ca.Speed;
        }
      }
      else if (ca.MoveType == CameraMoveType.STRAFE_LEFT)
      {        
        if (xComponent != 0)
        {
          if (cond < half) _cameraPos.z += xComponent * Time.deltaTime * ca.Speed;
          else _cameraPos.z -= xComponent * Time.deltaTime * ca.Speed;
        }
        else if (zComponent != 0)
        {
          if (cond < half) _cameraPos.x -= zComponent * Time.deltaTime * ca.Speed;
          else _cameraPos.x += zComponent * Time.deltaTime * ca.Speed;
        }
      }
      else if (ca.MoveType == CameraMoveType.STRAFE_RIGHT)
      {        
        if (xComponent != 0)
        {
          if (cond < half) _cameraPos.z -= xComponent * Time.deltaTime * ca.Speed;
          else _cameraPos.z += xComponent * Time.deltaTime * ca.Speed;
        }
        else if (zComponent != 0)
        {
          if (cond < half) _cameraPos.x += zComponent * Time.deltaTime * ca.Speed;
          else _cameraPos.x -= zComponent * Time.deltaTime * ca.Speed;
        }
      }
      
      yield return null;
    }
    
    _cameraPos.x = newX;
    _cameraPos.z = newZ;
    
    _isProcessing = false;    
  }

  Vector3 _cameraForwardVector = Vector3.zero;
  public Vector3 GetCameraForwardVector()
  {
    int newX = (int)_cameraPos.x;
    int newZ = (int)_cameraPos.z;
    
    int xComponent = Mathf.RoundToInt (Mathf.Sin (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
    int zComponent = Mathf.RoundToInt (Mathf.Cos (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
    
    if (xComponent != 0)
    {
      newX += xComponent * GlobalConstants.WallScaleFactor;
    }

    if (zComponent != 0)
    {
      newZ += zComponent * GlobalConstants.WallScaleFactor;
    }

    _cameraForwardVector.x = xComponent;
    _cameraForwardVector.z = zComponent;

    return _cameraForwardVector;
  }
}

public class CameraTurnArgument
{
  public int From;
  public int To;
  public float Speed;
  public bool TurnRight;
}

public class CameraMoveArgument
{
  public Vector2 From;
  public Vector2 To;
  public float Speed;
  public CameraMoveType MoveType;
}

public enum CameraMoveType
{
  FORWARD = 0,
  BACKWARD,
  STRAFE_LEFT,
  STRAFE_RIGHT
}
