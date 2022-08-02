using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject spawnObjectPrefab_M = null;
    public float spawnTime_M = 0.0f;
    public bool waitTillDestroyed_M = false;

    private GameObject spawnedObject_m;
    private Transform spawningTransform_m;
    private float timer_m = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(spawnObjectPrefab_M)
        {
            spawningTransform_m = spawnObjectPrefab_M.transform;
            spawningTransform_m.position = new Vector3(transform.position.x, 
                                                     transform.position.y, 
                                                     spawnObjectPrefab_M.transform.position.z);
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnObjectPrefab_M)
        {
            if ((spawnedObject_m == null || 
                (!waitTillDestroyed_M && Vector3.Distance(transform.position, spawnedObject_m.transform.position) > 1.5f)))
            {
                if (timer_m >= spawnTime_M)
                {
                    timer_m = 0.0f;

                    
                    spawnedObject_m = Instantiate(spawnObjectPrefab_M, transform);
                    spawnedObject_m.transform.position = spawningTransform_m.position;
                    
                }

                timer_m += Time.deltaTime;
            }
        }
    }

}
