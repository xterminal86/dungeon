using UnityEngine;
using UnityEngine.SceneManagement;
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
    StrValue.text = GameData.Instance.PlayerCharacterVariable.Strength.ToString();
    DexValue.text = GameData.Instance.PlayerCharacterVariable.Dexterity.ToString();
    WillValue.text = GameData.Instance.PlayerCharacterVariable.Willpower.ToString();

    HPValue.text = GameData.Instance.PlayerCharacterVariable.HitPoints.ToString();
    MPValue.text = GameData.Instance.PlayerCharacterVariable.Energy.ToString();
    ACValue.text = GameData.Instance.PlayerCharacterVariable.ArmorClass.ToString();

    ClassName.text = GlobalConstants.CharacterClassNames[_indexer];
    ClassDescrption.text = GlobalConstants.CharacterClassDescriptions[_indexer];

    MaleToggle.isOn = !GameData.Instance.PlayerCharacterVariable.IsFemale;

    ClassImage.sprite = MaleToggle.isOn ? GUIManager.Instance.MaleClassesPictures[_indexer] : GUIManager.Instance.FemaleClassesPictures[_indexer];    
  }

  public void ToggleMale()
  {
    GameData.Instance.PlayerCharacterVariable.IsFemale = false;
  }

  public void ToggleFemale()
  {
    GameData.Instance.PlayerCharacterVariable.IsFemale = true;
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

    GameData.Instance.PlayerCharacterVariable.ResetToDefault();
  }

  public void OK()
  {
    GameData.Instance.PlayerCharacterVariable.CharacterName = string.IsNullOrEmpty(CharacterName.text) ? "Nameless" : CharacterName.text;

    GUIManager.Instance.ButtonClickSound.Play();

    gameObject.SetActive(false);

    GUIManager.Instance.InventoryForm.SetPlayerNameAndTitle();

    ScreenFader.Instance.FadeCompleteCallback += FadeCompleteHandler;
    ScreenFader.Instance.FadeOut();
  }

  void FadeCompleteHandler()
  {
    ScreenFader.Instance.FadeCompleteCallback -= FadeCompleteHandler;
    SceneManager.LoadScene("main");
  }

  void ReadStats()
  {
    if (_indexer == 0)
    {
      GameData.Instance.PlayerCharacterVariable.InitSoldier();
    }
    else if (_indexer == 1)
    {
      GameData.Instance.PlayerCharacterVariable.InitThief();
    }
    else if (_indexer == 2)
    {
      GameData.Instance.PlayerCharacterVariable.InitMage();
    }
  }
}
