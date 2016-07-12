using UnityEngine;
using System.Collections;

public class FoodItemObject : ItemObject
{
  int _saturation = 0;
  public FoodItemObject(string name, string desc, int atlasIcon, BehaviourItemObject bio, int saturation, InputController inputController)
    : base(name, desc, atlasIcon, bio, inputController)
  {
    _saturation = saturation;
  }

  public override void RMBHandler(object sender)
  {
    SoundManager.Instance.PlaySound(GlobalConstants.SFXPlayerEat);
    GameData.Instance.PlayerCharacterVariable.AddHunger(_saturation);
    (sender as InventorySlot).DeleteItem();
    GameObject.Destroy(BIO.Model.gameObject);
    GameObject.Destroy(BIO.gameObject);
  }
}
