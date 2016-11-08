using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SunController : MonoBehaviour 
{
  public Transform SunTransform;
  public Light LightComponent;

  Vector3 _cameraAngles = Vector3.zero;

  float _sunMoveDelta = 0.0f;
  public float SunMoveDelta
  {
    get { return _sunMoveDelta; }
  }

  float _shadowsStrengthNightDelta = 0.0f;
  float _shadowsStrengthDawnDelta = 0.0f;
  float _sunIntensityDuskDelta = 0.0f;
  float _sunIntensityDawnDelta = 0.0f;
  float _ambientIntensityDelta = 0.0f;
  void Start()
  {
    _sunMoveDelta = 360.0f / GlobalConstants.InGameDayNightLength;

    // Procedural skybox is tied internally to directional light angle,
    // so complete darkness occures when light is around 340 (-20) degrees around X axis.
    // If light goes below the ground, shadows start to "detach" from objects, so
    // we need to fade out shadows before light goes below 0.
    // That's why we add some more to the shadow strength delta during dusk.
    // The dawn looks OK.

    _shadowsStrengthNightDelta = 1.0f / (GlobalConstants.InGameNightStartSeconds - GlobalConstants.InGameDuskStartSeconds);
    _shadowsStrengthNightDelta += _shadowsStrengthNightDelta / 1.5f;

    _shadowsStrengthDawnDelta = 1.0f / GlobalConstants.InGameDawnEndSeconds;

    _sunIntensityDuskDelta = 1.0f / (GlobalConstants.InGameNightStartSeconds - GlobalConstants.InGameDuskStartSeconds);
    _sunIntensityDuskDelta += _sunIntensityDuskDelta / 2.0f;

    _sunIntensityDawnDelta = 1.0f / GlobalConstants.InGameDawnEndSeconds;

    // Minimum ambient light is 0.2, maximum is 0.5
    _ambientIntensityDelta = 0.3f / (GlobalConstants.InGameNightStartSeconds - GlobalConstants.InGameDuskStartSeconds);

    _cameraAngles = SunTransform.eulerAngles;
  }

  void Update()
  {
    _cameraAngles.x = 180.0f - _sunMoveDelta * DateAndTime.Instance.InGameTime;
    _cameraAngles.y = 0.0f;
    _cameraAngles.z = 0.0f;

    if (DateAndTime.Instance.WasTick)
    {
      if (DateAndTime.Instance.InGameTime > GlobalConstants.InGameDuskStartSeconds)
      {
        LightComponent.intensity -= _sunIntensityDuskDelta;
      }
      else if (DateAndTime.Instance.InGameTime < GlobalConstants.InGameDawnEndSeconds)
      {
        LightComponent.intensity += _sunIntensityDawnDelta;
        RenderSettings.ambientIntensity += _ambientIntensityDelta;
      }

      if (DateAndTime.Instance.InGameTime > GlobalConstants.InGameDuskStartSeconds)
      {
        LightComponent.shadowStrength -= _shadowsStrengthNightDelta;
        RenderSettings.ambientIntensity -= _ambientIntensityDelta;
      }
      else if (DateAndTime.Instance.InGameTime < GlobalConstants.InGameDawnEndSeconds)
      {
        LightComponent.shadowStrength += _shadowsStrengthDawnDelta;
      }
    }

    LightComponent.intensity = Mathf.Clamp(LightComponent.intensity, 0.0f, 1.0f);
    LightComponent.shadowStrength = Mathf.Clamp(LightComponent.shadowStrength, 0.0f, 1.0f);
    RenderSettings.ambientIntensity = Mathf.Clamp(RenderSettings.ambientIntensity, 0.2f, 0.5f);

    SunTransform.eulerAngles = _cameraAngles;
  }
}
