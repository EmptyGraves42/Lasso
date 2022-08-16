using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public GameObject healthBar_M;
    public float invincibilityFrameTime_M = 0.5f;
    public int maxHealth_M = 5;
    public int maxArmor_M = 0;

    private float timer_m = 0;
    private int currentHealth_m;
    private int currentArmor_m;
    private bool invincible_m = false;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth_m = maxHealth_M;
        currentArmor_m = maxArmor_M;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer_m > 0 && invincible_m == true)
        {
            timer_m -= Time.deltaTime;
        }
        else
        {
            invincible_m = false;
        }
    }

    public int TakeDamage(int damage)
    {
        if(invincible_m)
        {
            return 0;
        }

        int returnDamage = 0;

        if(currentArmor_m + currentHealth_m >= damage)
        {
            returnDamage = damage;
        }
        else
        {
            returnDamage = damage - (currentHealth_m + currentArmor_m);
        }

        if(currentArmor_m >= damage)
        {
            currentArmor_m -= damage;
        }
        else
        {
            damage -= currentArmor_m;
            currentArmor_m = 0;

            currentHealth_m -= damage;
        }

        if(currentHealth_m <= 0 && currentArmor_m <= 0)
        {
            Destroy(gameObject);
        }

        if (healthBar_M)
        {
            healthBar_M.GetComponent<HealthBar>().UpdateBars(maxHealth_M, currentHealth_m, maxArmor_M, currentArmor_m);
        }

        invincible_m = true;
        timer_m = invincibilityFrameTime_M;

        return returnDamage;
    }

    public bool IsArmored()
    {
        if(currentArmor_m > 0)
        {
            return true;
        }
        
        return false;
    }

    /**************************************************** Get Functions *************************************************************/

    public int GetCurrentHealth()
    {
        return currentHealth_m;
    }
}
