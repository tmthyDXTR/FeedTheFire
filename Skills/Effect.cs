using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect", menuName = "New Effect")]
public class Effect : ScriptableObject
{
    public string name;
    public string description;


    public Type type;
    public Duration duration;
    public float timer;
    public Strength strength;

    public enum Type
    {
        Fire,
        Shadow,
    }
    public enum Duration
    {
        Instant,
        OverTime,
    }

    public enum Strength
    {
        VeryWeak,
        Weak,
        Mid,
        Strong,
        VeryStrong,
    }

}
