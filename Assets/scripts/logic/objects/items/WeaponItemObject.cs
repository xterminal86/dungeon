using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponItemObject : ItemObject
{
  int _minDamage = 0;
  int _maxDamage = 0;

  float _cooldown = 0;

  public WeaponItemObject(string name, string descriptionText, int atlasIcon, BehaviourItemObject bio, int minDam, int maxDam, int cool) 
    : base(name, descriptionText, atlasIcon, bio)
  {
    _minDamage = minDam;
    _maxDamage = maxDam;
    _cooldown = (float)cool / 1000.0f;
  }

  public override void RMBHandler(object sender)
  {
    FormPlayer playerForm = sender as FormPlayer;

    if (playerForm == null) return;

    JobManager.Instance.CreateCoroutine(AttackCooldownRoutine(playerForm));
  }

  IEnumerator AttackCooldownRoutine(FormPlayer fp)
  {
    GameData.Instance.PlayerCanAttack = false;

    int damage = Random.Range(_minDamage, _maxDamage + 1);

    if (fp.IsLeftSlotWasClicked)
    {
      fp.AttackBubbleL.SetActive(true);
      fp.AttackBubbleTextL.text = damage.ToString();
    }
    else
    {
      fp.AttackBubbleR.SetActive(true);
      fp.AttackBubbleTextR.text = damage.ToString();
    }
  

    fp.LockR.gameObject.SetActive(true);
    fp.LockL.gameObject.SetActive(true);

    float timer = 0.0f;

    //while (timer < GlobalConstants.AttackCooldown)
    while (timer < _cooldown)
    {
      if (timer > GlobalConstants.PlayerPunchAttackCooldown)
      {
        fp.AttackBubbleL.SetActive(false);
        fp.AttackBubbleR.SetActive(false);
      }

      timer += Time.smoothDeltaTime;

      yield return null;
    }

    GameData.Instance.PlayerCanAttack = true;

    fp.LockR.gameObject.SetActive(false);
    fp.LockL.gameObject.SetActive(false);

    yield return null;
  }
}
