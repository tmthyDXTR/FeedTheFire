using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellUtil : MonoBehaviour
{
    #region Variables
    [SerializeField]
    public Form form;
    [SerializeField]
    public Effect effect;

    public GameObject caster;
    private CastUtil _cast;

    [SerializeField] 
    public float power;
    #endregion


    void Start()
    {
        this.gameObject.name = form.name;
        _cast = caster.GetComponent<CastUtil>();

        SetSpellForm(form.type);
        LoadSpellGfx();
        power = SpellPowerAdjust(_cast.GetSpellPower());
    }


    public void SetSpellForm(Form.Type form)
    {
        if (form == Form.Type.Projectile)
        {
            // Add Projectile Scripts and Objects to
            // the spell gameobject
            this.gameObject.AddComponent<ProjectileController>();
            var collider = this.gameObject.AddComponent<SphereCollider>();
            collider.radius = 0.5f;
            collider.isTrigger = true;
            AdjustProjectile(_cast.GetProjectileRange(), _cast.GetProjectileSpeed());
        }
        if (form == Form.Type.Area)
        {
            // Add Area Scripts and Objects to
            // the spell gameobject
            this.gameObject.AddComponent<AreaController>();
        }
    }

    public void LoadSpellGfx()
    {
        GameObject gfxObj = null;
        // Gfx Prefab Files should look like "Gfx_Fire_Projectile"
        string gfxStringFileName = "Gfx_";

        if (effect.type == Effect.Type.Fire)
        {
            this.gameObject.tag = "Fire";
            gfxStringFileName += "Fire_";
            // Add Fire Projectile GFX Prefab
            if (form.type == Form.Type.Projectile)
            {
                gfxStringFileName += "Projectile";
            }
            // Add Fire Area GFX Prefab
            if (form.type == Form.Type.Area)
            {
                gfxStringFileName += "Area";
            }
        }
        if (effect.type == Effect.Type.Shadow)
        {
            this.gameObject.tag = "Shadow";
            gfxStringFileName += "Shadow_";
            // Add Shadow Projectile GFX Prefab
            if (form.type == Form.Type.Projectile)
            {
                gfxStringFileName += "Projectile";
            }
            // Add Shadow Area GFX Prefab
            if (form.type == Form.Type.Area)
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
        if (form.aim == Form.Aim.Auto)
        {
            _projectile.targetObj = _cast.castTarget;
        }
        if (form.aim == Form.Aim.Directional)
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
        if (form.quickness == Form.Quickness.VerySlow)
        {
            spellAdjustedSpeed *= 0.33f;
        }
        if (form.quickness == Form.Quickness.Slow)
        {
            spellAdjustedSpeed *= 0.66f;
        }
        if (form.quickness == Form.Quickness.Mid)
        {
            // standard base speed
        }
        if (form.quickness == Form.Quickness.Fast)
        {
            spellAdjustedSpeed *= 1.33f;
        }
        if (form.quickness == Form.Quickness.VeryFast)
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
