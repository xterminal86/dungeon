using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class LevelLoader : MonoSingleton<LevelLoader> 
{
  LevelBase _levelMap;
  public LevelBase LevelMap
  {
    get { return _levelMap; }
  }

  Int3 _levelSize = new Int3(20, 20, 20);
  public Int3 LevelSize
  {
    get { return _levelSize; }
  }

  Dictionary<int, NPCInfo> _npcInfo = new Dictionary<int, NPCInfo>();
  public Dictionary<int, NPCInfo> NPCInfo
  {
    get { return _npcInfo; }
  }

  public void LoadLevel(ScenesList scene)
  {
    switch (scene)
    {
      case ScenesList.VILLAGE:
        _levelMap = new DarwinVillage(_levelSize.X, _levelSize.Y, _levelSize.Z);
        break;

      case ScenesList.TEST1:
        _levelMap = new TestLevel(_levelSize.X, _levelSize.Y, _levelSize.Z);
        break;
    }

    _levelMap.Generate();

    InputController.Instance.SetupCamera(_levelMap.PlayerPos);

    //Camera.main.farClipPlane = _levelSize.X * GlobalConstants.WallScaleFactor;
  }

  // TODO: rewrite

  public void LoadNPCData()
  {
    //var resource = Resources.Load("text/OneVillager");
    //var resource = Resources.Load("text/NoVillagers");
    var resource = Resources.Load("text/Villagers");

    if (resource == null) return;

    TextAsset ta = resource as TextAsset;

    XmlDocument doc = new XmlDocument();
    doc.LoadXml(ta.text);
    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
    {      
      switch (node.Name)
      {
        case "CHARACTER":
          if (node.Attributes.GetNamedItem("prefab") == null)
          {
            continue;
          }

          NPCInfo vi = new NPCInfo();
          int hash = node.Attributes["name"].InnerText.GetHashCode();

          string prefabName = node.Attributes["prefab"].InnerText;
          string actorClass = node.Attributes["class"].InnerText;

          /*
          var prefab = FindCharacterPrefabByName(prefabName);
          if (prefab != null)
          {
            //Int2 pos = _generatedMap.GetRandomUnoccupiedCell();
            Int2 pos = new Int2();
            SpawnCharacter(prefab, pos, actorClass);
          }
          */

          vi.HailString = node.Attributes["hailString"].InnerText;
          vi.PortraitName = node.Attributes["portraitName"].InnerText;

          foreach (XmlNode tag in node.ChildNodes)
          {
            switch (tag.Name)
            {
              case "NAME":
                vi.Name = tag.Attributes["text"].InnerText;
                break;
              case "JOB":
                vi.Job = tag.Attributes["text"].InnerText;
                break;
              case "GOSSIP":
                foreach (XmlNode line in tag.ChildNodes)
                {
                  switch (line.Name)
                  {
                    case "LINE":
                      vi.GossipLines.Add(line.Attributes["text"].InnerText);
                      break;
                    default:
                      break;
                  }
                }
                break;
              default:
                break;
            }
          }

          _npcInfo.Add(hash, vi);

          break;
        default:
          break;
      }
    }
  }
}
