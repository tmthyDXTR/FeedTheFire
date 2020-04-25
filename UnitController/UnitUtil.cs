using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUtil : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent _navAgent;

    public Vector3 moveTarget;
    private float distanceToTarget;

    void Start()
    {
        _navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public void StopMoving()
    {
        _navAgent.ResetPath();
        moveTarget = Vector3.zero;
    }

    public void MoveTo(Vector3 position)
    {
        moveTarget = position;
        _navAgent.SetDestination(position);      
    }

    public bool CheckTargetReached()
    {
        if (_navAgent.pathPending)
        {
            distanceToTarget = Vector3.Distance(transform.position, moveTarget);
        }
        else
        {
            distanceToTarget = _navAgent.remainingDistance;
        }
        if (distanceToTarget <= _navAgent.stoppingDistance)
        {
            moveTarget = Vector3.zero;
            return true;
        }
        return false;
    }
}
