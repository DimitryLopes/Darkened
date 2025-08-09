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

    private Cardinal torchDirection = Cardinal.North;

    private void OnGUI()
    {
        GUILayout.Label($"Editing Node: ({node.X}, {node.Y})", EditorStyles.boldLabel);

        // Walls
        GUILayout.Label("Walls:");
        foreach (Cardinal dir in System.Enum.GetValues(typeof(Cardinal)))
        {
            EditorGUILayout.BeginHorizontal();

            bool prevWallState = wallStates[dir];
            bool nextWallState = GUILayout.Toggle(prevWallState, dir.ToString(), GUILayout.Width(70));

            WallType currentType = wallTypes[dir];
            WallType selectedType = (WallType)EditorGUILayout.EnumPopup(currentType, GUILayout.Width(100));

            // Torch checkbox
            bool hasTorch = HasTorchOnWall(dir);
            bool torchToggle = GUILayout.Toggle(hasTorch, "Torch", GUILayout.Width(60));

            // Update wall state
            if (nextWallState != prevWallState || selectedType != currentType)
            {
                wallStates[dir] = nextWallState;
                wallTypes[dir] = selectedType;
                UpdateWall(dir, nextWallState, selectedType);
            }

            // Torch handling
            if (torchToggle != hasTorch)
            {
                if (torchToggle)
                    AddTorchOnWall(dir);
                else
                    RemoveTorchFromWall(dir);
            }

            EditorGUILayout.EndHorizontal();
        }
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

    private void AddTorchOnWall(Cardinal dir)
    {
        var itemsField = typeof(PresetLevelData).GetField("items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var itemsList = (List<PresetItemData>)itemsField.GetValue(preset);

        string torchPath = "Assets/Prefabs/Maze/Items/Torch.prefab";
        GameObject torchPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(torchPath);
        var torchItem = torchPrefab.GetComponent<Item>();
        var torch = new PresetItemData(
            new Coordinate(node.X, node.Y),
            dir,
            SpawnType.Wall,
            ItemType.DefaultTorch,
            torchItem
        );

        itemsList.Add(torch);
        itemsField.SetValue(preset, itemsList);
    }
    private bool HasTorchOnWall(Cardinal dir)
    {
        var itemsField = typeof(PresetLevelData)
            .GetField("items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var itemsList = (List<PresetItemData>)itemsField.GetValue(preset);

        return itemsList.Exists(i =>
            i.Coordinate.X == node.X &&
            i.Coordinate.Y == node.Y &&
            i.Item.GenerationData.SpawnType == SpawnType.Wall &&
            i.Direction == dir &&
            i.Item.Type == ItemType.DefaultTorch
        );
    }

    private void RemoveTorchFromWall(Cardinal dir)
    {
        var itemsField = typeof(PresetLevelData)
            .GetField("items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var itemsList = (List<PresetItemData>)itemsField.GetValue(preset);

        itemsList.RemoveAll(i =>
            i.Coordinate.X == node.X &&
            i.Coordinate.Y == node.Y &&
            i.Item.GenerationData.SpawnType == SpawnType.Wall &&
            i.Direction == dir &&
            i.Item.Type == ItemType.DefaultTorch
        );

        itemsField.SetValue(preset, itemsList);
    }

}
#endif // UNITY_EDITOR