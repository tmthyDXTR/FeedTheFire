using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastUtil : MonoBehaviour
{
    #region Variables
    private SelectionManager _select;
    private SphereCollider _castRangeColl;
    private Animator _anim;
    private SkillManager _skill;



    [SerializeField]
    private float castRangeRadius = 7f;
    [SerializeField]
    private bool isWaitingForCoolDown = false;
    [SerializeField]
    private float globalCoolDown = 1.5f;
    public List<GameObject> inCastRange = new List<GameObject>();
    public GameObject castTarget;

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
        _select = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
        _castRangeColl = GameObject.Find("CastRange").GetComponent<SphereCollider>();
        _anim = gameObject.GetComponent<Animator>();
        _skill = gameObject.GetComponent<SkillManager>();

        #endregion

        _castRangeColl.radius = castRangeRadius;
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
        // Frame in animation where instant / early effect is
        // actually delivered
        if (CheckHasTarget() && CheckTargetInRange())
        {
            castPhase = CastPhase.Early;
            CastEffect(castPhase);
        }
    }

    public void MidCast()
    {   
        // Frame in animation where main / middle effect is
        // actually delivered
        if (CheckHasTarget() && CheckTargetInRange())
        {
            castPhase = CastPhase.Mid;
            CastEffect(castPhase);
        }
    }

    public void LateCast()
    {
        // Frame in animation where slow / late effect is
        // actually delivered
        if (CheckHasTarget() && CheckTargetInRange())
        {
            castPhase = CastPhase.Late;
            CastEffect(castPhase);
        }
    }

    public void CastEffect(CastPhase phase)
    {
        Effect _effect = _skill.GetEffect(phase);

        if (_effect != null)
        {
            Vector3 spellOrigin = Vector3.zero;
            // Get the origin position of the effect
            if (_effect.origin == Effect.Origin.Caster)
            {
                spellOrigin = this.transform.position + new Vector3(0, 3, 0);
            }

            Debug.Log("Cast " + castPhase + " Effect");
            // Instantiate the projectile/spell prefab
            GameObject spell = Instantiate(Resources.Load(_effect.name),
                spellOrigin,
                Quaternion.identity,
                GameObject.Find("Effects").transform) as GameObject;

            // Get the target position of the spell
            if (_effect.target == Effect.Target.Enemy)
            {
                ProjectileController _projectile = spell.GetComponent<ProjectileController>();
                // Give the Projectile a target obj or direction
                _projectile.targetObj = castTarget;
            }
        }        
    }

    public void StopCast()
    {
        Debug.Log("Stop Cast");
        _anim.SetBool("isCasting", false);
        castTarget = null;
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
}
