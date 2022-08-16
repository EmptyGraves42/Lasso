using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image barBack_M;
    public Image healthBarImage_M;
    public Image armorBarImage_M;
    public CanvasGroup canvasGroup_M;
    public float healthScale_M = 0.5f;
    public float fadeTime_M = 10.0f;

    private Health healthCmp_m;
    private float fadeStartTime_m;
    private bool damageTaken_m = false;

    // Start is called before the first frame update
    void Start()
    {
        healthCmp_m = GetComponentInParent<Health>();
        transform.rotation = Quaternion.identity;

        // maybe remove code below for efficency if canvas alpha starts at 0
        if (healthCmp_m.maxHealth_M > 0)
        {
            healthBarImage_M.fillAmount = 1.0f;
        }
        else
        {
            healthBarImage_M.fillAmount = 0.0f;
        }

        if(healthCmp_m.maxArmor_M > 0)
        {
            armorBarImage_M.fillAmount = 1.0f;
        }
        else
        {
            armorBarImage_M.fillAmount = 0.0f;
        }

        ArrangeBars(healthCmp_m.maxHealth_M, healthCmp_m.maxArmor_M);
    }  

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;

        if (damageTaken_m)
        {
            canvasGroup_M.alpha = Mathf.Lerp(1, 0.25f, (Time.time - fadeStartTime_m) / fadeTime_M);
        }
    }

    private void ShowHealthAndArmor()
    {
        // scale health bar hight
        Vector3 barScale = barBack_M.transform.localScale;
        barScale.y *= healthScale_M;
        healthBarImage_M.transform.localScale = barScale;
        
        // position health bar
        Vector3 barPosition = barBack_M.transform.localPosition;
        barPosition.y += ((barBack_M.transform.localScale.y / 2.0f) - barScale.y + (barScale.y / 2.0f));
        healthBarImage_M.transform.localPosition = barPosition;

        // scale armor bar hight
        barScale = barBack_M.transform.localScale;
        barScale.y *= 1 - healthScale_M;
        armorBarImage_M.transform.localScale = barScale;

        // position armor bar
        barPosition = barBack_M.transform.localPosition;
        barPosition.y -= ((barBack_M.transform.localScale.y / 2.0f) - barScale.y + (barScale.y / 2.0f));
        armorBarImage_M.transform.localPosition = barPosition;
    }

    private void ShowHealthOrArmor()
    {
        healthBarImage_M.transform.localScale = barBack_M.transform.localScale;
        healthBarImage_M.transform.localPosition = barBack_M.transform.localPosition;

        armorBarImage_M.transform.localScale = barBack_M.transform.localScale;
        armorBarImage_M.transform.localPosition = barBack_M.transform.localPosition;
    }

    public  void UpdateBars(float maxHealth, float currentHealth, float maxArmor, float currentArmor)
    {
        canvasGroup_M.alpha = 1;
        fadeStartTime_m = Time.time;
        damageTaken_m = true;

        if(currentHealth <= 0)
        {
            healthBarImage_M.fillAmount = 0.0f;
        }
        else
        {
            healthBarImage_M.fillAmount = Mathf.Clamp(currentHealth / maxHealth, 0.0f, 1.0f);
        }
        
        if(currentArmor <= 0)
        {
            armorBarImage_M.fillAmount = 0.0f;
        }
        else
        {
            armorBarImage_M.fillAmount = Mathf.Clamp(currentArmor / maxArmor, 0.0f, 1.0f);
        }

        ArrangeBars(currentHealth, currentArmor);
    }

    public void ArrangeBars(float currentHealth, float currentArmor)
    {
        if (currentHealth > 0 && currentArmor > 0)
        {
            ShowHealthAndArmor();
        }
        else
        {
            ShowHealthOrArmor();
        }
    }
}
