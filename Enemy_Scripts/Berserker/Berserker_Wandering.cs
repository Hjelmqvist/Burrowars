/// <summary>
/// This class sets up the berserker wandering behavior.
/// </summary>
public class Berserker_Wandering : EnemyState
{
    private readonly float newSpeed = 8;
    public Berserker_Wandering(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        owner.Target = FindClosestCharacter(SpawnManager.characters); 
        owner.Agent.isStopped = false;
    }

    public override void Execute()
    {
        if(owner.Stats.Health < owner.Stats.MaxHealth)
        {
            owner.StateMachine.ChangeState(new Berserker_Rampage(owner));
        }

        owner.Target = FindClosestCharacter(SpawnManager.characters);
        owner.Agent.SetDestination(owner.Target.transform.position);
    }

    public override void Exit()
    {
        owner.Stats.ModifyMovementSpeed(newSpeed);
    }
}
