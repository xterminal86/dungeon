using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPostRender : MonoBehaviour 
{
  public CameraAllLayers AllLayersRenderCamera;
  public CameraNoOcclusionPostRender NoOcclusionRenderCamera;

  public Material ActualCameraMaterial;

  void Awake()
  { 
    ActualCameraMaterial.SetTexture("_MainTex", AllLayersRenderCamera.AllLayersRenderTexture);
    ActualCameraMaterial.SetTexture("_AddTex", NoOcclusionRenderCamera.NoOcclusionTexture);
  }

  void OnRenderImage(RenderTexture src, RenderTexture dest)
  { 
    Graphics.Blit(src, null as RenderTexture, ActualCameraMaterial);
  }
}
