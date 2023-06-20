using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehavior : MonoBehaviour
{
    public float moveSpeed_M;
    public float rotationSpeed_M;

    private GameObject target_m;
    private GameObject source_m;
    private float spawnTime_m;

    // Start is called before the first frame update
    void Start()
    {
        target_m = GameObject.Find("Player");
        spawnTime_m = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - spawnTime_m > 0.5f)
        {
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                if (!collider.isTrigger && source_m)
                {
                    Physics2D.IgnoreCollision(collider, source_m.GetComponent<Collider2D>(), false);
                }
            }
        }

        if (GetComponent<Swing>().GetSwingState() == Swing.SwingState.NONE)
        {
            Vector3 dir = (target_m.transform.position - transform.position).normalized;
            Quaternion desiredRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSpeed_M * Time.deltaTime);
            
            GetComponent<Rigidbody2D>().velocity = transform.right * moveSpeed_M;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<ExplosionAttack>().Explode();
    }

    public void SetSource(GameObject source)
    {
        source_m = source;
    }
}
