using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehavior : MonoBehaviour
{
    public float moveSpeed_M;
    public float rotationSpeed_M;

    private GameObject target_m;

    // Start is called before the first frame update
    void Start()
    {
        target_m = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = (target_m.transform.position - transform.position).normalized;
        Quaternion desiredRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSpeed_M);

        GetComponent<Rigidbody2D>().velocity = dir * moveSpeed_M;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<ExplosionAttack>().Explode();
    }

    
}
