using UnityEngine;
using System.Collections;

public class SoundManager : MonoSingleton<SoundManager> 
{
  public AudioSource Footstep1Sound;
  public AudioSource Footstep2Sound;

  protected override void Init()
  {
    base.Init();
  }

  public void PlayFootstepSound()
  {    
    int which = Random.Range(0, 2);
    float pitchDiff = Random.Range(-0.1f, 0.1f);
    if (which == 0)
    {      
      Footstep1Sound.pitch = 1.0f + pitchDiff;
      Footstep1Sound.Play();
    }
    else
    {
      Footstep2Sound.pitch = 1.0f + pitchDiff;
      Footstep2Sound.Play();
    }
  }
}
