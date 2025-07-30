#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class NodeEditorWindow : EditorWindow
{
    private Coordinate node;
    private PresetLevelData preset;

    // Para seleção de direção de parede
    private Dictionary<Cardinal, bool> wallStates = new();
    // Para seleção de item
    private Item selectedItem;
    private int selectedItemIndex = -1;
    private string[] itemTypeNames;

    public static void Open(Coordinate node, PresetLevelData preset)
    {
        var window = GetWindow<NodeEditorWindow>("Editar Nó");
        window.node = node;
        window.preset = preset;
        window.InitWallStates();
        window.InitItemSelection();
        window.Show();
    }

    private void InitWallStates()
    {
        wallStates.Clear();
        var wallPositionsField = typeof(PresetLevelData).GetField("wallPositions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var wallPositions = (List<PresetWallPositionData>)wallPositionsField.GetValue(preset);

        foreach (Cardinal dir in System.Enum.GetValues(typeof(Cardinal)))
        {
            wallStates[dir] = wallPositions.Exists(w => w.Coordinate.X == node.X && w.Coordinate.Y == node.Y && w.Direction == dir);
        }
    }

    private void InitItemSelection()
    {
        var itemsField = typeof(PresetLevelData).GetField("items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var items = (List<PresetItemData>)itemsField.GetValue(preset);

        var itemOnNode = items.Find(i => i.Coordinate.X == node.X && i.Coordinate.Y == node.Y);
        selectedItem = itemOnNode.Item;
        selectedItemIndex = -1;

        // Prepara lista de tipos de item (exemplo: pode ser adaptado para sua lista real)
        itemTypeNames = System.Enum.GetNames(typeof(ItemType));
        if (selectedItem != null)
        {
            for (int i = 0; i < itemTypeNames.Length; i++)
            {
                if (selectedItem.Type.ToString() == itemTypeNames[i])
                {
                    selectedItemIndex = i;
                    break;
                }
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Label($"Editando Nó: ({node.X}, {node.Y})", EditorStyles.boldLabel);

        // Edita paredes
        GUILayout.Label("Paredes:");
        foreach (Cardinal dir in System.Enum.GetValues(typeof(Cardinal)))
        {
            bool prev = wallStates[dir];
            bool next = GUILayout.Toggle(prev, dir.ToString());
            if (next != prev)
            {
                wallStates[dir] = next;
                UpdateWall(dir, next);
            }
        }

        GUILayout.Space(10);

        EditorUtility.SetDirty(preset);
    }

    private void UpdateWall(Cardinal dir, bool add)
    {
        var wallPositionsField = typeof(PresetLevelData).GetField("wallPositions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var wallPositions = (List<PresetWallPositionData>)wallPositionsField.GetValue(preset);

        if (add)
        {
            if (!wallPositions.Exists(w => w.Coordinate.X == node.X && w.Coordinate.Y == node.Y && w.Direction == dir))
                wallPositions.Add(new PresetWallPositionData(new Coordinate(node.X, node.Y), dir));
        }
        else
        {
            wallPositions.RemoveAll(w => w.Coordinate.X == node.X && w.Coordinate.Y == node.Y && w.Direction == dir);
        }
        wallPositionsField.SetValue(preset, wallPositions);
    }

    private void RemoveItem()
    {
        var itemsField = typeof(PresetLevelData).GetField("items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var items = (List<PresetItemData>)itemsField.GetValue(preset);

        items.RemoveAll(i => i.Coordinate.X == node.X && i.Coordinate.Y == node.Y);
        itemsField.SetValue(preset, items);
        selectedItemIndex = -1;
    }
}
#endif // UNITY_EDITOR