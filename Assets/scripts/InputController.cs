using UnityEngine;
using System.Collections;

public class InputController : MonoSingleton<InputController> 
{
  CoroutineTurnArgument _cameraTurnRight = new CoroutineTurnArgument();
  CoroutineTurnArgument _cameraTurnLeft = new CoroutineTurnArgument();
  CoroutineTurnArgument _cameraMove = new CoroutineTurnArgument();

  void Start () 
  {	
    _cameraTurnRight.Speed = GlobalConstants.CameraTurnSpeed;
    _cameraTurnLeft.Speed = GlobalConstants.CameraTurnSpeed;
    _cameraMove.Speed = GlobalConstants.CameraMoveSpeed;

    App.Instance.MapLoadingFinished += MapLoadingFinishedHandler;
	}
	
  void MapLoadingFinishedHandler()
  {
    _cameraAngles = Camera.main.transform.eulerAngles;
    _cameraPosX = App.Instance.CameraPos.x;
    _cameraPosZ = App.Instance.CameraPos.z;
    _cameraPos.x = _cameraPosX;
    _cameraPos.y = Camera.main.transform.position.y;
    _cameraPos.z = _cameraPosZ;
  }

  bool _isProcessing = false;
  float _cameraAngleY = 0.0f;
  float _cameraPosX = 0.0f, _cameraPosZ = 0.0f;
  Vector3 _cameraAngles = Vector3.zero;
  Vector3 _cameraPos = Vector3.zero;
	void Update () 
  {
    if (Input.GetKeyUp(KeyCode.E) && !_isProcessing)
    {
      _cameraTurnRight.From = Camera.main.transform.eulerAngles.y;
      _cameraTurnRight.To = _cameraTurnRight.From + 90;
      StartCoroutine ("CameraTurnRoutine", _cameraTurnRight);
    }

    if (Input.GetKeyUp(KeyCode.Q) && !_isProcessing)
    {
      _cameraTurnLeft.From = Camera.main.transform.eulerAngles.y;
      _cameraTurnLeft.To = _cameraTurnLeft.From - 90;
      StartCoroutine ("CameraTurnRoutine", _cameraTurnLeft);
    }

    if (Input.GetKeyUp(KeyCode.W) && !_isProcessing)
    {
      int xFraction = Mathf.RoundToInt(Mathf.Sin(Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
      int zFraction = Mathf.RoundToInt(Mathf.Cos(Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad));
      //Debug.Log ("X: " + xFraction + " Z: " + zFraction);
      if (zFraction != 0)
      {
        _cameraMove.From = App.Instance.CameraPos.z;
        _cameraMove.To = _cameraMove.From + zFraction * GlobalConstants.WallScaleFactor;
        StartCoroutine ("CameraMoveForwardRoutine", _cameraMove);
      }    

    }

    _cameraPos.x = _cameraPosX;
    //_cameraPos.y = Camera.main.transform.position.y;
    _cameraPos.z = _cameraPosZ;

    _cameraAngles.y = _cameraAngleY;
    Camera.main.transform.eulerAngles = _cameraAngles;

    App.Instance.CameraPos = _cameraPos;
    Camera.main.transform.position = App.Instance.CameraPos;
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

    //Debug.Log (_cameraAngleY);

    _isProcessing = false;
  }

  IEnumerator CameraMoveForwardRoutine(object arg)
  {
    CoroutineTurnArgument ca = arg as CoroutineTurnArgument;
    if (ca == null) yield return null;
    _isProcessing = true;
    float res = ca.From;
    while (res < ca.To)
    {
      float speed = Time.deltaTime * ca.Speed;
      res += speed;
      _cameraPosZ += speed;
      if (res + speed > ca.To) break;
      yield return null;
    }

    _cameraPosZ = ca.To;
    _isProcessing = false;
  }
}

public class CoroutineTurnArgument
{
  public float From;
  public float To;
  public float Speed;
}