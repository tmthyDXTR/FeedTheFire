using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectileController : MonoBehaviour
{
    #region Variables
    private float range;
    private float speed;
    private float size;
    [SerializeField]
    private float distanceTraveled = 0.0001f;
    private Vector3 lastPosition;
    public Vector3 targetDir = Vector3.zero;
    public GameObject targetObj = null;

    #endregion

    void Start()
    {
        lastPosition = this.transform.position;
    }


    void Update()
    {
        // Move our position a step closer to the target.
        float step = speed * Time.deltaTime; // calculate distance to move
        
        if (targetObj != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetObj.transform.position, step);          
        }

        if (CheckRange())
        {
            // Destroy this Projectile
            GameObject.Destroy(this.gameObject);
        }
    }

    public void AdjustProjectile(float range, float speed)
    {
        this.speed = speed;
        this.range = range;
    }

    public bool CheckRange()
    {
        // Returns true if distance traveled is greater than the projectile range
        distanceTraveled += Vector3.Distance(this.transform.position, lastPosition);
        lastPosition = this.transform.position;
        return (distanceTraveled > range);
    }
}
