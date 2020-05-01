using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellUtil : MonoBehaviour
{
    #region Variables
    [SerializeField]
    public Spell spell;
    [SerializeField]
    public Effect effect;

    public GameObject _caster;
    private CastUtil _cast;

    [SerializeField] 
    public float power;
    #endregion


    void Start()
    {
        this.gameObject.name = spell.name;
        _cast = _caster.GetComponent<CastUtil>();

        SetSpellForm(spell.form);
        LoadSpellGfx();
        power = SpellPowerAdjust(_cast.GetSpellPower());
    }


    public void SetSpellForm(Spell.Form form)
    {
        if (form == Spell.Form.Projectile)
        {
            // Add Projectile Scripts and Objects to
            // the spell gameobject
            this.gameObject.AddComponent<ProjectileController>();
            var collider = this.gameObject.AddComponent<SphereCollider>();
            collider.radius = 0.5f;
            collider.isTrigger = true;
            AdjustProjectile(_cast.GetProjectileRange(), _cast.GetProjectileSpeed());
        }
        if (form == Spell.Form.Area)
        {
            // Add Area Scripts and Objects to
            // the spell gameobject
            this.gameObject.AddComponent<AreaController>();
        }
    }

    public void LoadSpellGfx()
    {
        GameObject gfxObj = null;
        string gfxStringFileName = "Gfx_";

        if (effect.type == Effect.Type.Fire)
        {
            this.gameObject.tag = "Fire";
            gfxStringFileName += "Fire_";
            // Add Fire Projectile GFX Prefab
            if (spell.form == Spell.Form.Projectile)
            {
                gfxStringFileName += "Projectile";
            }
            // Add Fire Area GFX Prefab
            if (spell.form == Spell.Form.Area)
            {
                gfxStringFileName += "Area";
            }
        }
        if (effect.type == Effect.Type.Shadow)
        {
            this.gameObject.tag = "Shadow";
            gfxStringFileName += "Shadow_";
            // Add Shadow Projectile GFX Prefab
            if (spell.form == Spell.Form.Projectile)
            {
                gfxStringFileName += "Projectile";
            }
            // Add Shadow Area GFX Prefab
            if (spell.form == Spell.Form.Area)
            {
                gfxStringFileName += "Area";
            }
        }

        // Instantiate and add the projectile/area prefab
        gfxObj = Instantiate(Resources.Load(gfxStringFileName),
            this.transform.position,
            Quaternion.identity,
            this.transform) as GameObject;
    }

    public void AdjustProjectile(float range, float speed)
    {
        var _projectile = gameObject.GetComponent<ProjectileController>();
        if (spell.aim == Spell.Aim.Auto)
        {
            _projectile.targetObj = _cast.castTarget;
        }
        if (spell.aim == Spell.Aim.Directional)
        {
            _projectile.targetDir = _cast.castDir;
        }
        _projectile.AdjustProjectile(range, QuicknessAdjust(speed));
    }


    // Calculates base projectile cast speed with spell
    // quickness multiplier
    private float QuicknessAdjust(float baseSpeed)
    {
        var spellAdjustedSpeed = baseSpeed;
        if (spell.quickness == Spell.Quickness.VerySlow)
        {
            spellAdjustedSpeed *= 0.33f;
        }
        if (spell.quickness == Spell.Quickness.Slow)
        {
            spellAdjustedSpeed *= 0.66f;
        }
        if (spell.quickness == Spell.Quickness.Mid)
        {
            // standard base speed
        }
        if (spell.quickness == Spell.Quickness.Fast)
        {
            spellAdjustedSpeed *= 1.33f;
        }
        if (spell.quickness == Spell.Quickness.VeryFast)
        {
            spellAdjustedSpeed *= 1.66f;
        }
        return spellAdjustedSpeed;
    }

    private float SpellPowerAdjust(float basePower)
    {
        var spellAdjustedPower = basePower;
        if (effect.strength == Effect.Strength.VeryWeak)
        {
            spellAdjustedPower *= 0.5f;
        }
        if (effect.strength == Effect.Strength.Weak)
        {
            spellAdjustedPower *= 0.75f;
        }
        if (effect.strength == Effect.Strength.Mid)
        {
            // standard base power
        }
        if (effect.strength == Effect.Strength.Strong)
        {
            spellAdjustedPower *= 1.25f;
        }
        if (effect.strength == Effect.Strength.VeryStrong)
        {
            spellAdjustedPower *= 1.5f;
        }
        return spellAdjustedPower;
    }
}
