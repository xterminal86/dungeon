using UnityEngine;
using System.Collections.Generic;

public delegate void Callback();
public delegate void CallbackO(object sender);
public delegate void CallbackB(bool arg);

public static class GlobalConstants 
{  
  // How much one unit actually is in the world
  public const int WallScaleFactor = 2;

  public const int CameraTurnSpeed = 350;
  public const int CameraMoveSpeed = 4;
  public const int CameraCannotMoveSpeed = 2;

  // Delay limits, which determine pause after which VillagerActor starts to walk the random path again.
  // (note, that path building is done via coroutines, so real pause time is longer)
  public const int WanderingMinDelaySeconds = 5;
  public const int WanderingMaxDelaySeconds = 10;
  
  // Seconds of inactivity after which talking citizen goes into "thinking" pose
  public const float CharacterThinkingDelay = 3.0f;

  // How often enemy tries to perform search for player routine (in seconds)
  public const float SearchingForPlayerRate = 0.1f;
  // FIXME: temporary, attack cooldown of enemies - every enemy will have its own
  public const float AttackCooldown = 3.0f;

  // Player bare hands attack cooldown
  public const float PlayerPunchAttackCooldown = 1.0f;

  // Maximum value of hunger
  public const int HungerMax = 100;
  // Number of seconds (without multiplier) after which hunger decreases for 1 point
  public const int HungerDefaultTick = 60;

  // Speed of camera bobbing
  public const float CameraBobSpeed = 0.5f;

  // ScreenFader speed
  public const float FadeSpeed = 1.0f;

  // VillagerActor rotation and speech speeds
  public const float CharacterRotationSpeed = 150.0f;
  public const float CharacterAnimationTalkSpeed = 2.0f;

  // Speed of attack trail drawing
  public const float AttackTrailSpeed = 20.0f;

  public static Vector3 DefaultVillageMountainsSize = new Vector3(25, 30, 25);

  // Greeting string

  public const string PlayerGreeting = "Welcome to Dungeon: There and Back Again!\n\nThis game is one-man work in progress, so expect a lot of bugs and not very active development.\nUse SPACE to talk with villagers and MOUSE to interact with objects.\nF9 to make a screenshot.\n\nxterminal86";
  public const string LoremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
  public const string Phrase1 = "- Sonny, when the hell are you wearin', are you from the future or sum'n'?\n- Keep your fucking distance, pal, will ya?!";
  public const string Phrase2 = "- This is delicious!\n- Guess you won't be needin' those tapes I made for ya. You want me to get rid of 'em?\n- Don't be hasty! Not until I see those \"street fighters\" pummeled to dust which should be any moment now... YESH!!! YESH!!!";

  // Model Animation Names

  public const string AnimationIdleName = "Idle";
  public const string AnimationWalkName = "Walk";
  public const string AnimationAttackName = "Attack";
  public const string AnimationTalkName = "Talk";
  public const string AnimationThinkingName = "Thinking";

  // Actor classes

  public const string ActorVillagerClass = "villager";

  // Sound Effects

  public const string SFXPunch = "atk-punch";
  public const string SFXSwing = "atk-swing";
  public const string SFXPlayerDeath = "player-death";
  public const string SFXPlayerCannotMove = "player-cannot-move2";
  public const string SFXItemTake = "player-item-take";
  public const string SFXItemPut = "player-item-put";
  public const string SFXPlayerEat = "player-eat";
  public const string SFXFemaleVillagerHuh = "act-huh-f";
  public const string SFXMaleVillagerHuh = "act-huh-m";
  public const string SFXTakeScreenshot = "act-photo";

  public static Dictionary<MapAttributes, string> MapAttributesDictionary = new Dictionary<MapAttributes, string>()
  {
    { MapAttributes.Floor, "floor" }, { MapAttributes.Wall, "wall" }, { MapAttributes.Ceiling, "ceiling" },
    { MapAttributes.CoordX, "x" }, { MapAttributes.CoordY, "y" }
  };

  public static Dictionary<int, Orientation> OrientationsMap = new Dictionary<int, Orientation>()
  {
    { 0, Orientation.NORTH },
    { 1, Orientation.EAST },
    { 2, Orientation.SOUTH },
    { 3, Orientation.WEST }
  };

  public static Dictionary<Orientation, int> OrientationAngles = new Dictionary<Orientation, int>()
  {
    { Orientation.NORTH, 270 },
    { Orientation.EAST, 0 },
    { Orientation.SOUTH, 90 },
    { Orientation.WEST, 180 }
  };

  public static Dictionary<int, Orientation> OrientationByAngle = new Dictionary<int, Orientation>()
  {
    { 270, Orientation.NORTH },
    { 0, Orientation.EAST },
    { 90, Orientation.SOUTH },
    { 180, Orientation.WEST }
  };

  public static Dictionary<int, Orientation> OppositeOrientationByAngle = new Dictionary<int, Orientation>()
  {
    { 270, Orientation.SOUTH },
    { 0, Orientation.WEST },
    { 90, Orientation.NORTH },
    { 180, Orientation.EAST }
  };

