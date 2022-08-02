using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowDamage : MonoBehaviour
{
    public int swingDmg_M = 2;

    private Health healthCmp_m;
    private Swing swingCmp_m;

    // Start is called before the first frame update
    void Start()
    {
        healthCmp_m = GetComponent<Health>();
        swingCmp_m = GetComponent<Swing>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Health otherHealthCmp = collision.gameObject.GetComponent<Health>();
        if (otherHealthCmp)
        {
            if (swingCmp_m.GetSwingState() == Swing.SwingState.THROW || swingCmp_m.GetSwingState() == Swing.SwingState.PULL)
            {
                healthCmp_m.TakeDamage(otherHealthCmp.TakeDamage(healthCmp_m.GetCurrentHealth()));
            }
            else if (swingCmp_m.GetSwingState() != Swing.SwingState.NONE)
            {
                healthCmp_m.TakeDamage(otherHealthCmp.TakeDamage(swingDmg_M));
            }
        }
    }
}
