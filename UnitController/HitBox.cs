﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fire")
        {
            var _spell = other.gameObject.GetComponent<SpellUtil>();
            Debug.Log(this.transform.parent.name + " hit by " + _spell.name + " for " + _spell.power + " " + _spell.effect.type);

            Destroy(other.gameObject);
        }

        if (other.tag == "Shadow")
        {
            var _spell = other.gameObject.GetComponent<SpellUtil>();
            Debug.Log(this.transform.parent.name + " hit by " + _spell.name + " for " + _spell.power + " " + _spell.effect.type);

            Destroy(other.gameObject);
        }
    }
}
