using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookMovement : MonoBehaviour
{
    public enum HookState
    {
        SHOOT,
        REEL,
        ATTACHED
    }

    public GameObject chainPrefab_M;

    private GameObject owner_m;
    private GameObject attachedObject_m;
    private Rigidbody2D rigid_m;
    private Vector3 reelStart_m;
    private Vector3 destination_m;
    private float projectileSpeed_m;
    private float reelTime_m;
    private float startReelTime_m;
    private HookState hookState_m = HookState.SHOOT;
    private bool hasChain_m = false;

    // Start is called before the first frame update
    void Start()
    {
        rigid_m = GetComponent<Rigidbody2D>();

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.Find("Player").GetComponent<Collider2D>());
    }

    // Update is called once per frame
    void Update()
    {
        if (owner_m)
        {
            if(!hasChain_m && Vector3.Distance(transform.position, owner_m.transform.position) > chainPrefab_M.transform.lossyScale.y)
            {
                hasChain_m = true;
                GameObject chain = Instantiate<GameObject>(chainPrefab_M);
                chain.GetComponent<HookChainMovement>().leader_M = gameObject;
                chain.GetComponent<HookChainMovement>().SetOwner(owner_m);
            }

            switch (hookState_m)
            {
                case HookState.SHOOT: // shoot hook away from player toward target destination
                    transform.position = Vector3.Lerp(transform.position, destination_m, projectileSpeed_m * Time.deltaTime);
                    if (Vector3.Distance(transform.position, destination_m) < 0.1)
                    {
                        StartReel();
                    }
                    break;
                case HookState.REEL: // pull hook back to player and destroy hook when done
                    transform.position = Vector3.Lerp(reelStart_m, owner_m.transform.position, (Time.time - startReelTime_m) / reelTime_m);
                    if (Vector3.Distance(transform.position, owner_m.transform.position) < 0.1)
                    {
                        owner_m.GetComponent<ShootGrapple>().SetCanShoot(true);
                        GameObject.Destroy(gameObject);
                    }
                    break;
                case HookState.ATTACHED: // follow attached object being swung
                    Swing.SwingState swingState = attachedObject_m.GetComponent<Swing>().GetSwingState();
                    if (attachedObject_m && swingState != Swing.SwingState.NONE && swingState != Swing.SwingState.THROW)
                    {
                        transform.position = attachedObject_m.transform.position;
                    }
                    else
                    {
                        StartReel();
                    }
                    break;
            }
        }
    }

    private void ChainMovement()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attachedObject_m == null)
        {
            if (collision.gameObject.tag == "Throwable")
            {
                if (!collision.gameObject.GetComponent<Health>().IsArmored()) // can only grab unarmored targets
                {
                    // start swing sequence for object grabbed
                    Physics2D.IgnoreCollision(collision, GameObject.Find("Player").GetComponent<Collider2D>());
                    Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>());
                    attachedObject_m = collision.gameObject;
                    attachedObject_m.GetComponent<Swing>().SetSwingState(Swing.SwingState.PULL);
                    attachedObject_m.GetComponent<Swing>().SetHook(gameObject);
                    
                    hookState_m = HookState.ATTACHED;
                    GameObject.Find("Player").GetComponent<GrabThrow>().SetProjectile(attachedObject_m);
                }
                else
                {
                    // reel back hook if it hits armored target
                    StartReel();
                }
            }
            else
            {
                // reel back hook if it hits an unthrowable object
                StartReel();
            }
        }
    }

    public void StartReel()
    {
        hookState_m = HookState.REEL;
        reelStart_m = transform.position;
        startReelTime_m = Time.time;
    }

    /**************************************************** Set Functions *************************************************************/
    
    public void SetOwner(GameObject owner)
    {
        owner_m = owner;
    }

    public void SetDestination(Vector3 positionVector)
    {
        destination_m = positionVector;
    }

    public void SetProjectileSpeed(float speed)
    {
        projectileSpeed_m = speed;
    }

    public void SetReelTime(float time)
    {
        reelTime_m = time;
    }
    
    public void SetHookState(HookState state)
    {
        hookState_m = state;
    }

    public void SetHasChain(bool hasChain)
    {
        hasChain_m = hasChain;
    }

    /**************************************************** Get Functions *************************************************************/
    
    public GameObject GetOwner()
    {
        return owner_m;
    }
}
