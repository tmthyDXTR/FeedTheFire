using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CastUtil : MonoBehaviour
{
    #region Variables
    private SphereCollider _castRangeColl;
    private Animator _anim;
    private SpellManager _spell;
    private UnitUtil _unit;


    /// <summary>
    /// Casting Stats of the casting unit
    /// </summary>
    /// 
    [SerializeField]
    private float spellPower = 1f;
    [SerializeField]
    private float projectileCastRange = 7f;
    [SerializeField]
    private float projectileSpeed = 10f;
    [SerializeField]
    private float areaCastRange = 7f;
    [SerializeField]////// Public for now
    public bool isWaitingForCoolDown = false;
    [SerializeField]
    private float globalCoolDown = 1.5f;

    public EventHandler OnIsCastUnlocked;
    [SerializeField]
    private bool isCastLocked;
    public bool IsCastLocked
    {
        get { return isCastLocked; }
        set
        {
            isCastLocked = value;
            if (value == false)
                if (OnIsCastUnlocked != null) OnIsCastUnlocked(null, EventArgs.Empty);
        }
    }


    public enum Type 
    { 
        AutoSpell,
        SingleSpell,
    }
    [SerializeField]
    private Type type;

    public List<GameObject> inCastRange = new List<GameObject>();
    public GameObject castTarget;
    public Vector3 castDir;
    [SerializeField]
    private Spell spell;

    #endregion
    void Start()
    {
        #region References
        _castRangeColl = GameObject.Find("CastRange").GetComponent<SphereCollider>();
        _anim = gameObject.GetComponent<Animator>();
        _spell = gameObject.GetComponent<SpellManager>();
        _unit = gameObject.GetComponent<UnitUtil>();
        #endregion

        _castRangeColl.radius = projectileCastRange;
    }




    public IEnumerator StartCast(Type type, Spell spell)
    {        
        this.spell = spell;
        this.type = type;

        if (!isWaitingForCoolDown)
        {
            // If the casted spell is not auto/rclick
            // lock the caster movement
            if (this.type == Type.SingleSpell)
            {
                IsCastLocked = true;
            }

            Debug.Log("Start Cast");
            isWaitingForCoolDown = true;
            _anim.SetBool("isCasting", true);
            //Initiate casting animation
            _anim.Play("Spell_1");
            //Wait for global cooldown time
            yield return new WaitForSeconds(globalCoolDown);
            isWaitingForCoolDown = false;
        }
    }          



    public void CastFrame()
    {
        // Frame in animation where main / middle spell is
        // actually delivered
        if (this.type == Type.SingleSpell)
        {
            // If spell to create isn't auto, it needs a
            // target object or target direction in cast range            
            CreateSpell(this.spell);
            
        }
        else if (this.type == Type.AutoSpell)
        { // if auto spell / rclick, it needs a target object in range
            if (CheckHasTarget() && CheckTargetInRange(this.spell, 0))
            {
                IsCastLocked = true;
                CreateSpell(this.spell);
            }
        }        
    }

    public void EndCast()
    {   // Frame at the end of the animation, signaling end
        // of cast
        if (this.type == Type.SingleSpell)
        {
            StopCast();
        }
        IsCastLocked = false;
    }

    public void CreateSpell(Spell spell)
    {
        // Instantiate spell dummy prefab
        GameObject spellDummy = CreateSpellDummy(spell);
        // Set the spell properties (form and effect)
        SetSpellProperties(spell, spellDummy);
        Debug.Log("Created Spell");
    }

    private void SetSpellProperties(Spell spell, GameObject spellDummy)
    {
        // Set the spell properties (form and effect)
        SpellUtil createdSpell = spellDummy.GetComponent<SpellUtil>();
        createdSpell.form = spell.form;
        createdSpell.effect = spell.effect;
        createdSpell.caster = this.gameObject;
        Debug.Log("Set Spell Properties");
    }

    private GameObject CreateSpellDummy(Spell spell)
    {
        // Instantiate spell dummy prefab
        return Instantiate(Resources.Load("NewSpell"),
            GetSpellOriginVector(spell),
            Quaternion.identity,
            GameObject.Find("Spells").transform) as GameObject;
    }

    private Vector3 GetSpellOriginVector(Spell spell)
    {
        // Gets the origin position of the spell

        Vector3 spellOrigin = Vector3.zero;
        if (spell.form.origin == Form.Origin.Caster)
        {
            spellOrigin = this.transform.position + new Vector3(0, 1f, 0) + transform.forward * 1.5f;
        }
        if (spell.form.origin == Form.Origin.Sky)
        {
            if (castTarget != null)
            {
                spellOrigin = castTarget.transform.position + new Vector3(0, 7, 0);
            }
            else
            {
                spellOrigin = castDir + new Vector3(0, 7, 0);
            }
        }
        if (spell.form.aim == Form.Aim.Point)
        {
            spellOrigin = castDir + new Vector3(0, 1f, 0);
        }

        return spellOrigin;
    }

    public void StopCast()
    {
        Debug.Log("Stop Cast");
        _anim.SetBool("isCasting", false);
        castTarget = null;
        castDir = Vector3.zero;
    }

    public bool CheckTargetTargetable(GameObject target, Spell spell)
    {
        var isTargetable = false;
        // Checks if the target object is targetable by the spell

        if (spell.form.targetable == Form.Targetable.AllUnits)
        {
            if (target.layer == 8 || target.layer == 9) // 8 = PlayerUnits 9 = Enemy unit
            {
                castTarget = target.transform.parent.gameObject;
                isTargetable = true;
            }
        }
        else if (spell.form.targetable == Form.Targetable.PlayerUnits)
        {
            if (target.layer == 8) // 8 = PlayerUnits 9 = Enemy unit
            {
                castTarget = target.transform.parent.gameObject;
                isTargetable = true;
            }
        }
        else if (spell.form.targetable == Form.Targetable.EnemyUnits)
        {
            if (target.layer == 9) // 8 = PlayerUnits 9 = Enemy unit
            {
                castTarget = target.transform.parent.gameObject;
                isTargetable = true;
            }
        }        
        if (!isTargetable)
        {
            castTarget = null;
        }
        return isTargetable;
    }

    public bool CheckTargetInRange(Spell spell, float mouseDistance)
    {
        if (spell.form.aim == Form.Aim.Auto && spell.form.origin == Form.Origin.Caster)
        {
            RaycastHit hit;
            // Debug vision ray from unit to attack target
            var dir = (castTarget.transform.position - this.transform.position);
            Debug.DrawRay(transform.position, dir);
            // Cast a ray from unit to attack target
            if (Physics.Raycast(transform.position, dir, out hit))
            {
                //Debug.Log(hit.transform);
                // If the hit target is the attack target
                if (hit.transform == castTarget.transform && inCastRange.Contains(castTarget))
                {
                    // And the attack target is in range return true
                    return true;
                }
            }
            return false;
        }
        else
        {
            if (spell.form.type == Form.Type.Projectile)
            {
                return mouseDistance < GetProjectileRange();
            }
            else if (spell.form.type == Form.Type.Area)
            {
                return mouseDistance < GetAreaRange();
            }
        }
        return false;
    }

    public bool CheckHasTarget()
    {
        return castTarget != null || castDir != Vector3.zero;
    }

    // Calculates the current area cast Range
    public float GetAreaRange()
    {
        return areaCastRange;
    }

    // Calculates the current projectile cast Range
    public float GetProjectileRange()
    {
        return projectileCastRange;
    }

    // Calculates the current projectile speed
    public float GetProjectileSpeed()
    {
        return projectileSpeed;
    }

    public float GetSpellPower()
    {
        return spellPower;
    }
}
