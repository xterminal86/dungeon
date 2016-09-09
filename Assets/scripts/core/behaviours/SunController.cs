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
  float _shadowsStrengthDelta = 0.0f;
  float _sunIntensityDelta = 0.0f;
  void Start()
  {
    _sunMoveDelta = 360.0f / GlobalConstants.InGameDayNightLength;
    _shadowsStrengthDelta = 1.0f / (GlobalConstants.InGameNightStartSeconds - GlobalConstants.InGameDuskStartSeconds);
    _sunIntensityDelta = 1.0f / (GlobalConstants.InGameNightStartSeconds - GlobalConstants.InGameDuskStartSeconds);

    _cameraAngles = SunTransform.eulerAngles;
  }

  void Update()
  {
    _cameraAngles.x = _sunMoveDelta * DateAndTime.Instance.InGameTime;
    _cameraAngles.y = 0.0f;
    _cameraAngles.z = 0.0f;

    if (DateAndTime.Instance.InGameTime > GlobalConstants.InGameDuskStartSeconds)
    {
      LightComponent.intensity -= _sunIntensityDelta;
    }
    else if (DateAndTime.Instance.InGameTime < GlobalConstants.InGameDawnEndSeconds)
    {
      LightComponent.intensity += _sunIntensityDelta;
    }

    if (DateAndTime.Instance.InGameTime > GlobalConstants.InGameDuskStartSeconds)
    {
      LightComponent.shadowStrength -= _shadowsStrengthDelta;
    }
    else if (DateAndTime.Instance.InGameTime < GlobalConstants.InGameDawnEndSeconds)
    {
      LightComponent.shadowStrength += _shadowsStrengthDelta;
    }

    LightComponent.intensity = Mathf.Clamp(LightComponent.intensity, 0.0f, 1.0f);
    LightComponent.shadowStrength = Mathf.Clamp(LightComponent.shadowStrength, 0.1f, 1.0f);

    SunTransform.eulerAngles = _cameraAngles;
  }
}
