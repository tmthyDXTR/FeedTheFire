using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    private AttackUtil _attack;
    void Start()
    {
        _attack = this.transform.parent.gameObject.GetComponent<AttackUtil>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9) // 9 = Enemy unit
        {
            if (!_attack.inAttackRange.Contains(other.transform.parent.gameObject))
                _attack.inAttackRange.Add(other.transform.parent.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9) // 9 = Enemy unit
        {
            if (_attack.inAttackRange.Contains(other.transform.parent.gameObject))
                _attack.inAttackRange.Remove(other.transform.parent.gameObject);
        }
    }
}
