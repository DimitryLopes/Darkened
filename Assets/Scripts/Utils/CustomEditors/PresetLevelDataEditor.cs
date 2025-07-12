using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(PresetLevelData))]
public class PresetLevelDataEditor : Editor
{
    private const int CellSize = 20;

    public override void OnInspectorGUI()
    {
        PresetLevelData preset = (PresetLevelData)target;

        // Sincroniza SpawnType antes de desenhar o Inspector
        var itemsField = typeof(PresetLevelData).GetField("items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var items = (System.Collections.IList)itemsField.GetValue(preset);

        if (items != null)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var itemData = items[i];
                var itemField = itemData.GetType().GetField("item", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var spawnTypeField = itemData.GetType().GetField("SpawnType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                var item = itemField.GetValue(itemData);
                if (item == null)
                {
                    // Desenha campo padrão vazio no Inspector
                    EditorGUILayout.LabelField($"Item {i}", "Vazio");
                    continue;
                }

                var generationDataProp = item.GetType().GetProperty("GenerationData", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var generationData = generationDataProp.GetValue(item);
                var spawnTypeProp = generationData.GetType().GetProperty("SpawnType", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var spawnType = spawnTypeProp.GetValue(generationData);

                spawnTypeField.SetValue(itemData, spawnType);
                items[i] = itemData; // Atualiza o struct na lista
            }
        }

        // Agora desenha o Inspector normalmente
        DrawDefaultInspector();

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
        var itemsList = (List<PresetItemData>)preset.GetType().GetField("items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(preset);
        foreach (var presetItem in itemsList)
        {
            // Verifica se o item é nulo antes de acessar suas propriedades
            if (presetItem.Item == null)
            {
                // Desenha uma célula padrão para itens nulos (opcional)
                DrawCell(preview, presetItem.Coordinate.X, presetItem.Coordinate.Y, Color.gray, width, height);
                continue;
            }

            Color itemColor = presetItem.Item.Type switch
            {
                ItemType.DefaultTorch => new Color(1f, 0.4f, 0, 1),
                ItemType.Exit => Color.black,
                _ => Color.cyan
            };

            switch (presetItem.Item.GenerationData.SpawnType)
            {
                case SpawnType.Wall:
                    DrawSmallCell(preview, presetItem.Coordinate.X, presetItem.Coordinate.Y, itemColor, width, height, presetItem.Direction);
                    break;
                case SpawnType.EdgeWalls:
                    DrawSmallCell(preview, presetItem.Coordinate.X, presetItem.Coordinate.Y, itemColor, width, height, presetItem.Direction);
                    break;
                default:
                    DrawCell(preview, presetItem.Coordinate.X, presetItem.Coordinate.Y, itemColor, width, height);
                    break;
            };
        }

        // Desenha posições especiais
        var startPosition = (Coordinate)preset.GetType().GetField("startPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(preset);
        DrawCell(preview, startPosition.X, startPosition.Y, Color.green, width, height);

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
    
    private void DrawSmallCell(Texture2D tex, int x, int y, Color color, int width, int height, Cardinal dir)
    {
        int cellSize = CellSize / 4;
        int offset = CellSize / 16; // Menor offset para encostar melhor na borda

        int startX = x * CellSize;
        int startY = y * CellSize;

        switch (dir)
        {
            case Cardinal.North:
                // Encosta no topo da célula
                startY += CellSize - cellSize - offset;
                startX += (CellSize - cellSize) / 2;
                break;
            case Cardinal.South:
                // Encosta na base da célula
                startY += offset;
                startX += (CellSize - cellSize) / 2;
                break;
            case Cardinal.East:
                // Encosta na direita da célula
                startX += CellSize - cellSize - offset;
                startY += (CellSize - cellSize) / 2;
                break;
            case Cardinal.West:
                // Encosta na esquerda da célula
                startX += offset;
                startY += (CellSize - cellSize) / 2;
                break;
        }

        for (int px = 0; px < cellSize; px++)
            for (int py = 0; py < cellSize; py++)
                tex.SetPixel(startX + px, startY + py, color);
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