using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoSingleton<SoundManager> 
{
  [Range(0.0f, 1.0f)]
  public float SoundVolume = 1.0f;
  [Range(0.0f, 1.0f)]
  public float MusicVolume = 1.0f;  

  public AudioSource AudioSourceOneShotPrefab;

  public List<AudioClip> MusicTracks = new List<AudioClip>();
  public List<AudioClip> SoundEffects = new List<AudioClip>();

  public Dictionary<int, int> LastPlayedSoundOfChar = new Dictionary<int, int>();

  Dictionary<int, AudioSource> _audioSourcesByHash = new Dictionary<int, AudioSource>();

  protected override void Init()
  {    
    MakeMusicDatabase();
    MakeSoundsDatabase();
  }

  public void RefreshMediaLists()
  {
    // FIXME:
    // It happened several times: for some reason after git pull there is a null in the sound effects list.
    // Some sound (might be music as well, hadn't tested) doesn't load.
    // Call this method during singleton's initialization.

    MakeMusicDatabase();
    MakeSoundsDatabase();
  }

  void MakeMusicDatabase()
  {
    foreach (var item in MusicTracks)
    {
      if (item == null)
      {
        Debug.LogWarning("Music track didn't load (is null) - rebuild media list in Inspector!");
        continue;
      }

      AudioSource s = (AudioSource)Instantiate(AudioSourceOneShotPrefab);
      s.transform.parent = transform;
    
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
      // It happened several times: for some reason after checkout there is a null in the list
      // if you try to run the game immediately (Debug.Log shows that player-thump.ogg didn't load
      // for some reason). Fixed after pressing "Generate Sounds List" in SoundManager Inspector.
      //
      // For safety same condition is applied to the music list.

      if (item == null)
      {
        Debug.LogWarning("Sound effect didn't load (is null) - rebuild media list in Inspector!");
        continue;
      }

      AudioSource s = (AudioSource)Instantiate(AudioSourceOneShotPrefab);
      s.transform.parent = transform;

      s.clip = item;
      s.volume = SoundVolume;
      s.name = item.name;

      int hash = s.name.GetHashCode();

      _audioSourcesByHash.Add(hash, s);
    }
  }

  public void PlaySound(string name)
  {
    PlaySound(name.GetHashCode());
  }

  public void PlaySound(int hash)
  {
    if (_audioSourcesByHash.ContainsKey(hash))
    {
      _audioSourcesByHash[hash].spatialBlend = 0.0f;
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

  public void PlaySound(string name, Vector3 position, bool is3D, float pitch = 1.0f)
  {
    PlaySound(name.GetHashCode(), position, is3D, pitch);
  }

  public void PlaySound(int hash, Vector3 position, bool is3D, float pitch = 1.0f)
  {
    if (_audioSourcesByHash.ContainsKey(hash))
    {      
      GameObject go = new GameObject("SFX-3D");
      go.transform.parent = transform;
      go.transform.position = position;
      AudioSource a = go.AddComponent<AudioSource>();
      a.playOnAwake = false;
      a.spatialBlend = is3D ? 1.0f : 0.0f;
      a.volume = is3D ? SoundVolume : 1.0f;
      a.pitch = pitch;
      a.maxDistance = AudioSourceOneShotPrefab.maxDistance;
      a.minDistance = AudioSourceOneShotPrefab.minDistance;
      a.rolloffMode = AudioRolloffMode.Custom;
      var curve = AudioSourceOneShotPrefab.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
      a.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
      a.clip = _audioSourcesByHash[hash].clip;
      float length = a.clip.length / pitch + 1.0f;
      a.Play();
      Destroy(go, length);
    }
  }

  public void PlaySound(AudioSource premade, float pitch)
  {    
    GameObject go = new GameObject("SFX-one-shot");
    go.transform.parent = transform;
    AudioSource a = go.AddComponent<AudioSource>();
    a.playOnAwake = false;
    a.volume = premade.volume;
    a.clip = premade.clip;    
    a.pitch = pitch;
    float length = a.clip.length / pitch + 1.0f;
    a.Play();
    Destroy(go, length);    
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
    else
    {
      Debug.LogWarning("No key " + type + " in FootstepsListByType!");
    }
  }

  public void PlayFootstepSound(string gameObjectName, GlobalConstants.FootstepSoundType type, Vector3 position, bool is3D = true)
  {
    if (GlobalConstants.FootstepsListByType.ContainsKey(type))
    {
      int which = Random.Range(0, GlobalConstants.FootstepsListByType[type].Count);
      int hash = gameObjectName.GetHashCode();
      if (LastPlayedSoundOfChar[hash] == which)
      {
        LastPlayedSoundOfChar[hash]++;

        if (which > GlobalConstants.FootstepsListByType[type].Count - 1)
        {
          which = 0;
        }
      }

      PlaySound(GlobalConstants.FootstepsListByType[type][which], position, is3D);

      LastPlayedSoundOfChar[hash] = which;
    }
    else
    {
      Debug.LogWarning("No key " + type + " in Footsteps3dListByType!");
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

  public void StopAllSounds()
  {
    foreach (var item in _audioSourcesByHash)
    {
      item.Value.Stop();
    }
  }

  public void MapLoadingFinishedHandler()
  {    
  }
}
