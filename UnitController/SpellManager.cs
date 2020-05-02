using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    // This class holds all the individually created projectiles,
    // area effects and buff/debuff effects
    // Also it holds the usable skills, that were created with the effects
    #region Variables

    [SerializeField] // Currently selected spell
    private Spell selectedSpell;



    [SerializeField] // List of all available spells
    private List<Spell> availableSpells;
    [SerializeField] // List of all available forms
    private List<Form> forms;
    [SerializeField] // List of all available effects
    private List<Effect> effects;


    #endregion
    void Start()
    {
        #region References


        #endregion

        // Select RightClick Attack at first // item 0 in array is RClick
        SelectSpell(0);
    }

    public Spell GetSelectedSpell()
    {
        return selectedSpell;
    }

    public SpellUtil GetSpellUtil(GameObject spell)
    {
        return spell.gameObject.GetComponent<SpellUtil>();        
    }

    public void SelectSpell(int slot)
    {
        if (slot < availableSpells.Count)
        {
            selectedSpell = availableSpells[slot];
            Debug.Log("Selected: " + selectedSpell);
        }
        Debug.Log("Selected Spell: " + slot);
    }
}
