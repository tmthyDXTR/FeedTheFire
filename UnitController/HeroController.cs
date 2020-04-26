using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    #region Variables
    private Selectable _selectable;
    private SelectionManager _select;
    private UnitUtil _unit;
    private Animator _anim;
    private AttackUtil _attack;


    [SerializeField] 
    private State state;
    private enum State
    {
        Idling,
        Moving,
        Attacking,
        Jumping,
        Stunned,
        Dead,
    }

    #endregion


    void Start()
    {
        #region References

        _select = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
        _selectable = gameObject.GetComponent<Selectable>();
        _unit = gameObject.GetComponent<UnitUtil>();
        _anim = gameObject.GetComponent<Animator>();
        _attack = gameObject.GetComponent<AttackUtil>();


        #endregion
    }

    void Update()
    {
        #region Input handler
        // First check if this unit is selected
        if (_selectable.isSelected)
        {   // Right click action
            if (Input.GetMouseButtonDown(1)) 
            {
                // Detect right click object here
                Debug.Log("Hero right click");
                GameObject clickedObj = _select.GetClickedObject();
                Vector3 clickedPos = _select.GetMousePos();
                // If the right clicked object is not attackable
                // If it is attackable, it is set as attack target
                if (!_attack.CheckTargetAttackable(clickedObj))
                {
                    // Set the clicked position as new move target
                    _unit.SetMoveTarget(clickedPos);
                }
            }
            // Space key action
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Jump");
                _unit.SetJumpTarget(_select.GetMousePos());
                _attack.StopAttack();
                state = State.Jumping;
            }
        }
        #endregion

        // State Machine
        switch (state)
        {
            #region State Idling
            case State.Idling:
                // If unit has a move target
                if (_unit.CheckHasMoveTarget())
                {
                    _unit.MoveToTarget();
                    state = State.Moving;
                }
                // If unit has an attack target and
                // it is in attack range
                if (_attack.CheckHasAttackTarget())
                {
                    if (_attack.CheckAttackTargetInRange())
                    {
                        _attack.Attack();
                        state = State.Attacking;
                    }
                    // If it is not in range, move towards it
                    else
                    {
                        state = State.Moving;
                    }
                }                               
                break;
            #endregion


            #region State Moving
            case State.Moving:
                // Update move target
                _unit.MoveToTarget();
                // If move target reached, stop moving
                if (_unit.CheckTargetReached())
                {
                    _unit.StopMoving();
                    state = State.Idling;
                }

                // If unit has an attack target and
                // it is in attack range
                if (_attack.CheckHasAttackTarget())
                {
                    if (_attack.CheckAttackTargetInRange())
                    {
                        _unit.StopMoving();
                        _attack.Attack();
                        state = State.Attacking;
                    }
                    // If it not in rage, keep moving towards it
                    else
                    {
                        _unit.SetMoveTarget(_attack.attackTarget.transform.position);
                    }
                }
                break;
            #endregion


            #region State Attacking
            case State.Attacking:
                // If unit has an attack targe               
                if (_attack.CheckHasAttackTarget())
                {
                    // and it is in attack range
                    if (_attack.CheckAttackTargetInRange())
                    {
                        // Start attacking
                        this.transform.LookAt(_attack.attackTarget.transform);
                        StartCoroutine(_attack.Attack());
                    }
                    else
                    {
                        _attack.StopAttack();
                        state = State.Moving;
                    }
                }
                // If unit has move target
                else if (_unit.CheckHasMoveTarget())
                {
                    _attack.StopAttack();
                    state = State.Moving;
                }
                // If unit has no attack an move target
                else
                {
                    _attack.StopAttack();
                    state = State.Idling;
                }
                break;
            #endregion


            #region State Jumping
            case State.Jumping:
                _unit.JumpToTarget();
                // If unit hits obstacle during jump
                // stop the jump


                // If jump target reached
                if (_unit.CheckJumpTargetReached())
                {
                    _unit.StopMoving();
                    state = State.Idling;
                }                
                break;
            #endregion
        }
    }
}
