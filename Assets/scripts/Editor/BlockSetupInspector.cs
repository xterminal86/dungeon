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

    if (GUI.changed)
    {
      EditorUtility.SetDirty(bs);
    }
  }
}
