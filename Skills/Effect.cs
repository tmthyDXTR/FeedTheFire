using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    private ProjectileController projectile;

    public enum Origin
    {
        Caster,
        Sky,
        ClosestFire,
    }
    public enum Target
    {
        Caster,
        Enemy,
        Friend,
        Direction,
        Ground,
    }
    public enum CastType
    {
        Projectile,
        Area,
    }

    public enum Type
    {
        Damage,
        Buff,
        Debuff,
    }


    public Origin origin;
    public Target target;
    public CastType castType;
    public Type type;


    void Start()
    {
        projectile = GetComponent<ProjectileController>();

        if (projectile != null)
        {

        }
    }
}
