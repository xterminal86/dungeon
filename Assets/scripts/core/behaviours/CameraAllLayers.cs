using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAllLayers : MonoBehaviour 
{
  public Material AllLayersMaterial;

  [HideInInspector]
  public RenderTexture AllLayersRenderTexture;

  void Awake()
  {    
    AllLayersRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    AllLayersRenderTexture.Create();

    GetComponent<Camera>().targetTexture = AllLayersRenderTexture;

    AllLayersMaterial.SetTexture("_MainTex", AllLayersRenderTexture);
  }

  void OnRenderImage(RenderTexture src, RenderTexture dest)
  { 
    Graphics.Blit(src, dest, AllLayersMaterial);
  }
}
