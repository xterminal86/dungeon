using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoSingleton<SoundManager> 
{
  public AudioSource AmbientSound;
  public AudioSource ChoirSound;
  public AudioSource Footstep1Sound;  
  public AudioSource Footstep2Sound;
  public AudioSource Footstep3Sound;
  public AudioSource Footstep4Sound;
  public AudioSource StoneDoorMovingSound;
  public AudioSource StoneDoorCloseSound;

  Dictionary<int, AudioSource> _soundMap = new Dictionary<int, AudioSource>();
  protected override void Init()
  {
    base.Init();

    _soundMap.Add(0, Footstep1Sound);
    _soundMap.Add(1, Footstep2Sound);
    _soundMap.Add(2, Footstep3Sound);
    _soundMap.Add(3, Footstep4Sound);
  }

  int _lastPlayedSound = -1;
  public void PlayFootstepSound()
  {
    int which = Random.Range(0, _soundMap.Count);
    if (_lastPlayedSound == which)
    {
      which++;
      if (which > _soundMap.Count - 1) which = 0;
      else if (which < 0) which = _soundMap.Count - 1;
    }
    _soundMap[which].Play();
    _lastPlayedSound = which;
  }

  public void MapLoadingFinishedHandler()
  {
    AmbientSound.Play();
  }
}
