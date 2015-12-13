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

  public void PlaySound(string name)
  {
    int hash = name.GetHashCode();
    if (_audioSourcesByHash.ContainsKey(hash))
    {
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

  public void PlaySound(int hash, float pitchOffset)
  {
    if (_audioSourcesByHash.ContainsKey(hash))
    {
      _audioSourcesByHash[hash].pitch = 1 + Random.Range(-pitchOffset, pitchOffset);
      _audioSourcesByHash[hash].Play();      
    }
  }

  public void PlaySound(int hash, float pitchOffset, Vector3 position, bool is3D)
  {
    if (_audioSourcesByHash.ContainsKey(hash))
    {
      _audioSourcesByHash[hash].pitch = 1 + Random.Range(-pitchOffset, pitchOffset);
      GameObject go = new GameObject("SFX-3D");
      go.transform.position = position;
      AudioSource a = go.AddComponent<AudioSource>();
      a.playOnAwake = false;
      a.panLevel = is3D ? 1.0f : 0.0f;
      a.volume = is3D ? SoundVolume : 1.0f;
      a.maxDistance = AudioSourcePrefab.maxDistance;
      a.minDistance = AudioSourcePrefab.minDistance;
      //a.rolloffMode = AudioSourcePrefab.rolloffMode;      
      a.rolloffMode = AudioRolloffMode.Linear;
      a.clip = _audioSourcesByHash[hash].clip;      
      a.Play();
      Destroy(go, a.clip.length);
    }
  }

  int _lastPlayedSound = -1;
  public void PlayFootstepSound(GlobalConstants.FootstepSoundType type)
  {
    if (GlobalConstants.FootstepsListByType.ContainsKey(type))
    {
      int which = Random.Range(0, GlobalConstants.FootstepsListByType[type].Count);
      if (_lastPlayedSound == which)
      {
        which++;

        if (which > GlobalConstants.FootstepsListByType[type].Count - 1)
        {
          which = 0;
        }
      }

      PlaySound(GlobalConstants.FootstepsListByType[type][which], 0.1f);
      _lastPlayedSound = which;
    }
  }

  public void PlayFootstepSound(GlobalConstants.FootstepSoundType type, Vector3 position, bool is3D = true)
  {
    if (GlobalConstants.FootstepsListByType.ContainsKey(type))
    {
      int which = Random.Range(0, GlobalConstants.FootstepsListByType[type].Count);
      if (_lastPlayedSound == which)
      {
        which++;

        if (which > GlobalConstants.FootstepsListByType[type].Count - 1)
        {
          which = 0;
        }
      }

      PlaySound(GlobalConstants.FootstepsListByType[type][which], 0.1f, position, is3D);

      _lastPlayedSound = which;
    }
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
