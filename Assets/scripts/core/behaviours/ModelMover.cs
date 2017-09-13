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

  public ActorBase Actor;

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

      _animationComponent[GlobalConstants.AnimationTalkName].speed = GlobalConstants.CharacterAnimationTalkSpeed;

      // FIXME: Attack speed is hardcoded
      if (_animationComponent.GetClip(GlobalConstants.AnimationAttackName) != null)
      {
        _animationComponent[GlobalConstants.AnimationAttackName].speed = 2.0f;
      }
    }
	}
  
  void Update () 
	{
    if (GameData.Instance.CurrentGameState != GameData.GameState.RUNNING)
    {
      if (!(Actor.ActorState is StoppedState))
      {
        StopAllCoroutines();
        Actor.ChangeState(Actor.StoppedStateVar);
      }

      return;
    }

    Actor.Perform();    
  }
}
