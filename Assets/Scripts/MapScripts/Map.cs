using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    private Vector2Int entrance_m;
    private Vector2Int exit_m;
    private int width_m;
    private int height_m;

    private MapNode[,] grid_m;

    public Map(Vector2Int entrance, Vector2Int exit, int width, int height)
    {
        width_m = width;
        height_m = height;
        grid_m = new MapNode[width, height];
        entrance_m = entrance;
        exit_m = exit;

        for(int i = 0; i < width; ++i)
        {
            for(int j = 0; j < height; ++j)
            {
                grid_m[i, j] = new MapNode();
            }
        }
    }

    

    public bool IsWall(int x, int y)
    {
        return grid_m[x, y].GetIsWall();
    }

    public bool IsWall(Vector2Int pos)
    {
        return grid_m[pos.x, pos.y].GetIsWall();
    }

    public bool IsValidPos(int x, int y)
    {
        return x >= 0 && x < width_m && y >= 0 && y < height_m;
    }

    public bool IsValidPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width_m && pos.y >= 0 && pos.y < height_m;
    }

/**************************************************** Get Functions *************************************************************/
public MapNode GetGridNode(int x, int y)
    {
        return grid_m[x, y];
    }

    public int GetWidth()
    {
        return width_m;
    }

    public int GetHeight()
    {
        return height_m;
    }

    public Vector2Int GetEntrance()
    {
        return entrance_m;
    }

    public Vector2Int GetExit()
    {
        return exit_m;
    }
}
