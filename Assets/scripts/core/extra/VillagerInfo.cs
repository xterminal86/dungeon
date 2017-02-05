using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds information about villager NPC.
/// </summary>
public class VillagerInfo
{
  public string HailString = string.Empty;
  public string PortraitName = string.Empty;
  public string VillagerName = string.Empty;
  public string VillagerJob = string.Empty;
  public List<string> VillagerGossipLines = new List<string>();

  public override string ToString()
  {
    string result = string.Format("[VillagerInfo] : Name \"{0}\", Job \"{1}\", Gossip lines:\n", VillagerName, VillagerJob);

    foreach (var item in VillagerGossipLines)
    {
      result += item + "\n";
    }

    return result;
  }
};