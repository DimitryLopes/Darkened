#if UNITY_EDITOR

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PresetLevelData))]
public class PresetLevelDataEditor : Editor
{
    private const int CellSize = 20;

    public override void OnInspectorGUI()
    {
        PresetLevelData preset = (PresetLevelData)target;
        var sizeDataField = typeof(PresetLevelData).GetField("sizeData", BindingFlags.NonPublic | BindingFlags.Instance);
        var wallPositionsField = typeof(PresetLevelData).GetField("wallPositions", BindingFlags.NonPublic | BindingFlags.Instance);

        var itemsField = typeof(PresetLevelData).GetField("items", BindingFlags.NonPublic | BindingFlags.Instance);
        var items = (System.Collections.IList)itemsField.GetValue(preset);

        if (items != null)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var itemData = items[i];
                var itemField = itemData.GetType().GetField("item", BindingFlags.NonPublic | BindingFlags.Instance);
                var spawnTypeField = itemData.GetType().GetField("SpawnType", BindingFlags.NonPublic | BindingFlags.Instance);
                var itemTypeField = itemData.GetType().GetField("ItemType", BindingFlags.NonPublic | BindingFlags.Instance);

                var item = itemField.GetValue(itemData);
                if (item == null)
                {
                    EditorGUILayout.LabelField($"Item {i}", "Vazio");
                    continue;
                }

                var generationDataProp = item.GetType().GetProperty("GenerationData", BindingFlags.Public | BindingFlags.Instance);
                var generationData = generationDataProp.GetValue(item);
                var spawnTypeProp = generationData.GetType().GetProperty("SpawnType", BindingFlags.Public | BindingFlags.Instance);
                var spawnType = spawnTypeProp.GetValue(generationData);
                var itemTyperProp = item.GetType().GetProperty("Type", BindingFlags.Public | BindingFlags.Instance);
                var itemType = itemTyperProp.GetValue(item);
                spawnTypeField.SetValue(itemData, spawnType);
                itemTypeField.SetValue(itemData, itemType);
                items[i] = itemData;
            }
        }

        DrawDefaultInspector();

        if (GUILayout.Button("Regenerate Walls From Size"))
        {
            var mazeSizeData = (MazeSizeData)sizeDataField.GetValue(preset);
            if (mazeSizeData != null)
            {
                FillAllWalls(preset, mazeSizeData);
                EditorUtility.SetDirty(preset);
            }
        }

        if (preset == null || sizeDataField.GetValue(preset) == null)
            return;

        var sizeData = (MazeSizeData)sizeDataField.GetValue(preset);
        int width = sizeData.Width;
        int height = sizeData.Height;

        GUILayout.Space(10);
        GUILayout.Label("Maze Preview", EditorStyles.boldLabel);

        Texture2D preview = new Texture2D(width * CellSize, height * CellSize);
        Color bgColor = new Color(0.85f, 0.85f, 0.85f);

        for (int y = 0; y < height * CellSize; y++)
            for (int x = 0; x < width * CellSize; x++)
                preview.SetPixel(x, y, bgColor);

        var wallPositions = (List<PresetWallPositionData>)wallPositionsField.GetValue(preset);
        foreach (var wall in wallPositions)
            DrawWall(preview, wall.Coordinate.X, wall.Coordinate.Y, wall.Direction, width, height, wall.WallType);

        var itemsList = (List<PresetItemData>)itemsField.GetValue(preset);
        foreach (var presetItem in itemsList)
        {
            if (presetItem.Item == null)
            {
                DrawCell(preview, presetItem.Coordinate.X, presetItem.Coordinate.Y, Color.gray, width, height);
                continue;
            }

            Color itemColor = presetItem.Item.Type switch
            {
                ItemType.DefaultTorch => new Color(1f, 0.4f, 0, 1),
                ItemType.Exit => Color.black,
                ItemType.Button => new Color(1f, 0.75f, 0, 1),
                ItemType.Wisp => Color.cyan,
                ItemType.Oil => Color.yellow,
                ItemType.Distraction => new Color(0.5f, 0.2f, 0, 1),
                ItemType.Orb => new Color(0, 1, 0.6f, 1),
                ItemType.Altar => new Color(0.4f, 0.4f, 0.4f, 1),
                ItemType.Spirit => new Color(1, 0, 0.3f, 1),
                ItemType.FakeSpirit => new Color(1, 0.2f, 0.3f, 1),
                ItemType.PressurePlate => new Color(0.7f, 0.7f, 0.7f, 1),
                ItemType.PoisonDartTrap => new Color(0.1f, 0.8f, 0.1f, 1),
                ItemType.Energetic => Color.magenta,
                _ => Color.white
            };

            switch (presetItem.Item.GenerationData.SpawnType)
            {
                case SpawnType.Wall:
                case SpawnType.EdgeWalls:
                    DrawSmallCell(preview, presetItem.Coordinate.X, presetItem.Coordinate.Y, itemColor, width, height, presetItem.Direction);
                    break;
                default:
                    DrawCell(preview, presetItem.Coordinate.X, presetItem.Coordinate.Y, itemColor, width, height);
                    break;
            }
            ;
        }

        var startPosition = (Coordinate)preset.GetType().GetField("startPosition", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(preset);
        DrawCell(preview, startPosition.X, startPosition.Y, Color.green, width, height);

        var enemySpawnPosition = (Coordinate)preset.GetType().GetField("enemySpawnPosition", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(preset);
        DrawCell(preview, enemySpawnPosition.X, enemySpawnPosition.Y, Color.red, width, height);

        preview.Apply();

        GUILayout.Space(10);
        Rect previewRect = GUILayoutUtility.GetRect(new GUIContent(""), GUIStyle.none, GUILayout.Width(width * CellSize), GUILayout.Height(height * CellSize));
        GUI.DrawTexture(previewRect, preview);

        if (Event.current.type == EventType.MouseDown && previewRect.Contains(Event.current.mousePosition))
        {
            int gridX = (int)((Event.current.mousePosition.x - previewRect.x) / CellSize);
            int gridY = height - 1 - (int)((Event.current.mousePosition.y - previewRect.y) / CellSize);

            Coordinate selectedNode = new Coordinate(gridX, gridY);
            NodeEditorWindow.Open(selectedNode, (PresetLevelData)target);
            Event.current.Use();
        }
    }

    private void DrawCell(Texture2D tex, int x, int y, Color color, int width, int height)
    {
        for (int px = 2; px < CellSize - 2; px++)
            for (int py = 2; py < CellSize - 2; py++)
                tex.SetPixel(x * CellSize + px, y * CellSize + py, color);
    }

    private void DrawSmallCell(Texture2D tex, int x, int y, Color color, int width, int height, Cardinal dir)
    {
        int cellSize = CellSize / 4;
        int offset = CellSize / 16;
        int startX = x * CellSize;
        int startY = y * CellSize;

        switch (dir)
        {
            case Cardinal.North:
                startY += CellSize - cellSize - offset;
                startX += (CellSize - cellSize) / 2;
                break;
            case Cardinal.South:
                startY += offset;
                startX += (CellSize - cellSize) / 2;
                break;
            case Cardinal.East:
                startX += CellSize - cellSize - offset;
                startY += (CellSize - cellSize) / 2;
                break;
            case Cardinal.West:
                startX += offset;
                startY += (CellSize - cellSize) / 2;
                break;
        }

        for (int px = 0; px < cellSize; px++)
            for (int py = 0; py < cellSize; py++)
                tex.SetPixel(startX + px, startY + py, color);
    }

    private void FillAllWalls(PresetLevelData preset, MazeSizeData sizeData)
    {
        var wallPositionsField = typeof(PresetLevelData).GetField("wallPositions", BindingFlags.NonPublic | BindingFlags.Instance);
        var wallPositions = new List<PresetWallPositionData>();

        int width = sizeData.Width;
        int height = sizeData.Height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x < width - 1)
                    wallPositions.Add(new PresetWallPositionData(new Coordinate(x, y), Cardinal.East, WallType.wall));
                if (y < height - 1)
                    wallPositions.Add(new PresetWallPositionData(new Coordinate(x, y), Cardinal.North, WallType.wall));
            }
        }

        for (int x = 0; x < width; x++)
        {
            wallPositions.Add(new PresetWallPositionData(new Coordinate(x, 0), Cardinal.South, WallType.wall));
            wallPositions.Add(new PresetWallPositionData(new Coordinate(x, height - 1), Cardinal.North, WallType.wall));
        }
        for (int y = 0; y < height; y++)
        {
            wallPositions.Add(new PresetWallPositionData(new Coordinate(0, y), Cardinal.West, WallType.wall));
            wallPositions.Add(new PresetWallPositionData(new Coordinate(width - 1, y), Cardinal.East, WallType.wall));
        }

        wallPositionsField.SetValue(preset, wallPositions);
    }

    private void DrawWall(Texture2D tex, int x, int y, Cardinal dir, int width, int height, WallType wallType)
    {
        int px = x * CellSize;
        int py = y * CellSize;
        Color wallColor = wallType switch
        {
            WallType.gate => Color.white,
            _ => Color.clear
        };


        switch (dir)
        {
            case Cardinal.North:
                for (int i = 0; i < CellSize; i++)
                    tex.SetPixel(px + i, py + CellSize - 1, wallColor);
                break;
            case Cardinal.South:
                for (int i = 0; i < CellSize; i++)
                    tex.SetPixel(px + i, py, wallColor);
                break;
            case Cardinal.East:
                for (int i = 0; i < CellSize; i++)
                    tex.SetPixel(px + CellSize - 1, py + i, wallColor);
                break;
            case Cardinal.West:
                for (int i = 0; i < CellSize; i++)
                    tex.SetPixel(px, py + i, wallColor);
                break;
        }
    }
}
#endif // UNITY_EDITOR