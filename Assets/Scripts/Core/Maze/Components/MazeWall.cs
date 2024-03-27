using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeWall : Activateable
{
    [SerializeField]
    public float WALL_ITEM_OFFSET = 1f;

    public bool IsAtBorder { get; private set; }
    public Cardinal AlignedWith { get; private set; }

    public override void OnActivate()
    {
        gameObject.SetActive(true);
    }

    public override void OnDeactivate()
    {
        gameObject.SetActive(false);
    }

    public void AlignWith(Cardinal direction, bool isAtBorder)
    {
        IsAtBorder = isAtBorder;
        AlignedWith = direction;
        float rotation = NodeUtils.GetWallRotationByCardinal(direction);
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    public void PositionObject(Item item)
    {
        item.transform.rotation = transform.rotation;
        item.transform.position = transform.position + transform.right * WALL_ITEM_OFFSET;
    }

}
