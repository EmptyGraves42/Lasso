using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float Speed = 100;
    [HideInInspector] public Vector2 force = Vector2.zero;
    private Rigidbody2D rigid;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        force = Vector2.zero;
        
        if(Input.GetKey(KeyCode.W))
        {
            force.y = 1.0f;
        }

        if(Input.GetKey(KeyCode.S))
        {
            force.y = -1.0f;
        }
        
        if(Input.GetKey(KeyCode.A))
        {
            force.x = -1.0f;
        }

        if(Input.GetKey(KeyCode.D))
        {
            force.x = 1.0f;
        }

        force.Normalize();
        force *= Speed * Time.deltaTime;

        rigid.AddForce(force);

        Vector3 mouse = GetMousePos();
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(mouse.y - transform.position.y, mouse.x - transform.position.x) * Mathf.Rad2Deg - 90);
    }

    public Vector3 GetMousePos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        return mousePos;
    }
}
