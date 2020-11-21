using UnityEngine;

/// <summary>
/// This class sets up the Cricket fleeing behavior.
/// </summary>
public class Cricket_Flee : EnemyState
{
    Cricket_Ability song = null;

    public Cricket_Flee(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        owner.Target = FindClosestCharacter(SpawnManager.characters);
        owner.Agent.isStopped = false;
        song = (Cricket_Ability)owner.Abilities.Find(ability => ability is Cricket_Ability);
    }

    public override void Execute()
    {
        if (!owner.IsInRange(owner.CurrentDistanceToTarget, owner.Stats.AttackRange))
        {
            owner.StateMachine.ChangeState(new Cricket_Wandering(owner));
        }

        song.Use(owner.Target);

        owner.Target = FindClosestCharacter(SpawnManager.characters);

        //Move opposite way of the player.
        Vector3 dirToPlayer = owner.transform.position - owner.Target.transform.position;
        Vector3 newPos = owner.transform.position + dirToPlayer;
        owner.Agent.SetDestination(newPos);
    }

    public override void Exit()
    {

    }
}
