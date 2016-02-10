using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class GameData : MonoSingleton<GameData> 
{
  Dictionary<int, SerializableItem> _placeholderItemsByHash = new Dictionary<int, SerializableItem>();
  Dictionary<int, SerializableItem> _weaponItemsByHash = new Dictionary<int, SerializableItem>();
  Dictionary<int, SerializableItem> _foodItemsByHash = new Dictionary<int, SerializableItem>();

  int[] _placeholderKeys;
  int[] _weaponsKeys;
  int[] _foodKeys;

  public PlayerCharacter PlayerCharacterVariable = new PlayerCharacter();

  protected override void Init()
  {
    base.Init();

    BuildItemsDatabase();

    _placeholderKeys = new int[_placeholderItemsByHash.Count];
    _placeholderItemsByHash.Keys.CopyTo(_placeholderKeys, 0);

    _weaponsKeys = new int[_weaponItemsByHash.Count];
    _weaponItemsByHash.Keys.CopyTo(_weaponsKeys, 0);

    _foodKeys = new int[_foodItemsByHash.Count];
    _foodItemsByHash.Keys.CopyTo(_foodKeys, 0);
  }

  void BuildItemsDatabase()
  {
    XmlDocument doc = new XmlDocument();
    doc.Load("Assets/Resources/text/items-db.xml");
    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
    { 
      CreateItemEntry(node);
    }
  }

  void CreateItemEntry(XmlNode node)
  { 
    SerializableItem item = null;
    int hash = node.Attributes["name"].InnerText.GetHashCode();
    string type = node.Attributes["type"].InnerText;
    var itemType = GlobalConstants.WorldItemTypes[type];
    switch (itemType)
    {
      case GlobalConstants.WorldItemType.FOOD:
        item = new SerializableFoodItem();
        ParseAttributes(node.ChildNodes, itemType, item);
        break;

      case GlobalConstants.WorldItemType.WEAPON_MELEE:        
        item = new SerializableWeaponItem();
        ParseAttributes(node.ChildNodes, itemType, item);
        break;

      case GlobalConstants.WorldItemType.PLACEHOLDER:
        item = new SerializableItem();
        break;

      default:
        break;
    }

    if (item != null)
    {
      item.ItemName = node.Attributes["name"].InnerText;
      item.AtlasIconIndex = int.Parse(node.Attributes["atlasIcon"].InnerText);
      item.PrefabName = node.Attributes["prefabName"].InnerText;
      item.ItemType = itemType;

      switch (itemType)
      {
        case GlobalConstants.WorldItemType.PLACEHOLDER:
          _placeholderItemsByHash.Add(hash, item);
          break;

        case GlobalConstants.WorldItemType.FOOD:          
          _foodItemsByHash.Add(hash, item);
          break;

        case GlobalConstants.WorldItemType.WEAPON_MELEE:
          _weaponItemsByHash.Add(hash, item);
          break;
      }
    }
  }

  void ParseAttributes(XmlNodeList nodes, GlobalConstants.WorldItemType type, SerializableItem item)
  {
    foreach (XmlNode attr in nodes)
    {
      string name = attr.Attributes["name"].InnerText;

      switch (name)
      {
        case "action":
          if (type == GlobalConstants.WorldItemType.WEAPON_MELEE 
           || type == GlobalConstants.WorldItemType.WEAPON_RANGED)
          {
            string[] damage = attr.Attributes["value"].InnerText.Split('d');
            (item as SerializableWeaponItem).MinimumDamage = int.Parse(damage[0]);
            (item as SerializableWeaponItem).MaximumDamage = int.Parse(damage[1]);
          }
          else if (type == GlobalConstants.WorldItemType.FOOD)
          {
            (item as SerializableFoodItem).Saturation = int.Parse(attr.Attributes["value"].InnerText);
          }
          break;

        case "cooldown":
          (item as SerializableWeaponItem).Cooldown = int.Parse(attr.Attributes["value"].InnerText);
          break;

        case "req":
          //string[] s = attr.Attributes["value"].InnerText.Split(':');
          break;

        case "cost":
          item.Cost = int.Parse(attr.Attributes["value"].InnerText);
          break;
      }
    }
  }

  public SerializableItem GetItem(GlobalConstants.WorldItemType type, string name)
  {
    int hash = name.GetHashCode();
    return GetItem(type, hash);
  }

  public SerializableItem GetItem(GlobalConstants.WorldItemType type, int hash)
  {
    switch (type)
    {
      case GlobalConstants.WorldItemType.WEAPON_MELEE:
      case GlobalConstants.WorldItemType.WEAPON_RANGED:
        return _weaponItemsByHash[hash];
        break;

      case GlobalConstants.WorldItemType.FOOD:
        return _foodItemsByHash[hash];
        break;

      case GlobalConstants.WorldItemType.PLACEHOLDER:
        return _placeholderItemsByHash[hash];
        break;

      default:
        break;
    }

    return null;
  }

  public SerializableItem GetRandomItem(GlobalConstants.WorldItemType type)
  {
    int index = 0;
    switch (type)
    {
      case GlobalConstants.WorldItemType.WEAPON_MELEE:
      case GlobalConstants.WorldItemType.WEAPON_RANGED:
        index = Random.Range(0, _weaponsKeys.Length);
        return _weaponItemsByHash[_weaponsKeys[index]];
        break;

      case GlobalConstants.WorldItemType.FOOD:
        index = Random.Range(0, _foodKeys.Length);
        return _foodItemsByHash[_foodKeys[index]];
        break;

      case GlobalConstants.WorldItemType.PLACEHOLDER:
        index = Random.Range(0, _placeholderKeys.Length);
        return _placeholderItemsByHash[_placeholderKeys[index]];
        break;

      default:
        break;
    }

    return null;
  }
}
