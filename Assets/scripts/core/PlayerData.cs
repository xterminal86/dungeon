using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerData : MonoSingleton<PlayerData> 
{
  public PlayerCharacter PlayerCharacterVariable = new PlayerCharacter();

  List<InventoryItemClass> InventoryItems = new List<InventoryItemClass>();

  protected override void Init()
  {
    base.Init();

    Sprite[] _spritesAtlas = Resources.LoadAll<Sprite>("sprites-atlas/atlas");
    if (_spritesAtlas != null)
    {
    }
  }  
}

