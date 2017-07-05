using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationWatcher : MonoBehaviour 
{
  public void StepHandler()
  {
    InputController.Instance.PlayFootstepSound();
  }
}
