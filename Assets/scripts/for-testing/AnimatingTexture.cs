using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatingTexture : MonoBehaviour 
{
  Material _material;

  // Minecraft water animation texture is just two vertical sheets of 16x16 frames
  const int _frames = 64;

  float _textureAnimationDelta = 1.0f / _frames;
	
  Vector2 _offset = Vector2.zero;
	void Start () 
	{
    _material = GetComponent<MeshRenderer>().material;
	}
  	
  const int _refreshRate = 15;
  float _timerLimit = 1.0f / _refreshRate;
  float _timer = 0.0f;
  int _framesPassed = 0;
	void Update () 
	{
    _timer += Time.deltaTime;

    if (_timer >= _timerLimit)
    {
      _timer = 0.0f;

      _offset.y = _textureAnimationDelta * _framesPassed;

      if (_framesPassed >= _frames)
      {
        _framesPassed = 0;
      }

      if (_framesPassed > _frames / 2)
      {
        _offset.x = 0.5f;
      }
      else
      {
        _offset.x = 0.0f;
      }

      _framesPassed++;

      _material.SetTextureOffset("_MainTex", _offset);
    }
	}
}
