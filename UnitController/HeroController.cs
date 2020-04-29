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
    private CastUtil _cast;
    private SkillManager _skill;

    [SerializeField] 
    private State state;
    private enum State
    {
        Idling,
        Moving,
        Casting,
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
        _cast = gameObject.GetComponent<CastUtil>();
        _skill = gameObject.GetComponent<SkillManager>();

        #endregion
    }

    void Update()
    {

        // Update Debug Log
        _unit.DrawJumpRay();


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
                // If it is attackable, the check sets it as attack target
                if (!_cast.CheckTargetTargetable(clickedObj))
                {
                    // Set the clicked position as new move target
                    _unit.SetMoveTarget(clickedPos);
                }
            }
            // Space key action
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!state.Equals(State.Jumping))
                {
                    Debug.Log("Jump");
                    _unit.SetJumpTarget(_select.GetMousePos());
                    _unit.lastPosition = this.transform.position;
                    _cast.StopCast();
                    state = State.Jumping;
                }
            }

            // Skill Selection
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _skill.SelectSkill(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _skill.SelectSkill(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _skill.SelectSkill(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _skill.SelectSkill(4);
            }
            #endregion
        }

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
                // If unit has an cast target                
                if (_cast.CheckHasTarget())
                {
                    // and it is in casting range
                    if (_cast.CheckTargetInRange())
                    {
                        _cast.StartCast();
                        state = State.Casting;
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
                // If unit has an cast target                
                if (_cast.CheckHasTarget())
                {
                    // and it is in casting range
                    if (_cast.CheckTargetInRange())
                    {
                        _unit.StopMoving();
                        _cast.StartCast();
                        state = State.Casting;
                    }
                    // If it not in range, keep moving towards it
                    else
                    {
                        _unit.SetMoveTarget(_cast.castTarget.transform.position);
                    }
                }
                break;
            #endregion


            #region State Casting
            case State.Casting:
                // If unit has an cast target              
                if (_cast.CheckHasTarget())
                {
                    // and it is in casting range
                    if (_cast.CheckTargetInRange())
                    {
                        // Start casting
                        this.transform.LookAt(_cast.castTarget.transform);
                        StartCoroutine(_cast.StartCast());
                    }
                    // If it not in range, keep moving towards it
                    else
                    {
                        state = State.Moving;
                    }
                }
                // If unit has move target
                else if (_unit.CheckHasMoveTarget())
                {
                    _cast.StopCast();
                    state = State.Moving;
                }
                // If unit has no cast and move target
                else
                {
                    _cast.StopCast();
                    state = State.Idling;
                }
                break;
            #endregion


            #region State Jumping
            case State.Jumping:
                _unit.JumpToTarget();
                // If jump target reached stop the jump
                if (_unit.CheckJumpTargetReached())
                {
                    _unit.StopMoving();
                    state = State.Idling;
                }
                // or if unit hits obstacle during jump
                if (_unit.CheckJumpCollision())
                {
                    _unit.StopMoving();
                    _unit.SetJumpCollision(false);
                    state = State.Idling;
                }
                // *** Detection Hotfix if unit got stuck on an obstacle ***
                if (_unit.CheckIfStoppedMoving())
                {
                    _unit.StopMoving();
                    state = State.Idling;
                }
                break;
            #endregion
        }
    }
}
