using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineWorldObject : WorldObject 
{
  ParticleSystem _orbAura;
  ParticleSystem _orbActivated;

  Animation _animation;

  AudioSource _loop;

  Light _auraLight;

  Material _orbMaterial;

  bool _wasActivated = false;

  int _cooldownSeconds = 1200;

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
      _orbActivated.Play();
      _orbAura.Stop();

      _animation.CrossFade(_animationInactiveName);

      _wasActivated = true;

      _job = new Job(CooldownRoutine());
    }
  }

  IEnumerator CooldownRoutine()
  {
    _animation.CrossFade(_animationInactiveName);

    Job auraLightJob = new Job(AuraLightFadeRoutine(-0.01f));
      
    float time = 0.0f;
    while (time < _cooldownSeconds)
    {      
      time += Time.smoothDeltaTime;

      yield return null;
    }

    _orbAura.Play();

    _animation.CrossFade(_animationActiveName);

    auraLightJob = new Job(AuraLightFadeRoutine(0.01f));

    yield return auraLightJob.CoroutineMethod;

    _wasActivated = false;

    yield return null;
  }

  Color _orbColor = Color.white;
  Color _orbFinalColor = Color.white;
  Color _orbEmissionColor = Color.white;
  Color _finalEmissionColor = Color.white;
  IEnumerator AuraLightFadeRoutine(float factor)
  {
    int sign = (int)Mathf.Sign(factor);

    bool cond = (sign == -1) ? (_auraLight.intensity > 0.0f) : (_auraLight.intensity < 2.0f);

    float emissionMultiplier = (sign == -1) ? 1.0f : 0.0f;

    while (cond)
    {
      cond = (sign == -1) ? (_auraLight.intensity > 0.0f) : (_auraLight.intensity < 2.0f);

      _loop.volume += factor;
      _loop.volume = Mathf.Clamp(_loop.volume, 0.0f, 1.0f);

      _auraLight.intensity += factor;
      _auraLight.intensity = Mathf.Clamp(_auraLight.intensity, 0.0f, 2.0f);

      emissionMultiplier += factor;
      emissionMultiplier = Mathf.Clamp(emissionMultiplier, 0.0f, 1.0f);

      _finalEmissionColor = _orbEmissionColor * emissionMultiplier;
      _orbFinalColor = _orbColor * emissionMultiplier;

      _finalEmissionColor.r = Mathf.Clamp(_finalEmissionColor.r, 0.0f, 1.0f);
      _finalEmissionColor.g = Mathf.Clamp(_finalEmissionColor.g, 0.0f, 1.0f);
      _finalEmissionColor.b = Mathf.Clamp(_finalEmissionColor.b, 0.0f, 1.0f);

      _orbFinalColor.r = Mathf.Clamp(_orbFinalColor.r, 0.0f, 1.0f);
      _orbFinalColor.g = Mathf.Clamp(_orbFinalColor.g, 0.0f, 1.0f);
      _orbFinalColor.b = Mathf.Clamp(_orbFinalColor.b, 0.0f, 1.0f);

      _orbMaterial.SetColor("_EmissionColor", _finalEmissionColor);
      _orbMaterial.SetColor("_Color", _orbFinalColor);

      yield return null;
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
      }
      else if (item.name == "activate")
      {
        _orbActivated = item;
      }
    }

    var lights = BWO.GetComponentsInChildren<Light>();
    foreach (var light in lights)
    {
      if (light.name == "aura-light")
      {
        _auraLight = light;
        break;
      }
    }      

    var meshRenderers = BWO.GetComponentsInChildren<MeshRenderer>();
    foreach (var renderers in meshRenderers)
    {
      if (renderers.name == "Orb")
      {
        _orbMaterial = renderers.material;
        _orbEmissionColor = _orbMaterial.GetColor("_EmissionColor");
        _orbColor = _orbMaterial.GetColor("_Color");

        break;
      }
    }

    _animation = BWO.GetComponentInChildren<Animation>();
    _loop = BWO.GetComponentInChildren<AudioSource>();
  }
}
