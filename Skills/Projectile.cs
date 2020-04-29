using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile", menuName = "New Effect/New Projectile")]
public class Projectile : ScriptableObject
{
    public Sprite icon;
    public GameObject projectilePrefab;



    // Projectile Stats
    public string name;
    public float damage;
    public float range;
    public float speed;
    public Effect.Origin origin;
    public Effect.Target target;
    public Effect.Type type;
}