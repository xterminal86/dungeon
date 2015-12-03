using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleScreen : MonoBehaviour 
{
  public void NewGameHandler()
  {
    Application.LoadLevel("main");
  }
}
