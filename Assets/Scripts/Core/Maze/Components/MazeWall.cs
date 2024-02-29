using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeWall : Activateable
{
    protected override void OnActivate()
    {
        gameObject.SetActive(true);
    }

    protected override void OnDeactivate()
    {
        gameObject.SetActive(false);
    }
}
