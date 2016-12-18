using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxNight : MonoBehaviour 
{  
  Material _skyboxMaterial;

  string[] _skyboxTextures = new string[] {"_FrontTex", "_BackTex", "_LeftTex", "_RightTex", "_UpTex", "_DownTex"}; // The skybox textures names

  void Start()
  {
    // Based on https://learninggeekblog.wordpress.com/2014/02/11/procedurally-generate-a-night-skybox/

    Texture2D texture = new Texture2D(1024, 1024, TextureFormat.ARGB32, false); // Create a new 1024x1024 texture ARGB32 (32 bit with alpha) and no mipmaps

    _skyboxMaterial = new Material(Shader.Find("Mobile/Skybox")); // Create a new material for the skybox.

    Color color; // Color that will be set for each pixel

    // Loop horizontally on all pixel
    for (int x = 0; x < texture.width; x++)
    {
      // Loop vertically on all pixel
      for (int y = 0; y < texture.height; y++)
      {
        float randomValue = Random.Range(0.0f, 1.0f); // Random value between 0 and 1
        if (randomValue < 0.001f) // 0.1% Of the pixel are set to a grey/white pixel with varying transparency
        {
          //float colorValue = Random.Range(0.6f, 1.0f); //Random value to use as RGB value for color
          float colorValue = 1.0f;
          color = new Color(colorValue, colorValue, colorValue, Random.Range(0.5f, 0.9f)); // Set the pixel to a white/grey color with variying transparency
          texture.SetPixel(x, y, color); // Set the pixel at (x,y) coordinate as the Color in color
        }
        else if (randomValue < 0.0015f && x > 0 && y > 0) // 0.05% Of the pixels that aren't on the first row/column
        {
          //float colorValue = Random.Range(0.85f, 1.0f); //Random value to use as RGB value for color
          float colorValue = 1.0f;
          color = new Color(colorValue, colorValue, 1.0f, Random.Range(0.6f, 0.9f)); // Set the pixel to a yellowish color with variying transparency

          // Set a square of 4 pixels to the current color. The 3 other pixel have been stepped on earlier so their color won't be modified afterward
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

    // Apply all SetPixel calls
    texture.Apply();

    // Create 6 different textures to use as skybox
    for (int i = 0; i < 6; i++)
    {      
      // Set the current texture on one side of the skybox
      _skyboxMaterial.SetTexture(_skyboxTextures[i], texture);
    }

    // Set the RenderSettings skybox to the created material
    RenderSettings.skybox = _skyboxMaterial;
  }

  Vector2 _textureOffset = Vector2.zero;
  void Update()
  {
    /*
    //_textureOffset.x += Time.deltaTime * 0.1f;
    _textureOffset.y += Time.deltaTime * 0.1f;

    for (int i = 0; i < _skyboxTextures.Length; i++)
    {
      _skyboxMaterial.SetTextureOffset(_skyboxTextures[i], _textureOffset);
    }
    */
  }
}
