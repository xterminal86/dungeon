using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceholderItemObject : ItemObject
{  
  public PlaceholderItemObject(string name, string descriptionText, int atlasIcon, BehaviourItemObject bio)
  {
    BIO = bio;
    AtlasIcon = atlasIcon;
    ItemNameText = name;
    DescriptionText = descriptionText;
  }

  public override void ActionHandler(object sender)
  {
    SoundManager.Instance.PlaySound(GlobalConstants.SFXItemTake);

    BIO.gameObject.SetActive(false);    
    GUIManager.Instance.ItemTakenSprite.sprite = GUIManager.Instance.GetIconFromAtlas(AtlasIcon);
    GUIManager.Instance.ItemTakenSprite.gameObject.SetActive(true);
    GUIManager.Instance.ItemTaken = this;    
  }

  public override void SaveReference()
  {
    GUIManager.Instance.ItemTakenCopy = this;
  }
}