  public static List<int> FootstepsDummy = new List<int>()
  {
    "fs-dummy1".GetHashCode(),
    "fs-dummy2".GetHashCode()
  };

  public static List<int> FootstepsDummy3d = new List<int>()
  {
    "fs-dummy1-3d".GetHashCode(),
    "fs-dummy2-3d".GetHashCode()
  };

  public static List<int> FootstepsGrass = new List<int>()
  {
    "fs-grass1".GetHashCode(),
    "fs-grass2".GetHashCode(),
    "fs-grass3".GetHashCode(),
    "fs-grass4".GetHashCode()
  };

  public static List<int> FootstepsGrass3d = new List<int>()
  {
    "fs-grass1-3d".GetHashCode(),
    "fs-grass2-3d".GetHashCode(),
    "fs-grass3-3d".GetHashCode(),
    "fs-grass4-3d".GetHashCode()
  };

  public static List<int> FootstepsTile = new List<int>()
  {
    "fs-tile1".GetHashCode(),
    "fs-tile2".GetHashCode(),
    "fs-tile3".GetHashCode(),
    "fs-tile4".GetHashCode()
  };

  public static List<int> FootstepsTile3d = new List<int>()
  {
    "fs-tile1-3d".GetHashCode(),
    "fs-tile2-3d".GetHashCode(),
    "fs-tile3-3d".GetHashCode(),
    "fs-tile4-3d".GetHashCode()
  };

  public static List<int> FootstepsWood = new List<int>()
  {
    "fs-wood1".GetHashCode(),
    "fs-wood2".GetHashCode(),
    "fs-wood3".GetHashCode(),
    "fs-wood4".GetHashCode()    
  };

  public static List<int> FootstepsWood3d = new List<int>()
  {
    "fs-wood1-3d".GetHashCode(),
    "fs-wood2-3d".GetHashCode(),
    "fs-wood3-3d".GetHashCode(),
    "fs-wood4-3d".GetHashCode()    
  };

  public static List<int> FootstepsDirt = new List<int>()
  {
    "fs-dirt1".GetHashCode(),
    "fs-dirt2".GetHashCode(),
    "fs-dirt3".GetHashCode(),
    "fs-dirt4".GetHashCode()
  };

  public static List<int> FootstepsDirt3d = new List<int>()
  {
    "fs-dirt1-3d".GetHashCode(),
    "fs-dirt2-3d".GetHashCode(),
    "fs-dirt3-3d".GetHashCode(),
    "fs-dirt4-3d".GetHashCode()
  };

  public static List<int> FootstepsStone = new List<int>()
  {
    "fs-stone1".GetHashCode(),
    "fs-stone2".GetHashCode(),
    "fs-stone3".GetHashCode(),
    "fs-stone4".GetHashCode()
  };

  public static List<int> FootstepsStone3d = new List<int>()
  {
    "fs-stone1-3d".GetHashCode(),
    "fs-stone2-3d".GetHashCode(),
    "fs-stone3-3d".GetHashCode(),
    "fs-stone4-3d".GetHashCode()
  };

  public static Dictionary<FootstepSoundType, List<int>> FootstepsListByType = new Dictionary<FootstepSoundType, List<int>>()
  {
    { FootstepSoundType.DUMMY, FootstepsDummy },
    { FootstepSoundType.DIRT, FootstepsDirt },
    { FootstepSoundType.GRASS, FootstepsGrass },
    { FootstepSoundType.STONE, FootstepsStone },
    { FootstepSoundType.TILE, FootstepsTile },
    { FootstepSoundType.WOOD, FootstepsWood },
    { FootstepSoundType.METAL, FootstepsDummy }
  };
    
  public static Dictionary<FootstepSoundType, List<int>> Footsteps3dListByType = new Dictionary<FootstepSoundType, List<int>>()
  {
    { FootstepSoundType.DUMMY, FootstepsDummy3d },
    { FootstepSoundType.DIRT, FootstepsDirt3d },
    { FootstepSoundType.GRASS, FootstepsGrass3d },
    { FootstepSoundType.STONE, FootstepsStone3d },
    { FootstepSoundType.TILE, FootstepsTile3d },
    { FootstepSoundType.WOOD, FootstepsWood3d },
    { FootstepSoundType.METAL, FootstepsDummy3d }
  };

  public static List<string> CharacterClassNames = new List<string>()
  {
    { "Soldier" },
    { "Thief" },
    { "Mage" }
  };
    
  public static List<string> CharacterClassDescriptions = new List<string>()
  {
    { "Regular military people of the City.\n\n- high might\n- low spirit\n- moderate defense" },
    { "Regular petty criminal elements of the City.\n\n- low might\n- low spirit\n- high defense" },
    { "Mysterious arcanists of unknown origin.\n\n- low might\n- high spirit\n- low defense" }
  };

  public enum FootstepSoundType
  {
    DUMMY = 0,
    DIRT,
    GRASS,
    STONE,
    TILE,
    WOOD,
    METAL
  }

  public enum MapAttributes
  {
    Floor = 0,
    Wall,
    Ceiling,
    CoordX,
    CoordY    
  }

