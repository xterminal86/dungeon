using UnityEngine;
using System.Collections;

public class ButtonMapObject : MapObject
{
  Animation _animation;

  bool _lockInteraction = false;

  string _animationName = "Action";

  float _animationSpeed = 4;

  public ButtonMapObject(string className, string prefabName, BehaviourMapObject bmo)
  {
    ClassName = className;
    PrefabName = prefabName;
    BMO = bmo;

    _animation = BMO.GetComponentInParent<Animation>();
    if (_animation != null)
    {
      _animation[_animationName].time = 0;
      _animation[_animationName].speed = _animationSpeed;    
    }
  }

  public override void ActionHandler(object sender)
  {
    if (_animation != null && !_lockInteraction)
    {
      BMO.StartSound.Play();

      Job _job = new Job(LeverToggleRoutine());

      _lockInteraction = true;
    }
  }

  public override void ActionCompleteHandler(object sender)
  {
    _lockInteraction = false;
  }

  IEnumerator LeverToggleRoutine()
  {    
    _animation.Play(_animationName);

    while (_animation.IsPlaying(_animationName))
    {
      yield return null;
    }

    if (ActionCompleteCallback != null)
      ActionCompleteCallback(this);
  }
}
