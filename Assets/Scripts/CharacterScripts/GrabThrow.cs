using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabThrow : MonoBehaviour
{
    private GameObject projectile_m = null;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(projectile_m && projectile_m.GetComponent<Swing>().GetSwingState() == Swing.SwingState.REVOLVE)
            {
                projectile_m.GetComponent<Swing>().SetSwingState(Swing.SwingState.WINDUP);
                projectile_m.GetComponent<Collider2D>().isTrigger = true;

                Vector3 direction = transform.position - GetMousePos();
                direction.z = 0;
                direction.Normalize();
                direction *= 2;

                projectile_m.GetComponent<Swing>().SetDirection(0, -direction.y, direction.x);
                projectile_m.GetComponent<Swing>().SetDirection(1, -direction.x, -direction.y);
                projectile_m.GetComponent<Swing>().SetDirection(2, direction.y, -direction.x);
                projectile_m.GetComponent<Swing>().SetDirection(3, direction);

                projectile_m.GetComponent<Swing>().SetWindupInitial(projectile_m.transform.position - transform.position);
                projectile_m.GetComponent<Swing>().SetStartTime(Time.time);
                projectile_m.GetComponent<Swing>().SetDirectionIndex(calculateQuadrant());
            }
        }
    }

    public Vector3 GetMousePos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        return mousePos;
    }

    int calculateQuadrant()
    {
        if (projectile_m)
        {
            Swing projectileSwingCmp = projectile_m.GetComponent<Swing>();

            if (lineCheck(transform.position, 
                          transform.position + projectileSwingCmp.GetDirection(3), 
                          projectile_m.transform.position))
            {
                if (lineCheck(transform.position, 
                              transform.position + projectileSwingCmp.GetDirection(0), 
                              projectile_m.transform.position))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (lineCheck(transform.position, 
                              transform.position + projectileSwingCmp.GetDirection(2), 
                              projectile_m.transform.position))
                {
                    return 3;
                }
                else
                {
                    return 2;
                }
            }
        }

        return -1;
    }

    // returns true if point is to the left of or on line ab
    bool lineCheck(Vector3 a, Vector3 b, Vector3 point)
    {
        if((point.y - a.y) * (b.x - a.x) - (point.x - a.x) * (b.y - a.y) >= 0.0f)
        {
            return true;
        }

        return false;
    }

    /**************************************************** Set Functions *************************************************************/

    public void SetProjectile(GameObject projectile)
    {
        projectile_m = projectile;
    }
}
