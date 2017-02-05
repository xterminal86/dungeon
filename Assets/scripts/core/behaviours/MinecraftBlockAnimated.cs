using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinecraftBlockAnimated : MinecraftBlock 
{  
  public Material AnimatedTextureMaterial;

  public int Frames = 32;

  float _textureAnimationDelta = 1.0f;
  void Start()
  {    
    _textureAnimationDelta = 1.0f / Frames;
  }

  Vector2 _offset = Vector2.zero;

  const int _refreshRate = 20;
  float _timerLimit = 1.0f / _refreshRate;
  float _timer = 0.0f;
  int _framesPassed = 0;
  void Update () 
  {
    if (AnimatedTextureMaterial == null)
    {
      Debug.LogWarning("No material on " + name);
      return;
    }

    _timer += Time.deltaTime;

    if (_timer >= _timerLimit)
    {
      _timer = 0.0f;

      _framesPassed++;

      if (_framesPassed > Frames - 1)
      {
        _framesPassed = 0;
      }

      _offset.y = _textureAnimationDelta * _framesPassed;

      AnimatedTextureMaterial.SetTextureOffset("_MainTex", _offset);
    }
  }

}
