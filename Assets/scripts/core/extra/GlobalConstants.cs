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
  public const float CameraMinZoom = 1.0f;
  public const float CameraMaxZoom = 10.0f;

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
  public const float CameraBobSpeed = 0.3f;

  // ScreenFader speed
  public const float FadeSpeed = 1.0f;

  // VillagerActor rotation and speech speeds
  public const float CharacterRotationSpeed = 300.0f;
  public const float CharacterAnimationTalkSpeed = 2.0f;

  // Speed of attack trail drawing
  public const float AttackTrailSpeed = 20.0f;
 
  // How fast in-game time flows
  public const float InGameTimeFlowSpeed = 100.0f;

  // Number of in-game time updates per real life second
  public const int TicksPerSecond = 40;  // 40
  // Length of full day in in-game seconds
  public const int InGameDayNightLength = 10000;   // 100000
  public const int DawnStartTime = 0;
  public const int DawnEndTime = (int)(0.05f * InGameDayNightLength);
  public const int DuskStartTime = (int)(0.45f * InGameDayNightLength);
  public const int DuskEndTime = (int)(0.5f * InGameDayNightLength);

  // For in-game calendar
  public const int InGameDaysInMonth = 30;
  public const int InGameMonthsInYear = 12;

  public static List<string> InGameMonthsNames = new List<string>()
  {    
    "Enas", "Dia", "Airt", "Tessera", "Quintus", "Senio", "Hepta", "Octus", "Ennea", "Decus", "Elva", "Ardes"
  };

  public static Vector3 DefaultVillageMountainsSize = new Vector3(25, 30, 25);

  // In game strings

  public static Dictionary<PlayerCharacter.CharacterClass, string> PersonalNotesByClass = new Dictionary<PlayerCharacter.CharacterClass, string>() 
  {
    { PlayerCharacter.CharacterClass.SOLDIER, "After finally made it to my vacation, I decided to visit some countryside and relax there, away from the crowds and marketplaces of the City.\nI've come across a village called Darwin. The place looks peaceful enough, but it looks like there's some kind of commotion going on." },
    { PlayerCharacter.CharacterClass.THIEF, "Job well done, time for some rest.\nThere wasn't much to take but at least it's something. Nobody will care much about some newborn middle-class upstart.\nSome backwater settlement should suffice. There are some strange soldiers here though, but they aren't from around here, judging by their looks. Anyways, I think it's a nice place to lay low for a while." },
    { PlayerCharacter.CharacterClass.MAGE, "The Order is concerned about suspicious activity near the City.\nThe Council sent me to investigate the matter as a part of my training. Since it is already coming to an end, I am pretty sure this task can be considered as the final test before I can be ordained.\nI can already sense some disturbance at this village. God knows what evil lurks here..." }
  };

  public const string PlayerGreeting = "Welcome to Dungeon: There and Back Again!\n\nThis game is one-man work in progress, so expect a lot of bugs and not very active development.\n\nxterminal86";
  public const string LoremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
  public const string ClothArmorDescription = "- Sonny, when the hell are you wearin', are you from the future or sum'n'?\n- Keep your fucking distance, pal, will ya?!";
  public const string BreadDescription = "- This is delicious!\n- Guess you won't be needin' those tapes I made for ya. You want me to get rid of 'em?\n- Don't be hasty! Not until I see those \"street fighters\" pummeled to dust which should be any moment now... YESH!!! YESH!!!";
  public const string AxeDescription = "- You have my sword.\n- And you have my bow!\n- And my ax!";
  public const string ManyForcedNewlines = "- Hi, Barbie!\n- Hi, Ken!\n- Wanna go for a ride?\n\n- NO!\n\n\n- ...\n- Now pick up your palette knife and work that into meat. Give your meat a good old rub... That's it... Nice and hot... Hot and spicy meat... He-he, boai!\n- Mudafucka, what's wrong wit' yo ass?";
  public const string BigTextLast = "- Hello World!\nLorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

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
  public const string SFXPlayerCannotMove = "player-cannot-move";
  public const string SFXItemTake = "player-item-take";
  public const string SFXItemPut = "player-item-put";
  public const string SFXPlayerEat = "player-eat";
  public const string SFXFemaleVillagerHuh = "act-huh-f";
  public const string SFXMaleVillagerHuh = "act-huh-m";
  public const string SFXTakeScreenshot = "act-photo";
  public const string SFXTeleportation = "snd-teleportation";
  public const string SFXPlayerFall = "player-fall-small";
  public const string SFXPlayerFallLong = "player-fall";
  public const string SFXPlayerPullStartMale = "player-pull-start-m";
  public const string SFXPlayerPullEndMale = "player-pull-end-m";
  public const string SFXPlayerPullStartFemale = "player-pull-start-f";
  public const string SFXPlayerPullEndFemale = "player-pull-end-f";

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

  public static List<string> FootstepsDummy = new List<string>()
  {
    "fs-dummy1",
    "fs-dummy2"
  };

  public static List<string> FootstepsDummy3d = new List<string>()
  {
    "fs-dummy1-3d",
    "fs-dummy2-3d"
  };

  public static List<string> FootstepsGrass = new List<string>()
  {
    "fs-grass1",
    "fs-grass2",
    "fs-grass3",
    "fs-grass4"
  };

  public static List<string> FootstepsGrass3d = new List<string>()
  {
    "fs-grass1-3d",
    "fs-grass2-3d",
    "fs-grass3-3d",
    "fs-grass4-3d"
  };

  public static List<string> FootstepsTile = new List<string>()
  {
    "fs-tile1",
    "fs-tile2",
    "fs-tile3",
    "fs-tile4"
  };

  public static List<string> FootstepsTile3d = new List<string>()
  {
    "fs-tile1-3d",
    "fs-tile2-3d",
    "fs-tile3-3d",
    "fs-tile4-3d"
  };

  public static List<string> FootstepsWood = new List<string>()
  {
    "fs-wood1",
    "fs-wood2",
    "fs-wood3",
    "fs-wood4"    
  };

  public static List<string> FootstepsWood3d = new List<string>()
  {
    "fs-wood1-3d",
    "fs-wood2-3d",
    "fs-wood3-3d",
    "fs-wood4-3d"    
  };

  public static List<string> FootstepsDirt = new List<string>()
  {
    "fs-dirt1",
    "fs-dirt2",
    "fs-dirt3",
    "fs-dirt4"
  };

  public static List<string> FootstepsDirt3d = new List<string>()
  {
    "fs-dirt1-3d",
    "fs-dirt2-3d",
    "fs-dirt3-3d",
    "fs-dirt4-3d"
  };

  public static List<string> FootstepsStone = new List<string>()
  {
    "fs-stone1",
    "fs-stone2",
    "fs-stone3",
    "fs-stone4"
  };

  public static List<string> FootstepsStone3d = new List<string>()
  {
    "fs-stone1-3d",
    "fs-stone2-3d",
    "fs-stone3-3d",
    "fs-stone4-3d"
  };

  public static Dictionary<FootstepSoundType, List<string>> FootstepsListByType = new Dictionary<FootstepSoundType, List<string>>()
  {
    { FootstepSoundType.DUMMY, FootstepsDummy },
    { FootstepSoundType.DIRT, FootstepsDirt },
    { FootstepSoundType.GRASS, FootstepsGrass },
    { FootstepSoundType.STONE, FootstepsStone },
    { FootstepSoundType.TILE, FootstepsTile },
    { FootstepSoundType.WOOD, FootstepsWood },
    { FootstepSoundType.METAL, FootstepsDummy }
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

  public enum BlockType
  {
    AIR = 0,
    GRASS,
    WATER,
    STONE,
    DIRT,
    OAK_PLANKS,
    STONE_BRICKS
  }

  /// <summary>
  /// Minecraft-style blocks prefab names by id.
  /// Id 0 is air block - not instantiated but used in calculations (e. g. in HideSides())
  /// </summary>
  public static Dictionary<BlockType, string> BlockPrefabByType = new Dictionary<BlockType, string>() 
  {
    { BlockType.GRASS, "block-grass" },
    { BlockType.WATER, "block-water" },
    { BlockType.STONE, "block-stone" },
    { BlockType.STONE_BRICKS, "block-stone-bricks" },
    { BlockType.DIRT, "block-dirt" },
    { BlockType.OAK_PLANKS, "block-oak-planks" }
  };

  public enum WorldObjectClass
  {
    PLACEHOLDER = 0,    // Occupies whole block
    WALL,               // Occupies border of the block
    DOOR_OPENABLE,
    DOOR_CONTROLLABLE,
    LEVER,
    BUTTON,
    SIGN,                // Any form of object with text  
    TELEPORTER,
    SHRINE
  }

  public enum WorldObjectPrefabType
  {
    NOTHING = 0,
    WALL_STONE_BRICKS,
    WALL_STONE_BRICKS_WINDOW,
    WALL_TILES,
    WALL_SUPPORT_STONE,
    DOOR_WOODEN_SWING,
    DOOR_IRON_SWING,
    DOOR_STONE_BRICKS_SLIDING,
    DOOR_PORTCULLIS,
    LEVER,
    BUTTON,
    SIGN_PLAQUE_METAL,
    SIGN_POST_WOODEN,
    TELEPORTER,
    TORCH,
    SHRINE_MIGHT,
    SHRINE_SPIRIT
  }

  public static Dictionary<WorldObjectPrefabType, string> WorldObjectPrefabByType = new Dictionary<WorldObjectPrefabType, string>()
  {
    { WorldObjectPrefabType.NOTHING, "" },
    { WorldObjectPrefabType.WALL_STONE_BRICKS, "wall-stone-bricks" },
    { WorldObjectPrefabType.WALL_STONE_BRICKS_WINDOW, "wall-stone-bricks-window" },
    { WorldObjectPrefabType.WALL_TILES, "wall-tiles" },
    { WorldObjectPrefabType.WALL_SUPPORT_STONE, "wall-support-stone" },
    { WorldObjectPrefabType.DOOR_WOODEN_SWING, "door-wooden-swing" },
    { WorldObjectPrefabType.DOOR_IRON_SWING, "door-iron-swing" },
    { WorldObjectPrefabType.DOOR_STONE_BRICKS_SLIDING, "door-stone-bricks-sliding" },
    { WorldObjectPrefabType.DOOR_PORTCULLIS, "door-portcullis" },
    { WorldObjectPrefabType.LEVER, "lever" },
    { WorldObjectPrefabType.BUTTON, "button" },
    { WorldObjectPrefabType.SIGN_PLAQUE_METAL, "sign-plaque-metal" },
    { WorldObjectPrefabType.SIGN_POST_WOODEN, "sign-post-wooden" },
    { WorldObjectPrefabType.TELEPORTER, "teleporter" },
    { WorldObjectPrefabType.TORCH, "torch-wall" },
    { WorldObjectPrefabType.SHRINE_MIGHT, "shrine-might" },
    { WorldObjectPrefabType.SHRINE_SPIRIT, "shrine-spirit" }
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

  /// <summary>
  /// All game items (food, weapons etc)
  /// </summary>
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

  public enum ActorRole
  {
    DUMMY = 0,
    NPC,
    MERCHANT,
    ENEMY_MELEE
  }
}



