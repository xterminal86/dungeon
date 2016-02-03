using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputController : MonoSingleton<InputController> 
{  
  public Transform RaycastPoint;

  public Int2 PlayerMapPos = new Int2();
  public Int2 PlayerPreviousMapPos = new Int2();

  Vector3 _centerOfScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0.0f);

  float _raycastDistance = GlobalConstants.WallScaleFactor + GlobalConstants.WallScaleFactor / 2;

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
    _compassSpriteAngles.z = _cameraAngles.y + 90.0f;
  }

  float _cameraBob = 0.0f;
  bool _isProcessing = false;
  Vector3 _cameraAngles = Vector3.zero;
  Vector3 _compassSpriteAngles = Vector3.zero;
  Vector3 _cameraPos = Vector3.zero;
  float _currentMoveSpeed = 0.0f;
	void Update () 
  {
    if (App.Instance.PlayerMoveState == App.PlayerMoveStateEnum.HOLD_PLAYER
     || App.Instance.CurrentGameState != App.GameState.RUNNING)
    {
      return;
    }

    if (!_isProcessing)
    {
      ProcessKeyboard();
      ProcessMouse();
    }

    _cameraPos.y += _cameraBob;

    App.Instance.CameraPivot.transform.eulerAngles = _cameraAngles;
    App.Instance.CameraPos = _cameraPos;
    App.Instance.CameraPivot.transform.position = App.Instance.CameraPos;

    GUIManager.Instance.CompassImage.transform.eulerAngles = _compassSpriteAngles;
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
      Ray ray = Camera.main.ScreenPointToRay(_centerOfScreen);

      if (Physics.Raycast(ray.origin, ray.direction, out _raycastHit, _raycastDistance))
      {
        //Debug.DrawRay(ray.origin, ray.direction * _raycastDistance, Color.yellow, 10.0f, false);

        if (_raycastHit.collider != null)
        {
          ModelMover mm = _raycastHit.collider.gameObject.GetComponent<ModelMover>();
          if (mm != null)
          {
            mm.Actor.Interact();
          }
        }
      }
    }
    else if (Input.GetKeyDown(KeyCode.Escape))
    {
      App.Instance.PlayerMoveState = App.PlayerMoveStateEnum.HOLD_PLAYER;
      GUIManager.Instance.InventoryFormWindow.gameObject.SetActive(false);
      GUIManager.Instance.FormGameMenu.SetActive(true);
    }

    _currentMoveSpeed = GlobalConstants.CameraMoveSpeed;

    if (Input.GetKey(KeyCode.LeftShift))
    {
      _currentMoveSpeed = GlobalConstants.CameraMoveSpeed * 2.0f;
    }

    if (_doMove)
    {
      int posX = (int)App.Instance.CameraPos.x;
      int posZ = (int)App.Instance.CameraPos.z;
      _cameraMoveArgument.From = new Vector2(posX, posZ);
      _cameraMoveArgument.Speed = _currentMoveSpeed;
      bool res = CanMove(posX, posZ, _cameraMoveArgument.MoveType);
      if (res)
      {
        StartCoroutine(CameraMoveRoutine(_cameraMoveArgument));
      }
      else
      {
        _cameraMoveArgument.Speed = GlobalConstants.CameraCannotMoveSpeed;
        StartCoroutine(CameraCannotMoveRoutine(_cameraMoveArgument));
      }
    }
  }

  RaycastHit _raycastHit;      
  void ProcessMouse()
  {
    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
    {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      // If we hit something before us
      if (Physics.Raycast(ray.origin, ray.direction, out _raycastHit, GlobalConstants.WallScaleFactor + 1))
      {          
        if (_raycastHit.collider != null)
        {
          BehaviourMapObject bmo = _raycastHit.collider.gameObject.GetComponentInParent<BehaviourMapObject>();
          if (bmo != null)
          {            
            ProcessBMO(bmo);
          }

          BehaviourItemObject bio = _raycastHit.collider.gameObject.GetComponentInParent<BehaviourItemObject>();
          if (bio != null)
          {
            // If we have item in hand and click on another item on the floor,
            // item in hand is put down
            if (GUIManager.Instance.ItemTaken != null)
            {
              PutItem();
            }
            else
            {
              ProcessBIO(bio);
            }
          }
        }
      }
      else
      {
        // If floor is empty, put item down
        if (GUIManager.Instance.ItemTaken != null)
        {
          PutItem();
        }
      }       
    }
  }
    
  Vector3 _itemPos = Vector3.zero;
  Vector3 _itemRotation = Vector3.zero;
  void PutItem()
  {
    int x = PlayerMapPos.X;
    int y = PlayerMapPos.Y;

    var o = App.Instance.CameraOrientation;

    if (GlobalConstants.OrientationsMap[o] == GlobalConstants.Orientation.NORTH)
    {
      x--;
    }
    else if (GlobalConstants.OrientationsMap[o] == GlobalConstants.Orientation.EAST) 
    {
      y++;
    }
    else if (GlobalConstants.OrientationsMap[o] == GlobalConstants.Orientation.SOUTH) 
    {
      x++;
    }
    else if (GlobalConstants.OrientationsMap[o] == GlobalConstants.Orientation.WEST) 
    {
      y--;
    }

    // Check bounds
    if (x < 0 || x > App.Instance.MapRows - 1 || y < 0 || y > App.Instance.MapColumns - 1)
    {
      return;
    }

    if (!App.Instance.GeneratedMap.PathfindingMap[x, y].Walkable) return;

    SoundManager.Instance.PlaySound(GlobalConstants.SFXItemPut);
    
    GUIManager.Instance.ItemTakenSprite.gameObject.SetActive(false);

    _itemPos.x = x * GlobalConstants.WallScaleFactor;
    _itemPos.z = y * GlobalConstants.WallScaleFactor;

    _itemRotation = GUIManager.Instance.ItemTaken.BIO.gameObject.transform.eulerAngles;

    GlobalConstants.Orientation or = GlobalConstants.OrientationsMap[App.Instance.CameraOrientation];
    _itemRotation.y = GlobalConstants.OrientationAngles[or];

    GUIManager.Instance.ItemTaken.BIO.transform.position = _itemPos;
    GUIManager.Instance.ItemTaken.BIO.transform.eulerAngles = _itemRotation;
    GUIManager.Instance.ItemTaken.BIO.CalculateMapPosition();
    GUIManager.Instance.ItemTaken.BIO.gameObject.SetActive(true);
    GUIManager.Instance.ItemTaken = null;
  }

  void ProcessBMO(BehaviourMapObject bmo)
  {
    float d = Vector3.Distance(App.Instance.CameraPos, bmo.transform.position);
    int facing = Mathf.Abs(bmo.MapObjectInstance.Facing - App.Instance.CameraOrientation);
    float dCond = d - float.Epsilon;
    //Debug.Log(_raycastHit.distance + " " + d);
    //if (dCond <= GlobalConstants.WallScaleFactor && (facing == 2 || facing == 0))
    if ((facing == 2 && dCond <= GlobalConstants.WallScaleFactor) || (facing == 0 && dCond <= 0.0f) || (bmo.transform.position.x == App.Instance.CameraPos.x && bmo.transform.position.z == App.Instance.CameraPos.z)) 
    {
      if (bmo.MapObjectInstance.ActionCallback != null)
        bmo.MapObjectInstance.ActionCallback(bmo.MapObjectInstance);
    }
  }

  void ProcessBIO(BehaviourItemObject bio)
  {
    int dx = PlayerMapPos.X - bio.MapPosition.X;
    int dy = PlayerMapPos.Y - bio.MapPosition.Y;

    // If object is on the same line as camera and can be taken
    if ((dx == 0 || dy == 0) && bio.CanBeTaken)
    {
      if (bio.ItemObjectInstance.LMBAction != null)
        bio.ItemObjectInstance.LMBAction(this);
    }
  }

  protected override void Init()
  {
    base.Init ();
  }

  bool CanMove(int posX, int posZ, CameraMoveType moveType)
  {
    // We might want to look into map array, so we use map coordinates (i.e. row and column)

    int newX = PlayerMapPos.X;
    int newZ = PlayerMapPos.Y;

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
        obstacleAhead = (hit.collider.gameObject.GetComponent<ModelMover>() == null);
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
    StartCoroutine(CameraTurnRoutine(_cameraTurnArgument));
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
      cond += Time.smoothDeltaTime * ca.Speed;

      if (cond + Time.smoothDeltaTime * ca.Speed > 90.0f)
      {
        break;
      }

      if (ca.TurnRight)
      {
        _cameraAngles.y += Time.smoothDeltaTime * ca.Speed;
        _compassSpriteAngles.z += Time.smoothDeltaTime * ca.Speed;
      }
      else
      {
        _cameraAngles.y -= Time.smoothDeltaTime * ca.Speed;
        _compassSpriteAngles.z -= Time.smoothDeltaTime * ca.Speed;
      }
      yield return null;
    }

    _cameraAngles.y = toAngle;
    _compassSpriteAngles.z = 90.0f + toAngle;

    App.Instance.CameraOrientation = ca.To;

    _isProcessing = false;   
  }

  // Same thing as commented above here, but instead we get error in position.
  IEnumerator CameraMoveRoutine(object arg)
  {
    CameraMoveArgument ca = arg as CameraMoveArgument;
    if (ca == null) yield return null;
   
    _isProcessing = true;

    // We move camera game object, so we work in world space here

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
      cond += Time.smoothDeltaTime * ca.Speed;

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
        if (xComponent != 0) _cameraPos.x += xComponent * Time.smoothDeltaTime * ca.Speed;
        else if (zComponent != 0) _cameraPos.z += zComponent * Time.smoothDeltaTime * ca.Speed;
      }
      else if (ca.MoveType == CameraMoveType.BACKWARD)
      {
        if (xComponent != 0) _cameraPos.x -= xComponent * Time.smoothDeltaTime * ca.Speed;
        else if (zComponent != 0) _cameraPos.z -= zComponent * Time.smoothDeltaTime * ca.Speed;
      }
      else if (ca.MoveType == CameraMoveType.STRAFE_LEFT)
      {
        if (xComponent != 0) _cameraPos.z += xComponent * Time.smoothDeltaTime * ca.Speed;
        else if (zComponent != 0) _cameraPos.x -= zComponent * Time.smoothDeltaTime * ca.Speed;
      }
      else if (ca.MoveType == CameraMoveType.STRAFE_RIGHT)
      {
        if (xComponent != 0) _cameraPos.z -= xComponent * Time.smoothDeltaTime * ca.Speed;
        else if (zComponent != 0) _cameraPos.x += zComponent * Time.smoothDeltaTime * ca.Speed;
      }

      yield return null;
    }

    _cameraBob = 0.0f;

    _cameraPos.x = newX;
    // Hardcoded camera pivot Y position - OK, since the actual camera is child of pivot.
    // If we want to change camera height, we should do this by changing the camera inside the pivot GameObject.
    _cameraPos.y = 0.0f;  
    _cameraPos.z = newZ;
         
    PlayerPreviousMapPos.X = PlayerMapPos.X;
    PlayerPreviousMapPos.Y = PlayerMapPos.Y;

    PlayerMapPos.X += dx;
    PlayerMapPos.Y += dz;
        
    if (App.Instance.FloorSoundTypeByPosition[PlayerMapPos.X, PlayerMapPos.Y] != -1)
    {
      SoundManager.Instance.PlayFootstepSoundPlayer((GlobalConstants.FootstepSoundType)App.Instance.FloorSoundTypeByPosition[PlayerMapPos.X, PlayerMapPos.Y]);
      //SoundManager.Instance.PlayFootstepSoundPlayer((GlobalConstants.FootstepSoundType)App.Instance.FloorSoundTypeByPosition[PlayerMapPos.X, PlayerMapPos.Y], App.Instance.CameraPos);
    }

    _isProcessing = false;
  }

  IEnumerator CameraCannotMoveRoutine(object arg)
  {
    CameraMoveArgument ca = arg as CameraMoveArgument;
    if (ca == null) yield return null;

    SoundManager.Instance.PlaySound(GlobalConstants.SFXPlayerCannotMove);

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
      cond += Time.smoothDeltaTime * ca.Speed;

      /*
      if (cond + Time.smoothDeltaTime * ca.Speed > nudge)
      {
        break;
      }
      */

      if (ca.MoveType == CameraMoveType.FORWARD)
      {
        if (xComponent != 0)
        {
          if (cond < half) _cameraPos.x += xComponent * Time.smoothDeltaTime * ca.Speed;
          else _cameraPos.x -= xComponent * Time.smoothDeltaTime * ca.Speed;
        }
        else if (zComponent != 0) 
        {
          if (cond < half) _cameraPos.z += zComponent * Time.smoothDeltaTime * ca.Speed;
          else _cameraPos.z -= zComponent * Time.smoothDeltaTime * ca.Speed;
        }
      }
      else if (ca.MoveType == CameraMoveType.BACKWARD)
      {        
        if (xComponent != 0)
        {
          if (cond < half) _cameraPos.x -= xComponent * Time.smoothDeltaTime * ca.Speed;
          else _cameraPos.x += xComponent * Time.smoothDeltaTime * ca.Speed;
        }
        else if (zComponent != 0)
        {
          if (cond < half) _cameraPos.z -= zComponent * Time.smoothDeltaTime * ca.Speed;
          else _cameraPos.z += zComponent * Time.smoothDeltaTime * ca.Speed;
        }
      }
      else if (ca.MoveType == CameraMoveType.STRAFE_LEFT)
      {        
        if (xComponent != 0)
        {
          if (cond < half) _cameraPos.z += xComponent * Time.smoothDeltaTime * ca.Speed;
          else _cameraPos.z -= xComponent * Time.smoothDeltaTime * ca.Speed;
        }
        else if (zComponent != 0)
        {
          if (cond < half) _cameraPos.x -= zComponent * Time.smoothDeltaTime * ca.Speed;
          else _cameraPos.x += zComponent * Time.smoothDeltaTime * ca.Speed;
        }
      }
      else if (ca.MoveType == CameraMoveType.STRAFE_RIGHT)
      {        
        if (xComponent != 0)
        {
          if (cond < half) _cameraPos.z -= xComponent * Time.smoothDeltaTime * ca.Speed;
          else _cameraPos.z += xComponent * Time.smoothDeltaTime * ca.Speed;
        }
        else if (zComponent != 0)
        {
          if (cond < half) _cameraPos.x += zComponent * Time.smoothDeltaTime * ca.Speed;
          else _cameraPos.x -= zComponent * Time.smoothDeltaTime * ca.Speed;
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
