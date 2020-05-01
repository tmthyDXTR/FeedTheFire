using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastUtil : MonoBehaviour
{
    #region Variables
    private SphereCollider _castRangeColl;
    private Animator _anim;
    private SpellManager _spell;


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
    [SerializeField]
    private bool isWaitingForCoolDown = false;
    [SerializeField]
    private float globalCoolDown = 1.5f;


    public List<GameObject> inCastRange = new List<GameObject>();
    public GameObject castTarget;
    public Vector3 castDir;

    #endregion
    void Start()
    {
        #region References
        _castRangeColl = GameObject.Find("CastRange").GetComponent<SphereCollider>();
        _anim = gameObject.GetComponent<Animator>();
        _spell = gameObject.GetComponent<SpellManager>();

        #endregion

        _castRangeColl.radius = projectileCastRange;
    }

    public IEnumerator StartCast()
    {
        if (!isWaitingForCoolDown)
        {
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
        if (CheckHasTarget() && CheckTargetInRange())
        {
            CastSpell(_spell.GetSelectedSpell());
        }
    }

    public void CastSpell(Spell spell)
    {
        // Instantiate spell dummy prefab
        GameObject spellDummy = CreateSpellDummy(spell);
        // Set the spell properties (form and effect)
        SetSpellProperties(spell, spellDummy);
        _spell.SelectSpell(0);
        Debug.Log("Cast Spell");
    }

    private void SetSpellProperties(Spell spell, GameObject spellDummy)
    {
        // Set the spell properties (form and effect)
        SpellUtil createdSpell = spellDummy.GetComponent<SpellUtil>();
        createdSpell.form = spell.form;
        createdSpell.effect = spell.effect;
        createdSpell.caster = this.gameObject;
    }

    private GameObject CreateSpellDummy(Spell spell)
    {
        // Instantiate spell dummy prefab
        return Instantiate(Resources.Load("NewSpell"),
            GetSpellOriginVector(spell.form.origin),
            Quaternion.identity,
            GameObject.Find("Spells").transform) as GameObject;
    }

    private Vector3 GetSpellOriginVector(Form.Origin origin)
    {
        // Gets the origin position of the spell

        Vector3 spellOrigin = Vector3.zero;
        if (origin == Form.Origin.Caster)
        {
            spellOrigin = this.transform.position + new Vector3(0, 3, 0);
        }
        if (origin == Form.Origin.Sky)
        {
            spellOrigin = castTarget.transform.position + new Vector3(0, 7, 0);
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

    public bool CheckTargetInRange()
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

    public bool CheckHasTarget()
    {
        return castTarget != null || castDir != Vector3.zero;
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
