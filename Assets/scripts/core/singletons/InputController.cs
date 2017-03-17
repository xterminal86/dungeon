using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputController : MonoSingleton<InputController>
{ 
  public Transform CameraHolder;

  // Origin of raycast ray for checking move availability status.
  // Goes from center of the tile that player (camera) currently occupies (same height as the camera).
  //
  // Camera itself is moved back a little from center of the tile as a child transform, so
  // when player rotates, the camera also rotates automatically as a child.
  public Transform CanMoveRayOrigin;

  public Text DebugText;

  public Int3 PlayerMapPos = new Int3();

  Vector3 _cameraPos = Vector3.zero;
  public Vector3 CameraPos
  {
    set { _cameraPos = value; }
    get { return _cameraPos; }
  }

  Vector3 _cameraAngles = Vector3.zero;
  public Vector3 CameraAngles
  {
    get { return _cameraAngles; }
  }

  GlobalConstants.Orientation _cameraOrientation = GlobalConstants.Orientation.EAST;
  public GlobalConstants.Orientation CameraOrientation
  {
    get { return _cameraOrientation; }
  }

  public void SetupCamera(Int3 position, GlobalConstants.Orientation orientation = GlobalConstants.Orientation.EAST)
  { 
    _cameraPos.x = position.X * GlobalConstants.WallScaleFactor;
    _cameraPos.y = position.Y * GlobalConstants.WallScaleFactor;
    _cameraPos.z = position.Z * GlobalConstants.WallScaleFactor;

    CameraHolder.position = _cameraPos;

    // Teleportation uses this method, so without resetting eulerAngles first we'll get wrong rotation
    _cameraAngles = Vector3.zero;
    CameraHolder.eulerAngles = _cameraAngles;

    CameraHolder.Rotate(Vector3.up, GlobalConstants.OrientationAngles[orientation]);
    _cameraAngles = CameraHolder.eulerAngles;

    _cameraOrientation = orientation;

    PlayerMapPos.X = position.X;
    PlayerMapPos.Y = position.Y;
    PlayerMapPos.Z = position.Z;
  }

  float _raycastDistance = GlobalConstants.WallScaleFactor + GlobalConstants.WallScaleFactor / 2;

  CameraTurnArgument _cameraTurnArgument = new CameraTurnArgument();
  CameraMoveArgument _cameraMoveArgument = new CameraMoveArgument();
  public override void Initialize()
  {        
    _cameraTurnArgument.Speed = GlobalConstants.CameraTurnSpeed;
    _cameraMoveArgument.Speed = GlobalConstants.CameraMoveSpeed;
    _compassSpriteAngles.z = _cameraAngles.y + 90.0f;
	}
	
  float _cameraBob = 0.0f;
  bool _isProcessing = false;
  Vector3 _compassSpriteAngles = Vector3.zero;
  float _currentMoveSpeed = 0.0f;
	void Update () 
  {
    if (GameData.Instance.PlayerMoveState == GameData.PlayerMoveStateEnum.HOLD_PLAYER
      || GameData.Instance.CurrentGameState != GameData.GameState.RUNNING)
    {
      return;
    }

    if (!_isProcessing)
    {
      ProcessKeyboard();
      ProcessMouse();
    }

    //_cameraPos.y += _cameraBob;

    CameraHolder.eulerAngles = _cameraAngles;
    CameraHolder.position = _cameraPos;

    GUIManager.Instance.CompassImage.transform.eulerAngles = _compassSpriteAngles;

    DebugText.text = string.Format("[{0};{1};{2}] - {3}\n{4:F2}", PlayerMapPos.X, PlayerMapPos.Y, PlayerMapPos.Z, CameraOrientation, _cameraAngles);
	}

  bool _doMove = false;
  void ProcessKeyboard ()
  {
    _doMove = false;

    int cameraOrientation = (int)_cameraOrientation;

    if (Input.GetKey(KeyCode.E))
    {      
      TurnCamera(cameraOrientation, cameraOrientation + 1, true);
    }
    else if (Input.GetKey(KeyCode.Q))
    {
      TurnCamera(cameraOrientation, cameraOrientation - 1, false);
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
    else if (Input.GetKeyDown(KeyCode.F))
    {
      CheckAndProcessClimbing();
    }
    else if (Input.GetKeyDown(KeyCode.Space))
    {      
      // Cast a ray forward from camera's (player) world position
      Ray ray = new Ray(Camera.main.transform.position, GetCameraForwardVector());

      //Debug.DrawRay(ray.origin, ray.direction * GlobalConstants.WallScaleFactor, Color.red, 10.0f);

      if (Physics.Raycast(ray.origin, ray.direction, out _raycastHit, _raycastDistance))
      {        
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
      GameData.Instance.PlayerMoveState = GameData.PlayerMoveStateEnum.HOLD_PLAYER;
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
      int posX = (int)_cameraPos.x;
      int posZ = (int)_cameraPos.z;
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

  Int3 _playerPosCached = new Int3();
  void CheckAndProcessClimbing()
  {
    // Try to get block before player

    _playerPosCached.X = PlayerMapPos.X;
    _playerPosCached.Y = PlayerMapPos.Y;
    _playerPosCached.Z = PlayerMapPos.Z;

    switch (CameraOrientation)
    {
      case GlobalConstants.Orientation.NORTH:
        _playerPosCached.X--;
        break;

      case GlobalConstants.Orientation.SOUTH:
        _playerPosCached.X++;
        break;

      case GlobalConstants.Orientation.EAST:
        _playerPosCached.Z++;
        break;

      case GlobalConstants.Orientation.WEST:
        _playerPosCached.Z--;
        break;
    }

    Int3 newPos = new Int3(_playerPosCached);

    newPos.Y++;
      
    BlockEntity blockBefore = LevelLoader.Instance.GetBlockByCoordinates(_playerPosCached);

    _playerPosCached.Y++;

    BlockEntity blockAboveBefore = LevelLoader.Instance.GetBlockByCoordinates(_playerPosCached);

    if (blockBefore == null)
    {      
      Debug.LogWarning("Map bounds reached!");
      return;
    }

    // Block above player

    _playerPosCached.Set(PlayerMapPos);

    _playerPosCached.Y++;

    BlockEntity blockAbove = LevelLoader.Instance.GetBlockByCoordinates(_playerPosCached);

    if (blockAbove == null || blockAboveBefore == null)
    {      
      Debug.LogWarning("Map roof reached!");
      return;
    }

    GlobalConstants.Orientation oppositeOrientation = GetOppositeOrientation(CameraOrientation);

    if (blockBefore.BlockType != GlobalConstants.BlockType.AIR 
     && blockAbove.BlockType == GlobalConstants.BlockType.AIR
     && blockAboveBefore.BlockType == GlobalConstants.BlockType.AIR
     && blockAbove.SidesWalkability[CameraOrientation] && blockAboveBefore.SidesWalkability[oppositeOrientation])
    {
      StartCoroutine(ClimbingRoutine(newPos));
    }
  }

  float _climbSpeed = 2.0f;
  float _cameraClimbPullFactor = 150.0f;
  IEnumerator ClimbingRoutine(Int3 newPlayerPos)
  {
    _isProcessing = true;

    if (GameData.Instance.PlayerCharacterVariable.IsFemale)
    {
      SoundManager.Instance.PlaySound(GlobalConstants.SFXPlayerPullStartFemale);
    }
    else
    {
      SoundManager.Instance.PlaySound(GlobalConstants.SFXPlayerPullStartMale);
    }

    float oldY = PlayerMapPos.Y;
    float newY = PlayerMapPos.Y + 1;

    // Pull camera down

    while (oldY < newY)
    {
      oldY += Time.smoothDeltaTime * _climbSpeed;

      _cameraAngles.x += Time.smoothDeltaTime * _cameraClimbPullFactor;
      _cameraAngles.x = Mathf.Clamp((int)_cameraAngles.x, 0, 90);

      _cameraPos.y = oldY * GlobalConstants.WallScaleFactor;
      _cameraPos.y = Mathf.Clamp(_cameraPos.y, oldY * GlobalConstants.WallScaleFactor, newY * GlobalConstants.WallScaleFactor);

      yield return null;
    }

    if (GameData.Instance.PlayerCharacterVariable.IsFemale)
    {
      SoundManager.Instance.PlaySound(GlobalConstants.SFXPlayerPullEndFemale);
    }
    else
    {
      SoundManager.Instance.PlaySound(GlobalConstants.SFXPlayerPullEndMale);
    }

    float oldX = PlayerMapPos.X;
    float oldZ = PlayerMapPos.Z;
    float newX = newPlayerPos.X;
    float newZ = newPlayerPos.Z;
          
    // Pull camera back and move to new position depending on player's facing

    if (newPlayerPos.X != PlayerMapPos.X)
    { 
      if (oldX < newX)
      {
        while (oldX < newX)
        {
          oldX += Time.smoothDeltaTime * _climbSpeed;

          _cameraPos.x = oldX * GlobalConstants.WallScaleFactor;
          _cameraPos.x = Mathf.Clamp(_cameraPos.x, oldX * GlobalConstants.WallScaleFactor, newX * GlobalConstants.WallScaleFactor);

          _cameraAngles.x -= Time.smoothDeltaTime * _cameraClimbPullFactor;
          _cameraAngles.x = Mathf.Clamp((int)_cameraAngles.x, 0, 90);

          yield return null;
        }
      }
      else
      {
        while (oldX > newX)
        {
          oldX -= Time.smoothDeltaTime * _climbSpeed;

          _cameraPos.x = oldX * GlobalConstants.WallScaleFactor;
          _cameraPos.x = Mathf.Clamp(_cameraPos.x, newX * GlobalConstants.WallScaleFactor, oldX * GlobalConstants.WallScaleFactor);

          _cameraAngles.x -= Time.smoothDeltaTime * _cameraClimbPullFactor;
          _cameraAngles.x = Mathf.Clamp((int)_cameraAngles.x, 0, 90);

          yield return null;
        }
      }
    }
    else if (newPlayerPos.Z != PlayerMapPos.Z)
    {      
      if (oldZ < newZ)
      {
        while (oldZ < newZ)
        {
          oldZ += Time.smoothDeltaTime * _climbSpeed;

          _cameraPos.z = oldZ * GlobalConstants.WallScaleFactor;
          _cameraPos.z = Mathf.Clamp(_cameraPos.z, oldZ * GlobalConstants.WallScaleFactor, newZ * GlobalConstants.WallScaleFactor);

          _cameraAngles.x -= Time.smoothDeltaTime * _cameraClimbPullFactor;
          _cameraAngles.x = Mathf.Clamp((int)_cameraAngles.x, 0, 90);

          yield return null;
        }
      }
      else
      {
        while (oldZ > newZ)
        {
          oldZ -= Time.smoothDeltaTime * _climbSpeed;

          _cameraPos.z = oldZ * GlobalConstants.WallScaleFactor;
          _cameraPos.z = Mathf.Clamp(_cameraPos.z, newZ * GlobalConstants.WallScaleFactor, oldZ * GlobalConstants.WallScaleFactor);

          _cameraAngles.x -= Time.smoothDeltaTime * _cameraClimbPullFactor;
          _cameraAngles.x = Mathf.Clamp((int)_cameraAngles.x, 0, 90);

          yield return null;
        }
      }
    }

    PlayerMapPos.Set(newPlayerPos);

    _isProcessing = false;

    yield return null;
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
          BehaviourWorldObject bwo = _raycastHit.collider.gameObject.GetComponentInParent<BehaviourWorldObject>();
          if (bwo != null)
          {            
            ProcessBWO(bwo);
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
    int z = PlayerMapPos.Z;

    if (_cameraOrientation == GlobalConstants.Orientation.NORTH)
    {
      x--;
    }
    else if (_cameraOrientation == GlobalConstants.Orientation.EAST) 
    {
      z++;
    }
    else if (_cameraOrientation == GlobalConstants.Orientation.SOUTH) 
    {
      x++;
    }
    else if (_cameraOrientation == GlobalConstants.Orientation.WEST) 
    {
      z--;
    }

    // Check bounds
    if (x < 0 || x > LevelLoader.Instance.LevelSize.X - 1 || z < 0 || z > LevelLoader.Instance.LevelSize.Z - 1)
    {
      return;
    }

    if (!LevelLoader.Instance.LevelMap.Level[x, y, z].Walkable)
    {      
      return;
    }

    SoundManager.Instance.PlaySound(GlobalConstants.SFXItemPut);
    
    GUIManager.Instance.ItemTakenSprite.gameObject.SetActive(false);

    _itemPos.x = x * GlobalConstants.WallScaleFactor;
    _itemPos.y = y * GlobalConstants.WallScaleFactor;
    _itemPos.z = z * GlobalConstants.WallScaleFactor;

    _itemRotation = GUIManager.Instance.ItemTaken.BIO.gameObject.transform.eulerAngles;

    _itemRotation.y = GlobalConstants.OrientationAngles[_cameraOrientation];

    GUIManager.Instance.ItemTaken.BIO.MapPosition.Set(x, y, z);
    GUIManager.Instance.ItemTaken.BIO.transform.position = _itemPos;
    GUIManager.Instance.ItemTaken.BIO.transform.eulerAngles = _itemRotation;
    GUIManager.Instance.ItemTaken.BIO.gameObject.SetActive(true);
    GUIManager.Instance.ItemTaken = null;
  }

  void ProcessBWO(BehaviourWorldObject bwo)
  {
    float d = Vector3.Distance(_cameraPos, bwo.transform.position);
    int facing = Mathf.Abs((int)bwo.WorldObjectInstance.ObjectOrientation - (int)_cameraOrientation);
    float dCond = d - float.Epsilon;
    //Debug.Log(_raycastHit.distance + " " + d);
    //if (dCond <= GlobalConstants.WallScaleFactor && (facing == 2 || facing == 0))
    if ((facing == 2 && dCond <= GlobalConstants.WallScaleFactor) 
      || (facing == 0 && dCond <= 0.0f) 
      || (bwo.transform.position.x == _cameraPos.x && bwo.transform.position.z == _cameraPos.z)) 
    {
      if (bwo.WorldObjectInstance.ActionCallback != null)
      {
        bwo.WorldObjectInstance.ActionCallback(bwo.WorldObjectInstance);
      }
    }
  }

  void ProcessBIO(BehaviourItemObject bio)
  {
    int dx = PlayerMapPos.X - bio.MapPosition.X;
    int dz = PlayerMapPos.Z - bio.MapPosition.Z;

    // If object is on the same line as camera and can be taken
    if ( ( (dx == 0 && dz != 0) || (dz == 0 && dx != 0) ) && bio.CanBeTaken)
    {
      if (bio.ItemObjectInstance.LMBAction != null)
        bio.ItemObjectInstance.LMBAction(this);
    }
  }

  /// <summary>
  /// Determines whether it is possible to move. Returns true meaning it's possible, false otherwise.
  /// </summary>
  bool CanMove(int posX, int posZ, CameraMoveType moveType)
  {
    // We might want to look into map array, so we use map coordinates (i.e. row and column)

    int newX = PlayerMapPos.X;
    int newZ = PlayerMapPos.Z;

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
    //if (newX < 0 || newX > AppScript.MapRows - 1 || newZ < 0 || newZ > AppScript.MapColumns - 1)
    if (newX < 0 || newX > LevelLoader.Instance.LevelMap.MapX - 1 || newZ < 0 || newZ > LevelLoader.Instance.LevelMap.MapZ - 1)
    {
      return false;
    }

    // Check walkability of the appropriate side based on the movement type 
    var res = CheckWalkability(newX, newZ, moveType);
   
    // Can move if next block is walkable and we don't have "walls" on current block and next block.
    return (res[0] && (res[1] && res[2]));
  }

  bool[] CheckWalkability(int newX, int newZ, CameraMoveType moveType)
  {
    bool[] _walkabilityCheckResult = new bool[3];

    bool isBlockWalkable = false;
    bool currentBlockSideWalkable = false;
    bool nextBlockSideWalkable = false;

    GlobalConstants.Orientation newOrientation = GlobalConstants.Orientation.EAST;

    int orientationInt = 0;

    switch (moveType)
    {
      case CameraMoveType.FORWARD:
        newOrientation = CameraOrientation;
        break;

      case CameraMoveType.BACKWARD:
        newOrientation = GetOppositeOrientation(CameraOrientation);
        break;

        // If we are facing NORTH, it is 0, so to deal with -1 we use special condition.

      case CameraMoveType.STRAFE_LEFT:
        if (CameraOrientation == GlobalConstants.Orientation.NORTH)
        {
          newOrientation = GlobalConstants.Orientation.WEST;
        }
        else
        {
          orientationInt = (int)CameraOrientation;
          orientationInt--;
          newOrientation = (GlobalConstants.Orientation)orientationInt;
        }
        break;

      case CameraMoveType.STRAFE_RIGHT:
        orientationInt = (int)CameraOrientation;
        orientationInt++;
        orientationInt %= 4;
        newOrientation = (GlobalConstants.Orientation)orientationInt;
        break;
    }

    isBlockWalkable = LevelLoader.Instance.LevelMap.Level[newX, PlayerMapPos.Y, newZ].Walkable;
    currentBlockSideWalkable = LevelLoader.Instance.LevelMap.Level[PlayerMapPos.X, PlayerMapPos.Y, PlayerMapPos.Z].SidesWalkability[newOrientation];
    nextBlockSideWalkable = LevelLoader.Instance.LevelMap.Level[newX, PlayerMapPos.Y, newZ].SidesWalkability[GetOppositeOrientation(newOrientation)];

    _walkabilityCheckResult[0] = isBlockWalkable;
    _walkabilityCheckResult[1] = currentBlockSideWalkable;
    _walkabilityCheckResult[2] = nextBlockSideWalkable;

    return _walkabilityCheckResult;
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
  //
  // EDIT: I should have used Mathf.Clamp method, but I didn't think about it back then. Derp.
  IEnumerator CameraTurnRoutine(object arg)
  {
    CameraTurnArgument ca = arg as CameraTurnArgument;
    if (ca == null) yield return null;

    _isProcessing = true;

    int fromAngle = GlobalConstants.OrientationAngles[GlobalConstants.OrientationsMap[ca.From]];
    int toAngle = GlobalConstants.OrientationAngles[GlobalConstants.OrientationsMap[ca.To]];

    // Special case when turning from 0 to -90 aka 270 degrees.
    // In order to write one clamp condition only and correct clamp min max values,
    // we change certain angles from 0 to 360
    if (ca.TurnRight && toAngle == 0)
    {
      toAngle = 360;
    }
    else if (!ca.TurnRight && toAngle == 270)
    {
      fromAngle = 360;
    }

    // Compass image itself is pointing to the north when its rotation is 0.
    // So, in order to match it with our directions angles in GlobalConstants, 
    // we have to add 90 degrees. That might overshoot 360, so we mod the value by 360.
    int compassFrom = (fromAngle + 90) % 360;
    int compassTo = (toAngle + 90) % 360;

    // Same thing as above
    if (ca.TurnRight && compassTo == 0)
    {
      compassTo = 360;
    }
    else if (!ca.TurnRight && compassTo == 270)
    {
      compassFrom = 360;
    }

    // To reset possible previous 360 degrees back to zero, 
    // we set starting angle manually at the start.
    _cameraAngles.y = fromAngle;
    _compassSpriteAngles.z = compassFrom;

    float cond = 0.0f;

    while (cond < 90.0f)
    { 
      //Debug.Log(fromAngle + " " + toAngle + " " + _cameraAngles.y);

      cond += Time.smoothDeltaTime * ca.Speed;

      if (ca.TurnRight)
      {
        _cameraAngles.y += Time.smoothDeltaTime * ca.Speed;
        _compassSpriteAngles.z += Time.smoothDeltaTime * ca.Speed;

        _cameraAngles.y = Mathf.Clamp(_cameraAngles.y, fromAngle, toAngle);
        _compassSpriteAngles.z = Mathf.Clamp(_compassSpriteAngles.z, compassFrom, compassTo);
      }
      else
      {
        _cameraAngles.y -= Time.smoothDeltaTime * ca.Speed;
        _compassSpriteAngles.z -= Time.smoothDeltaTime * ca.Speed;

        _cameraAngles.y = Mathf.Clamp(_cameraAngles.y, toAngle, fromAngle);
        _compassSpriteAngles.z = Mathf.Clamp(_compassSpriteAngles.z, compassTo, compassFrom);
      }

      yield return null;
    }

    _cameraOrientation = GlobalConstants.OrientationByAngle[toAngle == 360 ? 0 : toAngle];

    _isProcessing = false;   
  }

  // Same thing as commented above here, but instead we get error in position.
  IEnumerator CameraMoveRoutine(object arg)
  {
    CameraMoveArgument ca = arg as CameraMoveArgument;
    if (ca == null) yield return null;
   
    Vector3 cameraPosCached = new Vector3(_cameraPos.x, _cameraPos.y, _cameraPos.z);

    _isProcessing = true;

    // We move camera game object, so we work in world space here

    int endX = (int)_cameraPos.x;
    int endZ = (int)_cameraPos.z;
    int startX = endX;
    int startZ = endZ;

    int xComponent = Mathf.RoundToInt (Mathf.Sin (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
    int zComponent = Mathf.RoundToInt (Mathf.Cos (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));

    if (ca.MoveType == CameraMoveType.FORWARD)
    {
      if (xComponent != 0) endX += xComponent * GlobalConstants.WallScaleFactor;
      if (zComponent != 0) endZ += zComponent * GlobalConstants.WallScaleFactor;
    }
    else if (ca.MoveType == CameraMoveType.BACKWARD)
    {
      if (xComponent != 0) endX -= xComponent * GlobalConstants.WallScaleFactor;
      if (zComponent != 0) endZ -= zComponent * GlobalConstants.WallScaleFactor;
    }
    else if (ca.MoveType == CameraMoveType.STRAFE_LEFT)
    {
      if (xComponent != 0) endZ += xComponent * GlobalConstants.WallScaleFactor;
      if (zComponent != 0) endX -= zComponent * GlobalConstants.WallScaleFactor;
    }
    else if (ca.MoveType == CameraMoveType.STRAFE_RIGHT)
    {
      if (xComponent != 0) endZ -= xComponent * GlobalConstants.WallScaleFactor;
      if (zComponent != 0) endX += zComponent * GlobalConstants.WallScaleFactor;
    }

    int dx = (endX - (int)_cameraPos.x) / GlobalConstants.WallScaleFactor;
    int dz = (endZ - (int)_cameraPos.z) / GlobalConstants.WallScaleFactor;

    bool bobFlag = false;
    float cond = 0.0f;
    float half = GlobalConstants.WallScaleFactor / 2;
    while (cond < GlobalConstants.WallScaleFactor)
    {
      cond += Time.smoothDeltaTime * ca.Speed;

      if (cond > half)
      {
        bobFlag = true;
      }

      if (!bobFlag)
      {
        _cameraBob += Time.smoothDeltaTime * GlobalConstants.CameraBobSpeed;
      }
      else
      {
        _cameraBob -= Time.smoothDeltaTime * GlobalConstants.CameraBobSpeed;
      }

      if (ca.MoveType == CameraMoveType.FORWARD)
      {
        if (xComponent != 0)      _cameraPos.x += xComponent * Time.smoothDeltaTime * ca.Speed;
        else if (zComponent != 0) _cameraPos.z += zComponent * Time.smoothDeltaTime * ca.Speed;
      }
      else if (ca.MoveType == CameraMoveType.BACKWARD)
      {
        if (xComponent != 0)      _cameraPos.x -= xComponent * Time.smoothDeltaTime * ca.Speed;
        else if (zComponent != 0) _cameraPos.z -= zComponent * Time.smoothDeltaTime * ca.Speed;
      }
      else if (ca.MoveType == CameraMoveType.STRAFE_LEFT)
      {
        if (xComponent != 0)      _cameraPos.z += xComponent * Time.smoothDeltaTime * ca.Speed;
        else if (zComponent != 0) _cameraPos.x -= zComponent * Time.smoothDeltaTime * ca.Speed;
      }
      else if (ca.MoveType == CameraMoveType.STRAFE_RIGHT)
      {
        if (xComponent != 0)      _cameraPos.z -= xComponent * Time.smoothDeltaTime * ca.Speed;
        else if (zComponent != 0) _cameraPos.x += zComponent * Time.smoothDeltaTime * ca.Speed;
      }

      // Limit values so we don't overshoot and don't have to adjust
      // values after end of the loop which cause slight jitter.
      if (startX < endX)
      {
        _cameraPos.x = Mathf.Clamp(_cameraPos.x, startX, endX);
      }
      else
      {
        _cameraPos.x = Mathf.Clamp(_cameraPos.x, endX, startX);
      }

      if (startZ < endZ)
      {
        _cameraPos.z = Mathf.Clamp(_cameraPos.z, startZ, endZ);
      }
      else
      {
        _cameraPos.z = Mathf.Clamp(_cameraPos.z, endZ, startZ);
      }

      _cameraBob = Mathf.Clamp(_cameraBob, 0.0f, half);

      _cameraPos.y = cameraPosCached.y + _cameraBob;

      yield return null;
    }

    PlayerMapPos.X += dx;
    PlayerMapPos.Z += dz;

    if (LevelLoader.Instance.LevelMap.Level[PlayerMapPos.X, PlayerMapPos.Y, PlayerMapPos.Z].Teleporter != null)
    {
      ScreenFader.Instance.FlashScreen();
      SetupCamera(LevelLoader.Instance.LevelMap.Level[PlayerMapPos.X, PlayerMapPos.Y, PlayerMapPos.Z].Teleporter.CoordinatesToTeleport, CameraOrientation);
      SoundManager.Instance.PlaySound(GlobalConstants.SFXTeleportation);
    }
    else
    {
      if (LevelLoader.Instance.LevelMap.Level[PlayerMapPos.X, PlayerMapPos.Y - 1, PlayerMapPos.Z].FootstepSound != GlobalConstants.FootstepSoundType.DUMMY)
      {
        SoundManager.Instance.PlayFootstepSoundPlayer(LevelLoader.Instance.LevelMap.Level[PlayerMapPos.X, PlayerMapPos.Y - 1, PlayerMapPos.Z].FootstepSound);
      }
    }

    CheckAndProcessFalling();
  }

  int _numberOfCoroutineCalls = 0;
  void CheckAndProcessFalling()
  {
    BlockEntity blockUnderneath = LevelLoader.Instance.LevelMap.Level[PlayerMapPos.X, PlayerMapPos.Y - 1, PlayerMapPos.Z];

    if (blockUnderneath.BlockType != GlobalConstants.BlockType.AIR)
    {
      _isProcessing = false;

      return;
    }

    _fallingAcceleration = 0.0f;
    _numberOfCoroutineCalls = 0;

    StartCoroutine(PlayerFallRoutine());
  }

  float _fallingAccelerationMax = 10.0f;
  float _fallingAccelerationMultiplier = 10.0f;
  float _fallingAcceleration = 0.0f;
  IEnumerator PlayerFallRoutine()
  {  
    _numberOfCoroutineCalls++;

    float oldY = PlayerMapPos.Y;
    float newY = PlayerMapPos.Y - 1;

    while (oldY > newY)
    {
      oldY -= Time.smoothDeltaTime * _fallingAcceleration;

      _fallingAcceleration += Time.smoothDeltaTime * _fallingAccelerationMultiplier;
      _fallingAcceleration = Mathf.Clamp(_fallingAcceleration, 0.0f, _fallingAccelerationMax);

      _cameraPos.y = oldY * GlobalConstants.WallScaleFactor;
      _cameraPos.y = Mathf.Clamp(_cameraPos.y, newY * GlobalConstants.WallScaleFactor, oldY * GlobalConstants.WallScaleFactor);

      yield return null;
    }

    PlayerMapPos.Y--;

    BlockEntity blockUnderneath = LevelLoader.Instance.LevelMap.Level[PlayerMapPos.X, PlayerMapPos.Y - 1, PlayerMapPos.Z];

    if (blockUnderneath.BlockType == GlobalConstants.BlockType.AIR)
    {
      yield return StartCoroutine(PlayerFallRoutine());
    }
    else
    {
      _isProcessing = false;

      if (_numberOfCoroutineCalls > 1)
      {
        SoundManager.Instance.PlaySound(GlobalConstants.SFXPlayerFallLong);
      }
      else
      {
        SoundManager.Instance.PlaySound(GlobalConstants.SFXPlayerFall);
      }
    }
      
    yield return null;
  }

  IEnumerator CameraCannotMoveRoutine(object arg)
  {
    CameraMoveArgument ca = arg as CameraMoveArgument;
    if (ca == null) yield return null;

    SoundManager.Instance.PlaySound(GlobalConstants.SFXPlayerCannotMove);

    _isProcessing = true;
    
    int endX = (int)_cameraPos.x;
    int endZ = (int)_cameraPos.z;

    int xComponent = Mathf.RoundToInt (Mathf.Sin (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
    int zComponent = Mathf.RoundToInt (Mathf.Cos (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));

    float cond = 0.0f;
    float nudge = (float)GlobalConstants.WallScaleFactor / 6.0f;
    float half = nudge / 2.0f;
    while (cond < nudge)
    {
      cond += Time.smoothDeltaTime * ca.Speed;

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

    _cameraPos.x = endX;
    _cameraPos.z = endZ;
    
    _isProcessing = false;    
  }

  GlobalConstants.Orientation GetOppositeOrientation(GlobalConstants.Orientation orientation)
  {
    int oppositeOrientation = (int)orientation;

    oppositeOrientation += 2;
    oppositeOrientation %= 4;

    return (GlobalConstants.Orientation)oppositeOrientation;
  }

  Vector3 _cameraForwardVector = Vector3.zero;
  Vector3 GetCameraForwardVector()
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
