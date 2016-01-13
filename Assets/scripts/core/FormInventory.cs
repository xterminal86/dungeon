using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FormInventory : MonoBehaviour 
{
  int _hungerBarMaxWidth = 0;

  public Image HungerBarBorder;
  public Image HungerBar;
  public Image ClassPortrait;

  public Text CharName;
  public Text StrVal;
  public Text DexVal;
  public Text IntVal;
  public Text HealthVal;
  public Text MagicVal;

  string _charNameAndTitle = string.Empty;
  Vector2 _rectTransformSize = Vector2.zero;
  void Awake()
  {
    _hungerBarMaxWidth = (int)HungerBarBorder.rectTransform.rect.width - 6;
    _rectTransformSize.Set((int)HungerBar.rectTransform.rect.width, (int)HungerBar.rectTransform.rect.height);

    _charNameAndTitle = PlayerData.Instance.PlayerCharacterVariable.CharacterNameAndTitle;
    CharName.text = _charNameAndTitle;

    SetPortrait();
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

    CalculateHungerBar();
	}

  int _hungerBarWidth = 0;
  void CalculateHungerBar()
  {
    int res = (PlayerData.Instance.PlayerCharacterVariable.Hunger * _hungerBarMaxWidth ) / GlobalConstants.HungerMax;
    _rectTransformSize.x = res;
    HungerBar.rectTransform.sizeDelta = _rectTransformSize;
  }

  void SetPortrait()
  {
    bool isFemale = PlayerData.Instance.PlayerCharacterVariable.IsFemale;

    if (PlayerData.Instance.PlayerCharacterVariable.GetCharacterClass == PlayerCharacter.CharacterClass.SOLDIER)
    {
      ClassPortrait.sprite = isFemale ? GUIManager.Instance.MaleClassesPictures[0] : GUIManager.Instance.FemaleClassesPictures[0];
    }
    else if (PlayerData.Instance.PlayerCharacterVariable.GetCharacterClass == PlayerCharacter.CharacterClass.THIEF)
    {
      ClassPortrait.sprite = isFemale ? GUIManager.Instance.MaleClassesPictures[1] : GUIManager.Instance.FemaleClassesPictures[1];
    }
    else if (PlayerData.Instance.PlayerCharacterVariable.GetCharacterClass == PlayerCharacter.CharacterClass.MAGE)
    {
      ClassPortrait.sprite = isFemale ? GUIManager.Instance.MaleClassesPictures[2] : GUIManager.Instance.FemaleClassesPictures[2];
    }
  }

  void Start () 
  { 
  }
}
