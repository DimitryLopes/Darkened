using UnityEngine;

public class NodeUtils : MonoBehaviour
{
    public const int NODE_SIZE = 1;
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
}
