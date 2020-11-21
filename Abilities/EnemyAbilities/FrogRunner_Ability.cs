using System.Collections;
using UnityEngine;

/// <summary>
/// This class defines the frogrunner Grab ability.
/// </summary>
public class FrogRunner_Ability : EnemyAbility
{
    protected override IEnumerator Attack()
    {
        owner.CanAttack = false;
        yield return new WaitForSeconds(owner.Stats.AttackSpeed);
        if (owner.IsInRange(owner.CurrentDistanceToTarget, owner.Stats.AttackRange))
            owner.Target.Stats.ModifyHealth(-(owner.Stats.AttackDamage + (int)owner.CurrentModifier));
        owner.CurrentModifier += Mathf.Pow(owner.Stats.DamageModifier, owner.Stats.Power);
        owner.CanAttack = true;
    }

    public override void AnimationCall()
    {
        throw new System.NotImplementedException();
    }
}
