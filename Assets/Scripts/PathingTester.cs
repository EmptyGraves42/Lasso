using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathingTester : MonoBehaviour
{
    public GameObject startMarkerPrefab_m;
    public GameObject gaolMarkerPrefab_m;
    public GameObject markerPrefab_m;
    public PathFinder pathFinder_M;

    private List<GameObject> markers_m = new List<GameObject>();
    private Vector3 startPos_m = new Vector3();
    private Vector3 goalPos_m = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        
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
    }

    private void CreatePath()
    {
        foreach(GameObject marker in markers_m)
        {
            GameObject.Destroy(marker);
        }

        markers_m.Clear();

        LinkedList<Vector3> path = new LinkedList<Vector3>();
        if (pathFinder_M.AStartPath(startPos_m, goalPos_m, out path))
        {
            foreach(Vector3 node in path)
            {
                if(node == path.First.Value)
                {
                    GameObject marker = Instantiate(startMarkerPrefab_m);
                    marker.transform.position = node;
                    markers_m.Add(marker);
                }
                else if(node == path.Last.Value)
                {
                    GameObject marker = Instantiate(gaolMarkerPrefab_m);
                    marker.transform.position = node;
                    markers_m.Add(marker);
                }
                else
                {
                    GameObject marker = Instantiate(markerPrefab_m);
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
