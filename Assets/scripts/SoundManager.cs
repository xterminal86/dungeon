using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoSingleton<SoundManager> 
{
  public AudioSource Footstep1Sound;  
  public AudioSource Footstep2Sound;
  public AudioSource Footstep3Sound;
  public AudioSource Footstep4Sound;

  Dictionary<int, AudioSource> _soundMap = new Dictionary<int, AudioSource>();
  protected override void Init()
  {
    base.Init();

    _soundMap.Add(0, Footstep1Sound);
    _soundMap.Add(1, Footstep2Sound);
    _soundMap.Add(2, Footstep3Sound);
    _soundMap.Add(3, Footstep4Sound);
  }
  
  public void PlayFootstepSound()
  {
    int which = Random.Range(0, _soundMap.Count);
    _soundMap[which].Play();    

    /*
    int which = Random.Range(0, 4);
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
    */
  }
}
