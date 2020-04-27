using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackUtil : MonoBehaviour
{
    #region Variables
    private SelectionManager _select;
    private SphereCollider _attackRangeColl;
    private Animator _anim;

    [SerializeField]
    private float attackRangeRadius = 7f;
    [SerializeField]
    private bool isWaitingForCoolDown = false;
    [SerializeField]
    private float globalCoolDown = 1.5f;
    public List<GameObject> inAttackRange = new List<GameObject>();
    public GameObject attackTarget;

    #endregion
    void Start()
    {
        #region References
        _select = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
        _attackRangeColl = GameObject.Find("AttackRange").GetComponent<SphereCollider>();
        _anim = gameObject.GetComponent<Animator>();

        #endregion

        _attackRangeColl.radius = attackRangeRadius;
    }

    public IEnumerator Attack()
    {
        if (!isWaitingForCoolDown)
        {
            Debug.Log("Start Attack");
            isWaitingForCoolDown = true;
            _anim.SetBool("IsAttacking", true);
            //Initiate casting animation
            _anim.Play("Spell_1");
            //Wait for global cooldown time
            yield return new WaitForSeconds(globalCoolDown);
            isWaitingForCoolDown = false;
        }
    }          

    public void AttackFrame()
    {   
        // Frame in animation where attack is
        // actually delivered
        if (CheckAttackTargetInRange())
        {
            CastAttack();
        }
    }

    public void CastAttack()
    {
        Debug.Log("Cast Attack");
        // Instantiate the projectile/spell prefab
        GameObject _spell = Instantiate(Resources.Load("FireBall"),
            this.transform.position + new Vector3(0, 0.5f, 0) + transform.forward * 1,
            Quaternion.identity,
            GameObject.Find("Projectiles").transform) as GameObject;
        Projectile _projectile = _spell.GetComponent<Projectile>();
        // Give the Projectile a target obj or direction
        _projectile.targetObj = attackTarget;
    }

    public void StopAttack()
    {
        Debug.Log("Stop Attack");
        _anim.SetBool("IsAttacking", false);
        attackTarget = null;
    }

    public bool CheckTargetAttackable(GameObject obj)
    {
        if (obj.layer == 9) // 9 = Enemy unit
        {
            attackTarget = obj.transform.parent.gameObject;
            return true;
        }
        else
        {
            attackTarget = null;
            return false;
        }
    }

    public bool CheckAttackTargetInRange()
    {
        RaycastHit hit;
        // Debug vision ray from unit to attack target
        var dir = (attackTarget.transform.position - this.transform.position);
        Debug.DrawRay(transform.position, dir);
        // Cast a ray from unit to attack target
        if (Physics.Raycast(transform.position + transform.forward * 0.25f, (attackTarget.transform.position - transform.position), out hit))
        {
            Debug.Log(hit.transform);
            // If the hit target is the attack target
            if (hit.transform == attackTarget.transform && inAttackRange.Contains(attackTarget))
            {                
                // And the attack target is in range return true
                return true;
            }
        }
        return false;
    }

    public bool CheckHasAttackTarget()
    {
        return attackTarget != null;
    }
}
