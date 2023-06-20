using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMovement : MonoBehaviour
{
    public float moveSpeed_M = 100.0f;
    public float chaseDistance_M;
    public float rotationSpeed_M;

    private GameObject player_m;
    private PathFinder pathFinder_m;
    private Rigidbody2D rigid_m;
    private LinkedList<Vector3> path_m;
    private LinkedListNode<Vector3> destination_m;
    private bool validPath_m = false;

    // Start is called before the first frame update
    void Start()
    {
        player_m = GameObject.Find("Player");
        rigid_m = GetComponent<Rigidbody2D>();
        pathFinder_m = new PathFinder(GameObject.Find("MapManager").GetComponent<MapManager>());
    }

    // Update is called once per frame
    void Update()
    {
        if (player_m)
        {
            // do nothing of being swung/thrown by player
            if(GetComponent<Swing>() && GetComponent<Swing>().GetSwingState() != Swing.SwingState.NONE)
            {
                return;
            }

            if (Vector3.Distance(player_m.transform.position, transform.position) <= chaseDistance_M) // distance check
            {
                // line of sight check
                RaycastHit2D castResult = Physics2D.Raycast(transform.position, player_m.transform.position - transform.position, chaseDistance_M, 1 << LayerMask.NameToLayer("Enemy Sight"));
                if(castResult.collider != null)
                {
                    if (castResult.collider.gameObject == player_m)
                    {
                        validPath_m = pathFinder_m.AStartPath(transform.position, player_m.transform.position, out path_m);

                        if (validPath_m)
                        {
                            // skip first node in path becuase it is where the enemy currently is
                            destination_m = path_m.First.Next;
                        }
                    }
                }   
            }
            
            if(validPath_m && destination_m != null)
            {
                // rotate toward destination
                Vector2 direction = destination_m.Value - transform.position;
                Quaternion desiredRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90);
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSpeed_M);

                // add force in directoin of destination
                direction.Normalize();
                rigid_m.AddForce(direction * moveSpeed_M * Time.deltaTime);

                if(Vector3.Distance(transform.position, destination_m.Value) <= 1)
                {
                    destination_m = destination_m.Next;
                }
            }
        }
    }
}
