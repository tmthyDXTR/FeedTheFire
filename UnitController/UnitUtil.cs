using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUtil : MonoBehaviour
{
    #region Variables
    private UnityEngine.AI.NavMeshAgent _navAgent;
    private Animator _anim;

    [SerializeField]
    private Vector3 moveTarget;
    private float distanceToTarget;

    [SerializeField]
    private Vector3 jumpTarget;
    [SerializeField]
    private float jumpDistance = 5f;
    [SerializeField]
    private float jumpSpeed = 10f;

    #endregion
    void Start()
    {
        #region References
        _navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _anim = gameObject.GetComponent<Animator>();

        #endregion
    }

    public void StopMoving()
    {
        _navAgent.ResetPath();
        moveTarget = Vector3.zero;
        jumpTarget = Vector3.zero;
        _anim.SetBool("IsMoving", false);
        _anim.SetBool("IsAttacking", false);
        _anim.SetBool("IsJumping", false);
    }

    public void MoveToTarget()
    {
        _navAgent.SetDestination(moveTarget);
        _anim.SetBool("IsMoving", true);
        _anim.SetBool("IsAttacking", false);
        _anim.SetBool("IsJumping", false);
    }

    public bool CheckHasMoveTarget()
    {
        return moveTarget != Vector3.zero;
    }

    public void SetMoveTarget(Vector3 position)
    {
        moveTarget = position;
    }

    public void SetJumpTarget(Vector3 targetPosition)
    {
        var jumpDirection = (targetPosition - this.transform.position).normalized;
        jumpTarget = this.transform.position + jumpDirection * jumpDistance;        
    }

    public void JumpToTarget()
    {
        _navAgent.ResetPath();
        moveTarget = Vector3.zero;
        float step = jumpSpeed * Time.fixedDeltaTime; // calculate distance to move
        this.transform.position = Vector3.MoveTowards(this.transform.position, jumpTarget, step);
        this.transform.LookAt(jumpTarget);
        _anim.SetBool("IsMoving", false);
        _anim.SetBool("IsAttacking", false);
        _anim.SetBool("IsJumping", true);
    }

    public bool CheckJumpTargetReached()
    {
        return (Vector3.Distance(transform.position, jumpTarget) < 0.1f);    
    }

    public bool CheckTargetReached()
    {
        if (_navAgent.pathPending)
        {
            distanceToTarget = Vector3.Distance(this.transform.position, moveTarget);
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
