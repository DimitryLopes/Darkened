using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(PresetLevelData))]
public class PresetLevelDataEditor : Editor
{
    private const int CellSize = 20;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PresetLevelData preset = (PresetLevelData)target;
        if (preset == null || preset.GetType().GetField("sizeData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(preset) == null)
            return;

        // Obtém tamanho do labirinto
        var sizeData = (MazeSizeData)preset.GetType().GetField("sizeData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(preset);
        int width = sizeData.Width;
        int height = sizeData.Height;

        GUILayout.Space(10);
        GUILayout.Label("Maze Preview", EditorStyles.boldLabel);

        // Cria textura para preview
        Texture2D preview = new Texture2D(width * CellSize, height * CellSize);
        Color bgColor = new Color(0.85f, 0.85f, 0.85f);

        // Preenche fundo
        for (int y = 0; y < height * CellSize; y++)
            for (int x = 0; x < width * CellSize; x++)
                preview.SetPixel(x, y, bgColor);

        // Desenha paredes
        var wallPositions = (List<PresetWallPositionData>)preset.GetType().GetField("wallPositions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(preset);
        foreach (var wall in wallPositions)
        {
            DrawWall(preview, wall.Coordinate.X, wall.Coordinate.Y, wall.Direction, width, height);
        }

        // Desenha itens
        var items = (List<PresetItemData>)preset.GetType().GetField("items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(preset);
        foreach (var item in items)
        {
            DrawCell(preview, item.Coordinate.X, item.Coordinate.Y, Color.cyan, width, height);
        }

        // Desenha posições especiais
        var startPosition = (Coordinate)preset.GetType().GetField("startPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(preset);
        DrawCell(preview, startPosition.X, startPosition.Y, Color.green, width, height);

        var endPosition = (Coordinate)preset.GetType().GetField("endPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(preset);
        DrawCell(preview, endPosition.X, endPosition.Y, Color.yellow, width, height);

        var enemySpawnPosition = (Coordinate)preset.GetType().GetField("enemySpawnPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(preset);
        DrawCell(preview, enemySpawnPosition.X, enemySpawnPosition.Y, Color.red, width, height);

        preview.Apply();

        GUILayout.Space(10);
        GUILayout.Label(preview, GUILayout.Width(width * CellSize), GUILayout.Height(height * CellSize));
    }

    private void DrawCell(Texture2D tex, int x, int y, Color color, int width, int height)
    {
        for (int px = 2; px < CellSize - 2; px++)
            for (int py = 2; py < CellSize - 2; py++)
                tex.SetPixel(x * CellSize + px, y * CellSize + py, color);
    }

    private void DrawWall(Texture2D tex, int x, int y, Cardinal dir, int width, int height)
    {
        int px = x * CellSize;
        int py = y * CellSize;
        Color wallColor = Color.black;

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