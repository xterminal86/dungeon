using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class GameData : MonoSingleton<GameData> 
{
  public PlayerCharacter PlayerCharacterVariable = new PlayerCharacter();

  List<PlaceholderMapObject> InventoryItems = new List<PlaceholderMapObject>();

  protected override void Init()
  {
    base.Init();

    BuildItemsDatabase();
  }

  void BuildItemsDatabase()
  {
    var resource = Resources.Load("text/ItemsDatabase");

    if (resource == null) return;

    TextAsset ta = resource as TextAsset;

    XmlDocument doc = new XmlDocument();
    doc.LoadXml(ta.text);

    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
    {      
      switch (node.Name)
      {
        case "ITEM":
          break;

        default:
          break;
      }
    }
  }
}