  public enum Orientation
  {
    NORTH = 0,
    EAST,
    SOUTH,
    WEST
  }

  public enum StaticPrefabsEnum
  {
    FLOOR_GRASS = 0,
    FLOOR_DIRT,
    FLOOR_WOODEN_BLACK,
    FLOOR_COBBLESTONE,
    FLOOR_COBBLESTONE_BRICKS,
    BLOCK_BRICKS_RED,
    BLOCK_WOODEN_LOG,
    ROOF_WOODEN_LINE,
    ROOF_WOODEN_CORNER,
    ROOF_COBBLESTONE_LINE,
    ROOF_COBBLESTONE_CORNER,
    ROOF_COBBLESTONE_CLOSING,
    WALL_THIN_WOODEN,
    WALL_THIN_WOODEN_WINDOW,
    TREE_BIRCH,
    FENCE
  }

  public static Dictionary<StaticPrefabsEnum, string> StaticPrefabsNamesById = new Dictionary<StaticPrefabsEnum, string>()
  {
    { StaticPrefabsEnum.FLOOR_GRASS, "floor-grass" },
    { StaticPrefabsEnum.FLOOR_DIRT, "floor-dirt" },
    { StaticPrefabsEnum.FLOOR_WOODEN_BLACK, "floor-wooden-black" },
    { StaticPrefabsEnum.FLOOR_COBBLESTONE, "floor-cobblestone" },
    { StaticPrefabsEnum.FLOOR_COBBLESTONE_BRICKS, "floor-cobblestone-bricks" },
    { StaticPrefabsEnum.BLOCK_BRICKS_RED, "block-bricks-red" },
    { StaticPrefabsEnum.TREE_BIRCH, "block-tree-birch" },
    { StaticPrefabsEnum.BLOCK_WOODEN_LOG, "block-wooden-log" },
    { StaticPrefabsEnum.ROOF_WOODEN_LINE, "roof-wooden-line" },
    { StaticPrefabsEnum.ROOF_WOODEN_CORNER, "roof-wooden-corner" },
    { StaticPrefabsEnum.ROOF_COBBLESTONE_LINE, "roof-cobblestone-line" },
    { StaticPrefabsEnum.ROOF_COBBLESTONE_CORNER, "roof-cobblestone-corner" },
    { StaticPrefabsEnum.ROOF_COBBLESTONE_CLOSING, "roof-cobblestone-closing" },
    { StaticPrefabsEnum.WALL_THIN_WOODEN, "wall-thin-wooden" },
    { StaticPrefabsEnum.WALL_THIN_WOODEN_WINDOW, "wall-thin-wooden-window" },
    { StaticPrefabsEnum.FENCE, "block-fence" }
  };

  public enum ObjectPrefabsEnum
  {
    DOOR_IRON = 0,
    DOOR_PORTCULLIS,
    DOOR_STONE,
    DOOR_WOODEN,
    BUTTON,
    LEVER,
    VILLAGE_SIGN
  }

  public static Dictionary<ObjectPrefabsEnum, string> ObjectPrefabsNamesById = new Dictionary<ObjectPrefabsEnum, string>()
  {
    { ObjectPrefabsEnum.DOOR_IRON, "door-iron" },
    { ObjectPrefabsEnum.DOOR_PORTCULLIS, "door-portcullis" },
    { ObjectPrefabsEnum.DOOR_STONE, "door-stone" },
    { ObjectPrefabsEnum.DOOR_WOODEN, "door-wooden" },
    { ObjectPrefabsEnum.BUTTON, "button" },
    { ObjectPrefabsEnum.LEVER, "lever" },
    { ObjectPrefabsEnum.VILLAGE_SIGN, "sign-village" }
  };

  public enum WorldItemType
  {
    PLACEHOLDER = 0,
    ARMOR_HEAD,
    ARMOR_CHEST,
    ARMOR_PANTS,
    ARMOR_BOOTS,
    ACCESSORY_CLOAK,
    ACCESSORY_HAND,
    ACCESSORY_NECK,
    FOOD,
    POTION,
    WEAPON_MELEE,
    WEAPON_RANGED
  }

  public static Dictionary<string, WorldItemType> WorldItemTypes = new Dictionary<string, WorldItemType>()
  {
    { "placeholder", WorldItemType.PLACEHOLDER },
    { "armor-head", WorldItemType.ARMOR_HEAD },
    { "armor-chest", WorldItemType.ARMOR_CHEST },
    { "armor-pants", WorldItemType.ARMOR_PANTS },
    { "armor-boots", WorldItemType.ARMOR_BOOTS },
    { "accessory-cloak", WorldItemType.ACCESSORY_CLOAK },
    { "accessory-neck", WorldItemType.ACCESSORY_NECK },
    { "accessory-hand", WorldItemType.ACCESSORY_HAND },
    { "food", WorldItemType.FOOD },
    { "potion", WorldItemType.POTION },
    { "weapon-melee", WorldItemType.WEAPON_MELEE },
    { "weapon-ranged", WorldItemType.WEAPON_RANGED },
  };
}

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

