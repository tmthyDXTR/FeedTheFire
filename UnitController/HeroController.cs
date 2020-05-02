﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HeroController : MonoBehaviour
{
    #region Variables
    private Selectable _selectable;
    private SelectionManager _select;
    private UnitUtil _unit;
    private Animator _anim;
    private CastUtil _cast;
    private SpellManager _spell;
    private UnitUIUtil _ui;

    [SerializeField] 
    private State state;
    private enum State
    {
        Idling,
        Moving,
        AutoCasting,
        Casting,
        Aiming,
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
        _spell = gameObject.GetComponent<SpellManager>();
        _ui = gameObject.GetComponent<UnitUIUtil>();

        #endregion

        #region EventListener 
        // When cast lock gets deactivated/after cast, switch to idle state,
        // and change back to RClick spell
        _cast.OnIsCastUnlocked += delegate (object sender, EventArgs e)
        {
            Debug.Log("Cast Unlocked");
            state = State.Idling;
            _spell.SelectSpell(0);
        };

        #endregion
    }

    void Update()
    {

        // Update Debug Log
        _unit.DrawJumpRay();


        #region Input handler
        // First check if this unit is selected
        if (_selectable.isSelected)
        {   // On Right click action
            if (Input.GetMouseButtonDown(1))
            {
                if (!_cast.IsCastLocked)
                {
                    // Detect right click target here
                    Debug.Log("Hero right click");
                    GameObject clickedObj = _select.GetClickedObject();
                    Vector3 clickedPos = _select.GetMousePos();
                    // If the right clicked object is not targetable
                    // If it is targetable, the check sets it as target object
                    if (_cast.CheckTargetTargetable(clickedObj, _spell.GetSelectedSpell()))
                    {
                        if (state == State.Aiming)
                        {
                            //Select RClick action
                            _select.SetSelectManagerActive(true);
                            _spell.SelectSpell(0);
                            _ui.RemoveArrow();
                            state = State.AutoCasting;
                        }
                    }
                    else
                    {
                        // Set the clicked position as new move target
                        _unit.SetMoveTarget(clickedPos);
                    }
                }
            }                             
            
            // Space key action
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!state.Equals(State.Jumping) && !_cast.IsCastLocked)
                {
                    Debug.Log("Jump");
                    _unit.SetJumpTarget(_select.GetMousePos());
                    _unit.lastPosition = this.transform.position;
                    _cast.StopCast();
                    state = State.Jumping;
                }
            }

            
            // Spell Selection
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _select.SetSelectManagerActive(false);
                _cast.StopCast();
                _spell.SelectSpell(1);
                state = State.Aiming;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _select.SetSelectManagerActive(false);
                _cast.StopCast();
                _spell.SelectSpell(2);
                state = State.Aiming;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _select.SetSelectManagerActive(false);
                _cast.StopCast();
                _spell.SelectSpell(3);
                state = State.Aiming;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _select.SetSelectManagerActive(false);
                _cast.StopCast();
                _spell.SelectSpell(4);
                state = State.Aiming;
            }


            // ESC Key
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _select.SetSelectManagerActive(true);
                _spell.SelectSpell(0);
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
                        _cast.StartCast(CastUtil.Type.AutoSpell, _spell.GetSelectedSpell());
                        state = State.AutoCasting;
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
                        _cast.StartCast(CastUtil.Type.AutoSpell, _spell.GetSelectedSpell());
                        state = State.AutoCasting;
                    }
                    // If it not in range, keep moving towards it
                    else
                    {
                        _unit.SetMoveTarget(_cast.castTarget.transform.position);
                    }
                }
                break;
            #endregion


            #region State AutoCasting
            case State.AutoCasting:
                // If unit has an cast target              
                if (_cast.CheckHasTarget())
                {
                    // and it is in casting range
                    if (_cast.CheckTargetInRange())
                    {
                        // Start casting
                        this.transform.LookAt(_cast.castTarget.transform);
                        StartCoroutine(_cast.StartCast(CastUtil.Type.AutoSpell, _spell.GetSelectedSpell()));
                        if (!_select.isActive)
                        {
                            _select.SetSelectManagerActive(true);
                        }
                    }
                    // If it is not in range, keep moving towards it
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


            #region State Aiming
            case State.Aiming:
                _ui.DrawArrow(new Vector3(this.transform.position.x, 0.1f, this.transform.position.z), _select.GetMousePos());
                
                if (_unit.CheckHasMoveTarget())
                {
                    _unit.MoveToTarget();
                }
                // If move target reached, stop moving
                if (_unit.CheckTargetReached())
                {
                    _unit.StopMoving();
                }

                // On Left click action
                if (Input.GetMouseButtonDown(0))
                {
                    // Detect left click target here
                    Debug.Log("Aim Try Trigger");
                    GameObject clickedObj = null;
                    if (_select.GetClickedObject() != null)
                        clickedObj = _select.GetClickedObject();                    
                        //Debug.Log("Hit: " + clickedObj.transform.parent.gameObject.name);

                    Vector3 clickedPos = _select.GetMousePos();
                    Debug.Log("Hit Vector: " + clickedPos);
                    // If the left click target is in range                                             

                    // Check what target info the spell needs
                    // Directional or object position
                    Spell spell = _spell.GetSelectedSpell();

                    // Check if spell needs a target object
                    if (spell.form.aim == Form.Aim.Auto)
                    {
                        // If it is targetable, the check sets it as target object
                        if (_cast.CheckTargetTargetable(clickedObj, spell))
                        {
                            Debug.Log("Cast Auto Target Spell");
                            this.transform.LookAt(_cast.castTarget.transform);
                            _unit.StopMoving();
                            // Start casting
                            StartCoroutine(_cast.StartCast(CastUtil.Type.SingleSpell, _spell.GetSelectedSpell()));
                            state = State.Casting;
                            _ui.RemoveArrow();
                        }
                        else
                        {
                            Debug.Log("Not Targetable");
                        }
                    }
                    else if (spell.form.aim == Form.Aim.Directional || spell.form.aim == Form.Aim.Point)
                    {
                        Debug.Log("Cast Directional Spell");
                        var castDir = clickedPos;
                        _cast.castDir = castDir;
                        this.transform.LookAt(clickedPos);
                        _unit.StopMoving();
                        // Start casting
                        StartCoroutine(_cast.StartCast(CastUtil.Type.SingleSpell, _spell.GetSelectedSpell()));
                        state = State.Casting;
                        _ui.RemoveArrow();
                    }
                }


                break;
            #endregion


            #region State Casting
            case State.Casting:
                // If unit has move target
                
                if (_unit.CheckHasMoveTarget() && !_cast.IsCastLocked)
                {
                    _cast.StopCast();
                    state = State.Moving;
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
