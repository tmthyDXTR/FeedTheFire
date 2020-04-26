using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Projectile")
        {
            var projectile = other.gameObject.GetComponent<Projectile>();
            Debug.Log(this.transform.parent.name + " hit by " + other.name + " for " + projectile.baseDamage);

            Destroy(other.gameObject);
        }
    }
}
