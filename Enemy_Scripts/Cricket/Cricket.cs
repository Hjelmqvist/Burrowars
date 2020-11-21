/// <summary>
/// This class defines the Cricket enemy.
/// </summary>
public class Cricket : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        stateMachine.ChangeState(new Cricket_Wandering(this));
    }
}
