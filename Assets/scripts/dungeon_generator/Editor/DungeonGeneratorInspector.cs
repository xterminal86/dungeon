using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorInspector : Editor 
{
  string[] _options = { "Binary Tree", "Sidewinder", "Rooms" };

  public override void OnInspectorGUI()
  {
    DungeonGenerator dg = (DungeonGenerator)target;

    dg.MapWidth = EditorGUILayout.IntSlider("Map Width", dg.MapWidth, 3, 100);
    dg.MapHeight = EditorGUILayout.IntSlider("Map Height", dg.MapHeight, 3, 100);
    dg.MazeGenerationMethod = EditorGUILayout.Popup("Generation Method", dg.MazeGenerationMethod, _options);

    if (dg.MazeGenerationMethod == (int)GenerationMethods.ROOMS)
    {
      dg.RoomMaxWidth = EditorGUILayout.IntSlider("Room Max Width", dg.RoomMaxWidth, 3, dg.MapWidth);
      dg.RoomMaxHeight = EditorGUILayout.IntSlider("Room Max Height", dg.RoomMaxHeight, 3, dg.MapHeight);
      int maxRooms = (dg.MapWidth * dg.MapHeight) / (dg.RoomMaxWidth * dg.RoomMaxHeight);
      dg.MaxRooms = EditorGUILayout.IntSlider("Max Rooms", dg.MaxRooms, 1, maxRooms);
    }
  }

  public enum GenerationMethods
  {
    BINARY_TREE = 0,
    SIDEWINDER,
    ROOMS
  }
}
