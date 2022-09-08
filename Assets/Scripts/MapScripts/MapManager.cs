using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject floorPrefab_M;
    public GameObject wallPrefab_M;
    public GameObject boxSpawnerPrefab_M;
    public GameObject enemySpawnerPrefab_M;
    public GameObject startFloorPrefab_M;

    public int mapWidth_M = 10;
    public int mapHeight_M = 10;
    public int keyPoints_M = 10;
    public float boxSpawnChance_M = 0.5f;
    public float enemySpawnChance_M = 0.25f;

    

    private Map map_m;
    private float boxSpawnerOffset_m;
    private float scale_m;

    // Start is called before the first frame update
    void Start()
    {   
        scale_m = floorPrefab_M.transform.lossyScale.x;
        boxSpawnerOffset_m = scale_m / 3;

        
        map_m = GenerateMap(mapWidth_M, mapHeight_M, keyPoints_M);
        InstantiateMap(map_m);
        Vector2 entraceWorldPos = InstantiateGateway(map_m, map_m.GetEntrance());
        InstantiateGateway(map_m, map_m.GetExit());
        PupulateMap(map_m);

        GameObject player = GameObject.Find("Player");
        if(player)
        {
            player.transform.position = entraceWorldPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /****************************************************************************************************
    
    Name: GenerateMap

    Description: Generates data for map. A different funciton handles instantiating the map

    param: entrance - must be on edge of grid and not a corner.
    param: width - width of the grid
    param: height - height of the grid
    param: keyPointsNum - how many random points to generate paths between

    return: the maplayout that the fucntion generates

    ****************************************************************************************************/
    public Map GenerateMap(int width, int height, int keyPointsNum)
    {
        // determine wich sides of the map the entrance and exits will be located
        int entranceSide = Random.Range(0, 4);
        int exitSide = entranceSide;

        // ensure the entrance and exit are on different sides
        while(entranceSide == exitSide) 
        {
            exitSide = Random.Range(0, 4);
        }

        // determine exact location for entrance and exit
        Vector2Int entrance = new Vector2Int();
        Vector2Int exit = new Vector2Int();

        switch(entranceSide)
        {
            case 0: // top side
                entrance.x = Random.Range(1, width - 1);
                entrance.y = 0;
                break;
            case 1: // bottom side
                entrance.x = Random.Range(1, width - 1);
                entrance.y = height - 1;
                break;
            case 2: // left side
                entrance.x = 0;
                entrance.y = Random.Range(1, height - 1);
                break;
            case 3: // right side
                entrance.x = width - 1;
                entrance.y = Random.Range(1, height - 1);
                break;
        }

        switch (exitSide)
        {
            case 0: // top side
                exit.x = Random.Range(1, width - 1);
                exit.y = 0;
                break;
            case 1: // bottom side
                exit.x = Random.Range(1, width - 1);
                exit.y = height - 1;
                break;
            case 2: // left side
                exit.x = 0;
                exit.y = Random.Range(1, height - 1);
                break;
            case 3: // right side
                exit.x = width - 1;
                exit.y = Random.Range(1, height - 1);
                break;
        }

        Map map = new Map(entrance, exit, width, height);

        Vector2Int[] keyPoints = new Vector2Int[keyPointsNum];

        // generate random points to make paths between
        for(int i = 0; i < keyPointsNum; ++i)
        {
            // points should not be on the very edge of the map
            keyPoints[i].x = Random.Range(1, width - 1);
            keyPoints[i].y = Random.Range(1, height - 1);
        }

        // create entrance to map
        map.GetGridNode(entrance.x, entrance.y).SetIsWall(false);

        if(entrance.x == 0)
        {
            ++entrance.x;
        }
        else if(entrance.x == width - 1)
        {
            --entrance.x;
        }
        else if(entrance.y == 0)
        {
            ++entrance.y;
        }
        else if(entrance.y == height - 1)
        {
            --entrance.y;
        }

        CreatePath(map, entrance, keyPoints[0]);

        // create exit to map
        map.GetGridNode(exit.x, exit.y).SetIsWall(false);

        if (exit.x == 0)
        {
            ++exit.x;
        }
        else if (exit.x == width - 1)
        {
            --exit.x;
        }
        else if (exit.y == 0)
        {
            ++exit.y;
        }
        else if (exit.y == height - 1)
        {
            --exit.y;
        }

        CreatePath(map, exit, keyPoints[keyPoints.Length - 1]);

        // crete paths between randomly generated points
        for (int i = 0; i < keyPointsNum - 1; ++i)
        {
            Vector2Int startNode = keyPoints[i];
            Vector2Int endNode = keyPoints[i + 1];

            CreatePath(map, startNode, endNode);

            map.GetGridNode(endNode.x, endNode.y).SetIsWall(false);
        }

        return map;
    }

    public void CreatePath(Map map, Vector2Int start, Vector2Int end)
    {
        // remove walls long path between points
        while (start != end)
        {
            // remove wall
            MapNode node = map.GetGridNode(start.x, start.y);
            node.SetIsWall(false);

            // find next node along path

            int distanceX = start.x - end.x;
            int distanceY = start.y - end.y;

            if (Mathf.Abs(distanceX) > Mathf.Abs(distanceY))
            {
                if (distanceX > 0)
                {
                    --start.x;
                }
                else
                {
                    ++start.x;
                }
            }
            else
            {
                if (distanceY > 0)
                {
                    --start.y;
                }
                else
                {
                    ++start.y;
                }
            }
        }
    }

    public void PupulateMap(Map map)
    {
        for (int i = 1; i < mapWidth_M - 1; ++i)
        {
            for (int j = 1; j < mapHeight_M - 1; ++j)
            {
                if (!map.GetGridNode(i, j).GetIsWall())
                {
                    if(Random.Range(0.0f, 1.0f) <= boxSpawnChance_M)
                    {
                        GameObject boxSpawner;
                        Vector3 gridWorldPosition = GridToWorldPos(i, j);

                        if (map.GetGridNode(i + 1, j).GetIsWall()) // node to right
                        {
                            boxSpawner = Instantiate(boxSpawnerPrefab_M);
                            boxSpawner.transform.position = gridWorldPosition + new Vector3(boxSpawnerOffset_m, 0, 0);
                        }

                        if (map.GetGridNode(i - 1, j).GetIsWall()) // node to left
                        {
                            boxSpawner = Instantiate(boxSpawnerPrefab_M);
                            boxSpawner.transform.position = gridWorldPosition - new Vector3(boxSpawnerOffset_m, 0, 0);
                        }

                        if (map.GetGridNode(i, j + 1).GetIsWall()) // node below
                        {
                            boxSpawner = Instantiate(boxSpawnerPrefab_M);
                            boxSpawner.transform.position = gridWorldPosition - new Vector3(0, boxSpawnerOffset_m, 0);
                        }

                        if (map.GetGridNode(i, j - 1).GetIsWall()) // node above
                        {
                            boxSpawner = Instantiate(boxSpawnerPrefab_M);
                            boxSpawner.transform.position = gridWorldPosition + new Vector3(0, boxSpawnerOffset_m, 0);
                        }
                    }

                    if(Random.Range(0.0f, 1.0f) <= enemySpawnChance_M)
                    {
                        GameObject enemySpawner = Instantiate(enemySpawnerPrefab_M);
                        enemySpawner.transform.position = GridToWorldPos(i, j);
                    }
                }
            }
        }
    }

    Vector2 InstantiateGateway(Map map, Vector2Int position)
    {
        Vector2 gatePos = GridToWorldPos(position);

        if (position.x == 0) // entrence is to the left
        {
            gatePos.x -= scale_m;

            GameObject tile = Instantiate(startFloorPrefab_M);
            tile.transform.position = gatePos;

            // top wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x, gatePos.y + scale_m, 0);

            // left wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x - scale_m, gatePos.y, 0);

            // bottom wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x, gatePos.y - scale_m, 0);

            // top left wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x - scale_m, gatePos.y + scale_m, 0);

            // bottom left wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x - scale_m, gatePos.y - scale_m, 0);
        }
        else if(position.x == map.GetWidth() - 1) // entrance is to the right
        {
            gatePos.x += scale_m;

            GameObject tile = Instantiate(startFloorPrefab_M);
            tile.transform.position = gatePos;

            // top wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x, gatePos.y + scale_m, 0);

            // right wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x + scale_m, gatePos.y, 0);

            // bottom wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x, gatePos.y - scale_m, 0);

            // top right wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x + scale_m, gatePos.y + scale_m, 0);

            // bottom right wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x + scale_m, gatePos.y - scale_m, 0);
        }
        else if(position.y == 0) // entrance is at the top
        {
            gatePos.y += scale_m;

            GameObject tile = Instantiate(startFloorPrefab_M);
            tile.transform.position = gatePos;

            // top wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x, gatePos.y + scale_m, 0);

            // left wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x - scale_m, gatePos.y, 0);

            // right wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x + scale_m, gatePos.y, 0);

            // top left wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x - scale_m, gatePos.y + scale_m, 0);

            // top right wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x + scale_m, gatePos.y + scale_m, 0);
        }
        else if(position.y == map.GetHeight() - 1) // entrance is at the bottom
        {
            gatePos.y -= scale_m;

            GameObject tile = Instantiate(startFloorPrefab_M);
            tile.transform.position = gatePos;

            // bottom wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x, gatePos.y - scale_m, 0);

            // left wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x - scale_m, gatePos.y, 0);

            // right wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x + scale_m, gatePos.y, 0);

            // bottom left wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x - scale_m, gatePos.y - scale_m, 0);

            // bottom right wall
            tile = Instantiate(wallPrefab_M);
            tile.transform.position = new Vector3(gatePos.x + scale_m, gatePos.y - scale_m, 0);
        }

        return gatePos;
    }

    public Vector3 GridToWorldPos(int x, int y)
    {
        return new Vector3(x * scale_m + transform.position.x,
                          -(y * scale_m) + transform.position.y,
                          0.0f);
    }

    public Vector3 GridToWorldPos(Vector2Int pos)
    {
        return new Vector3(pos.x * scale_m + transform.position.x,
                          -(pos.y * scale_m) + transform.position.y,
                          0.0f);
    }

    public Vector2Int WorldToGridPos(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt((pos.x - transform.position.x) / scale_m),
                              Mathf.RoundToInt((transform.position.y - pos.y) / scale_m));
    }

    public void InstantiateMap(Map map)
    {
        for(int i = 0; i < map.GetWidth(); ++i)
        {
            for(int j = 0; j < map.GetHeight(); ++j)
            {
                GameObject node;

                if(map.GetGridNode(i, j).GetIsWall())
                {
                    node = Instantiate(wallPrefab_M);
                }
                else
                {
                    node = Instantiate(floorPrefab_M);
                }

                node.transform.position = GridToWorldPos(i, j);
            }
        }
    }

    public Map GetMap()
    {
        return map_m;
    }
}
