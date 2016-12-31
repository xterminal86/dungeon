using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinecraftBlock : MonoBehaviour 
{
  public Transform LeftQuad;
  public Transform RightQuad;
  public Transform ForwardQuad;
  public Transform BackQuad;
  public Transform UpQuad;
  public Transform DownQuad;

  public bool ColliderOn = false;

  public Material TextureMaterial;

  // Minecraft water animation texture is just two vertical sheets of 16x16 frames
  public int Frames = 64;

  void Start()
  {
    GetComponent<BoxCollider>().enabled = ColliderOn;

    _textureAnimationDelta = 1.0f / Frames;
  }

  float _textureAnimationDelta = 1.0f;

  Vector2 _offset = Vector2.zero;

  const int _refreshRate = 15;
  float _timerLimit = 1.0f / _refreshRate;
  float _timer = 0.0f;
  int _framesPassed = 0;
  void Update () 
  {
    if (TextureMaterial == null)
    {
      return;
    }

    _timer += Time.deltaTime;

    if (_timer >= _timerLimit)
    {
      _timer = 0.0f;

      _offset.y = _textureAnimationDelta * _framesPassed;

      if (_framesPassed >= Frames)
      {
        _framesPassed = 0;
      }

      if (_framesPassed > Frames / 2)
      {
        _offset.x = 0.5f;
      }
      else
      {
        _offset.x = 0.0f;
      }

      _framesPassed++;

      TextureMaterial.SetTextureOffset("_MainTex", _offset);
    }
  }
}
