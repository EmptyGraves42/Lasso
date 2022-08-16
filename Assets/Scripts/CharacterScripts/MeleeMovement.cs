using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMovement : MonoBehaviour
{
    public float moveSpeed_M = 100.0f;

    private GameObject player_m;
    private Rigidbody2D rigid_m;

    // Start is called before the first frame update
    void Start()
    {
        player_m = GameObject.Find("Player");
        rigid_m = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player_m)
        {
            if(GetComponent<Swing>() && GetComponent<Swing>().GetSwingState() != Swing.SwingState.NONE)
            {
                return;
            }

            Vector3 playerPos = player_m.transform.position;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(playerPos.y - transform.position.y, playerPos.x - transform.position.x) * Mathf.Rad2Deg - 90);
            Vector2 direction = playerPos - transform.position;
            direction.Normalize();
            rigid_m.AddForce(direction * moveSpeed_M * Time.deltaTime);
        }
    }
}
