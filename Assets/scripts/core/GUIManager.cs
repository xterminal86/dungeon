using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GUIManager : MonoSingleton<GUIManager> 
{
  public AudioSource ButtonClickSound;
  public AudioSource ButtonClickSoundShort;
  public AudioSource CharacterSpeakSound;

  public GameObject TitleScreenButtonsHolder;
  public GameObject FormTalking;
  public GameObject FormCompass;
  public GameObject FormGameMenu;

  public Image CompassImage;
  public Image PortraitImage;

  public Text FormTalkingName;
  public Text FormTalkingText;

  public FormNewGame NewGameForm;
  public FormPlayer PlayerForm;
  public FormInventory InventoryForm;

  public GameObject InventoryFormWindow;

  [HideInInspector]
  public List<Sprite> Portraits = new List<Sprite>();

  public List<Sprite> MaleClassesPictures = new List<Sprite>();
  public List<Sprite> FemaleClassesPictures = new List<Sprite>();

  public Sprite FindPortraitByName(string name)
  {
    foreach (var item in Portraits)
    {
      if (item.name == name)
      {
        return item;
      }
    }

    return null;
  }

  public void SetCompassVisibility(bool showFlag)
  {
    FormCompass.SetActive(showFlag);
  }

  public void ToggleInventoryWindow()
  {
    InventoryFormWindow.gameObject.SetActive(!InventoryFormWindow.gameObject.activeSelf);
  }

  // FormTalking

  public Callback ButtonNameCallback;
  public Callback ButtonJobCallback;
  public Callback ButtonGossipCallback;
  public Callback ButtonByeCallback;
  
  // Buttons handlers

  public void FormTalkingNameHandler()
  {
    ButtonClickSound.Play();

    if (ButtonNameCallback != null)
      ButtonNameCallback();  
  }

  public void FormTalkingJobHandler()
  {
    ButtonClickSound.Play();

    if (ButtonJobCallback != null)
      ButtonJobCallback();    
  }

  public void FormTalkingGossipHandler()
  {
    ButtonClickSound.Play();

    if (ButtonGossipCallback != null)
      ButtonGossipCallback();    
  }

  public void FormTalkingByeHandler()
  {
    ButtonClickSound.Play();

    if (ButtonByeCallback != null)
      ButtonByeCallback();    
  }

  // Form Game Menu (stubs)

  public void FormMenuSaveAndQuitHandler()
  {
    ButtonClickSound.Play();

    App.Instance.CurrentGameState = App.GameState.PAUSED;

    ScreenFader.Instance.FadeCompleteCallback += SaveAndQuitHandler;
    ScreenFader.Instance.FadeOut();
  }

  public void FormMenuReturnToMenuHandler()
  {
    ButtonClickSound.Play();

    App.Instance.CurrentGameState = App.GameState.PAUSED;

    ScreenFader.Instance.FadeCompleteCallback += ReturnToMenuHandler;
    ScreenFader.Instance.FadeOut();
  }

  public void FormMenuResumeHandler()
  {
    ButtonClickSound.Play();

    FormGameMenu.SetActive(false);

    App.Instance.PlayerMoveState = App.PlayerMoveStateEnum.NORMAL;
  }

  // Form New Game

  public void SetupNewGameForm()
  {
    TitleScreenButtonsHolder.SetActive(false);
    NewGameForm.gameObject.SetActive(true);

    NewGameForm.SetupForm();
  }

  // Form Inventory

  public void InventorySlotClicked()
  {
    if (Input.GetMouseButtonUp(0))
    {
      Debug.Log("Left button");
    }
    else if (Input.GetMouseButtonUp(1))
    {
      Debug.Log("Right button");
    }
  }

  // Private Methods

  void SaveAndQuitHandler()
  {
    StopAllCoroutines();

    ScreenFader.Instance.FadeCompleteCallback -= SaveAndQuitHandler;
    Application.Quit();
  }

  void ReturnToMenuHandler()
  {
    StopAllCoroutines();

    var objects = FindObjectsOfType<GameObject>();
    foreach (var item in objects)
    {
      Destroy(item.gameObject);
    }

    ScreenFader.Instance.FadeCompleteCallback -= ReturnToMenuHandler;
    Application.LoadLevel("entry");
  }  
}
