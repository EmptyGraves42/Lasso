using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject floorPrefab_M;
    public GameObject wallPrefab_M;
    public GameObject boxSpawnerPrefab_M;
    public GameObject enemySpawnerPrefab_M;

    public int mapWidth_M = 10;
    public int mapHeight_M = 10;
    public int keyPoints_M = 10;
    public float boxSpawnChance_M = 0.5f;
    public float enemySpawnChance_M = 0.25f;

    private float boxSpawnerOffset;

    // Start is called before the first frame update
    void Start()
    {
        boxSpawnerOffset = floorPrefab_M.transform.lossyScale.x / 3;

        Map map = GenerateMap(new Vector2Int(0, 1), mapWidth_M, mapHeight_M, keyPoints_M);
        InstantiateMap(map);
        PupulateMap(map);
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
    public Map GenerateMap(Vector2Int entrance, int width, int height, int keyPointsNum)
    {
        Map map = new Map(width, height);

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

        // crete paths between randomly generated points
        for(int i = 0; i < keyPointsNum - 1; ++i)
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
                            boxSpawner.transform.position = gridWorldPosition + new Vector3(boxSpawnerOffset, 0, 0);
                        }

                        if (map.GetGridNode(i - 1, j).GetIsWall()) // node to left
                        {
                            boxSpawner = Instantiate(boxSpawnerPrefab_M);
                            boxSpawner.transform.position = gridWorldPosition - new Vector3(boxSpawnerOffset, 0, 0);
                        }

                        if (map.GetGridNode(i, j + 1).GetIsWall()) // node below
                        {
                            boxSpawner = Instantiate(boxSpawnerPrefab_M);
                            boxSpawner.transform.position = gridWorldPosition - new Vector3(0, boxSpawnerOffset, 0);
                        }

                        if (map.GetGridNode(i, j - 1).GetIsWall()) // node above
                        {
                            boxSpawner = Instantiate(boxSpawnerPrefab_M);
                            boxSpawner.transform.position = gridWorldPosition + new Vector3(0, boxSpawnerOffset, 0);
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

    public Vector3 GridToWorldPos(int x, int y)
    {
        return new Vector3(x * floorPrefab_M.transform.lossyScale.x + transform.position.x,
                          -(y * floorPrefab_M.transform.lossyScale.y) + transform.position.y,
                          0.0f);
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
}
