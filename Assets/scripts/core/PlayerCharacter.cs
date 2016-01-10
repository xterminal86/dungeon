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
  
  int _armorClass = -1;
  public int ArmorClass
  {
    get { return _armorClass; }
  }

  int _magicPoints = -1;
  public int MagicPoints
  {
    get { return _magicPoints; }
  }

  int _magicPointsMax = -1;

  // Modifiers

  public int StrModifier = 0;
  public int DexModifier = 0;
  public int WillModifier = 0;
  public int AcModifier = 0;
  public int HpModifier = 0;  // To max HP
  public int MpModifier = 0;  // To max MP

  // Various

  public string CharacterName = string.Empty;

  CharacterClass _characterClass = CharacterClass.NONE;
  public CharacterClass GetCharacterClass
  {
    get { return _characterClass; }
  }

  int _startingHitPoints = 0;
  int _startingMagicPoints = 0;
  int _hitPointsToAddOnNextLevel = -1;
  int _magicPointsToAddOnNextLevel = -1;
  
  public bool IsFemale = false;

  int _hunger = -1;
  
  float _hungerDecreaseMultiplier = 1.0f;
  public float HungerDecreaseMultiplier
  {
    get { return _hungerDecreaseMultiplier; }
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

    _level = 1;
    _experience = 0;

    _strength = 6;
    _dexterity = 3;
    _willpower = 1;

    _hitPointsToAddOnNextLevel = 10;
    _magicPointsToAddOnNextLevel = 3;
    _startingHitPoints = 20;
    
    CalculateHitPoints();
    _hitPoints = _hitPointsMax;

    CalculateMagicPoints();    
    _magicPoints = _magicPointsMax;

    CalculateArmorClass();
    
    _hunger = GlobalConstants.HungerMax;    
    _hungerDecreaseMultiplier = 1.25f;
  }

  public void InitThief()
  {
    _characterClass = CharacterClass.THIEF;

    _level = 1;
    _experience = 0;

    _strength = 2;
    _dexterity = 5;
    _willpower = 2;

    _hitPointsToAddOnNextLevel = 6;
    _magicPointsToAddOnNextLevel = 5;
    _startingHitPoints = 15;

    CalculateHitPoints();
    _hitPoints = _hitPointsMax;

    CalculateMagicPoints();
    _magicPoints = _magicPointsMax;

    CalculateArmorClass();
        
    _hunger = GlobalConstants.HungerMax;    
    _hungerDecreaseMultiplier = 0.8f;
  }

  public void InitMage()
  {
    _characterClass = CharacterClass.MAGE;

    _level = 1;
    _experience = 0;

    _strength = 1;
    _dexterity = 2;
    _willpower = 6;

    _hitPointsToAddOnNextLevel = 3;
    _magicPointsToAddOnNextLevel = 10;
    _startingHitPoints = 10;

    CalculateHitPoints();
    _hitPoints = _hitPointsMax;

    CalculateMagicPoints();
    _magicPoints = _magicPointsMax;

    CalculateArmorClass();

    _hunger = GlobalConstants.HungerMax;
    _hungerDecreaseMultiplier = 1.0f;
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

    int res = ((_willpower + WillModifier) * mpCoeff + WillModifier) + _magicPointsToAddOnNextLevel * (_level - 1) + MpModifier;
    _magicPointsMax = res;
  }

  public void AddHunger(int value)
  {
    _hunger += value;

    _hunger = Mathf.Clamp(_hunger, 0, GlobalConstants.HungerMax);
  }

  public enum CharacterClass
  {
    SOLDIER = 0,
    MAGE,
    THIEF,
    NONE
  };
}

