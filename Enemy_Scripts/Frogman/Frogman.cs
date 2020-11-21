/// <summary>
/// This class defines the frogman enemy.
/// </summary>
public class Frogman : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        stateMachine.ChangeState(new Frogman_Chase(this));
    }
}
