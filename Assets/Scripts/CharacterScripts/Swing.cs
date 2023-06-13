using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    public float RPM_M = 1000.0f;
    public float revolveDistance_M = 20000.0f;
    public float pullSpeed_M = 20000.0f;
    public float pullOffsetX_M = 1.5f;
    public float pullOffsetY_M = 0.5f;
    public float throwSpeed_M = 3000.0f;
    public float throwTime_M = 0.05f;

    public enum SwingState
    {
        NONE,
        PULL,
        TRANSITION,
        REVOLVE,
        WINDUP,
        THROW
    }
    
    private float revolveSpeed_m;
    private float tension_m;
    private GameObject hook_m;
    private GameObject player_m;
    private Rigidbody2D rigid_m;
    private Vector3 [] direction_m = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
    private Vector3 windupInitial_m = Vector3.zero;
    private Vector3 originalPos_m = Vector3.zero;
    private Vector3 destination_m = Vector3.zero;
    private float startTime_m = 0.0f;
    private int directionIndex_m = 0;
    private SwingState swingState_m = SwingState.NONE;

    // Start is called before the first frame update
    void Start()
    {
        player_m = GameObject.Find("Player");
        rigid_m = GetComponent<Rigidbody2D>();

        revolveSpeed_m = 2 * 3.14f * RPM_M;
        tension_m = (revolveSpeed_m * revolveSpeed_m) / revolveDistance_M;
    }

    // Update is called once per frame
    void Update()
    {
        if (player_m)
        {
            switch (swingState_m)
            {
                case SwingState.NONE:
                    break;
                case SwingState.PULL:
                    destination_m = pull();

                    if (Vector3.Distance(player_m.transform.position, transform.position) <= 
                        Vector3.Distance(player_m.transform.position, destination_m))
                    {
                        ++swingState_m;
                        GetComponent<Collider2D>().isTrigger = true;
                    }
                    break;
                case SwingState.TRANSITION:
                    transition(destination_m);

                    if (Vector3.Distance(destination_m, transform.position) < 1)
                    {
                        ++swingState_m;
                        GetComponent<Collider2D>().isTrigger = false;
                    }
                    break;
                case SwingState.REVOLVE:
                    revolve();
                    break;
                case SwingState.WINDUP:
                    Windup();
                    if ((Time.time - startTime_m) / throwTime_M >= 1)
                    {
                        if (directionIndex_m == direction_m.Length - 1)
                        {
                            ++swingState_m;
                            Throw();
                        }
                        else
                        {
                            ++directionIndex_m;
                            startTime_m = Time.time;
                            windupInitial_m = transform.position - player_m.transform.position;
                        }
                    }
                    break;
                case SwingState.THROW:
                    if (rigid_m.velocity.magnitude <= 1)
                    {
                        swingState_m = SwingState.NONE;
                        //Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player_m.GetComponent<Collider2D>(), false);
                    }
                    break;
            }
        }
        else
        {
            swingState_m = SwingState.NONE;
        }
    }

    Vector3 pull()
    {
        Vector3 playerPos = player_m.transform.position;
        Vector3 playerDirection = player_m.transform.position - transform.position;
        playerDirection.Normalize();

        Vector3 playerDirNormal = new Vector3(playerDirection.y, -playerDirection.x);

        Vector3 destination = playerPos + playerDirNormal * pullOffsetX_M + playerDirection * pullOffsetY_M;
        
        Vector2 pullDirection = destination - transform.position;
        pullDirection.Normalize();

        rigid_m.AddForce(pullDirection * pullSpeed_M * Time.deltaTime + player_m.GetComponent<Movement>().force);

        return destination;
    }

    // transition from pull state to revolve state
    Vector3 transition(Vector3 destination)
    {
        Vector2 pullDirection = destination - transform.position;
        pullDirection.Normalize();

        rigid_m.AddForce(pullDirection * pullSpeed_M * Time.deltaTime);

        return destination;
    }

    void revolve()
    {
        Vector3 revolvePoint = player_m.transform.position;
        
        Vector2 force = Vector2.zero;
        float distance = Vector3.Distance(revolvePoint, transform.position);

        Vector2 centerDirection = revolvePoint - transform.position;
        centerDirection.Normalize();

        force += centerDirection * tension_m * Time.deltaTime * distance;

        Vector2 SwingDirection = new Vector2(centerDirection.y, -centerDirection.x);
        SwingDirection.Normalize();

        force += SwingDirection * revolveSpeed_m * Time.deltaTime;

        force += player_m.GetComponent<Movement>().force;

        rigid_m.AddForce(force);
    }

    // transition from revolve state to throw state
    void Windup()
    {
        transform.position = Vector3.Slerp(windupInitial_m, direction_m[directionIndex_m], (Time.time - startTime_m) / throwTime_M);
        transform.position += player_m.transform.position;
    }

    void Throw()
    {
        direction_m[0] = player_m.GetComponent<GrabThrow>().GetMousePos() - transform.position;
        direction_m[0].Normalize();
        rigid_m.AddForce(-direction_m[3] * throwSpeed_M);
        hook_m.GetComponent<HookMovement>().SetHookState(HookMovement.HookState.REEL);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player_m.GetComponent<Collider2D>(), false);
    }

    /**************************************************** Get Functions *************************************************************/

    public Vector3 GetDirection(int index)
    {
        return direction_m[index];
    }
    public SwingState GetSwingState()
    {
        return swingState_m;
    }

    public GameObject GetHook()
    {
        return hook_m;
    }

    /**************************************************** Set Functions *************************************************************/

    public void SetHook(GameObject newHook)
    {
        hook_m = newHook;
    }

    public void SetDirection(int index, float x, float y, float z = 0)
    {
        if (index >= 0 && index < direction_m.Length)
        {
            direction_m[index].x = x;
            direction_m[index].y = y;
            direction_m[index].z = z;
        }
    }

    public void SetDirection(int index, Vector3 directionVector)
    {
        if (index >= 0 && index < direction_m.Length)
        {
            direction_m[index] = directionVector;
        }
    }

    public void SetWindupInitial(Vector3 positionVector)
    {
        windupInitial_m = positionVector;
    }

    public void SetStartTime(float time)
    {
        startTime_m = time;
    }

    public void SetDirectionIndex(int index)
    {
        if (index >= 0 && index < direction_m.Length)
        {
            directionIndex_m = index;
        }
    }

    public void SetSwingState(SwingState state)
    {
        swingState_m = state;
    }
}
