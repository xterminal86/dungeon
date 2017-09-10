using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineMeshes : MonoBehaviour 
{
  public GameObject GrassBlockPrefab;
  public GameObject ObjectsHolder;
  public GameObject CombinedMesh;

  void Start()
  {
    for (int x = 0; x < 50; x++)
    {
      for (int y = 0; y < 50; y++)
      {
        Instantiate(GrassBlockPrefab, new Vector3(x, 0, y), Quaternion.identity, ObjectsHolder.transform);
      }
    }

    // Combining improves FPS dramatically (~30 without combining on home PC, ~75 with combining).
    CombineBlocks();
  }	

  /// <summary>
  /// https://docs.unity3d.com/ScriptReference/Mesh.CombineMeshes.html
  /// </summary>
  void CombineBlocks()
  {
    MeshFilter[] meshFilters = ObjectsHolder.GetComponentsInChildren<MeshFilter>();
    CombineInstance[] combine = new CombineInstance[meshFilters.Length];
    int i = 0;
    while (i < meshFilters.Length) 
    {
      combine[i].mesh = meshFilters[i].sharedMesh;
      combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
      meshFilters[i].gameObject.active = false;
      i++;
    }

    CombinedMesh.transform.GetComponent<MeshFilter>().mesh = new Mesh();
    CombinedMesh.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
    CombinedMesh.transform.gameObject.active = true;
  }
}
