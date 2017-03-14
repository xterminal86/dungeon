using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour 
{ 
  public ScenesList SceneToLoad;

  public bool SkipTitleScreen = false;

  public PlayerCharacter.CharacterClass CharacterClass;

  void Start() 
	{ 
    GameData.Instance.Initialize();
    GUIManager.Instance.Initialize();
    JobManager.Instance.Initialize();
    PrefabsManager.Instance.Initialize();
    ThreadWatcher.Instance.Initialize();
    ScreenFader.Instance.Initialize();
    SoundManager.Instance.Initialize();
    DateAndTime.Instance.Initialize();
    LevelLoader.Instance.Initialize();
    InputController.Instance.Initialize();

    LevelLoader.Instance.LoadLevel(SceneToLoad);

    if (SkipTitleScreen)
    {
      switch (CharacterClass)
      {
        case PlayerCharacter.CharacterClass.SOLDIER:
          GameData.Instance.PlayerCharacterVariable.InitSoldier();
          break;

        case PlayerCharacter.CharacterClass.THIEF:
          GameData.Instance.PlayerCharacterVariable.InitThief();
          break;

        case PlayerCharacter.CharacterClass.MAGE:
          GameData.Instance.PlayerCharacterVariable.InitMage();
          break;

        default:
          GameData.Instance.PlayerCharacterVariable.ResetToDefault();
          break;
      }

      GUIManager.Instance.InventoryForm.SetPlayerNameAndTitle();
      GUIManager.Instance.InventoryForm.CoinsVal.text = GameData.Instance.PlayerCharacterVariable.Coins.ToString();

      SceneManager.LoadScene("main");
    }
    else
    {
      SceneManager.LoadScene("title");
    }
	}
}

public enum ScenesList
{
  VILLAGE = 0,
  TEST1
}
