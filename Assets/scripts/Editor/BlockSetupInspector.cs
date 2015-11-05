using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(BlockSetup))]
public class BlockSetupInspector : Editor
{
  public override void OnInspectorGUI()
  {
    //base.OnInspectorGUI();

    BlockSetup bs = (BlockSetup)target;

    GUI.changed = false;

    bs.DragMaterialHere = (Material)EditorGUILayout.ObjectField("Drag Material Here", bs.DragMaterialHere, typeof(Material));

    if (bs.DragMaterialHere != null)
    {
      bs.Quad1.renderer.material = bs.DragMaterialHere;
      bs.Quad2.renderer.material = bs.DragMaterialHere;
      bs.Quad3.renderer.material = bs.DragMaterialHere;
      bs.Quad4.renderer.material = bs.DragMaterialHere;
      bs.Quad5.renderer.material = bs.DragMaterialHere;
      bs.Quad6.renderer.material = bs.DragMaterialHere;
    }

    if (GUI.changed)
    {
      EditorUtility.SetDirty(bs);
    }
  }
}
