using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArmorItemObject : ItemObject 
{
  int _armorClassModifier = 0;
  public ArmorItemObject(string name, string desc, int atlasIcon, BehaviourItemObject bio, int armorClassModifier)
    : base(name, desc, atlasIcon, bio)
  {
    _armorClassModifier = armorClassModifier;
  }	
}
