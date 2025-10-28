using UnityEngine;

public class AssetService
{
    public static Sprite GetMazeFloor(MazeFloorType type)
    {
        string path = string.Format(Constants.Assets.MAZE_FLOOR_SPRITE_PATH, type);
        return LoadSprite(path);
    }

    public static Sprite GetMazeWall(Cardinal direction)
    {
        string path = string.Format(Constants.Assets.MAZE_WALL_SPRITE_PATH, direction);
        return LoadSprite(path);
    }

    private static Sprite LoadSprite(string path)
    {
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite == null)
        {
            Debug.LogError($"Failed to load sprite at path: {path}");
        }
        return sprite;
    }
}

public enum MazeFloorType
{
    topleft,
    top,
    topright,
    left,
    middle,
    right,
    bottomleft,
    bottom,
    bottomright,
}
