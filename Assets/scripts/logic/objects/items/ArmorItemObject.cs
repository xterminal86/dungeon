using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArmorItemObject : ItemObject 
{
  int _armorClassModifier = 0;
  public ArmorItemObject(string name, string desc, int atlasIcon, BehaviourItemObject bio, int armorClassModifier, InputController inputController)
    : base(name, desc, atlasIcon, bio, inputController)
  {
    _armorClassModifier = armorClassModifier;
  }	
}
