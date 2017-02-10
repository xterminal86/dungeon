using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents one block in a map.
/// </summary>
public class BlockEntity
{
  public bool Walkable = true;
  public bool IsLiquid = false;
  public bool SkipInstantiation = false;
  public string BlockName = string.Empty;
  public Vector3 ArrayCoordinates = Vector3.zero;
  public Vector3 WorldCoordinates = Vector3.zero;
  public GlobalConstants.FootstepSoundType FootstepSound = GlobalConstants.FootstepSoundType.DUMMY;

  public Dictionary<GlobalConstants.Orientation, bool> SidesWalkability = new Dictionary<GlobalConstants.Orientation, bool>();

  GlobalConstants.BlockType _blockType = GlobalConstants.BlockType.AIR;
  public GlobalConstants.BlockType BlockType
  {
    get { return _blockType; }
    set
    {
      _blockType = value;

      // TODO: what about lava, water etc.?
      if (_blockType != GlobalConstants.BlockType.AIR)
      {
        Walkable = false;
      }

      SetFootstepSound();
    }
  }

  public BlockEntity()
  {
    Walkable = true;

    SidesWalkability[GlobalConstants.Orientation.EAST] = true;
    SidesWalkability[GlobalConstants.Orientation.SOUTH] = true;
    SidesWalkability[GlobalConstants.Orientation.WEST] = true;
    SidesWalkability[GlobalConstants.Orientation.NORTH] = true;
  }

  public void SetFootstepSound()
  {
    switch (_blockType)
    {
      case GlobalConstants.BlockType.GRASS:
        FootstepSound = GlobalConstants.FootstepSoundType.GRASS;
        break;

      case GlobalConstants.BlockType.DIRT:
        FootstepSound = GlobalConstants.FootstepSoundType.DIRT;
        break;

      case GlobalConstants.BlockType.STONE:
        FootstepSound = GlobalConstants.FootstepSoundType.STONE;
        break;

      case GlobalConstants.BlockType.WOOD_OAK:
        FootstepSound = GlobalConstants.FootstepSoundType.WOOD;
        break;

      default:        
        break;        
    }
  }
}
