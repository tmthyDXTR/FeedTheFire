using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackUtil : MonoBehaviour
{
    private SelectionManager _select;
    private SphereCollider _attackRangeColl;
    private Animator _anim;

    [SerializeField]
    private float attackRangeRadius = 7f;
    [SerializeField]
    private bool isAttacking = false;
    [SerializeField]
    private float globalCoolDown = 1.5f;
    public List<GameObject> inAttackRange = new List<GameObject>();
    public GameObject attackTarget;

    void Start()
    {
        _select = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
        _attackRangeColl = GameObject.Find("AttackRange").GetComponent<SphereCollider>();
        _anim = gameObject.GetComponent<Animator>();

        _attackRangeColl.radius = attackRangeRadius;
    }

    public IEnumerator Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            //Initiate casting
            _anim.Play("Spell_1");
            //Cooldown time
            yield return new WaitForSeconds(globalCoolDown);
            isAttacking = false;
        }
    }          

    public void AttackFrame()
    {
        if (CheckIfAttackTargetInRange())
        {
            CastAttack();
        }
    }

    public void CastAttack()
    {
        Debug.Log("Cast Attack");
        //GameObject _spell = Instantiate(Resources.Load("FireBall")) as GameObject;
        GameObject _spell = Instantiate(Resources.Load("FireBall"),
            this.transform.position + new Vector3(0, 0.5f, 0),
            Quaternion.identity,
            GameObject.Find("Projectiles").transform) as GameObject;
        Projectile _projectile = _spell.GetComponent<Projectile>();
        _projectile.targetObj = attackTarget;
    }

    public void CheckIfTargetAttackable()
    {
        var clickedObj = _select.GetClickedObject();
        if (clickedObj.layer == 9) // 9 = Enemy unit
        {
            attackTarget = clickedObj.transform.parent.gameObject;
        }
        else
        {
            attackTarget = null;
        }
    }

    public bool CheckIfAttackTargetInRange()
    {
        if (attackTarget != null && inAttackRange.Contains(attackTarget))
        {
            return true;
        }
        return false;
    }
}
