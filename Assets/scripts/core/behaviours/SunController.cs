using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SunController : MonoBehaviour 
{
  public Transform SunTransform;
  public Light LightComponent;
  public Skybox SkyboxShader;

  Vector3 _cameraAngles = Vector3.zero;

  float _sunMoveDelta = 0.0f;
  public float SunMoveDelta
  {
    get { return _sunMoveDelta; }
  }

  float _dayNightFadeDelta = 0.0f;
  public float DayNightFadeDelta
  {
    get { return _dayNightFadeDelta; }
  }

  float _ambientIntensityDelta = 0.0f;
  float _skyboxAtmosphereThickness = 0.0f;

  void Start()
  {
    _sunMoveDelta = 360.0f / GlobalConstants.InGameDayNightLength;

    _dayNightFadeDelta = 1.0f / (GlobalConstants.DawnEndTime - GlobalConstants.DawnStartTime);

    _cameraAngles = SunTransform.eulerAngles;

    //DateAndTime.Instance.InGameTime = 300;

    AdjustSunParametersForTime();
  }

  void AdjustSunParametersForTime()
  {
    int igt = DateAndTime.Instance.InGameTime;

    if (igt > GlobalConstants.DawnEndTime && igt < GlobalConstants.DuskStartTime)
    {
      LightComponent.shadowStrength = 1.0f;
      _skyboxAtmosphereThickness = 1.0f;
      LightComponent.intensity = 1.0f;
    }
    else if (igt > GlobalConstants.DuskEndTime)
    {
      LightComponent.shadowStrength = 0.0f;
      _skyboxAtmosphereThickness = 0.0f;
      LightComponent.intensity = 0.0f;
    }
    else if (igt >= GlobalConstants.DawnStartTime && igt <= GlobalConstants.DawnEndTime)
    {
      LightComponent.shadowStrength = _dayNightFadeDelta * igt;
      _skyboxAtmosphereThickness = _dayNightFadeDelta * igt;
      LightComponent.intensity = _dayNightFadeDelta * igt;
    }
    else if (igt >= GlobalConstants.DuskStartTime && igt <= GlobalConstants.DuskEndTime)
    {
      int normalizedTime = igt - GlobalConstants.DuskStartTime;
      LightComponent.shadowStrength = _dayNightFadeDelta * normalizedTime;
      _skyboxAtmosphereThickness = _dayNightFadeDelta * normalizedTime;
      LightComponent.intensity = _dayNightFadeDelta * normalizedTime;
    }
  }

  void Update()
  {   
    _cameraAngles.x = 180.0f -_sunMoveDelta * DateAndTime.Instance.InGameTime;
    //_cameraAngles.x -= _sunMoveDelta * Time.deltaTime;
    _cameraAngles.y = 0.0f;
    _cameraAngles.z = 0.0f;

    if (DateAndTime.Instance.WasTick)
    {
      //_cameraAngles.x = 180.0f - _sunMoveDelta * DateAndTime.Instance.InGameTime;

      if (DateAndTime.Instance.InGameTime > GlobalConstants.DawnStartTime && DateAndTime.Instance.InGameTime < GlobalConstants.DawnEndTime)
      {
        LightComponent.shadowStrength += _dayNightFadeDelta;
        _skyboxAtmosphereThickness += _dayNightFadeDelta;
        LightComponent.intensity += _dayNightFadeDelta;
      }
      else if (DateAndTime.Instance.InGameTime > GlobalConstants.DuskStartTime && DateAndTime.Instance.InGameTime < GlobalConstants.DuskEndTime)
      {
        LightComponent.shadowStrength -= _dayNightFadeDelta;
        _skyboxAtmosphereThickness -= _dayNightFadeDelta;
        LightComponent.intensity -= _dayNightFadeDelta;
        //RenderSettings.ambientIntensity += _ambientIntensityDelta;
      }
    }

    _skyboxAtmosphereThickness = Mathf.Clamp(_skyboxAtmosphereThickness, 0.0f, 1.0f);

    SkyboxShader.material.SetFloat("_AtmosphereThickness", _skyboxAtmosphereThickness);
    LightComponent.intensity = Mathf.Clamp(LightComponent.intensity, 0.0f, 1.0f);
    LightComponent.shadowStrength = Mathf.Clamp(LightComponent.shadowStrength, 0.0f, 1.0f);
    //RenderSettings.ambientIntensity = Mathf.Clamp(RenderSettings.ambientIntensity, 0.2f, 0.5f);

    SunTransform.eulerAngles = _cameraAngles;
  }
}
