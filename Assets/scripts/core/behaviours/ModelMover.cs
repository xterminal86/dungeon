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

      _animationComponent[GlobalConstants.AnimationTalkName].speed = GlobalConstants.CharacterAnimationTalkSpeed;

      // FIXME: Attack speed is hardcoded
      if (_animationComponent.GetClip(GlobalConstants.AnimationAttackName) != null)
      {
        _animationComponent[GlobalConstants.AnimationAttackName].speed = 2.0f;
      }
    }
	}

  // Used in Walk animation state event as method name to call at specific animation time (see CopiedAnimations folder)
  public void StepHandler()
  {
    GlobalConstants.FootstepSoundType footstepSound = LevelLoader.Instance.LevelMap.Level[Actor.ActorPosition.X, Actor.ActorPosition.Y - 1, Actor.ActorPosition.Z].FootstepSound;
    SoundManager.Instance.PlayFootstepSound(Actor.GameObjectName, footstepSound, transform.position);
  }

  void Update () 
	{
    if (GameData.Instance.CurrentGameState != GameData.GameState.RUNNING)
    {
      StopAllCoroutines();
      Actor.ChangeState(Actor.StoppedStateVar);

      return;
    }

    Actor.Perform();    
  }
}
