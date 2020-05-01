using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    // This class holds all the individually created projectiles,
    // area effects and buff/debuff effects
    // Also it holds the usable skills, that were created with the effects
    #region Variables


    [SerializeField]
    public Skill selectedSkill;

    [SerializeField]
    private SpellUtil earlySpell;
    [SerializeField]
    private SpellUtil midSpell;
    [SerializeField]
    private SpellUtil lateSpell;


    [SerializeField]
    private List<Skill> activeSkills;

    [SerializeField]
    private List<Spell> spells;


    #endregion
    void Start()
    {
        #region References


        #endregion

        SelectSkill(0);
    }

    public SpellUtil GetSpell(CastUtil.CastPhase phase)
    {
        SpellUtil spell = null;
        if (phase == CastUtil.CastPhase.Early)
            spell = earlySpell;
        if (phase == CastUtil.CastPhase.Mid)
            spell = midSpell;
        if (phase == CastUtil.CastPhase.Late)
            spell = lateSpell;

        return spell;
    }

    public void SelectSkill(int slot)
    {
        if (slot < activeSkills.Count)
        {
            selectedSkill = activeSkills[slot];
            LoadSkillSpells();
            Debug.Log("Selected: " + selectedSkill);
        }
        Debug.Log("Selected Skill: " + slot);
    }

    private void LoadSkillSpells()
    {        
        // Loads the spell scripts of the selected skill
        if (selectedSkill != null)
        {
            if (selectedSkill.earlySpell != null)
            {
                earlySpell = selectedSkill.earlySpell.GetComponent<SpellUtil>();
            }
            if (selectedSkill.mediumSpell != null)
            {
                midSpell = selectedSkill.mediumSpell.GetComponent<SpellUtil>();
            }
            if (selectedSkill.lateSpell != null)
            {
                lateSpell = selectedSkill.lateSpell.GetComponent<SpellUtil>();
            }
        }        
    }
}
