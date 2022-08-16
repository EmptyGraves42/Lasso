using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    private int width_m;
    private int height_m;

    private MapNode[,] grid_m;

    public Map(int width, int height)
    {
        width_m = width;
        height_m = height;
        grid_m = new MapNode[width, height];

        for(int i = 0; i < width; ++i)
        {
            for(int j = 0; j < height; ++j)
            {
                grid_m[i, j] = new MapNode();
            }
        }
    }

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
}
