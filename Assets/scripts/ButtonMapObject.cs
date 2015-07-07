using UnityEngine;
using System.Collections;

public class ButtonMapObject : MapObject
{
  const float _buttonDepressMax = 1.05f;
  const float _buttonPressSpeed = 0.25f;

  public override void ActionHandler(object sender)
  {
    if (!_pushing)
    {
      SoundManager.Instance.ButtonSound.Play();
      JobManager.Instance.CreateJob(ButtonAnimationPressRoutine());
    }
  }

  bool _pushing = false;
  IEnumerator ButtonAnimationPressRoutine()
  {
    _pushing = true;

    Vector3 position = GameObjectToControl.transform.localPosition;
    float cond = 1.0f;
    while (cond < 1.1f)
    {
      cond += Time.smoothDeltaTime * _buttonPressSpeed;

      if (cond < 1.05f)
      {  
        position.z -= Time.smoothDeltaTime * _buttonPressSpeed;
      }
      else
      {
        position.z += Time.smoothDeltaTime * _buttonPressSpeed;
      }

      GameObjectToControl.transform.localPosition = position;

      yield return null;
    }

    position.z = 1.0f;

    GameObjectToControl.transform.localPosition = position;    

    _pushing = false;

    if (ActionCompleteCallback != null)
    {
      ActionCompleteCallback(this);
    }
  }  
}
