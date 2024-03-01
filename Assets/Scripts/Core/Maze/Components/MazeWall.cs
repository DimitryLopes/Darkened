using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeWall : Activateable
{
    public bool IsAtBorder { get; private set; }

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
        float rotation = NodeUtils.GetWallRotationByCardinal(direction);
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

}
