using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeWall : Activateable
{
    public bool IsAtBorder { get; private set; }
    public Cardinal AlignedWith { get; private set; }

    protected override void OnActivate()
    {
        gameObject.SetActive(true);
    }

    protected override void OnDeactivate()
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
        item.transform.position = transform.position + transform.forward / 2;
    }

}
