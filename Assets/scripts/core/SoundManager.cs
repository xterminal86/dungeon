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
  
  public List<AudioClip> MusicTracks = new List<AudioClip>();
  public List<AudioClip> SoundEffects = new List<AudioClip>();

  Dictionary<int, AudioSource> _audioSourcesByHash = new Dictionary<int, AudioSource>();

  protected override void Init()
  {
    base.Init();

    MakeMusicDatabase();
    MakeSoundsDatabase();
  }

  void MakeMusicDatabase()
  {
    foreach (var item in MusicTracks)
    {
      AudioSource s = (AudioSource)Instantiate(AudioSourcePrefab);
      s.transform.parent = AudioSourcesHolder;
      
      s.clip = item;
      s.volume = MusicVolume;
      s.name = item.name;
      s.loop = true;

      int hash = s.name.GetHashCode();
      
      _audioSourcesByHash.Add(hash, s);
    }
  }

  void MakeSoundsDatabase()
  {
    foreach (var item in SoundEffects)
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

  int _currentPlayingTrack = -1;
  public void PlayMusicTrack(string trackName)
  {    
    int hash = trackName.GetHashCode();
    if (_audioSourcesByHash.ContainsKey(hash))
    {
      if (_currentPlayingTrack != -1 && _audioSourcesByHash[_currentPlayingTrack].isPlaying)
      {
        _audioSourcesByHash[_currentPlayingTrack].Stop();
      }

      _audioSourcesByHash[hash].Play();
      _currentPlayingTrack = hash;
    }
  }

  public void MapLoadingFinishedHandler()
  {    
  }
}
