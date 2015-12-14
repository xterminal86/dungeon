using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoSingleton<SoundManager> 
{
  [Range(0.0f, 1.0f)]
  public float SoundVolume = 1.0f;
  [Range(0.0f, 1.0f)]
  public float MusicVolume = 1.0f;

  public Transform AudioSourcesHolder;

  public AudioSource AudioSourceOneShotPrefab;

  public List<AudioClip> MusicTracks = new List<AudioClip>();
  public List<AudioClip> SoundEffects = new List<AudioClip>();

  public Dictionary<int, int> LastPlayedSoundOfChar = new Dictionary<int, int>();

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
      AudioSource s = (AudioSource)Instantiate(AudioSourceOneShotPrefab);
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
      AudioSource s = (AudioSource)Instantiate(AudioSourceOneShotPrefab);
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
      a.maxDistance = AudioSourceOneShotPrefab.maxDistance;
      a.minDistance = AudioSourceOneShotPrefab.minDistance;
      //a.rolloffMode = AudioSourcePrefab.rolloffMode;      
      a.rolloffMode = AudioRolloffMode.Linear;
      a.clip = _audioSourcesByHash[hash].clip;
      a.Play();
      Destroy(go, a.clip.length);
    }
  }

  int _lastPlayedSoundOfPlayer = -1;
  public void PlayFootstepSoundPlayer(GlobalConstants.FootstepSoundType type)
  {
    if (GlobalConstants.FootstepsListByType.ContainsKey(type))
    {
      int which = Random.Range(0, GlobalConstants.FootstepsListByType[type].Count);
      if (_lastPlayedSoundOfPlayer == which)
      {
        which++;

        if (which > GlobalConstants.FootstepsListByType[type].Count - 1)
        {
          which = 0;
        }
      }

      PlaySound(GlobalConstants.FootstepsListByType[type][which], 0.1f);
      _lastPlayedSoundOfPlayer = which;
    }
  }

  public void PlayFootstepSound(string gameObjectName, GlobalConstants.FootstepSoundType type, Vector3 position, bool is3D = true)
  {
    if (GlobalConstants.FootstepsListByType.ContainsKey(type))
    {
      int which = Random.Range(0, GlobalConstants.Footsteps3dListByType[type].Count);
      int hash = gameObjectName.GetHashCode();
      if (LastPlayedSoundOfChar[hash] == which)
      {
        LastPlayedSoundOfChar[hash]++;

        if (which > GlobalConstants.Footsteps3dListByType[type].Count - 1)
        {
          which = 0;
        }
      }

      PlaySound(GlobalConstants.Footsteps3dListByType[type][which], 0.1f, position, is3D);

      LastPlayedSoundOfChar[hash] = which;
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
