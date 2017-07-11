using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinecraftBlock : MonoBehaviour 
{
  public Transform LeftQuad;
  public MeshRenderer LeftQuadRenderer;
  public Transform RightQuad;
  public MeshRenderer RightQuadRenderer;
  public Transform ForwardQuad;
  public MeshRenderer ForwardQuadRenderer;
  public Transform BackQuad;
  public MeshRenderer BackQuadRenderer;
  public Transform UpQuad;
  public MeshRenderer UpQuadRenderer;
  public Transform DownQuad;
  public MeshRenderer DownQuadRenderer;

  public void SetLayer(string layerName)
  { 
    SetLayerRecursively(transform, layerName);
  }

  void SetLayerRecursively(Transform t, string layerName)
  {
    t.gameObject.layer = LayerMask.NameToLayer(layerName);

    foreach (Transform item in t)
    {
      SetLayerRecursively(item, layerName);
    }
  }
}
