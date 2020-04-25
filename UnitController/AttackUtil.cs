using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackUtil : MonoBehaviour
{
    private SelectionManager _select;

    public List<GameObject> inAttackRange = new List<GameObject>();
    public GameObject attackTarget;

    void Start()
    {
        _select = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
    }

    public void CheckIfEnemyUnit()
    {
        var clickedObj = _select.GetClickedObject();
        if (clickedObj.layer == 9) // 9 = Enemy unit
        {
            attackTarget = clickedObj.transform.parent.gameObject;
        }
        else
        {
            attackTarget = null;
        }
    }

    public bool CheckIfAttackTargetInRange()
    {
        if (attackTarget != null && inAttackRange.Contains(attackTarget))
        {
            return true;
        }
        return false;
    }
}
