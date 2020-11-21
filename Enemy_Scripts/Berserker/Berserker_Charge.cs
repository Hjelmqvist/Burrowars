/// <summary>
/// This class sets up the berserker charge behavior.
/// </summary>
public class Berserker_Charge : EnemyState
{
    Berserker_Ability charge = null;

    public Berserker_Charge(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        owner.Agent.isStopped = true;
        owner.Charged = false;
        charge = (Berserker_Ability)owner.Abilities.Find(ability => ability is Berserker_Ability);
    }

    public override void Execute()
    {
        if(owner.Target.IsDead)
            owner.StateMachine.ChangeState(new Berserker_Rampage(owner));
        if (!owner.Charged)
        {
            charge.Use(owner.Target);
            owner.Charged = true;
        }
        else if(owner.CanAttack && owner.Charged)
        {
            owner.StateMachine.ChangeState(new Berserker_Rampage(owner));
            owner.StopAllCoroutines();
        }
        else
            owner.RotateTowards(owner.Target.transform.position, 3);
    }

    public override void Exit()
    {
        if(owner.Agent != null)
            owner.Agent.isStopped = false;
    }
}
