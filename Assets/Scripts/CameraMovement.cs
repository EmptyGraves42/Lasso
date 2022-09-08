using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player_M;
    public Vector3 offset_M;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player_M)
        {
            // camera follows player with a given offset
            transform.position = new Vector3(player_M.position.x + offset_M.x, player_M.position.y + offset_M.y, player_M.position.z + offset_M.z);
        }
    }
}
