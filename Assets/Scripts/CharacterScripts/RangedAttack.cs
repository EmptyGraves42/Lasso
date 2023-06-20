using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    public GameObject projectilePrefab_M;
    public float attackSpeed_M;
    public float attackDistance_M;
    public float rotationSpeed_M;

    private GameObject player_m;
    private float attackTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        player_m = GameObject.Find("Player");
        GetComponent<Rigidbody2D>().angularVelocity = rotationSpeed_M;
    }

    // Update is called once per frame
    void Update()
    {
        if (player_m)
        {
            // line of sight and distance check
            RaycastHit2D castResult = Physics2D.Raycast(transform.position, player_m.transform.position - transform.position, attackDistance_M, 1 << LayerMask.NameToLayer("Enemy Sight"));
            if (castResult.collider != null && castResult.collider.gameObject == player_m &&
                Vector3.Distance(player_m.transform.position, transform.position) < attackDistance_M)
            {
                // rotate to face target
                Vector3 dir = (player_m.transform.position - transform.position).normalized;
                Quaternion desiredRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSpeed_M * Time.deltaTime);

                attackTimer += Time.deltaTime;

                if (attackTimer >= attackSpeed_M)
                {
                    attackTimer = attackTimer - attackSpeed_M;

                    GameObject projectile = Instantiate<GameObject>(projectilePrefab_M);
                    projectile.transform.position = transform.position;
                    projectile.transform.rotation = transform.rotation;
                    projectile.GetComponent<MissileBehavior>().SetSource(gameObject);

                    Collider2D[] colliders = projectile.GetComponents<Collider2D>();
                    foreach (Collider2D collider in colliders)
                    {
                        if (!collider.isTrigger)
                        {
                            Physics2D.IgnoreCollision(collider, GetComponent<Collider2D>());
                        }
                    }
                }

            }
            else
            {
                attackTimer = 0.0f;
            }
        }
    }
}
