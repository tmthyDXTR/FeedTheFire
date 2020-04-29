using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Projectile")
        {
            var _effect = other.transform.parent.gameObject.GetComponent<Effect>();
            var _projectile = other.transform.parent.gameObject.GetComponent<ProjectileController>();
            Debug.Log(this.transform.parent.name + " hit by " + _effect.name + " for " + _projectile.damage);

            Destroy(other.transform.parent.gameObject);
        }
    }
}
