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
  public Vector3 ArrayCoordinates = Vector3.zero;
  public Vector3 WorldCoordinates = Vector3.zero;
  public GlobalConstants.FootstepSoundType FootstepSound = GlobalConstants.FootstepSoundType.DUMMY;

  public TeleporterWorldObject Teleporter = null;

  public List<WorldObject> WorldObjects = new List<WorldObject>();
  public Dictionary<GlobalConstants.Orientation, bool> SidesWalkability = new Dictionary<GlobalConstants.Orientation, bool>();

  // In order to place wall columns we'll be going to rely on this dictionary
  // where wall will be shared between two cells. This way we can simplify the conditions in SetWallColumns().
  // We shouldn't rely on SidesWalkability because it might be possible to place
  // wall and don't block the path (illusionary wall or something).
  public Dictionary<GlobalConstants.Orientation, WallWorldObject> WallsByOrientation = new Dictionary<GlobalConstants.Orientation, WallWorldObject>();

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

    WallsByOrientation[GlobalConstants.Orientation.EAST] = null;
    WallsByOrientation[GlobalConstants.Orientation.SOUTH] = null;
    WallsByOrientation[GlobalConstants.Orientation.WEST] = null;
    WallsByOrientation[GlobalConstants.Orientation.NORTH] = null;
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

      case GlobalConstants.BlockType.OAK_PLANKS:
        FootstepSound = GlobalConstants.FootstepSoundType.WOOD;
        break;

      default:        
        break;        
    }
  }
}
