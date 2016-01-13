using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FormNewGame : MonoBehaviour 
{
  // Player data fields

  public Text StrValue;
  public Text DexValue;
  public Text WillValue;

  public Text HPValue;
  public Text MPValue;
  public Text ACValue;

  public Text CharacterName;

  // To read

  public Text ClassName;
  public Text ClassDescrption;

  public Toggle MaleToggle;
  public Toggle FemaleToggle;

  public Image ClassImage;


  void Update() 
  {
    StrValue.text = PlayerData.Instance.PlayerCharacterVariable.Strength.ToString();
    DexValue.text = PlayerData.Instance.PlayerCharacterVariable.Dexterity.ToString();
    WillValue.text = PlayerData.Instance.PlayerCharacterVariable.Willpower.ToString();

    HPValue.text = PlayerData.Instance.PlayerCharacterVariable.HitPoints.ToString();
    MPValue.text = PlayerData.Instance.PlayerCharacterVariable.Energy.ToString();
    ACValue.text = PlayerData.Instance.PlayerCharacterVariable.ArmorClass.ToString();

    ClassName.text = GlobalConstants.CharacterClassNames[_indexer];
    ClassDescrption.text = GlobalConstants.CharacterClassDescriptions[_indexer];

    MaleToggle.isOn = !PlayerData.Instance.PlayerCharacterVariable.IsFemale;

    ClassImage.sprite = MaleToggle.isOn ? GUIManager.Instance.MaleClassesPictures[_indexer] : GUIManager.Instance.FemaleClassesPictures[_indexer];    
  }

  public void ToggleMale()
  {
    PlayerData.Instance.PlayerCharacterVariable.IsFemale = false;
  }

  public void ToggleFemale()
  {
    PlayerData.Instance.PlayerCharacterVariable.IsFemale = true;
  }

  int _indexer = 0;
  public void LeftArrowHandler()
  {
    GUIManager.Instance.ButtonClickSoundShort.Play();

    _indexer--;

    if (_indexer < 0)
    {
      _indexer = GlobalConstants.CharacterClassNames.Count - 1;
    }

    ReadStats();
  }

  public void RightArrowHandler()
  {
    GUIManager.Instance.ButtonClickSoundShort.Play();

    _indexer++;

    if (_indexer == GlobalConstants.CharacterClassNames.Count)
    {
      _indexer = 0;
    }

    ReadStats();
  }

  public void CloseForm()
  {
    GUIManager.Instance.ButtonClickSound.Play();

    gameObject.SetActive(false);
    GUIManager.Instance.TitleScreenButtonsHolder.SetActive(true);
  }

  public void SetupForm()
  {
    _indexer = 0;

    PlayerData.Instance.PlayerCharacterVariable.ResetToDefault();
  }

  public void OK()
  {
    PlayerData.Instance.PlayerCharacterVariable.CharacterName = string.IsNullOrEmpty(CharacterName.text) ? "Nameless" : CharacterName.text;

    GUIManager.Instance.ButtonClickSound.Play();

    gameObject.SetActive(false);

    ScreenFader.Instance.FadeCompleteCallback += FadeCompleteHandler;
    ScreenFader.Instance.FadeOut();
  }

  void FadeCompleteHandler()
  {
    ScreenFader.Instance.FadeCompleteCallback -= FadeCompleteHandler;
    Application.LoadLevel("main");
  }

  void ReadStats()
  {
    if (_indexer == 0)
    {
      PlayerData.Instance.PlayerCharacterVariable.InitSoldier();
    }
    else if (_indexer == 1)
    {
      PlayerData.Instance.PlayerCharacterVariable.InitThief();
    }
    else if (_indexer == 2)
    {
      PlayerData.Instance.PlayerCharacterVariable.InitMage();
    }
  }
}
