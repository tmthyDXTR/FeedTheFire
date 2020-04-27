using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    private UnitUtil _unit;
    private string hitBoxTag = "HitBox";

    void Start()
    {
        _unit = this.transform.parent.gameObject.GetComponent<UnitUtil>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == hitBoxTag)
        {
            Debug.Log("Jump Collision");
            _unit.SetJumpCollision(true);
        }        
    }
}
