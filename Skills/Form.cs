using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Form", menuName = "New Form")]
public class Form : ScriptableObject
{
    public enum Origin
    {
        Caster,
        Sky,
        ClosestFire,
    }
    public enum Targetable
    {
        EnemyUnits,
        PlayerUnits,
        AllUnits,
    }
    public enum Type
    {
        Projectile,
        Area,
    }        
    public enum Aim
    {
        Directional,
        Point,
        Auto,
    }

    public enum Quickness
    {
        VerySlow,
        Slow,
        Mid,
        Fast,
        VeryFast,
    }



    public Origin origin;
    public Targetable targetable;
    public Type type;
    public Aim aim;
    public Quickness quickness;
}
