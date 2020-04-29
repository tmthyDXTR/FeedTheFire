using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUtil : MonoBehaviour
{
    #region Variables
    private UnityEngine.AI.NavMeshAgent _navAgent;
    private Animator _anim;
    private SelectionManager _select;


    [SerializeField]
    private Vector3 moveTarget;
    private float distanceToTarget;

    [SerializeField]
    private Vector3 jumpTarget;
    [SerializeField]
    private float jumpDistance = 5f;
    [SerializeField]
    private float jumpSpeed = 10f;
    [SerializeField]
    private bool jumpCollision = false;


    public Vector3 lastPosition;

    #endregion
    void Start()
    {
        #region References
        _navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _anim = gameObject.GetComponent<Animator>();
        _select = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();

        #endregion

    }

    public void StopMoving()
    {
        _navAgent.ResetPath();
        moveTarget = Vector3.zero;
        jumpTarget = Vector3.zero;
        _anim.SetBool("IsMoving", false);
        _anim.SetBool("isCasting", false);
        _anim.SetBool("IsJumping", false);
    }

    public void MoveToTarget()
    {
        _navAgent.SetDestination(moveTarget);
        _anim.SetBool("IsMoving", true);
        _anim.SetBool("isCasting", false);
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

    public void SetJumpTarget(Vector3 mousePos)
    {
        var jumpDirection = (mousePos - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
        jumpTarget = this.transform.position + jumpDirection * jumpDistance;
        _anim.SetBool("IsMoving", false);
        _anim.SetBool("isCasting", false);
        _anim.SetBool("IsJumping", true);
    }

    public void JumpToTarget()
    {
        this.transform.LookAt(jumpTarget);
        moveTarget = Vector3.zero;
        float step = jumpSpeed * Time.fixedDeltaTime; // calculate distance to move
        this.transform.position = Vector3.MoveTowards(this.transform.position, jumpTarget, step);
    }

    public void SetJumpCollision(bool value)
    {
        jumpCollision = value;
    }

    public bool CheckJumpCollision()
    {
        return jumpCollision;
    }

    public bool CheckIfStoppedMoving()
    {        
        if (this.transform.position != lastPosition)
        {
            lastPosition = this.transform.position;
            return false;
        }
        else
        {
            return true;
        }        
    }

    public void DrawJumpRay()
    {
        //Debug.Log(_select.GetMousePos());
        var jumpDirection = (_select.GetMousePos() - this.transform.position).normalized;
        Debug.DrawRay(this.transform.position, jumpDirection * jumpDistance);
    }

    public bool CheckJumpTargetReached()
    {
        return (Vector3.Distance(transform.position, jumpTarget) < .1f);    
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
