using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "New Spell")]

public class Spell : ScriptableObject
{
    public Sprite icon;
    public string name;
    public string description;

    public Form form;
    public Effect effect;
}
