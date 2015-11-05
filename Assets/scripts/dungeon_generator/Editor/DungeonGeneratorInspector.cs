using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorInspector : Editor 
{
  string[] _options = { "Binary Tree", "Sidewinder", "Rooms", "Growing Tree" };

  public override void OnInspectorGUI()
  {
    DungeonGenerator dg = (DungeonGenerator)target;

    dg.MapWidth = EditorGUILayout.IntSlider("Map Width", dg.MapWidth, 5, 100);
    dg.MapHeight = EditorGUILayout.IntSlider("Map Height", dg.MapHeight, 5, 100);
    dg.MazeGenerationMethod = EditorGUILayout.Popup("Generation Method", dg.MazeGenerationMethod, _options);

    if (dg.MazeGenerationMethod == (int)GenerationMethods.ROOMS)
    {
      dg.RoomMaxWidth = EditorGUILayout.IntSlider("Room Max Width", dg.RoomMaxWidth, 5, dg.MapWidth);
      dg.RoomMaxHeight = EditorGUILayout.IntSlider("Room Max Height", dg.RoomMaxHeight, 5, dg.MapHeight);
      int maxRooms = (dg.MapWidth * dg.MapHeight) / (dg.RoomMaxWidth * dg.RoomMaxHeight);
      dg.MaxRooms = EditorGUILayout.IntSlider("Max Rooms", dg.MaxRooms, 1, maxRooms);
      dg.NoRoomsIntersection = EditorGUILayout.Toggle("No Rooms Intersection", dg.NoRoomsIntersection);
      dg.ConnectRooms = EditorGUILayout.Toggle("Connect Rooms", dg.ConnectRooms);
      if (dg.ConnectRooms)
      {
        dg.RoomsRemoveDeadEnds = EditorGUILayout.Toggle("Remove Dead Ends", dg.RoomsRemoveDeadEnds);
        if (dg.RoomsRemoveDeadEnds)
        {
          dg.RoomsDeadEndsToRemove = EditorGUILayout.IntSlider("Dead Ends to Remove", dg.RoomsDeadEndsToRemove, 1, dg.MapWidth * dg.MapHeight);
        }
      }

      dg.RoomsDistance = EditorGUILayout.IntSlider("Rooms Minimum Spread Distance", dg.RoomsDistance, 0, Mathf.Max(dg.MapWidth, dg.MapHeight) / 2);
    }
    else if (dg.MazeGenerationMethod == (int)GenerationMethods.GROWING_TREE)
    {
      dg.PassageType = (DecisionType)EditorGUILayout.EnumPopup("Passage Carving Option", dg.PassageType);
      dg.RemoveDeadEnds = EditorGUILayout.Toggle("Remove Dead Ends", dg.RemoveDeadEnds);
      if (dg.RemoveDeadEnds)
      {
        dg.DeadEndsToRemove = EditorGUILayout.IntSlider("Dead Ends to Remove", dg.DeadEndsToRemove, 1, dg.MapWidth * dg.MapHeight);
      }
    }

    dg.DoCleanup = EditorGUILayout.Toggle("Cleanup Walls", dg.DoCleanup);
  }

  public enum GenerationMethods
  {
    BINARY_TREE = 0,
    SIDEWINDER,
    ROOMS,
    GROWING_TREE
  }
}
