using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootGrapple : MonoBehaviour
{
    public GameObject hookPrefab_M;
    public float projectileSpeed_M = 100;
    public float projectileDistance_M = 100;
    public float reelTime_M = 1.0f;

    private GameObject player_m;
    private Rigidbody2D rigid_m;
    private bool canShoot_m = true;

    // Start is called before the first frame update
    void Start()
    {
        player_m = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && canShoot_m)
        {
            canShoot_m = false;
            Quaternion rotaion = transform.rotation;
            rotaion *= Quaternion.Euler(0, 0, 90);
            GameObject hook = Instantiate(hookPrefab_M, transform.position, rotaion);

            hook.GetComponent<HookMovement>().SetProjectileSpeed(projectileSpeed_M);
            
            Vector3 destination = GetMousePos() - player_m.transform.position;
            destination.z = 0;
            destination.Normalize();
            destination *= projectileDistance_M;
            destination += transform.position;
            hook.GetComponent<HookMovement>().SetDestination(destination);

            hook.GetComponent<HookMovement>().SetOwner(gameObject);

            hook.GetComponent<HookMovement>().SetReelTime(reelTime_M);
        }
    }

    public Vector3 GetMousePos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        return mousePos;
    }

    /**************************************************** Set Functions *************************************************************/

    public void SetCanShoot(bool canShoot)
    {
        canShoot_m = canShoot;
    }
}
