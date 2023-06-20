using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public float explosionSpeed_M;
    public Color finalColor_M;

    private float radius_m;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float diameter = radius_m * 2;
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(diameter, diameter), explosionSpeed_M * Time.deltaTime);
        GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, finalColor_M, explosionSpeed_M * Time.deltaTime);
        
        if(transform.localScale.x > diameter - 0.1)
        {
            Destroy(gameObject);
        }
    }

    public void SetRadius(float radius)
    {
        radius_m = radius;
    }
}
