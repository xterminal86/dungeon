using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FormPlayer : MonoBehaviour
{
  int _barMaxWidth = 0;

  public GameObject FormHolder;

  public EquipmentSlot HandEqSlotLeft;
  public EquipmentSlot HandEqSlotRight;

  public Image HealthBar;
  public Image MagicBar;

  public Image BarBorder;

  public Image ItemLH;
  public Image ItemRH;

  public Image LockR;
  public Image LockL;

  public Sprite HandSpriteL;
  public Sprite HandSpriteR;

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

    ItemLH.sprite = (HandEqSlotLeft.ItemRef != null) ? HandEqSlotLeft.ItemImage.sprite : HandSpriteL;
    ItemRH.sprite = (HandEqSlotRight.ItemRef != null) ? HandEqSlotRight.ItemImage.sprite : HandSpriteR;
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
    if (GameData.Instance.PlayerMoveState == GameData.PlayerMoveStateEnum.NORMAL)
    {
      GUIManager.Instance.ToggleInventoryWindow();
    }
  }

  public void LeftHandClicked()
  {   
    if (Input.GetMouseButtonDown(0))
    {
      HandEqSlotLeft.ProcessItem();

      return;
    }
    else if (Input.GetMouseButtonDown(1))
    {
      ProcessPlayerAttack(HandEqSlotLeft.ItemRef);
    }
  }

  public void RightHandClicked()
  {
    if (Input.GetMouseButtonDown(0))
    {
      HandEqSlotRight.ProcessItem();

      return;
    }
    else if (Input.GetMouseButtonDown(1))
    {
      ProcessPlayerAttack(HandEqSlotRight.ItemRef);
    }
  }

  void ProcessPlayerAttack(ItemObject io)
  {
    if (GameData.Instance.PlayerCanAttack)
    {
      SoundManager.Instance.PlaySound(GlobalConstants.SFXSwing);
      //InputController.Instance.DrawTrail();

      // Attack with weapon in hand
      if (io != null)
      {
        if (io.RMBAction != null)
          io.RMBAction(this);
      }
      // Process attack with bare hand
      else
      {
        StartCoroutine(PunchRoutine());
      }
    }
  }

  IEnumerator PunchRoutine()
  {
    GameData.Instance.PlayerCanAttack = false;

    LockR.gameObject.SetActive(true);
    LockL.gameObject.SetActive(true);

    float timer = 0.0f;

    while (timer < GlobalConstants.PlayerPunchAttackCooldown)
    {
      timer += Time.smoothDeltaTime;

      yield return null;
    }

    GameData.Instance.PlayerCanAttack = true;

    LockR.gameObject.SetActive(false);
    LockL.gameObject.SetActive(false);

    yield return null;
  }

  void Start()
  {
  }
}
