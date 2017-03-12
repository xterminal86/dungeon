using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FormInventory : MonoBehaviour 
{
  int _hungerBarMaxWidth = 0;

  public GameObject InventorySlot;
  public RectTransform InventorySlotsHolder;

  Dictionary<int, InventorySlot> _inventorySlots = new Dictionary<int, InventorySlot>();
  public Dictionary<int, InventorySlot> InventorySlots
  {
    get { return _inventorySlots; }
  }

  int _invWidth = 40;
  int _invHeight = 40;

  const int _inventoryRows = 2;
  const int _inventoryCols = 7;

  public Sprite BarGreen;
  public Sprite BarYellow; 
  public Sprite BarRed;

  public Image HungerBarBorder;
  public Image HungerBar;
  public Image ClassPortrait;

  public Text CharName;
  public Text StrVal;
  public Text DexVal;
  public Text IntVal;
  public Text HealthVal;
  public Text MagicVal;
  public Text AcVal;
  public Text CoinsVal;

  string _charNameAndTitle = string.Empty;
  Vector2 _hungerBarRectTransformSize = Vector2.zero;
  float _hungerYellowZone = 0.0f, _hungerRedZone = 0.0f;
  void Awake()
  {
    _hungerBarMaxWidth = (int)HungerBarBorder.rectTransform.rect.width - 6;
    _hungerBarRectTransformSize.Set((int)HungerBar.rectTransform.rect.width, (int)HungerBar.rectTransform.rect.height);
        
    _hungerYellowZone = _hungerBarMaxWidth / 3;
    _hungerRedZone = _hungerBarMaxWidth / 6;

    int slotNumber = 0;

    Vector2 slotsPosition = Vector2.zero;
    for (int i = 0; i < _inventoryRows; i++)
    {
      for (int j = 0; j < _inventoryCols; j++)
      {
        var go = (GameObject)Instantiate(InventorySlot);
        var rt = go.GetComponent<RectTransform>();
        if (rt != null)
        {
          rt.SetParent(InventorySlotsHolder, false);
          slotsPosition.x = -j * _invWidth;
          slotsPosition.y = i * _invHeight;
          rt.transform.localPosition = slotsPosition;
        }

        var slot = go.GetComponent<InventorySlot>();
        if (slot != null)
        {
          _inventorySlots.Add(slotNumber, slot);
        }

        slotNumber++;
      }
    }
  }  	

	void Update () 
	{
    StrVal.text = GameData.Instance.PlayerCharacterVariable.Strength.ToString();
    DexVal.text = GameData.Instance.PlayerCharacterVariable.Dexterity.ToString();
    IntVal.text = GameData.Instance.PlayerCharacterVariable.Willpower.ToString();

    HealthVal.text = string.Format("{0}/{1}", GameData.Instance.PlayerCharacterVariable.HitPoints.ToString(), 
                                              GameData.Instance.PlayerCharacterVariable.HitPointsMax.ToString());

    MagicVal.text = string.Format("{0}/{1}", GameData.Instance.PlayerCharacterVariable.Energy.ToString(), 
                                             GameData.Instance.PlayerCharacterVariable.EnergyMax.ToString());

    AcVal.text = GameData.Instance.PlayerCharacterVariable.ArmorClass.ToString();

    CalculateHungerBar();
    SetHungerBarColor();
	}

  void CalculateHungerBar()
  {
    int res = (GameData.Instance.PlayerCharacterVariable.Hunger * _hungerBarMaxWidth ) / GlobalConstants.HungerMax;
    _hungerBarRectTransformSize.x = res;
    HungerBar.rectTransform.sizeDelta = _hungerBarRectTransformSize;
  }

  void SetPortrait()
  {
    bool isFemale = GameData.Instance.PlayerCharacterVariable.IsFemale;

    if (GameData.Instance.PlayerCharacterVariable.GetCharacterClass == PlayerCharacter.CharacterClass.SOLDIER)
    {
      ClassPortrait.sprite = !isFemale ? GUIManager.Instance.MaleClassesPictures[0] : GUIManager.Instance.FemaleClassesPictures[0];
    }
    else if (GameData.Instance.PlayerCharacterVariable.GetCharacterClass == PlayerCharacter.CharacterClass.THIEF)
    {
      ClassPortrait.sprite = !isFemale ? GUIManager.Instance.MaleClassesPictures[1] : GUIManager.Instance.FemaleClassesPictures[1];
    }
    else if (GameData.Instance.PlayerCharacterVariable.GetCharacterClass == PlayerCharacter.CharacterClass.MAGE)
    {
      ClassPortrait.sprite = !isFemale ? GUIManager.Instance.MaleClassesPictures[2] : GUIManager.Instance.FemaleClassesPictures[2];
    }
  }

  void SetHungerBarColor()
  {
    float barWidth = HungerBar.rectTransform.sizeDelta.x;

    if (barWidth < _hungerYellowZone && barWidth > _hungerRedZone)
    {
      HungerBar.sprite = BarYellow;
    }
    else if (barWidth < _hungerRedZone && barWidth > 0)
    {
      HungerBar.sprite = BarRed;
    }
    else if (barWidth > _hungerYellowZone)
    {
      HungerBar.sprite = BarGreen;
    }
  }

  public void SetPlayerNameAndTitle()
  {
    _charNameAndTitle = GameData.Instance.PlayerCharacterVariable.CharacterNameAndTitle;
    CharName.text = _charNameAndTitle;

    SetPortrait();
  }

  public void AddItemToInventory(ItemObject item)
  {
    if (item == null)
    {
      Debug.LogWarning("Trying to add null item to inventory!");
      return;
    }

    int totalSlots = _inventoryRows * _inventoryCols;

    for (int i = totalSlots - 1; i >= 0; i--)
    {
      if (_inventorySlots[i].ItemRef == null)
      {
        _inventorySlots[i].SetItem(item);
        return;
      }
    }
  }

  public void CleanInventory()
  {
    for (int i = 0; i < _inventorySlots.Count; i++)
    {
      _inventorySlots[i].ItemRef = null;
    }
  }
}
