using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineWorldObject : WorldObject 
{
  ParticleSystem _orbAura;

  Animation _animation;

  AudioSource _loop;

  bool _wasActivated = false;

  int _cooldownSeconds = 10;

  Job _job;

  string _animationActiveName = "OrbFloat";
  string _animationInactiveName = "OrbRest";

  public ShrineWorldObject(string inGameName, string prefabName) : base(inGameName, prefabName)
  {    
  }

  public override void ActionHandler(object sender)
  {
    if (!_wasActivated)
    {
      _orbAura.Stop();

      _animation.CrossFade(_animationInactiveName);

      _wasActivated = true;

      _job = new Job(CooldownRoutine());
    }
  }

  IEnumerator CooldownRoutine()
  {
    Job loopVolumeJob = new Job(LoopFadeRoutine(-0.01f));

    float time = 0.0f;
    while (time < _cooldownSeconds)
    {
      _loop.volume -= 0.05f;
      _loop.volume = Mathf.Clamp(_loop.volume, 0.0f, 1.0f);

      time += Time.smoothDeltaTime;

      yield return null;
    }

    loopVolumeJob = new Job(LoopFadeRoutine(0.01f));

    Job j = new Job(OrbActivateRoutine());

    yield return null;
  }

  IEnumerator OrbActivateRoutine()
  {
    _animation[_animationInactiveName].normalizedTime = 1.0f;
    _animation[_animationInactiveName].speed = -1.0f;

    _orbAura.Play();

    _animation.CrossFade(_animationInactiveName);

    // FIXME: animation jitter

    while (_animation[_animationInactiveName].normalizedTime > 0.5f)
    {
      yield return null;
    }

    _animation.CrossFade(_animationActiveName);

    _animation[_animationInactiveName].normalizedTime = 0.0f;
    _animation[_animationInactiveName].speed = 1.0f;

    _wasActivated = false;

    yield return null;
  }

  IEnumerator LoopFadeRoutine(float factor)
  {
    int sign = (int)Mathf.Sign(factor);

    if (sign == -1)
    {
      while (_loop.volume > 0.0f)
      {
        _loop.volume += factor;
        _loop.volume = Mathf.Clamp(_loop.volume, 0.0f, 1.0f);

        yield return null;
      }
    }
    else
    {
      while (_loop.volume < 1.0f)
      {
        _loop.volume += factor;
        _loop.volume = Mathf.Clamp(_loop.volume, 0.0f, 1.0f);

        yield return null;
      }
    }

    yield return null;
  }

  public void InitBWO()
  {
    var particleSystems = BWO.GetComponentsInChildren<ParticleSystem>();
    foreach (var item in particleSystems)
    {
      if (item.name == "aura")
      {
        _orbAura = item;
        break;
      }
    }

    _animation = BWO.GetComponentInChildren<Animation>();
    _loop = BWO.GetComponentInChildren<AudioSource>();
  }
}
