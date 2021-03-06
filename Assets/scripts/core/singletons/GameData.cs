using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class GameData : MonoSingleton<GameData> 
{ 
  [HideInInspector]
  public bool PlayerCanAttack = true;

  [HideInInspector]
  public PlayerMoveStateEnum PlayerMoveState;

  [HideInInspector]
  public GameState CurrentGameState;

  Dictionary<int, SerializableItem> _placeholderItemsByHash = new Dictionary<int, SerializableItem>();
  Dictionary<int, SerializableItem> _weaponItemsByHash = new Dictionary<int, SerializableItem>();
  Dictionary<int, SerializableItem> _foodItemsByHash = new Dictionary<int, SerializableItem>();
  Dictionary<int, SerializableItem> _armorItemsByHash = new Dictionary<int, SerializableItem>();
  Dictionary<int, SerializableItem> _accessoryItemsByHash = new Dictionary<int, SerializableItem>();

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
    var resource = Resources.Load("text/items-db");

    if (resource == null)
    {
      Debug.LogError("Could not load items database!");
      return;
    }

    TextAsset ta = resource as TextAsset;

    XmlDocument doc = new XmlDocument();
    doc.LoadXml(ta.text);
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

      case GlobalConstants.WorldItemType.ARMOR_CHEST:
        item = new SerializableArmorItem();
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
        case GlobalConstants.WorldItemType.WEAPON_RANGED:
          _weaponItemsByHash.Add(hash, item);
          break;

        case GlobalConstants.WorldItemType.ARMOR_CHEST:
        case GlobalConstants.WorldItemType.ARMOR_BOOTS:
        case GlobalConstants.WorldItemType.ARMOR_HEAD:
        case GlobalConstants.WorldItemType.ARMOR_PANTS:
          _armorItemsByHash.Add(hash, item);
          break;

        case GlobalConstants.WorldItemType.ACCESSORY_NECK:
        case GlobalConstants.WorldItemType.ACCESSORY_HAND:
        case GlobalConstants.WorldItemType.ACCESSORY_CLOAK:
          _accessoryItemsByHash.Add(hash, item);
          break;

        default:
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

        case "armor":
          (item as SerializableArmorItem).ArmorClassModifier = int.Parse(attr.Attributes["value"].InnerText);
          break;

        case "cost":
          item.Cost = int.Parse(attr.Attributes["value"].InnerText);
          break;

        default:
          break;
      }
    }
  }

  /// <summary>
  /// Returns reference to database item instance.
  /// </summary>
  /// <returns>Should not be modified</returns>
  /// <param name="type">Type of the item to return</param>
  /// <param name="hash">Database item name</param>
  public SerializableItem GetItem(GlobalConstants.WorldItemType type, string name)
  {
    int hash = name.GetHashCode();
    return GetItem(type, hash);
  }

  public SerializableItem GetItem(GlobalConstants.WorldItemType type, int hash)
  {
    SerializableItem result = null;

    switch (type)
    {
      case GlobalConstants.WorldItemType.WEAPON_MELEE:
      case GlobalConstants.WorldItemType.WEAPON_RANGED:
        result = _weaponItemsByHash[hash];
        break;

      case GlobalConstants.WorldItemType.FOOD:
        result = _foodItemsByHash[hash];
        break;

      case GlobalConstants.WorldItemType.PLACEHOLDER:
        result =  _placeholderItemsByHash[hash];
        break;

      case GlobalConstants.WorldItemType.ARMOR_CHEST:
      case GlobalConstants.WorldItemType.ARMOR_BOOTS:
      case GlobalConstants.WorldItemType.ARMOR_HEAD:
      case GlobalConstants.WorldItemType.ARMOR_PANTS:
        result = _armorItemsByHash[hash];
        break;

      default:        
        break;
    }

    return result;
  }

  public SerializableItem GetRandomItem(GlobalConstants.WorldItemType type)
  {
    SerializableItem result = null;

    int index = 0;
    switch (type)
    {
      case GlobalConstants.WorldItemType.WEAPON_MELEE:
      case GlobalConstants.WorldItemType.WEAPON_RANGED:
        index = Random.Range(0, _weaponsKeys.Length);
        result = _weaponItemsByHash[_weaponsKeys[index]];
        break;

      case GlobalConstants.WorldItemType.FOOD:
        index = Random.Range(0, _foodKeys.Length);
        result = _foodItemsByHash[_foodKeys[index]];
        break;

      case GlobalConstants.WorldItemType.PLACEHOLDER:
        index = Random.Range(0, _placeholderKeys.Length);
        result = _placeholderItemsByHash[_placeholderKeys[index]];
        break;

      default:        
        break;
    }

    return result;
  }

  float _hungerTimer = 0.0f;
  void Update()
  {
    if (GameData.Instance.CurrentGameState != GameData.GameState.RUNNING)
    {
      return;
    }

    /*
    if (GameData.Instance.PlayerCharacterVariable.HitPoints == 0)
    {
      SoundManager.Instance.StopAllSounds();
      SoundManager.Instance.PlaySound(GlobalConstants.SFXPlayerDeath);

      GameData.Instance.CurrentGameState = GameData.GameState.PAUSED;

      ScreenFader.Instance.FadeCompleteCallback += GameOverHandler;
      ScreenFader.Instance.FadeOut();
    }
    */

    _hungerTimer += Time.smoothDeltaTime * GameData.Instance.PlayerCharacterVariable.HungerDecreaseMultiplier;

    if (_hungerTimer > GameData.Instance.PlayerCharacterVariable.HungerTick)
    {      
      GameData.Instance.PlayerCharacterVariable.AddHunger(-1);
    }
  }

  public void AddHunger(int value)
  {
    PlayerCharacterVariable.AddHunger(value);
  }

  public void ResetHungerTimer()
  {
    _hungerTimer = 0.0f;
  }

  public enum PlayerMoveStateEnum
  {
    NORMAL = 0,
    HOLD_PLAYER
  }

  public enum GameState
  {
    RUNNING = 0,
    PAUSED
  }
}
