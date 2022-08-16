using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    private bool isWall_m = true;

    public MapNode(bool isWall = true)
    {
        isWall_m = isWall;
    }

    public bool GetIsWall()
    {
        return isWall_m;
    }

    public void SetIsWall(bool isWall)
    {
        isWall_m = isWall;
    }
}
