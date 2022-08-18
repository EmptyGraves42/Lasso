using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowDamage : MonoBehaviour
{
    public float repeatDmgTimer = 1.0f;
    public int swingDmgReduction_M = 2;

    private Health healthCmp_m;
    private Swing swingCmp_m;
    private Dictionary<int, float> dmgTimers_m = new Dictionary<int, float>();
    private List<int> dmgTimerKeys_m = new List<int>();
    

    // Start is called before the first frame update
    void Start()
    {
        healthCmp_m = GetComponent<Health>();
        swingCmp_m = GetComponent<Swing>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dmgTimers_m.Count > 0)
        {
            foreach (int key in dmgTimerKeys_m)
            {
                dmgTimers_m[key] -= Time.deltaTime;
            }
        }
    }
 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Health otherHealthCmp = collision.gameObject.GetComponent<Health>();
        
        if (otherHealthCmp && collision.gameObject.name != "Player" && swingCmp_m.GetSwingState() != Swing.SwingState.NONE)
        {
            int otherID = otherHealthCmp.GetInstanceID();

            if (!dmgTimers_m.ContainsKey(otherID))
            {
                dmgTimers_m.Add(otherID, 0.0f);
                dmgTimerKeys_m.Add(otherID);
            }

            if (dmgTimers_m[otherID] <= 0)
            {
                if (swingCmp_m.GetSwingState() == Swing.SwingState.THROW)
                {
                    healthCmp_m.TakeDamage(otherHealthCmp.TakeDamage(healthCmp_m.GetCurrentHealth()));

                    dmgTimers_m[otherID] = repeatDmgTimer;
                }
                else if (swingCmp_m.GetSwingState() == Swing.SwingState.REVOLVE)
                {
                    healthCmp_m.TakeDamage(otherHealthCmp.TakeDamage(healthCmp_m.GetMaxHealth() / swingDmgReduction_M));

                    dmgTimers_m[otherID] = repeatDmgTimer;
                }
            }
        }
    }
   
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            GetComponent<Collider2D>().isTrigger = false;
        }
    }
}
