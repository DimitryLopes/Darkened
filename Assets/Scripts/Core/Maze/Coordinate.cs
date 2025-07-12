using UnityEngine;
using System;

[Serializable]
public struct Coordinate 
{
    [SerializeField]
    private int x;
    [SerializeField]
    private int y;

    public int X => x;
    public int Y => y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
