using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponItemObject : ItemObject
{
  int _minDamage = 0;
  int _maxDamage = 0;
  int _cooldown = 0;

  public WeaponItemObject(string name, string descriptionText, int atlasIcon, BehaviourItemObject bio, int minDam, int maxDam, int cool) 
    : base(name, descriptionText, atlasIcon, bio)
  {
    _minDamage = minDam;
    _maxDamage = maxDam;
    _cooldown = cool;
  }
}
