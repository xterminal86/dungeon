using UnityEngine;
using System.Collections;

public class ButtonMapObject : MapObject
{
  const float _buttonDepressMax = 1.05f;
  const float _buttonPressSpeed = 0.25f;
  const float _colorChangeSpeed = 2.0f;

  public override void ActionHandler(object sender)
  {
    if (!_pushing)
    {
      _defaultColor = GameObjectToControl.renderer.material.color;

      if (BMO.ShortSound != null)
      {
        BMO.ShortSound.Play();
      }

      JobManager.Instance.CreateJob(ButtonAnimationPressRoutine());
    }
  }

  Color _defaultColor = Color.white;
  bool _pushing = false;
  IEnumerator ButtonAnimationPressRoutine()
  {
    _pushing = true;

    Color color = _defaultColor;
    Vector3 position = GameObjectToControl.transform.localPosition;
    float cond = 1.0f;
    while (cond < 1.1f)
    {
      cond += Time.smoothDeltaTime * _buttonPressSpeed;

      if (cond < 1.05f)
      {  
        position.z -= Time.smoothDeltaTime * _buttonPressSpeed;

        color.r -= Time.smoothDeltaTime * _colorChangeSpeed;
        color.g -= Time.smoothDeltaTime * _colorChangeSpeed;
        color.b -= Time.smoothDeltaTime * _colorChangeSpeed;
      }
      else
      {
        position.z += Time.smoothDeltaTime * _buttonPressSpeed;

        color.r += Time.smoothDeltaTime * _colorChangeSpeed;
        color.g += Time.smoothDeltaTime * _colorChangeSpeed;
        color.b += Time.smoothDeltaTime * _colorChangeSpeed;
      }

      GameObjectToControl.transform.localPosition = position;
      GameObjectToControl.renderer.material.color = color;

      yield return null;
    }

    position.z = 1.0f;

    GameObjectToControl.transform.localPosition = position;    
    GameObjectToControl.renderer.material.color = _defaultColor;

    _pushing = false;

    if (ActionCompleteCallback != null)
    {
      ActionCompleteCallback(this);
    }
  }  
}
