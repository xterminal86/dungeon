using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Logic class of world item (sword, amulet, scroll etc.)
/// </summary>
public abstract class ItemObject
{
  // Reference to BehaviourItemObject component
  public BehaviourItemObject BIO;

  // Hash of icon in atlas
  public int AtlasIcon = -1;

  public CallbackO LMBAction;
  public CallbackO RMBAction;

  // For information window
  public string ItemNameText = string.Empty;
  public string DescriptionText = string.Empty;

  public ItemObject()
  {
  }

  // Method for saving reference to this instance of IO
  // (used for items swapping in inventory)
  public virtual void SaveReference() 
  { 
    GUIManager.Instance.ItemTakenCopy = this;
  }

  public virtual void LMBHandler(object sender) 
  {
    SoundManager.Instance.PlaySound(GlobalConstants.SFXItemTake);
    
    BIO.gameObject.SetActive(false);    
    GUIManager.Instance.ItemTakenSprite.sprite = GUIManager.Instance.GetIconFromAtlas(AtlasIcon);
    GUIManager.Instance.ItemTakenSprite.gameObject.SetActive(true);
    GUIManager.Instance.ItemTaken = this;
  }

  public virtual void RMBHandler(object sender) 
  {
  }
}
