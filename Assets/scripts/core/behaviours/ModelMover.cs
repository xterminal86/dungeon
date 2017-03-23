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

  public Int3 MapPos = new Int3();

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
        
      // TODO: The smaller the model, the faster the animation. Kind of hack about transform.localScale.
      // Footsteps sounds don't coincide with speed, but would be great to implement.
      //_animationComponent[GlobalConstants.AnimationWalkName].speed = transform.localScale.x < 0.5f ? GlobalConstants.WallScaleFactor * 4 : GlobalConstants.WallScaleFactor * 2;
      _animationComponent[GlobalConstants.AnimationWalkName].speed = GlobalConstants.WallScaleFactor * 2;

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
      StopAllCoroutines();
      Actor.ChangeState(Actor.StoppedStateVar);

      return;
    }

    Actor.Perform();    
  }
}
