﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GUIManager : MonoSingleton<GUIManager> 
{
  public AudioSource ButtonClickSound;
  public AudioSource CharacterSpeakSound;
  
  public GameObject FormTalking;
  public GameObject FormCompass;
  public GameObject FormGameMenu;

  public Image CompassImage;
  public Image PortraitImage;

  public Text FormTalkingName;
  public Text FormTalkingText;

  [HideInInspector]
  public List<Sprite> Portraits = new List<Sprite>();

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

    ScreenFader.Instance.FadeCompleteCallback += SaveAndQuitHandler;
    ScreenFader.Instance.FadeOut();
  }

  public void FormMenuReturnToMenuHandler()
  {
    ButtonClickSound.Play();

    ScreenFader.Instance.FadeCompleteCallback += ReturnToMenuHandler;
    ScreenFader.Instance.FadeOut();
  }

  public void FormMenuResumeHandler()
  {
    ButtonClickSound.Play();

    FormGameMenu.SetActive(false);

    App.Instance.PlayerMoveState = App.PlayerMoveStateEnum.NORMAL;
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
