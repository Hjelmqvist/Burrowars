/// <summary>
/// This class defines sthe berserker enemy.
/// </summary>
public class Berserker : Enemy
{  
    protected override void Awake()
    {
        base.Awake();
        stateMachine.ChangeState(new Berserker_Wandering(this));
    }
}
