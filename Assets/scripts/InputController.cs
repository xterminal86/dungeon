using UnityEngine;
using System.Collections;

public class InputController : MonoSingleton<InputController> 
{
  CameraTurnArgument arg = new CameraTurnArgument();
  CoroutineMoveArgument _cameraMove = new CoroutineMoveArgument();
  void Awake () 
  {	
    arg.Speed = GlobalConstants.CameraTurnSpeed;
    _cameraMove.Speed = GlobalConstants.CameraMoveSpeed;
	}
	
  public void MapLoadingFinishedHandler()
  {
    _cameraAngles = App.Instance.CameraAngles;
    _cameraPosX = App.Instance.CameraPos.x;
    _cameraPosZ = App.Instance.CameraPos.z;
    _cameraPos.x = _cameraPosX;
    _cameraPos.z = _cameraPosZ;
  }

  bool _isProcessing = false;
  float _cameraPosX = 0.0f, _cameraPosZ = 0.0f;
  Vector3 _cameraAngles = Vector3.zero;
  Vector3 _cameraPos = Vector3.zero;
	void Update () 
  {
    if (!_isProcessing)
    {
      ProcessKeyboard();
    }

    //_cameraPos.x = _cameraPosX;
    //_cameraPos.z = _cameraPosZ;

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

    if (Input.GetKey(KeyCode.Q)) 
    {
      TurnCamera(App.Instance.CameraOrientation, App.Instance.CameraOrientation - 1, false);
    }

    if (Input.GetKey(KeyCode.W)) 
    {
      int posX = (int)App.Instance.CameraPos.x;
      int posZ = (int)App.Instance.CameraPos.z;
      CameraMoveArgument arg = new CameraMoveArgument();
      arg.From = new Vector2(posX, posZ);
      arg.Speed = GlobalConstants.CameraMoveSpeed;
      StartCoroutine("CameraMoveRoutine", arg);
    }

    if (Input.GetKey(KeyCode.S)) 
    {
      int xComponent = Mathf.RoundToInt (Mathf.Sin (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
      int zComponent = Mathf.RoundToInt (Mathf.Cos (Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
      if (zComponent != 0) 
      {
        _cameraMove.From = App.Instance.CameraPos.z;
        _cameraMove.To = _cameraMove.From - zComponent * GlobalConstants.WallScaleFactor;
        _cameraMove.MoveZ = true;
        StartCoroutine ("CameraMoveForwardRoutine", _cameraMove);
      }
      else if (xComponent != 0) 
      {
        _cameraMove.From = App.Instance.CameraPos.x;
        _cameraMove.To = _cameraMove.From - xComponent * GlobalConstants.WallScaleFactor;
        _cameraMove.MoveZ = false;
        StartCoroutine ("CameraMoveForwardRoutine", _cameraMove);
      }
    }
  }

  protected override void Init()
  {
    base.Init ();
  }

  void TurnCamera(int from, int to, bool turnRight)
  {
    if (from < 0) from = GlobalConstants.OrientationsMap.Count - 1;
    if (from == GlobalConstants.OrientationsMap.Count) from = 0;
    if (to < 0) to = GlobalConstants.OrientationsMap.Count - 1;
    if (to == GlobalConstants.OrientationsMap.Count) to = 0;

    arg.From = from;
    arg.To = to;
    arg.TurnRight = turnRight;
    StartCoroutine("CameraTurnRoutine", arg);
  }

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
      if (cond + Time.deltaTime * ca.Speed > 90.0f) break;
      if (ca.TurnRight) _cameraAngles.y += Time.deltaTime * ca.Speed;
      else _cameraAngles.y -= Time.deltaTime * ca.Speed;
      yield return null;
    }

    _cameraAngles.y = toAngle;

    App.Instance.CameraOrientation = ca.To;

    _isProcessing = false;
  }

  IEnumerator CameraMoveRoutine(object arg)
  {
    CameraMoveArgument ca = arg as CameraMoveArgument;
    if (ca == null) yield return null;
    _isProcessing = true;

    int newX = (int)_cameraPos.x;
    int newZ = (int)_cameraPos.z;

    int cameraOrientation = App.Instance.CameraOrientation;
    switch (cameraOrientation)
    {
      case 0:
        newX -= GlobalConstants.WallScaleFactor;
        break;
      case 1:
        newZ += GlobalConstants.WallScaleFactor;
        break;
      case 2:
        newX += GlobalConstants.WallScaleFactor;
        break;
      case 3:
        newZ -= GlobalConstants.WallScaleFactor;
        break;
      default:
        break;
    }

    float cond = 0.0f;
    while (cond < GlobalConstants.WallScaleFactor)
    {
      cond += Time.deltaTime * ca.Speed;
      if (cond + Time.deltaTime * ca.Speed > GlobalConstants.WallScaleFactor) break;

      if (cameraOrientation == 0) _cameraPos.x -= Time.deltaTime * ca.Speed;
      else if (cameraOrientation == 1) _cameraPos.z += Time.deltaTime * ca.Speed;
      else if (cameraOrientation == 2) _cameraPos.x += Time.deltaTime * ca.Speed;
      else if (cameraOrientation == 3) _cameraPos.z -= Time.deltaTime * ca.Speed;
      yield return null;
    }

    _cameraPos.x = newX;
    _cameraPos.z = newZ;

    _isProcessing = false;

    SoundManager.Instance.PlayFootstepSound();

    /*
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
    */
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
}

public class CoroutineMoveArgument
{
  public float From;
  public float To;
  public float Speed;
  public bool MoveZ;
}