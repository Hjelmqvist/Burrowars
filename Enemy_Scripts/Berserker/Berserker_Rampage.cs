using UnityEngine;
/// <summary>
/// This class sets up the berserker rampage behavior.
/// </summary>
public class Berserker_Rampage : EnemyState
{
    public Berserker_Rampage(Enemy owner) : base(owner) { }
    private Vector3 chargeTarget;

    public override void Enter()
    {
        owner.Target = FindClosestCharacter(SpawnManager.characters); 
        owner.Agent.isStopped = false;
    }

    public override void Execute()
    {
        if(owner.Target.IsDead)
            owner.Target = FindClosestCharacter(SpawnManager.characters);
        if (owner.CanSeeTarget(out chargeTarget, owner.Stats.AttackRange, owner.Navmask, owner.Raymask))
        {
            owner.ChargeTarget = chargeTarget;
            owner.StateMachine.ChangeState(new Berserker_Charge(owner));
        }
        else
        {
            owner.Agent.SetDestination(owner.Target.transform.position);
        }
    }

    public override void Exit()
    {
    }
}
