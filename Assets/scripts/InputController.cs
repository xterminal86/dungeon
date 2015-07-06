using UnityEngine;
using System.Collections;

public class InputController : MonoSingleton<InputController> 
{
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
    }

    _cameraPos.y += _cameraBob;

    App.Instance.CameraPivot.transform.eulerAngles = _cameraAngles;
    App.Instance.CameraPos = _cameraPos;
    App.Instance.CameraPivot.transform.position = App.Instance.CameraPos;
	}

  void ProcessKeyboard ()
  {
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
      int posX = (int)App.Instance.CameraPos.x;
      int posZ = (int)App.Instance.CameraPos.z;
      _cameraMoveArgument.From = new Vector2(posX, posZ);
      _cameraMoveArgument.Speed = GlobalConstants.CameraMoveSpeed;
      _cameraMoveArgument.MoveBackwards = false;
      bool res = CanMove(posX, posZ, false);
      if (res)
      {
        StartCoroutine("CameraMoveRoutine", _cameraMoveArgument);
      }
      else
      {
        StartCoroutine("CameraCannotMoveRoutine", _cameraMoveArgument);
      }
    }
    else if (Input.GetKey(KeyCode.S)) 
    {
      int posX = (int)App.Instance.CameraPos.x;
      int posZ = (int)App.Instance.CameraPos.z;
      _cameraMoveArgument.From = new Vector2(posX, posZ);
      _cameraMoveArgument.Speed = GlobalConstants.CameraMoveSpeed;
      _cameraMoveArgument.MoveBackwards = true;
      bool res = CanMove(posX, posZ, true);
      if (res)
      {
        StartCoroutine("CameraMoveRoutine", _cameraMoveArgument);
      }
      else
      {
        StartCoroutine("CameraCannotMoveRoutine", _cameraMoveArgument);
      }
    }
    else if (Input.GetKeyDown(KeyCode.Space))
    {
      //Debug.Log (App.Instance.GetMapObjectByName("door_1"));
      //Debug.Log (App.Instance.GetGameObjectByName("door_1"));
      //Debug.Log (App.Instance.GetMapObjectByName("test"));
    }
  }

  protected override void Init()
  {
    base.Init ();
  }

  bool CanMove(int posX, int posZ, bool moveBackwards)
  {
    int newX = (int)_cameraPos.x / GlobalConstants.WallScaleFactor;
    int newZ = (int)_cameraPos.z / GlobalConstants.WallScaleFactor;
    
    int xComponent = Mathf.RoundToInt (Mathf.Sin (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
    int zComponent = Mathf.RoundToInt (Mathf.Cos (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
    
    if (!moveBackwards)
    {
      if (xComponent != 0) newX += xComponent;
      if (zComponent != 0) newZ += zComponent;
    }
    else
    {
      if (xComponent != 0) newX -= xComponent;
      if (zComponent != 0) newZ -= zComponent;
    }

    char res = App.Instance.GetMapLayoutPoint(newX, newZ);

    return (res == '.');
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

    if (!ca.MoveBackwards)
    {
      if (xComponent != 0) newX += xComponent * GlobalConstants.WallScaleFactor;
      if (zComponent != 0) newZ += zComponent * GlobalConstants.WallScaleFactor;
    }
    else
    {
      if (xComponent != 0) newX -= xComponent * GlobalConstants.WallScaleFactor;
      if (zComponent != 0) newZ -= zComponent * GlobalConstants.WallScaleFactor;
    }

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

      if (!ca.MoveBackwards)
      {
        if (xComponent != 0) _cameraPos.x += xComponent * Time.deltaTime * ca.Speed;
        else if (zComponent != 0) _cameraPos.z += zComponent * Time.deltaTime * ca.Speed;
      }
      else
      {
        if (xComponent != 0) _cameraPos.x -= xComponent * Time.deltaTime * ca.Speed;
        else if (zComponent != 0) _cameraPos.z -= zComponent * Time.deltaTime * ca.Speed;
      }

      yield return null;
    }

    _cameraBob = 0.0f;

    _cameraPos.x = newX;
    // Hardcoded camera pivot Y position - OK, since the actual camera is child of pivot.
    // If we want to change camera height, we should do this by changing the camera inside the pivot GameObject.
    _cameraPos.y = 0.0f;  
    _cameraPos.z = newZ;

    SoundManager.Instance.PlayFootstepSound();

    _isProcessing = false;
  }

  IEnumerator CameraCannotMoveRoutine(object arg)
  {
    CameraMoveArgument ca = arg as CameraMoveArgument;
    if (ca == null) yield return null;
    _isProcessing = true;
    
    int newX = (int)_cameraPos.x;
    int newZ = (int)_cameraPos.z;
    
    int xComponent = Mathf.RoundToInt (Mathf.Sin (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
    int zComponent = Mathf.RoundToInt (Mathf.Cos (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));

    float cond = 0.0f;
    float nudge = (float)GlobalConstants.WallScaleFactor / 2.0f;
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

      if (!ca.MoveBackwards)
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
      else
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
      
      yield return null;
    }
    
    _cameraPos.x = newX;
    _cameraPos.z = newZ;
    
    _isProcessing = false;
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
  public bool MoveBackwards;
}
