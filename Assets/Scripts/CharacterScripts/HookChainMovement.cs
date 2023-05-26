using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookChainMovement : MonoBehaviour
{
    public float spawnDistance_M = 0.25f;

    private GameObject leader_m;
    private GameObject follower_m;
    private GameObject owner_m;
    private bool hasFollower_m = false;
    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (leader_m)
        {
            Vector3 dir = (owner_m.transform.position - leader_m.transform.position).normalized;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            transform.position = leader_m.transform.position + (dir * transform.lossyScale.y);

            // Despawning chain links
            if (Vector3.Distance(transform.position, owner_m.transform.position) < transform.lossyScale.y * spawnDistance_M ||
                Vector3.Distance(transform.position, owner_m.transform.position) > Vector3.Distance(owner_m.transform.position, leader_m.transform.position))
            {
                if (leader_m.GetComponent<HookChainMovement>())
                {
                    leader_m.GetComponent<HookChainMovement>().hasFollower_m = false;
                }
                else
                {
                    leader_m.GetComponent<HookMovement>().SetHasChain(false);
                }

                GameObject.Destroy(gameObject);
            }
            // Spawning chain links
            else if (!hasFollower_m && Vector3.Distance(transform.position, owner_m.transform.position) > transform.lossyScale.y * spawnDistance_M)
            {
                hasFollower_m = true;
                GameObject chain = Instantiate<GameObject>(gameObject);
                chain.GetComponent<HookChainMovement>().leader_m = gameObject;
                chain.GetComponent<HookChainMovement>().SetOwner(owner_m);
            }
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

    private void CreateFollower()
    {
        follower_m = Instantiate(gameObject);
        follower_m.GetComponent<HookChainMovement>().leader_m = gameObject;
        follower_m.GetComponent<HookChainMovement>().SetOwner(owner_m);

        hasFollower_m = true;
    }

    /**************************************************** Get Functions *************************************************************/


    /**************************************************** Set Functions *************************************************************/
    
    public void SetOwner(GameObject owner)
    {
        owner_m = owner;
    }

    public void SetLeader(GameObject leader)
    {
        leader_m = leader;
    }

    public void RemoveFollower()
    {
        hasFollower_m = false;
    }
}
