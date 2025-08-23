using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShadowCasterHelper : Activateable
{
    [SerializeField]
    public ShadowCaster2D ShadowCaster;

    public List<MazeWall> Walls = new();
}
