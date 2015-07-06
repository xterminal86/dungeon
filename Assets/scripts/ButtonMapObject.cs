using UnityEngine;
using System.Collections;

public class ButtonMapObject : MapObject
{
  const float _buttonDepressMax = 1.05f;
  const float _buttonPressSpeed = 1.0f;

  public override void ActionHandler(object sender)
  {
    SoundManager.Instance.ButtonSound.Play();    
  }

  IEnumerator ButtonAnimationPressRoutine()
  {
    Vector3 transform = GameObjectToControl.transform.position;
    float cond = 1.0f;
    while (cond < 1.1f)
    {
      if (cond < 1.05f)
      {
        transform.z += Time.smoothDeltaTime * _buttonPressSpeed;
      }
      else
      {
        transform.z -= Time.smoothDeltaTime * _buttonPressSpeed;
      }

      GameObjectToControl.transform.position = transform;
      yield return null;
    }

    transform.z = 1.0f;
    GameObjectToControl.transform.position = transform;    
  }  
}
