using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class GameData : MonoSingleton<GameData> 
{
  public PlayerCharacter PlayerCharacterVariable = new PlayerCharacter();

  protected override void Init()
  {
    base.Init();
  }
}

