using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerData : MonoSingleton<PlayerData> 
{
  public PlayerCharacter PlayerCharacterVariable = new PlayerCharacter();

  protected override void Init()
  {
    base.Init();
  }  
}

