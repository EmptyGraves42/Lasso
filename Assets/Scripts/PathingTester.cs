using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathingTester : MonoBehaviour
{
    public GameObject startMarkerPrefab_M;
    public GameObject gaolMarkerPrefab_M;
    public GameObject markerPrefab_M;
    public Camera camera_M;

    private PathFinder pathFinder_m;
    private List<GameObject> markers_m = new List<GameObject>();
    private Vector3 startPos_m = new Vector3();
    private Vector3 goalPos_m = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        pathFinder_m = new PathFinder(GameObject.Find("MapManager").GetComponent<MapManager>());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            startPos_m = GetMousePos();
            CreatePath();
        }

        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            goalPos_m = GetMousePos();
            CreatePath();
        }

        if(Input.GetKey(KeyCode.W))
        {
            camera_M.transform.position = camera_M.transform.position + new Vector3(0, 1, 0);
        }

        if(Input.GetKey(KeyCode.S))
        {
            camera_M.transform.position = camera_M.transform.position + new Vector3(0, -1, 0);
        }

        if (Input.GetKey(KeyCode.A))
        {
            camera_M.transform.position = camera_M.transform.position + new Vector3(-1, 0, 0);
        }

        if (Input.GetKey(KeyCode.D))
        {
            camera_M.transform.position = camera_M.transform.position + new Vector3(1, 0, 0);
        }
    }

    private void CreatePath()
    {
        foreach(GameObject marker in markers_m)
        {
            GameObject.Destroy(marker);
        }

        markers_m.Clear();

        LinkedList<Vector3> path = new LinkedList<Vector3>();
        if (pathFinder_m.AStartPath(startPos_m, goalPos_m, out path))
        {
            // put markers at each node in path
            foreach(Vector3 node in path)
            {
                if(node == path.First.Value)
                {
                    GameObject marker = Instantiate(startMarkerPrefab_M);
                    marker.transform.position = node;
                    markers_m.Add(marker);
                }
                else if(node == path.Last.Value)
                {
                    GameObject marker = Instantiate(gaolMarkerPrefab_M);
                    marker.transform.position = node;
                    markers_m.Add(marker);
                }
                else
                {
                    GameObject marker = Instantiate(markerPrefab_M);
                    marker.transform.position = node;
                    markers_m.Add(marker);
                }
            }
        }
        else
        {
            print("INVALID PATH!!!!!!!!!");
        }
    }

    private Vector3 GetMousePos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        return mousePos;
    }
}
