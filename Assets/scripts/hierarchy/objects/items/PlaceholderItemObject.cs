using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceholderItemObject : ItemObject
{
  string _descriptionText = string.Empty;

  public PlaceholderItemObject(string descriptionText, int atlasIcon, BehaviourItemObject bio)
  {
    BIO = bio;
    AtlasIcon = atlasIcon;

    _descriptionText = descriptionText;
  }

  public override void ActionHandler(object sender)
  {
    SoundManager.Instance.PlaySound("player-item-take");

    BIO.gameObject.SetActive(false);
    GUIManager.Instance.ItemTakenSprite.sprite = GUIManager.Instance.GetIconFromAtlas(AtlasIcon);
    GUIManager.Instance.ItemTakenSprite.gameObject.SetActive(true);
    GUIManager.Instance.ItemTaken = this;
  }
}