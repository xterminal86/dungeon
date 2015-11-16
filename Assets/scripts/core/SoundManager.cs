using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoSingleton<SoundManager> 
{
  [Range(0.0f, 1.0f)]
  public float SoundVolume = 1.0f;
  [Range(0.0f, 1.0f)]
  public float MusicVolume = 1.0f;

  public Transform AudioSourcesHolder;

  public AudioSource AudioSourcePrefab;
  public AudioSource MusicTrack;

  public List<AudioClip> SoundEffects;

  Dictionary<int, AudioSource> _audioSourcesByHash = new Dictionary<int, AudioSource>();

  protected override void Init()
  {
    base.Init();

    MusicTrack.volume = MusicVolume;

    MakeSoundsDatabase(SoundEffects);
  }

  void MakeSoundsDatabase(List<AudioClip> listOfSounds)
  {
    foreach (var item in listOfSounds)
    {
      AudioSource s = (AudioSource)Instantiate(AudioSourcePrefab);
      s.transform.parent = AudioSourcesHolder;

      s.clip = item;
      s.volume = SoundVolume;
      s.name = item.name;

      int hash = s.name.GetHashCode();

      _audioSourcesByHash.Add(hash, s);
    }
  }

  public void PlaySound(GlobalConstants.SoundNames soundName)
  {
    if (GlobalConstants.SoundHashByName.ContainsKey(soundName))
    {
      int hash = GlobalConstants.SoundHashByName[soundName];
      _audioSourcesByHash[hash].Play();
    }
  }

  public void PlaySound(int hash)
  {
    if (_audioSourcesByHash.ContainsKey(hash))
    {
      _audioSourcesByHash[hash].Play();
    }
  }

  public void PlaySound(GlobalConstants.SoundNames soundName, Vector3 pos)
  {
    if (GlobalConstants.SoundHashByName.ContainsKey(soundName))
    {
      int hash = GlobalConstants.SoundHashByName[soundName];
      AudioSource.PlayClipAtPoint(_audioSourcesByHash[hash].clip, pos, _audioSourcesByHash[hash].volume);
    }
  }

  int _lastPlayedSound = -1;
  public void PlayFootstepSound()
  {
    int which = Random.Range(0, GlobalConstants.FootstepsGrass.Count);
    if (_lastPlayedSound == which)
    {
      which++;
      if (which > GlobalConstants.FootstepsGrass.Count - 1) which = 0;
      else if (which < 0) which = GlobalConstants.FootstepsGrass.Count - 1;
    }
    PlaySound(GlobalConstants.FootstepsGrass[which]);
    _lastPlayedSound = which;
  }

  public void MapLoadingFinishedHandler()
  {
    //MusicTrack.Play();
  }
}
