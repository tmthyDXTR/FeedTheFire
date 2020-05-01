using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastUtil : MonoBehaviour
{
    #region Variables
    private SphereCollider _castRangeColl;
    private Animator _anim;
    private SkillManager _skill;


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

    public enum CastPhase
    {
        Early,
        Mid,
        Late,
    }
    public CastPhase castPhase;

    #endregion
    void Start()
    {
        #region References
        _castRangeColl = GameObject.Find("CastRange").GetComponent<SphereCollider>();
        _anim = gameObject.GetComponent<Animator>();
        _skill = gameObject.GetComponent<SkillManager>();

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

    public void EarlyCast()
    {
        // Frame in animation where instant / early spell is
        // actually delivered
        if (CheckHasTarget() && CheckTargetInRange())
        {
            castPhase = CastPhase.Early;
            CastSpell(castPhase);
        }
    }

    public void MidCast()
    {   
        // Frame in animation where main / middle spell is
        // actually delivered
        if (CheckHasTarget() && CheckTargetInRange())
        {
            castPhase = CastPhase.Mid;
            CastSpell(castPhase);
        }
    }

    public void LateCast()
    {
        // Frame in animation where slow / late spell is
        // actually deliveredEffect
        if (CheckHasTarget() && CheckTargetInRange())
        {
            castPhase = CastPhase.Late;
            CastSpell(castPhase);
        }
    }

    public void CastSpell(CastPhase phase)
    {
        SpellUtil _spell = _skill.GetSpell(phase);

        if (_spell != null)
        {
            Vector3 spellOrigin = Vector3.zero;
            // Get the origin position of the spell
            if (_spell.spell.origin == Spell.Origin.Caster)
            {
                spellOrigin = this.transform.position + new Vector3(0, 3, 0);
            }
            if (_spell.spell.origin == Spell.Origin.Sky)
            {
                spellOrigin = castTarget.transform.position + new Vector3(0, 7, 0);
            }

            Debug.Log("Cast " + castPhase + " Spell");
            // Instantiate the projectile/spell prefab
            GameObject spell = Instantiate(Resources.Load(_spell.name),
                spellOrigin,
                Quaternion.identity,
                GameObject.Find("Spells").transform) as GameObject;

            _spell._caster = this.gameObject;
        }        
    }

    public void StopCast()
    {
        Debug.Log("Stop Cast");
        _anim.SetBool("isCasting", false);
        castTarget = null;
        castDir = Vector3.zero;
    }

    public bool CheckTargetTargetable(GameObject obj)
    {
        if (obj.layer == 9) // 9 = Enemy unit
        {
            castTarget = obj.transform.parent.gameObject;
            return true;
        }
        else
        {
            castTarget = null;
            return false;
        }
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
        return castTarget != null;
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
