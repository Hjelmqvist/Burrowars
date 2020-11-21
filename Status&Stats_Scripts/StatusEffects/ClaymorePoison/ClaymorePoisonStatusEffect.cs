using System.Collections;
using UnityEngine;

/// <summary>
/// This class defines the ClaymorePoisonStatuseffect, dealing damage over a duration.
/// </summary>
public class ClaymorePoisonStatusEffect : StatusEffect
{
    public ClaymorePoisonStatusEffect(StatusEffectBlueprint blueprint) : base(blueprint) { }

    public override IEnumerator Status(Stats stats)
    {

        if (stats.PoisonParticle != null)
            stats.PoisonParticle.gameObject.SetActive(true);

        while (ticks > 0)
        {
            yield return new WaitForSeconds(duration / maxTicks);
            stats.ModifyHealth(-blueprint.Damage);
            ticks--;
        }
        if (stats.PoisonParticle != null)
            stats.PoisonParticle.gameObject.SetActive(false);

        stats.RemoveEffectFromList(this);
    }
}
