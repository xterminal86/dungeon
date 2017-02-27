using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds information about villager NPC.
/// </summary>
public class NPCInfo
{
  public string HailString = string.Empty;
  public string PortraitName = string.Empty;
  public string Name = string.Empty;
  public string Job = string.Empty;
  public List<string> GossipLines = new List<string>();

  public override string ToString()
  {
    string result = string.Format("[VillagerInfo] : Name \"{0}\", Job \"{1}\", Gossip lines:\n", Name, Job);

    foreach (var item in GossipLines)
    {
      result += item + "\n";
    }

    return result;
  }
};