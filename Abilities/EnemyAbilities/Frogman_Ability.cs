using System.Collections;
using UnityEngine;

/// <summary>
/// This class defines the Frogman punch ability.
/// </summary>
public class Frogman_Ability : EnemyAbility
{
    [SerializeField] private float animationsSpeed = 2;

    public override void AnimationCall()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator Attack()
    {
        owner.Agent.isStopped = true;
        owner.Anim.SetBool("Attacking", true);
        yield return new WaitForSeconds(cooldown/animationsSpeed);
        if(owner.IsInRange(owner.CurrentDistanceToTarget, owner.Stats.AttackRange))
            owner.Target.Stats.ModifyHealth(-owner.Stats.AttackDamage);
        owner.Agent.isStopped = false;
        owner.Anim.SetBool("Attacking", false);
    }
}
