using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class GameData : MonoSingleton<GameData> 
{
  Dictionary<int, ItemObject> _gameItems = new Dictionary<int, ItemObject>();

  public PlayerCharacter PlayerCharacterVariable = new PlayerCharacter();

  protected override void Init()
  {
    base.Init();

    BuildItemsDatabase();
  }

  void BuildItemsDatabase()
  {
    XmlDocument doc = new XmlDocument();
    doc.Load("Assets/Resources/text/items-db.xml");
    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
    {        
      Debug.Log(node.Name);
    }
  }
}

