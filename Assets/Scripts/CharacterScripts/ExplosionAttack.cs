using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAttack : MonoBehaviour
{
    public GameObject explosion_M;
    public int damage_M = 5;

    private List<GameObject> objects_m = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Explode()
    {
        GameObject explosion = Instantiate<GameObject>(explosion_M);
        explosion.transform.position = transform.position;
        explosion.GetComponent<ExplosionEffect>().SetRadius(GetComponent<CircleCollider2D>().radius);

        for(int i = objects_m.Count - 1; i >= 0 && objects_m.Count > 0; --i)
        {
            GameObject otherObject = objects_m[i];
            objects_m.RemoveAt(i);
            otherObject.GetComponent<Health>().TakeDamage(damage_M);
        }

        Swing swingCmp = GetComponent<Swing>();
        if (swingCmp != null && swingCmp.GetHook() != null)
        {
            swingCmp.GetHook().GetComponent<HookMovement>().StartReel();
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Health>() != null)
        {
            objects_m.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Health>() != null)
        {
            objects_m.Remove(collision.gameObject);
        }
    }
}
