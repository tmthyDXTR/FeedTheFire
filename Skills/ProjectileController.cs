using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectileController : MonoBehaviour
{
    public string name;
    public float damage;
    public float range;
    public float speed;



    public Vector3 targetDir = Vector3.zero;
    public GameObject targetObj = null;

    void Update()
    {
        // Move our position a step closer to the target.
        float step = speed * Time.deltaTime; // calculate distance to move
        
        if (targetObj != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetObj.transform.position, step);
        }
    }
}
