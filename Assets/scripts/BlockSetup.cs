using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BlockSetup : MonoBehaviour 
{
  public GameObject Quad1;
  public GameObject Quad2;
  public GameObject Quad3;
  public GameObject Quad4;
  public GameObject Quad5;
  public GameObject Quad6;

  public Material DragMaterialHere;

  void Update()
  {
    if (DragMaterialHere != null)
    {
      Quad1.renderer.material = DragMaterialHere;
      Quad2.renderer.material = DragMaterialHere;
      Quad3.renderer.material = DragMaterialHere;
      Quad4.renderer.material = DragMaterialHere;
      Quad5.renderer.material = DragMaterialHere;
      Quad6.renderer.material = DragMaterialHere;
    }
  }
}
