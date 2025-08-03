#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeEditorWindow : EditorWindow
{
    private Coordinate node;
    private PresetLevelData preset;

    // Para seleção de direção de parede
    private Dictionary<Cardinal, bool> wallStates = new();
    private Dictionary<Cardinal, WallType> wallTypes = new();
    // Para seleção de item
    private Item selectedItem;
    private string[] itemTypeNames;

    public static void Open(Coordinate node, PresetLevelData preset)
    {
        var window = GetWindow<NodeEditorWindow>("Editar Nó");
        window.node = node;
        window.preset = preset;
        window.InitWallStates();
        window.Show();
    }

    private void InitWallStates()
    {
        wallStates.Clear();
        wallTypes.Clear();
        var wallPositionsField = typeof(PresetLevelData).GetField("wallPositions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var wallPositions = (List<PresetWallPositionData>)wallPositionsField.GetValue(preset);

        foreach (Cardinal dir in System.Enum.GetValues(typeof(Cardinal)))
        {
            var wall = wallPositions.Find(w => w.Coordinate.X == node.X && w.Coordinate.Y == node.Y && w.Direction == dir);
            bool exists = wallPositions.Exists(w => w.Coordinate.X == node.X && w.Coordinate.Y == node.Y && w.Direction == dir);

            wallStates[dir] = exists;
            wallTypes[dir] = exists ? wall.WallType : WallType.wall;
        }
    }

    private void OnGUI()
    {
        GUILayout.Label($"Editing Node: ({node.X}, {node.Y})", EditorStyles.boldLabel);

        // Edita paredes
        GUILayout.Label("Walls:");
        foreach (Cardinal dir in System.Enum.GetValues(typeof(Cardinal)))
        {
            EditorGUILayout.BeginHorizontal();

            bool prev = wallStates[dir];
            bool next = GUILayout.Toggle(prev, dir.ToString(), GUILayout.Width(70));
            WallType currentType = wallTypes.ContainsKey(dir) ? wallTypes[dir] : WallType.wall;
            WallType selectedType = (WallType)EditorGUILayout.EnumPopup(currentType, GUILayout.Width(100));
            if (next != prev || selectedType != currentType)
            {
                wallStates[dir] = next;
                wallTypes[dir] = selectedType;
                UpdateWall(dir, next, selectedType);
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        EditorUtility.SetDirty(preset);
    }

    private void UpdateWall(Cardinal dir, bool add, WallType wallType)
    {
        var wallPositionsField = typeof(PresetLevelData).GetField("wallPositions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var wallPositions = (List<PresetWallPositionData>)wallPositionsField.GetValue(preset);

        wallPositions.RemoveAll(w => w.Coordinate.X == node.X && w.Coordinate.Y == node.Y && w.Direction == dir);

        if (add)
        {
            wallPositions.Add(new PresetWallPositionData(new Coordinate(node.X, node.Y), dir, wallType));
        }

        wallPositionsField.SetValue(preset, wallPositions);
    }
}
#endif // UNITY_EDITOR