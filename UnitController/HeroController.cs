using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    private Selectable _selectable;
    private SelectionManager _select;
    private UnitUtil _unit;
    private Animator _anim;
    private AttackUtil _attack;


    [SerializeField] private State state;
    private enum State
    {
        Idling,
        Moving,
        Attacking,
        Strafing,
        Stunned,
        Dead,
    }


    void Start()
    {
        _select = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
        _selectable = gameObject.GetComponent<Selectable>();
        _unit = gameObject.GetComponent<UnitUtil>();
        _anim = gameObject.GetComponent<Animator>();
        _attack = gameObject.GetComponent<AttackUtil>();
    }

    void Update()
    {
        // First check if this unit is selected
        if (_selectable.isSelected == true)
        {   // Enemy Right clicked
            if (Input.GetMouseButtonDown(1)) 
            {
                _attack.CheckIfEnemyUnit();
                _unit.MoveTo(_select.GetMousePos());
            }
        }       

        // State Machine
        switch (state)
        {
            case State.Idling:
                // If unit has a target
                if (_unit.moveTarget != Vector3.zero)
                {                    
                    _anim.SetBool("IsMoving", true);
                    state = State.Moving;
                }

                if (_attack.CheckIfAttackTargetInRange())
                {
                    _anim.SetBool("IsAttacking", true);
                    state = State.Attacking;
                }
                break;

            case State.Moving:
                if (_unit.CheckTargetReached())
                {
                    _anim.SetBool("IsMoving", false);
                    state = State.Idling;
                }

                if (_attack.CheckIfAttackTargetInRange())
                {
                    _anim.SetBool("IsAttacking", true);
                    _anim.SetBool("IsMoving", false);
                    _unit.StopMoving();
                    state = State.Attacking;
                }
                break;

            case State.Attacking:
                if (_unit.moveTarget != Vector3.zero)
                {
                    _anim.SetBool("IsAttacking", false);
                    _anim.SetBool("IsMoving", true);
                    state = State.Moving;
                }
                break;
        }
    }
}
