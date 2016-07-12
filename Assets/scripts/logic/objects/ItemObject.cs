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

  // Type of item for equipment slots resolution
  public GlobalConstants.WorldItemType ItemType = GlobalConstants.WorldItemType.PLACEHOLDER;

  protected InputController _inputControllerRef;

  public ItemObject(string name, string descriptionText, int atlasIcon, BehaviourItemObject bio, InputController inputController)
  {
    _inputControllerRef = inputController;
    ItemNameText = name;
    DescriptionText = descriptionText;
    AtlasIcon = atlasIcon;
    BIO = bio;
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
