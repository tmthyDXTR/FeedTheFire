using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Create Skill from Spells")]

public class Skill : ScriptableObject
{
    public Sprite icon;
    public string name;

    public GameObject earlySpell;
    public GameObject mediumSpell;
    public GameObject lateSpell;
}
