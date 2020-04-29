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
    private Effect earlyEffect;
    [SerializeField]
    private Effect midEffect;
    [SerializeField]
    private Effect lateEffect;


    [SerializeField]
    private List<Skill> activeSkills;

    [SerializeField]
    private List<Projectile> projectiles;


    #endregion
    void Start()
    {
        #region References


        #endregion

        SelectSkill(0);
    }

    public Effect GetEffect(CastUtil.CastPhase phase)
    {
        Effect effect = null;
        if (phase == CastUtil.CastPhase.Early)
            effect = earlyEffect;
        if (phase == CastUtil.CastPhase.Mid)
            effect = midEffect;
        if (phase == CastUtil.CastPhase.Late)
            effect = lateEffect;

        return effect;
    }

    public void SelectSkill(int slot)
    {
        if (slot < activeSkills.Count)
        {
            selectedSkill = activeSkills[slot];
            LoadSkillEffects();
            Debug.Log("Selected: " + selectedSkill);
        }
        Debug.Log("Selected Skill: " + slot);
    }

    private void LoadSkillEffects()
    {        
        // Loads the effect scripts of the selected skill
        if (selectedSkill != null)
        {
            if (selectedSkill.earlyEffect != null)
            {
                earlyEffect = selectedSkill.earlyEffect.GetComponent<Effect>();
            }
            if (selectedSkill.mediumEffect != null)
            {
                midEffect = selectedSkill.mediumEffect.GetComponent<Effect>();
            }
            if (selectedSkill.lateEffect != null)
            {
                lateEffect = selectedSkill.lateEffect.GetComponent<Effect>();
            }
        }        
    }
}
