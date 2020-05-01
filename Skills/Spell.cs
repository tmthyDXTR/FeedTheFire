using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "New Spell")]
public class Spell : ScriptableObject
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
    public enum Form
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
    public Form form;
    public Aim aim;
    public Quickness quickness;
}
