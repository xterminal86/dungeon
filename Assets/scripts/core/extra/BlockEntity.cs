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

  // Separate field for teleported and stairs are for quick access.
  public TeleporterWorldObject Teleporter = null;
  public StairsWorldObject Stairs = null;

  public List<WorldObject> WorldObjects = new List<WorldObject>();
  public Dictionary<GlobalConstants.Orientation, bool> SidesWalkability = new Dictionary<GlobalConstants.Orientation, bool>();

  // Special dictionary that holds wall world object references for easy checking.
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
