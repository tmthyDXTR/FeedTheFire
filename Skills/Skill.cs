using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Create Skill from Effects")]

public class Skill : ScriptableObject
{
    public Sprite icon;
    public string name;

    public GameObject earlyEffect;
    public GameObject mediumEffect;
    public GameObject lateEffect;
}
