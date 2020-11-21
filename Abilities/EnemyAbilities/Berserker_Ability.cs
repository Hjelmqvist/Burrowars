using System.Collections;
using UnityEngine;

/// <summary>
/// This class defines the berserkers charge ability.
/// </summary>
public class Berserker_Ability : EnemyAbility
{
    protected override IEnumerator Attack()
    {
        owner.CanAttack = false;
        yield return new WaitForSeconds(owner.Stats.AttackSpeed);
        owner.Rb.isKinematic = true;
        owner.Agent.isStopped = false;
        owner.Agent.velocity = Vector3.ClampMagnitude(owner.Target.transform.position - transform.position, 1) * owner.Stats.ChargeSpeed;
        owner.Agent.isStopped = true;
        yield return new WaitForSeconds(owner.Stats.AttackSpeed);
        owner.CanAttack = true;
        owner.Rb.isKinematic = false;
    }

    public override void AnimationCall()
    {
        throw new System.NotImplementedException();
    }
}
