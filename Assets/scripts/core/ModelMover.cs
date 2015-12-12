using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base script for model animation.
/// Contains ActorBase class member for specific functionality that should be initialized during 
/// this GameObject's instantiation.
/// </summary>
public class ModelMover : MonoBehaviour 
{
  public string CharacterName = string.Empty;

  public Int2 ModelPos = new Int2();
  
  public ActorBase Actor;
  
  float _walkingSpeed = 0.0f;
  public float WalkingSpeed
  {
    get { return _walkingSpeed; }
  }

  Animation _animationComponent;
  public Animation AnimationComponent
  {
    get { return _animationComponent; }
  }

  void Awake() 
	{
    _animationComponent = GetComponent<Animation>();
    if (_animationComponent != null)
    {
      _animationComponent[GlobalConstants.AnimationIdleName].speed = 0.5f;

      _animationComponent.Play(GlobalConstants.AnimationIdleName);

      float speed = _animationComponent[GlobalConstants.AnimationWalkName].speed;
      float length = _animationComponent[GlobalConstants.AnimationWalkName].length;

      _walkingSpeed = (length / speed) * GlobalConstants.WallScaleFactor;

      _animationComponent[GlobalConstants.AnimationWalkName].speed = GlobalConstants.WallScaleFactor * 2;
      _animationComponent[GlobalConstants.AnimationTalkName].speed = GlobalConstants.CharacterAnimationTalkSpeed;
    }
	}
  
  void Update () 
	{
    Actor.ActorState.Run();    
  }
}
