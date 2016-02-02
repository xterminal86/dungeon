using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class GameData : MonoSingleton<GameData> 
{
  public PlayerCharacter PlayerCharacterVariable = new PlayerCharacter();

  List<PlaceholderItemObject> InventoryItems = new List<PlaceholderItemObject>();

  protected override void Init()
  {
    base.Init();
  }
}

