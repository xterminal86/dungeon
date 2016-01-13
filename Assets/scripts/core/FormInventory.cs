using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FormInventory : MonoBehaviour 
{
  int _hungerBarMaxWidth = 0;

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

  string _charNameAndTitle = string.Empty;
  Vector2 _hungerBarRectTransformSize = Vector2.zero;
  float _hungerYellowZone = 0.0f, _hungerRedZone = 0.0f;
  void Awake()
  {    
    _hungerBarMaxWidth = (int)HungerBarBorder.rectTransform.rect.width - 6;
    _hungerBarRectTransformSize.Set((int)HungerBar.rectTransform.rect.width, (int)HungerBar.rectTransform.rect.height);

    _charNameAndTitle = PlayerData.Instance.PlayerCharacterVariable.CharacterNameAndTitle;
    CharName.text = _charNameAndTitle;

    SetPortrait();

    _hungerYellowZone = _hungerBarMaxWidth / 3;
    _hungerRedZone = _hungerBarMaxWidth / 6;
  }  	

	void Update () 
	{
    StrVal.text = PlayerData.Instance.PlayerCharacterVariable.Strength.ToString();
    DexVal.text = PlayerData.Instance.PlayerCharacterVariable.Dexterity.ToString();
    IntVal.text = PlayerData.Instance.PlayerCharacterVariable.Willpower.ToString();

    HealthVal.text = string.Format("{0}/{1}", PlayerData.Instance.PlayerCharacterVariable.HitPoints.ToString(), 
                                              PlayerData.Instance.PlayerCharacterVariable.HitPointsMax.ToString());

    MagicVal.text = string.Format("{0}/{1}", PlayerData.Instance.PlayerCharacterVariable.Energy.ToString(), 
                                             PlayerData.Instance.PlayerCharacterVariable.EnergyMax.ToString());

    AcVal.text = PlayerData.Instance.PlayerCharacterVariable.ArmorClass.ToString();

    CalculateHungerBar();
    SetHungerBarColor();
	}

  int _hungerBarWidth = 0;
  void CalculateHungerBar()
  {
    int res = (PlayerData.Instance.PlayerCharacterVariable.Hunger * _hungerBarMaxWidth ) / GlobalConstants.HungerMax;
    _hungerBarRectTransformSize.x = res;
    HungerBar.rectTransform.sizeDelta = _hungerBarRectTransformSize;
  }

  void SetPortrait()
  {
    bool isFemale = PlayerData.Instance.PlayerCharacterVariable.IsFemale;

    if (PlayerData.Instance.PlayerCharacterVariable.GetCharacterClass == PlayerCharacter.CharacterClass.SOLDIER)
    {
      ClassPortrait.sprite = !isFemale ? GUIManager.Instance.MaleClassesPictures[0] : GUIManager.Instance.FemaleClassesPictures[0];
    }
    else if (PlayerData.Instance.PlayerCharacterVariable.GetCharacterClass == PlayerCharacter.CharacterClass.THIEF)
    {
      ClassPortrait.sprite = !isFemale ? GUIManager.Instance.MaleClassesPictures[1] : GUIManager.Instance.FemaleClassesPictures[1];
    }
    else if (PlayerData.Instance.PlayerCharacterVariable.GetCharacterClass == PlayerCharacter.CharacterClass.MAGE)
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

  void Start ()
  {    
  }
}
