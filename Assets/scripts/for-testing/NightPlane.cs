using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightPlane : MonoBehaviour 
{
  public MeshRenderer PlaneRenderer;

  Material _material;

  void Start()
  {
    Texture2D texture = new Texture2D(1024, 1024, TextureFormat.ARGB32, false);

    _material = new Material(Shader.Find("Unlit/Texture"));

    Color color;

    for (int x = 0; x < texture.width; x++)
    {
      for (int y = 0; y < texture.height; y++)
      {
        float randomValue = Random.Range(0.0f, 1.0f);
        if (randomValue < 0.001f)
        {
          float colorValue = 1.0f;
          color = new Color(colorValue, colorValue, colorValue, Random.Range(0.5f, 0.9f));
          texture.SetPixel(x, y, color);
        }
        else if (randomValue < 0.0015f && x > 0 && y > 0)
        {
          float colorValue = 1.0f;
          color = new Color(colorValue, colorValue, 1.0f, Random.Range(0.6f, 0.9f));

          texture.SetPixel(x, y, color);
          texture.SetPixel(x - 1, y, color);
          texture.SetPixel(x, y - 1, color);
          texture.SetPixel(x - 1, y - 1, color);
        }
        else
        {
          texture.SetPixel(x, y, Color.black); // Set the pixel as black for the default background color
        }
      }
    }

    texture.Apply();

    PlaneRenderer.material.SetTexture("_MainTex", texture);
  }

  Vector2 _textureOffset = Vector2.zero;
  void Update()
  {
    _textureOffset.x += Time.deltaTime * 0.005f;
    _textureOffset.y += Time.deltaTime * 0.01f;

    PlaneRenderer.material.SetTextureOffset("_MainTex", _textureOffset);
  }
}
