using UnityEngine;
using System.Collections;

public class InputController : MonoSingleton<InputController> 
{
  CoroutineTurnArgument _cameraTurnRight = new CoroutineTurnArgument();
  CoroutineTurnArgument _cameraTurnLeft = new CoroutineTurnArgument();
  CoroutineMoveArgument _cameraMove = new CoroutineMoveArgument();

  void Awake () 
  {	
    _cameraTurnRight.Speed = GlobalConstants.CameraTurnSpeed;
    _cameraTurnLeft.Speed = GlobalConstants.CameraTurnSpeed;
    _cameraMove.Speed = GlobalConstants.CameraMoveSpeed;
	}
	
  public void MapLoadingFinishedHandler()
  {
    _cameraAngles = Camera.main.transform.eulerAngles;
    _cameraPosX = App.Instance.CameraPos.x;
    _cameraPosZ = App.Instance.CameraPos.z;
    _cameraPos.x = _cameraPosX;
    _cameraPos.z = _cameraPosZ;
  }

  bool _isProcessing = false;
  float _cameraAngleY = 0.0f;
  float _cameraPosX = 0.0f, _cameraPosZ = 0.0f;
  Vector3 _cameraAngles = Vector3.zero;
  Vector3 _cameraPos = Vector3.zero;
	void Update () 
  {
    if (!_isProcessing)
    {
      ProcessKeyboard();
    }

    _cameraPos.x = _cameraPosX;
    _cameraPos.z = _cameraPosZ;

    _cameraAngles.y = _cameraAngleY;

    App.Instance.CameraPivot.transform.eulerAngles = _cameraAngles;
    App.Instance.CameraPos = _cameraPos;
    App.Instance.CameraPivot.transform.position = App.Instance.CameraPos;
	}

  void ProcessKeyboard ()
  {
    if (Input.GetKeyDown (KeyCode.E)) 
    {      
      _cameraTurnRight.From = Camera.main.transform.eulerAngles.y;
      _cameraTurnRight.To = _cameraTurnRight.From + 90;
      StartCoroutine ("CameraTurnRoutine", _cameraTurnRight);
    }

    if (Input.GetKeyDown (KeyCode.Q)) 
    {
      _cameraTurnLeft.From = Camera.main.transform.eulerAngles.y;
      _cameraTurnLeft.To = _cameraTurnLeft.From - 90;
      StartCoroutine ("CameraTurnRoutine", _cameraTurnLeft);
    }

    if (Input.GetKeyDown (KeyCode.W)) 
    {
      int xFraction = Mathf.RoundToInt (Mathf.Sin (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
      int zFraction = Mathf.RoundToInt (Mathf.Cos (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
      if (zFraction != 0) 
      {
        _cameraMove.From = App.Instance.CameraPos.z;
        _cameraMove.To = _cameraMove.From + zFraction * GlobalConstants.WallScaleFactor;
        _cameraMove.MoveZ = true;
        StartCoroutine ("CameraMoveForwardRoutine", _cameraMove);
      }
      else if (xFraction != 0) 
      {
        _cameraMove.From = App.Instance.CameraPos.x;
        _cameraMove.To = _cameraMove.From + xFraction * GlobalConstants.WallScaleFactor;
        _cameraMove.MoveZ = false;
        StartCoroutine ("CameraMoveForwardRoutine", _cameraMove);
      }
    }

    if (Input.GetKeyDown (KeyCode.S)) 
    {
      int xFraction = Mathf.RoundToInt (Mathf.Sin (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
      int zFraction = Mathf.RoundToInt (Mathf.Cos (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
      if (zFraction != 0) 
      {
        _cameraMove.From = App.Instance.CameraPos.z;
        _cameraMove.To = _cameraMove.From - zFraction * GlobalConstants.WallScaleFactor;
        _cameraMove.MoveZ = true;
        StartCoroutine ("CameraMoveForwardRoutine", _cameraMove);
      }
      else if (xFraction != 0) 
      {
        _cameraMove.From = App.Instance.CameraPos.x;
        _cameraMove.To = _cameraMove.From - xFraction * GlobalConstants.WallScaleFactor;
        _cameraMove.MoveZ = false;
        StartCoroutine ("CameraMoveForwardRoutine", _cameraMove);
      }
    }
  }

  protected override void Init()
  {
    base.Init ();
  }

  IEnumerator CameraTurnRoutine(object arg)
  {
    CoroutineTurnArgument ca = arg as CoroutineTurnArgument;
    if (ca == null) yield return null;
    _isProcessing = true;
    float res = ca.From;
    // Right Turn
    if (ca.From < ca.To)
    {
      while (res < ca.To)
      {
        float speed = Time.deltaTime * ca.Speed;
        res += speed;
        _cameraAngleY += speed;
        if (res + speed > ca.To) break;
        yield return null;
      }
    }
    else
    {
      while (res > ca.To)
      {
        float speed = Time.deltaTime * ca.Speed;
        res -= speed;
        _cameraAngleY -= speed;
        if (res - speed < ca.To) break;
        yield return null;
      }
    }

    _cameraAngleY = ca.To;
    _cameraAngleY = Mathf.Round(_cameraAngleY);

    //Debug.Log (_cameraAngleY);

    _isProcessing = false;
  }

  IEnumerator CameraMoveForwardRoutine(object arg)
  {
    CoroutineMoveArgument ca = arg as CoroutineMoveArgument;
    if (ca == null) yield return null;
    _isProcessing = true;
    float res = ca.From;
    if (ca.From < ca.To)
    {
      while (res < ca.To)
      {
        float speed = Time.deltaTime * ca.Speed;
        res += speed;
        if (ca.MoveZ) _cameraPosZ += speed;
        else _cameraPosX += speed;
        if (res + speed > ca.To) break;
        yield return null;
      }
    }
    else
    {
      while (res > ca.To)
      {
        float speed = Time.deltaTime * ca.Speed;
        res -= speed;
        if (ca.MoveZ) _cameraPosZ -= speed;
        else _cameraPosX -= speed;
        if (res - speed < ca.To) break;
        yield return null;
      }
    }

    if (ca.MoveZ) _cameraPosZ = ca.To;
    else _cameraPosX = ca.To;

    _isProcessing = false;

    SoundManager.Instance.PlayFootstepSound();
  }
}

public class CoroutineTurnArgument
{
  public float From;
  public float To;
  public float Speed;
}

public class CoroutineMoveArgument
{
  public float From;
  public float To;
  public float Speed;
  public bool MoveZ;
}