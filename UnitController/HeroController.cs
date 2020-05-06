using System.Collections;
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
        {
            HeroInputs();
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
                // If unit has an cast target                
                if (_cast.CheckHasTarget())
                {
                    // and it is in casting range
                    if (_cast.CheckTargetInRange(_spell.GetSelectedSpell(), 0))
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
                    if (_cast.CheckTargetInRange(_spell.GetSelectedSpell(), 0))
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
                    if (_cast.CheckTargetInRange(_spell.GetSelectedSpell(), 0))
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
                // Right click movement while aiming is still possible
                if (_unit.CheckHasMoveTarget())
                {
                    _unit.MoveToTarget();
                }
                // If move target reached, stop moving
                if (_unit.CheckTargetReached())
                {
                    _unit.StopMoving();
                }

                Spell spell = _spell.GetSelectedSpell();

                // UI Draw / Cast range, aim indicators
                if (spell.form.type == Form.Type.Projectile)
                    _ui.DrawUI(spell, _cast.GetProjectileRange());
                else
                    _ui.DrawUI(spell, _cast.GetAreaRange());

                var distanceMouse = Vector3.Distance(_unit.GetCorrectPosition(), _select.GetMousePos());
                // On Left click action
                if (Input.GetMouseButtonDown(0))
                {
                    // Detect left click target here
                    Debug.Log("Aim Try Trigger");
                    if (!_cast.CheckTargetInRange(spell, distanceMouse))
                    {
                        Debug.Log("Out of cast range");
                    }
                    // If the left click target is in range                                             
                    else
                    {
                        if (!_cast.isWaitingForCoolDown)
                        {
                            GameObject clickedObj = null;
                            if (_select.GetClickedObject() != null)
                                clickedObj = _select.GetClickedObject();
                            //Debug.Log("Hit: " + clickedObj.transform.parent.gameObject.name);

                            Vector3 clickedPos = _select.GetMousePos();
                            Debug.Log("Hit Vector: " + clickedPos);

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
                                    StartCoroutine(_cast.StartCast(CastUtil.Type.SingleSpell, spell));
                                    state = State.Casting;
                                    _ui.RemoveUnitDraws();
                                    _select.SetSelectManagerActive(true);
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
                                StartCoroutine(_cast.StartCast(CastUtil.Type.SingleSpell, spell));
                                state = State.Casting;
                                _ui.RemoveUnitDraws();
                                _select.SetSelectManagerActive(true);
                            }
                        }                                               
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



    /// <summary>
    /// Hero Helper methods for hero specific mouse input,
    /// </summary>
    private void HeroInputs()
    {
        // On Right click action
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
                        _ui.RemoveUnitDraws();
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
                _ui.RemoveUnitDraws();
                state = State.Jumping;
            }
        }

        // ESC Key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _ui.RemoveUnitDraws();
            _select.SetSelectManagerActive(true);
            _spell.SelectSpell(0);
            state = State.Idling;
        }

        #region KeyCode Spell selection
        // Spell Selection
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HeroSelectSpell(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HeroSelectSpell(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            HeroSelectSpell(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            HeroSelectSpell(4);
        }
        #endregion
    }


    private void HeroSelectSpell(int slot)
    {
        if (state != State.Casting)
        {
            _ui.RemoveUnitDraws();
            _select.SetSelectManagerActive(false);
            _cast.StopCast();
            _spell.SelectSpell(slot);
            state = State.Aiming;
        }   
        else
        {
            Debug.Log("Casting - can't switch spell");
        }
    }
}
