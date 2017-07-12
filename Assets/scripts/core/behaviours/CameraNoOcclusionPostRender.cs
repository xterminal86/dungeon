using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraNoOcclusionPostRender : MonoBehaviour 
{
  public CameraAllLayers CameraAllLayersRef;

  public Material NoOcclusionMaterial;

  [HideInInspector]
  public RenderTexture DumpTexture;

  [HideInInspector]
  public RenderTexture TextureToRender;

  Texture2D _maskedTexture;

  float _currentZoom = 1.0f, _prevZoom = 1.0f;

  Camera _cameraComponent;
  void Awake()
  { 
    DumpTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    TextureToRender = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);

    DumpTexture.Create();
    TextureToRender.Create();

    _cameraComponent = GetComponent<Camera>();

    _cameraComponent.targetTexture = DumpTexture;

    _maskedTexture = new Texture2D(Screen.width, Screen.height);
    _maskedTexture.alphaIsTransparency = true;

    _textureColors = new Color[Screen.width * Screen.height];

    DrawCircle(_maskedTexture, _maskedTexture.width / 2, _maskedTexture.height / 2, 32, Color.white);

    NoOcclusionMaterial.SetTexture("_MainTex", CameraAllLayersRef.AllLayersRenderTexture);
    NoOcclusionMaterial.SetInt("_ScreenWidth", Screen.width);
    NoOcclusionMaterial.SetInt("_ScreenHeight", Screen.height);

    //NoOcclusionMaterial.SetTexture("_MaskTex", _maskedTexture);

    _currentZoom = _cameraComponent.orthographicSize;
    _prevZoom = _cameraComponent.orthographicSize;
  }

  Color[] _textureColors;
  void DrawCircle(Texture2D tex, int cx, int cy, int r, Color c)
  {
    for (int i = 0; i < _textureColors.Length; i++)
    {
      _textureColors[i] = Color.clear;
    }

    _maskedTexture.SetPixels(_textureColors);

    int x, y, px, nx, py, ny, d;

    for (x = 0; x <= r; x++)
    {
      d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
      for (y = 0; y <= d; y++)
      {
        px = cx + x;
        nx = cx - x;
        py = cy + y;
        ny = cy - y;

        _textureColors[py * tex.width + px] = c;
        _textureColors[py * tex.width + nx] = c;
        _textureColors[ny * tex.width + px] = c;
        _textureColors[ny * tex.width + nx] = c;
      }
    }  

    tex.SetPixels(_textureColors);
    tex.Apply();
  }

  void OnRenderImage(RenderTexture src, RenderTexture dest)
  {   
    // Radius is hardcoded from manual visual adjustment
    float r = 0.06f * (10.0f / _cameraComponent.orthographicSize);

    NoOcclusionMaterial.SetFloat("_Radius", r);

    /*
    _currentZoom = _cameraComponent.orthographicSize;

    if (_currentZoom != _prevZoom)
    {
      _prevZoom = _currentZoom;

      float r = 32.0f * (10.0f / _currentZoom);
      DrawCircle(_maskedTexture, _maskedTexture.width / 2, _maskedTexture.height / 2, (int)r, Color.white);
    }
    */

    Graphics.Blit(src, TextureToRender, NoOcclusionMaterial);
  }
}
