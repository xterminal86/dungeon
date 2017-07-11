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
    LeftQuad.gameObject.layer = LayerMask.NameToLayer(layerName);
    RightQuad.gameObject.layer = LayerMask.NameToLayer(layerName);
    ForwardQuad.gameObject.layer = LayerMask.NameToLayer(layerName);
    BackQuad.gameObject.layer = LayerMask.NameToLayer(layerName);
    UpQuad.gameObject.layer = LayerMask.NameToLayer(layerName);
    DownQuad.gameObject.layer = LayerMask.NameToLayer(layerName);
  }
}
