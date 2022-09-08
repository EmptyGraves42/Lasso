using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    public enum List
    {
        OPEN,
        CLOSED,
        NEITHER,
    };

    public class Node
    {
        Vector2Int pos_M;
        public float cost_M;
        public float given_M;
        public Vector2Int parent_M;
        public List list_M;
        
        public Node()
        {
            pos_M = new Vector2Int(0, 0);
            cost_M = 0;
            parent_M = new Vector2Int(-1, -1);
            given_M = 0;
            list_M = List.NEITHER;
        }

        public Node(Vector2Int position, float cost = 0, int parentX = -1, int parentY = -1, float given = 0)
        {
            pos_M = position;
            cost_M = cost;
            parent_M = new Vector2Int(parentX, parentY);
            given_M = given;
            list_M = List.NEITHER;
        }
    };

    private List<List<Node>> nodeMap_m = new List<List<Node>>();
    private List<Vector2Int> openList_m = new List<Vector2Int>();
    private List<Vector2Int> closedList_m = new List<Vector2Int>();

    Vector2Int firstNode_m;

    MapManager mapManager_m;
    Map map_m;

    public PathFinder(MapManager mapManager)
    {
        mapManager_m = mapManager;
        map_m = mapManager_m.GetMap();
        CreateNodeMap();
    }

    void CreateNodeMap()
    {
        for(int i = 0; i < map_m.GetWidth(); ++i)
        {
            nodeMap_m.Add(new List<Node>());

            for(int j = 0; j < map_m.GetHeight(); ++j)
            {
                nodeMap_m[i].Add(new Node(new Vector2Int(i, j)));
            }
        }
    }

    public bool AStartPath(Vector3 startWorldPos, Vector3 goalWorldPos, out LinkedList<Vector3> path)
    {

        Vector2Int start = mapManager_m.WorldToGridPos(startWorldPos);
        Vector2Int goal = mapManager_m.WorldToGridPos(goalWorldPos);
        path = new LinkedList<Vector3>();
        
        if(!map_m.IsValidPos(start) || map_m.IsWall(start) || !map_m.IsValidPos(goal) || map_m.IsWall(goal))
        {
            return false;
        }
        
        ResetNodes();
        
        // Push Start Node onto Open List
        firstNode_m = new Vector2Int(start.x, start.y);
        nodeMap_m[firstNode_m.x][firstNode_m.y].cost_M = nodeMap_m[firstNode_m.x][firstNode_m.y].given_M + OctileHuristic(start, goal);

        openList_m.Add(firstNode_m);
        nodeMap_m[firstNode_m.x][firstNode_m.y].list_M = List.OPEN;
        
        // While Open List is not empty
        while(openList_m.Count > 0)
        {
            // Pop cheapest node off Open List
            Vector2Int cheapest = openList_m[0];
            foreach(Vector2Int node in openList_m)
            {
                if(nodeMap_m[node.x][node.y].cost_M < nodeMap_m[cheapest.x][cheapest.y].cost_M)
                {
                    cheapest = node;
                }
            }

            openList_m.Remove(cheapest);

            // If node is Goal, path is found
            if(cheapest == goal)
            {
                closedList_m.Add(cheapest);
                nodeMap_m[cheapest.x][cheapest.y].list_M = List.CLOSED;

                // push path onto out path
                while (cheapest.x != -1)
                {
                    path.AddFirst(mapManager_m.GridToWorldPos(cheapest));

                    if(nodeMap_m[cheapest.x][cheapest.y].parent_M.x == -1)
                    {
                        cheapest.x = -1;
                        cheapest.y = -1;
                    }
                    else
                    {
                        cheapest = nodeMap_m[cheapest.x][cheapest.y].parent_M;
                    }
                }

                path.First.Value = startWorldPos;

                if (path.Count == 1) // if start and goal have the same grid pos
                {
                    path.AddLast(goalWorldPos);
                }
                else
                {
                    path.Last.Value = goalWorldPos;
                }

                RubberBanding(ref path);

                return true;
            }

            // Check all neighboring nodes
            List<Vector2Int> neighbors = GetNeighbors(cheapest);

            for(int i = 0; i < neighbors.Count; ++i)
            {
                Vector2Int neighbor = neighbors[i];

                // skip if neighbor is wall or invalid
                if(neighbor.x == -1)  
                {
                    continue;
                }

                // If child node isn't on a List, put it on Open List
                if(nodeMap_m[neighbor.x][neighbor.y].list_M == List.NEITHER)
                {
                    openList_m.Add(neighbor);
                    nodeMap_m[neighbor.x][neighbor.y].list_M = List.OPEN;
                    nodeMap_m[neighbor.x][neighbor.y].parent_M = cheapest;
                    nodeMap_m[neighbor.x][neighbor.y].given_M = IncreaseGiven(nodeMap_m[cheapest.x][cheapest.y].given_M, i);
                    nodeMap_m[neighbor.x][neighbor.y].cost_M = nodeMap_m[neighbor.x][neighbor.y].given_M + OctileHuristic(neighbor, goal);
                }
                // If child node is on a List and is cheaper, put it only on open list
                else if(nodeMap_m[neighbor.x][neighbor.y].given_M > IncreaseGiven(nodeMap_m[cheapest.x][cheapest.y].given_M, i))
                {
                    if(nodeMap_m[neighbor.x][neighbor.y].list_M == List.OPEN) // remove from open list
                    {
                        openList_m.Remove(neighbor);
                    }

                    if(nodeMap_m[neighbor.x][neighbor.y].list_M == List.CLOSED) // remove from closed list
                    {
                        closedList_m.Remove(neighbor);
                    }

                    // put on open list
                    openList_m.Add(neighbor);
                    nodeMap_m[neighbor.x][neighbor.y].list_M = List.OPEN;
                    nodeMap_m[neighbor.x][neighbor.y].parent_M = cheapest;
                    nodeMap_m[neighbor.x][neighbor.y].given_M = IncreaseGiven(nodeMap_m[cheapest.x][cheapest.y].given_M, i);
                    nodeMap_m[neighbor.x][neighbor.y].cost_M = nodeMap_m[neighbor.x][neighbor.y].given_M + OctileHuristic(neighbor, goal);
                }
            }

            // Place parent node on the Closed List
            closedList_m.Add(cheapest);
            nodeMap_m[cheapest.x][cheapest.y].list_M = List.CLOSED;
        }
        // if Open list empty, no path possible
        return false;
    }

    private void ResetNodes()
    {
        foreach(Vector2Int node in openList_m)
        {
            nodeMap_m[node.x][node.y].parent_M.x = -1;
            nodeMap_m[node.x][node.y].parent_M.y = -1;
            nodeMap_m[node.x][node.y].list_M = List.NEITHER;
            nodeMap_m[node.x][node.y].given_M = 0;
            nodeMap_m[node.x][node.y].cost_M = 0;
        }
        openList_m.Clear();

        foreach(Vector2Int node in closedList_m)
        {
            nodeMap_m[node.x][node.y].parent_M.x = -1;
            nodeMap_m[node.x][node.y].parent_M.y = -1;
            nodeMap_m[node.x][node.y].list_M = List.NEITHER;
            nodeMap_m[node.x][node.y].given_M = 0;
            nodeMap_m[node.x][node.y].cost_M = 0;
        }
        closedList_m.Clear();
    }

    private float OctileHuristic(Vector2Int current, Vector2Int goal)
    {
        float xDiff = Mathf.Abs(goal.x - current.x);
        float yDiff = Mathf.Abs(goal.y - current.y);

        return Mathf.Min(xDiff, yDiff) * Mathf.Sqrt(2.0f) + Mathf.Max(xDiff, yDiff) - Mathf.Min(xDiff, yDiff);
    }


    private List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        // initialize capacity to 8 because it it the maximum number of neighbors
        List<Vector2Int> neighbors = new List<Vector2Int>(8);
        for(int i = 0; i < 8; ++i)
        {
            neighbors.Add(new Vector2Int(-1, -1));
        }

        Vector2Int neighborPos = new Vector2Int();

        // top center : 0
        neighborPos.x = pos.x;
        neighborPos.y = pos.y - 1;

        if(map_m.IsValidPos(neighborPos) && !map_m.IsWall(neighborPos))
        {
            neighbors[0] = neighborPos;
        }

        // center right : 1
        neighborPos.x = pos.x + 1;
        neighborPos.y = pos.y;

        if (map_m.IsValidPos(neighborPos) && !map_m.IsWall(neighborPos))
        {
            neighbors[1] = neighborPos;
        }

        // bottom center : 2
        neighborPos.x = pos.x;
        neighborPos.y = pos.y + 1;

        if (map_m.IsValidPos(neighborPos) && !map_m.IsWall(neighborPos))
        {
            neighbors[2] = neighborPos;
        }

        // center left : 3
        neighborPos.x = pos.x - 1;
        neighborPos.y = pos.y;

        if (map_m.IsValidPos(neighborPos) && !map_m.IsWall(neighborPos))
        {
            neighbors[3] = neighborPos;
        }

        // top left : 4
        neighborPos.x = pos.x - 1;
        neighborPos.y = pos.y - 1;

        if (map_m.IsValidPos(neighborPos) && !map_m.IsWall(neighborPos) && neighbors[0].x != -1 && neighbors[3].x != -1)
        {
            neighbors[4] = neighborPos;
        }

        // top right : 5
        neighborPos.x = pos.x + 1;
        neighborPos.y = pos.y - 1;

        if (map_m.IsValidPos(neighborPos) && !map_m.IsWall(neighborPos) && neighbors[0].x != -1 && neighbors[1].x != -1)
        {
            neighbors[5] = neighborPos;
        }

        // bottom right : 6
        neighborPos.x = pos.x + 1;
        neighborPos.y = pos.y + 1;

        if (map_m.IsValidPos(neighborPos) && !map_m.IsWall(neighborPos) && neighbors[1].x != -1 && neighbors[2].x != -1)
        {
            neighbors[6] = neighborPos;
        }

        // bottom left : 7
        neighborPos.x = pos.x - 1;
        neighborPos.y = pos.y + 1;

        if (map_m.IsValidPos(neighborPos) && !map_m.IsWall(neighborPos) && neighbors[2].x != -1 && neighbors[3].x != -1)
        {
            neighbors[7] = neighborPos;
        }

        return neighbors;
    }

    private float IncreaseGiven(float start, int index)
    {
        if(index >= 4) // is a diagonal neighbor
        {
            // use 1.41 as an approximation for sqare root of 2
            return start + 1.41f;
        }
        else
        {
            return start + 1.0f;
        }
    }

    private void RubberBanding(ref LinkedList<Vector3> path)
    {
        if(path.Count >= 3)
        {
            LinkedListNode<Vector3> start = path.First;
            LinkedListNode<Vector3> middle = start.Next;
            LinkedListNode<Vector3> end = middle.Next;

            while (end != null)
            {
                Vector2Int edge1 = mapManager_m.WorldToGridPos(start.Value);
                Vector2Int edge2 = mapManager_m.WorldToGridPos(end.Value);

                int xMin = Mathf.Min(edge1.x, edge2.x);
                int xMax = Mathf.Max(edge1.x, edge2.x);
                int yMin = Mathf.Min(edge1.y, edge2.y);
                int yMax = Mathf.Max(edge1.y, edge2.y);

                bool wallFound = false;

                for (int i = yMin; i <= yMax; ++i)
                {
                    for (int j = xMin; j <= xMax; ++j)
                    {
                        if (map_m.IsWall(j, i))
                        {
                            wallFound = true;
                            break;
                        }
                    }

                    if (wallFound)
                    {
                        break;
                    }
                }

                if (wallFound)
                {
                    start = middle;
                }
                else
                {
                    path.Remove(middle);
                }

                middle = end;
                end = end.Next;
            } 
        }
    }
}
