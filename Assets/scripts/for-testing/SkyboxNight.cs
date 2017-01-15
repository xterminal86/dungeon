using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxNight : MonoBehaviour 
{  
  Material _skyboxMaterial;

  // The skybox textures names
  string[] _skyboxTextures = new string[] {"_FrontTex", "_BackTex", "_LeftTex", "_RightTex", "_UpTex", "_DownTex"};

  void Start()
  {
    // Based on https://learninggeekblog.wordpress.com/2014/02/11/procedurally-generate-a-night-skybox/

    // Create a new 1024x1024 texture ARGB32 (32 bit with alpha) and no mipmaps
    Texture2D texture = new Texture2D(1024, 1024, TextureFormat.ARGB32, false);

    // Create a new material for the skybox.
    _skyboxMaterial = new Material(Shader.Find("Mobile/Skybox"));

    // Color that will be set for each pixel
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
        // 0.15% Of the pixels that aren't on the first row/column
        else if (randomValue < 0.0015f && x > 0 && y > 0)
        {
          float colorValue = 1.0f;
          color = new Color(colorValue, colorValue, 1.0f, Random.Range(0.6f, 0.9f));

          // Set a square of 4 pixels to the current color. 
          // The 3 other pixel have been stepped on earlier so their color won't be modified afterward
          texture.SetPixel(x, y, color);
          texture.SetPixel(x - 1, y, color);
          texture.SetPixel(x, y - 1, color);
          texture.SetPixel(x - 1, y - 1, color);
        }
        else
        {
          // Set the pixel as black for the default background color
          texture.SetPixel(x, y, Color.black); 
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
    if (Input.GetKeyDown(KeyCode.F9))
    {
      Application.CaptureScreenshot(string.Format("scr-{0}{1}{2}.png", System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second));
    }

    // Animate texture

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
