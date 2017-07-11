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

  Camera _cameraComponent;
  void Awake()
  { 
    DumpTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    TextureToRender = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);

    DumpTexture.Create();
    TextureToRender.Create();

    _cameraComponent = GetComponent<Camera>();

    _cameraComponent.targetTexture = DumpTexture;

    Texture2D maskedTexture = new Texture2D(Screen.width, Screen.height);
    maskedTexture.alphaIsTransparency = true;

    Color[] colors = new Color[Screen.width * Screen.height];
    for (int i = 0; i < colors.Length; i++)
    {
      colors[i] = Color.clear;
    }

    maskedTexture.SetPixels(colors);

    DrawCircle(maskedTexture, maskedTexture.width / 2, maskedTexture.height / 2, 64, Color.white);

    NoOcclusionMaterial.SetTexture("_MainTex", CameraAllLayersRef.AllLayersRenderTexture);
  }

  void DrawCircle(Texture2D tex, int cx, int cy, int r, Color c)
  {
    int x, y, px, nx, py, ny, d;
    Color[] tempArray = tex.GetPixels();

    for (x = 0; x <= r; x++)
    {
      d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
      for (y = 0; y <= d; y++)
      {
        px = cx + x;
        nx = cx - x;
        py = cy + y;
        ny = cy - y;

        tempArray[py * tex.width + px] = c;
        tempArray[py * tex.width + nx] = c;
        tempArray[ny * tex.width + px] = c;
        tempArray[ny * tex.width + nx] = c;
      }
    }  

    tex.SetPixels(tempArray);
    tex.Apply();
  }

  void OnRenderImage(RenderTexture src, RenderTexture dest)
  {   
    // Radius is hardcoded from manual visual adjustment

    float r = 0.04f * (10.0f / _cameraComponent.orthographicSize);

    NoOcclusionMaterial.SetFloat("_Radius", r);

    Graphics.Blit(src, TextureToRender, NoOcclusionMaterial);
  }
}
