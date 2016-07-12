using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class for holding all player related data (hitpoints, damage, hunger etc.)
/// </summary>
public class PlayerCharacter
{
  // Base stats
  int _strength = -1;
  public int Strength
  {
    get { return _strength + StrModifier; }
  }

  int _dexterity = -1;
  public int Dexterity
  {
    get { return _dexterity + DexModifier; }
  }

  int _willpower = -1;
  public int Willpower
  {
    get { return _willpower + WillModifier; }
  }

  // Derived stats  
  int _hitPoints = -1;
  public int HitPoints
  {
    get { return _hitPoints; }
  }

  int _hitPointsMax = -1;
  public int HitPointsMax
  {
    get { return _hitPointsMax; }
  }

  int _armorClass = -1;
  public int ArmorClass
  {
    get { return _armorClass; }
  }

  int _energy = -1;
  public int Energy
  {
    get { return _energy; }
  }

  int _energyMax = -1;
  public int EnergyMax
  {
    get { return _energyMax; }
  }

  // Modifiers

  public int StrModifier = 0;
  public int DexModifier = 0;
  public int WillModifier = 0;
  public int AcModifier = 0;
  public int HpModifier = 0;  // To max HP
  public int EnergyModifier = 0;  // To max MP

  // Various

  public string CharacterName = string.Empty;

  CharacterClass _characterClass = CharacterClass.NONE;
  public CharacterClass GetCharacterClass
  {
    get { return _characterClass; }
  }

  string _charTitle = string.Empty;
  public string CharacterNameAndTitle
  {
    get
    {
      return string.Format("{0} the {1}", CharacterName, _charTitle);
    }
  }

  int _startingHitPoints = 0;
  int _startingEnergy = 0;
  int _hitPointsToAddOnNextLevel = -1;
  int _energyToAddOnNextLevel = -1;
  
  public bool IsFemale = false;

  int _hunger = -1;
  public int Hunger
  {
    get { return _hunger; }
  }

  // Scales deltaTime in App.Update() for hunger counting
  float _hungerDecreaseMultiplier = 1.0f;
  public float HungerDecreaseMultiplier
  {
    get { return _hungerDecreaseMultiplier; }
  }
   
  // Number of seconds (without multiplier) after which hunger decreases for 1 point
  int _hungerTick = 1;
  public int HungerTick
  {
    get { return _hungerTick; }
  }

  int _experience = -1;
  
  int _level = 1;

  public PlayerCharacter()
  {
    InitSoldier();
  }

  public void InitSoldier()
  {
    _characterClass = CharacterClass.SOLDIER;
    _charTitle = "Soldier";

    _level = 1;
    _experience = 0;

    _strength = 6;
    _dexterity = 3;
    _willpower = 1;

    _hitPointsToAddOnNextLevel = 10;
    _energyToAddOnNextLevel = 3;
    _startingHitPoints = 20;
    
    CalculateHitPoints();
    _hitPoints = _hitPointsMax;

    CalculateMagicPoints();    
    _energy = _energyMax;

    CalculateArmorClass();
    
    _hunger = GlobalConstants.HungerMax;    
    _hungerDecreaseMultiplier = 1.0f;
    _hungerTick = GlobalConstants.HungerDefaultTick;
  }

  public void InitThief()
  {
    _characterClass = CharacterClass.THIEF;
    _charTitle = "Rogue";

    _level = 1;
    _experience = 0;

    _strength = 2;
    _dexterity = 5;
    _willpower = 2;

    _hitPointsToAddOnNextLevel = 6;
    _energyToAddOnNextLevel = 5;
    _startingHitPoints = 10;

    CalculateHitPoints();
    _hitPoints = _hitPointsMax;

    CalculateMagicPoints();
    _energy = _energyMax;

    CalculateArmorClass();
        
    _hunger = GlobalConstants.HungerMax;    
    _hungerDecreaseMultiplier = 0.8f;
    _hungerTick = GlobalConstants.HungerDefaultTick;
  }

  public void InitMage()
  {
    _characterClass = CharacterClass.MAGE;
    _charTitle = "Arcanist";

    _level = 1;
    _experience = 0;

    _strength = 1;
    _dexterity = 2;
    _willpower = 6;

    _hitPointsToAddOnNextLevel = 3;
    _energyToAddOnNextLevel = 10;
    _startingHitPoints = 5;

    CalculateHitPoints();
    _hitPoints = _hitPointsMax;

    CalculateMagicPoints();
    _energy = _energyMax;

    CalculateArmorClass();

    _hunger = GlobalConstants.HungerMax;
    _hungerDecreaseMultiplier = 2.0f;
    _hungerTick = GlobalConstants.HungerDefaultTick;
  }

  public void ResetToDefault()
  {
    InitSoldier();
  }

  void CalculateArmorClass()
  {
    int acCoeff = 1;

    if (_characterClass == CharacterClass.SOLDIER)
    {
      acCoeff = 2;
    }
    else if (_characterClass == CharacterClass.THIEF)
    {
      acCoeff = 3;
    }
    else if (_characterClass == CharacterClass.MAGE)
    {
      acCoeff = 1;
    }

    int res = ((_dexterity + DexModifier) * acCoeff + AcModifier);
    _armorClass = res;
  }

  void CalculateHitPoints()
  {
    int res = _startingHitPoints + HpModifier + _hitPointsToAddOnNextLevel * (_level - 1);
    _hitPointsMax = res;
  }

  void CalculateMagicPoints()
  {
    int mpCoeff = 1;

    if (_characterClass == CharacterClass.SOLDIER)
    {
      mpCoeff = 1;
    }
    else if (_characterClass == CharacterClass.THIEF)
    {
      mpCoeff = 2;
    }
    else if (_characterClass == CharacterClass.MAGE)
    {
      mpCoeff = 3;
    }

    int res = (_willpower + WillModifier) * mpCoeff + _energyToAddOnNextLevel * (_level - 1) + EnergyModifier;
    _energyMax = res;
  }

  public void AddHunger(int value)
  {
    _hunger += value;

    _hunger = Mathf.Clamp(_hunger, 0, GlobalConstants.HungerMax);

    GameData.Instance.ResetHungerTimer();

    //Debug.Log("Hunger: " + _hunger);
  }

  public void AddDamage(int value)
  {
    _hitPoints += value;

    _hitPoints = Mathf.Clamp(_hitPoints, 0, _hitPointsMax);
  }

  public enum CharacterClass
  {
    SOLDIER = 0,
    MAGE,
    THIEF,
    NONE
  };
}

