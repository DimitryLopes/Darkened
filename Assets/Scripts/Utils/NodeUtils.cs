using UnityEngine;

public class NodeUtils : MonoBehaviour
{
    public const float NODE_SIZE = 0.975f;
    public static Vector2 GetWallPositionOffset(Cardinal direction)
    {
        Vector2 offset = Vector2.zero;

        switch (direction)
        {
            case Cardinal.North:
                offset = Vector2.up;
                break;
            case Cardinal.East:
                offset = Vector2.right;
                break;
            case Cardinal.South:
                offset = Vector2.down;
                break;
            case Cardinal.West:
                offset = Vector2.left;
                break;
        }

        return offset;
    }

    public static float GetWallRotationByCardinal(Cardinal direction)
    {
        float rotation = 0f;

        switch (direction)
        {
            case Cardinal.North:
                rotation = -90f;
                break;
            case Cardinal.East:
                rotation = 0f;
                break;
            case Cardinal.South:
                rotation = 90f;
                break;
            case Cardinal.West:
                rotation = 180f;
                break;
        }

        return rotation;
    }
}
