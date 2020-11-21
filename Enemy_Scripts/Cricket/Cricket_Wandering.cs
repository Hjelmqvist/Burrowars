/// <summary>
/// This class sets up the cricket wandering behavior.
/// </summary>
public class Cricket_Wandering : EnemyState
{
    Cricket_Ability song = null;

    public Cricket_Wandering(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        owner.Target = FindClosestCharacter(SpawnManager.characters);
        owner.Agent.isStopped = false;
        song = (Cricket_Ability)owner.Abilities.Find(ability => ability is Cricket_Ability);
    }

    public override void Execute()
    {
        owner.Target = FindClosestCharacter(SpawnManager.characters);
        if (owner.IsInRange(owner.CurrentDistanceToTarget, owner.Stats.AttackRange))
        {
            owner.StateMachine.ChangeState(new Cricket_Flee(owner));
        }

        song.Use(owner.Target);

        owner.Target = FindClosestCharacter(SpawnManager.characters);
        owner.Agent.SetDestination(owner.Target.transform.position);
    }

    public override void Exit()
    {

    }
}
