using System.Collections;
using UnityEngine;

/// <summary>
/// This class defines the CricketSong statuseffect, increasing movementspeed for a duration.
/// </summary>
public class CricketSong : StatusEffect
{
    public CricketSong(StatusEffectBlueprint blueprint) : base(blueprint) { }

    public override IEnumerator Status(Stats stats)
    {
        stats.ModifyMovementSpeed(stats.BaseMovementSpeed * blueprint.Slow);
        while (ticks > 0)
        {
            yield return new WaitForSeconds(duration / maxTicks);
            ticks--;
        }
        stats.MovementSpeed = stats.BaseMovementSpeed;
        stats.RemoveEffectFromList(this);
    }
}
