using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterWorldObject : WorldObject 
{
  public Int3 CoordinatesToTeleport = Int3.Zero;

  public TeleporterWorldObject(string inGameName, string prefabName) : base(inGameName, prefabName)
  {    
  }

  public void Teleport()
  {
    InputController.Instance.SetupCamera(CoordinatesToTeleport, InputController.Instance.CameraOrientation);
  }
}
