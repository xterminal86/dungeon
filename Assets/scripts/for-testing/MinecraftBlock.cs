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

  public int Frames = 32;

  void Start()
  {
    GetComponent<BoxCollider>().enabled = ColliderOn;

    _textureAnimationDelta = 1.0f / Frames;
  }

  float _textureAnimationDelta = 1.0f;

  Vector2 _offset = Vector2.zero;

  const int _refreshRate = 20;
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

      _framesPassed++;

      if (_framesPassed > Frames - 1)
      {
        _framesPassed = 0;
      }

      _offset.y = _textureAnimationDelta * _framesPassed;

      TextureMaterial.SetTextureOffset("_MainTex", _offset);
    }
  }
}
