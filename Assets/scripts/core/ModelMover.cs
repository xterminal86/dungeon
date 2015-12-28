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
  public string ActorName = string.Empty;
  public bool IsFemale = false;

  public Transform RaycastPoint;

  public ActorBase Actor;

  public Int2 ModelPos = new Int2();

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

      // TODO: The smaller the model, the faster the animation. Kind of hack about transform.localScale.
      // Footsteps sounds doesn't coincide with speed, but would be great to implement.
      _animationComponent[GlobalConstants.AnimationWalkName].speed = transform.localScale.x < 1.0f ? GlobalConstants.WallScaleFactor * 4 : GlobalConstants.WallScaleFactor * 2;
      //_animationComponent[GlobalConstants.AnimationWalkName].speed = GlobalConstants.WallScaleFactor * 2;

      _animationComponent[GlobalConstants.AnimationTalkName].speed = GlobalConstants.CharacterAnimationTalkSpeed;

      // FIXME: Hardcode
      if (_animationComponent.GetClip(GlobalConstants.AnimationAttackName) != null)
      {
        _animationComponent[GlobalConstants.AnimationAttackName].speed = 2.0f;
      }
    }
	}
  
  void Update () 
	{
    Actor.Perform();    
  }
}
