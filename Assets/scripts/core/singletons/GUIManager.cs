using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GUIManager : MonoSingleton<GUIManager> 
{
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
  public FormItemInfo ItemInfoForm;
  
  // Icon of item being manipulated
  public Image ItemTakenSprite;
  // Item that is currently in hand
  public ItemObject ItemTaken;
  public ItemObject ItemTakenCopy;

  public GameObject InventoryFormWindow;

  [HideInInspector]
  public List<Sprite> Portraits = new List<Sprite>();

  public List<Sprite> MaleClassesPictures = new List<Sprite>();
  public List<Sprite> FemaleClassesPictures = new List<Sprite>();

  // Inventory icons
  List<Sprite> _itemsIcons = new List<Sprite>();  

  protected override void Init()
  {
    base.Init();
    
    Sprite[] _spritesAtlas = Resources.LoadAll<Sprite>("sprites-atlas/atlas");
    if (_spritesAtlas != null)
    {
      foreach (var item in _spritesAtlas)
      {        
        _itemsIcons.Add(item);
      }
    }
  }  

  public Sprite GetIconFromAtlas(int index)
  {
    return _itemsIcons[index];
  }

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

  // Title Screen Buttons

  public void ButtonNewGameHandler()
  {
    SetupNewGameForm();
  }

  public void ButtonOptionsHandler()
  {
  }

  public void ButtonStatisticsHandler()
  {
  }

  public void ButtonExitHandler()
  {    
    ScreenFader.Instance.FadeOut(() => { Application.Quit(); });
  }

  // FormTalking

  public Callback ButtonNameCallback;
  public Callback ButtonJobCallback;
  public Callback ButtonGossipCallback;
  public Callback ButtonByeCallback;
  
  // Buttons handlers

  public void FormTalkingNameHandler()
  {
    if (ButtonNameCallback != null)
      ButtonNameCallback();  
  }

  public void FormTalkingJobHandler()
  {
    if (ButtonJobCallback != null)
      ButtonJobCallback();    
  }

  public void FormTalkingGossipHandler()
  {
    if (ButtonGossipCallback != null)
      ButtonGossipCallback();    
  }

  public void FormTalkingByeHandler()
  {
    if (ButtonByeCallback != null)
      ButtonByeCallback();    
  }

  // Form Game Menu (stubs)

  public void FormMenuSaveAndQuitHandler()
  {
    FormGameMenu.SetActive(false);

    CloseGameForms();

    GameData.Instance.CurrentGameState = GameData.GameState.PAUSED;

    JobManager.Instance.StopAllCoroutines();

    ScreenFader.Instance.FadeOut(() => 
      { 
        SoundManager.Instance.StopAllSounds();
        Application.Quit(); 
      });
  }

  public void FormMenuReturnToMenuHandler()
  {
    FormGameMenu.SetActive(false);

    CloseGameForms();

    GameData.Instance.CurrentGameState = GameData.GameState.PAUSED;

    JobManager.Instance.StopAllCoroutines();

    ScreenFader.Instance.FadeOut(() => 
      { 
        InventoryForm.CleanInventory();
        SoundManager.Instance.StopAllSounds();
        SceneManager.LoadScene("title"); 
      });
  }

  public void FormMenuResumeHandler()
  {
    FormGameMenu.SetActive(false);

    GameData.Instance.PlayerMoveState = GameData.PlayerMoveStateEnum.NORMAL;
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

  public void CloseGameForms()
  {
    PlayerForm.gameObject.SetActive(false); 
    CompassImage.gameObject.SetActive(false);
    FormTalking.SetActive(false);
  }

  public void SetupGameForms()
  {
    PlayerForm.gameObject.SetActive(true); 
    CompassImage.gameObject.SetActive(true);
    FormTalking.SetActive(false);
  }

  Vector2 _mousePosition = Vector2.zero;
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.F9)) 
    {
      ScreenCapture.CaptureScreenshot(string.Format("scr-{0}{1}{2}.png", System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second));
      SoundManager.Instance.PlaySound(GlobalConstants.SFXTakeScreenshot);
    }

    _mousePosition = Input.mousePosition;

    ItemTakenSprite.rectTransform.position = _mousePosition;
  }
}
