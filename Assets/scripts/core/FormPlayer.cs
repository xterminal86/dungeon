using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FormPlayer : MonoBehaviour
{
  int _barMaxWidth = 0;

  public GameObject FormHolder;

  public Image HealthBar;
  public Image MagicBar;

  public Image BarBorder;

  public Image ItemLH;
  public Image ItemRH;

  void Awake()
  {
    _barMaxWidth = (int)BarBorder.rectTransform.rect.width - 6;
    _rectTransformSize.Set((int)HealthBar.rectTransform.rect.width, (int)HealthBar.rectTransform.rect.height);
  }

  int _healthWidth = 0, _magicWidth = 0;
  Vector2 _rectTransformSize = Vector2.zero;
  void Update()
  {
    CalculateBarWidth(GameData.Instance.PlayerCharacterVariable.HitPoints, 
                      GameData.Instance.PlayerCharacterVariable.HitPointsMax, 
                      HealthBar);

    CalculateBarWidth(GameData.Instance.PlayerCharacterVariable.Energy, 
                      GameData.Instance.PlayerCharacterVariable.EnergyMax, 
                      MagicBar);
  }

  void CalculateBarWidth(int current, int max, Image bar)
  {
    int res = (current * _barMaxWidth) / max;
    _rectTransformSize.x = res;
    bar.rectTransform.sizeDelta = _rectTransformSize;
  }

  public void ShowForm(bool visibilityFlag)
  {
    FormHolder.SetActive(visibilityFlag);
  }

  public void BackpackClicked()
  {
    if (App.Instance.PlayerMoveState == App.PlayerMoveStateEnum.NORMAL)
    {
      GUIManager.Instance.ToggleInventoryWindow();
    }
  }

  public void LeftHandClicked()
  {
    if (Input.GetMouseButtonDown(0))
    {
      return;
    }

    SoundManager.Instance.PlaySound(GlobalConstants.SFXSwing);

    InputController.Instance.DrawTrail();

    //Debug.Log("Left Hand");
  }

  public void RightHandClicked()
  {
    if (Input.GetMouseButtonDown(0))
    {
      return;
    }
    
    //Debug.Log("Right Hand");
  }

  void Start()
  {
  }
}
