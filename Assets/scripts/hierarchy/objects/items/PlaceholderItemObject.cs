using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Item that has no purpose (rock, note etc.)
/// </summary>
public class PlaceholderItemObject : ItemObject
{  
  public PlaceholderItemObject(string name, string descriptionText, int atlasIcon, BehaviourItemObject bio)
  {
    BIO = bio;
    AtlasIcon = atlasIcon;
    ItemNameText = name;
    DescriptionText = descriptionText;
  }
}