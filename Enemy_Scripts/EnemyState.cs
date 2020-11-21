using System.Collections.Generic;

/// <summary>
/// This class defines a general enemy state. It contains methods to take actions on entering and exiting the state, and a method to continually execute.
/// Additionally there are multiple methods to find targets by prerequisites. 
/// </summary>
public abstract class EnemyState
{
    protected Enemy owner = null;

    public EnemyState(Enemy owner) { this.owner = owner;}

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();

    #region Find Target

    /// <summary>
    /// This method finds the closest playercharacter by looping through all active playercharacters and comparing their squaredistance to the enemy.
    /// </summary>
    /// <param name="Characters">The list of all active characters.</param>
    /// <param name="shortestDistance">The currently known shortest squareddistance to an active playercharacter. Int.Maxvalue if none are known.</param>
    /// <returns>Returns the closest character if found, else null.</returns>
    protected Character FindClosestCharacter(List<Character> Characters, float shortestDistance=int.MaxValue)
    {
        Character closestTarget = null;
        foreach (Character potentialTarget in Characters)
        {
            if (owner.SquareDistanceToTarget(owner, potentialTarget) < shortestDistance + 0.1f && !potentialTarget.IsDead)
            {
                shortestDistance = owner.SquareDistanceToTarget(owner, potentialTarget);
                closestTarget = potentialTarget;
            }
        }
        owner.SetDistance(shortestDistance);
        return closestTarget;
    }

    /// <summary>
    /// This method finds all characters that are in range.
    /// </summary>
    /// <param name="Characters">The list of all active characters.</param>
    /// <returns>A list of all characters in attackrange.</returns>
    protected List<Character> FindCharactersInRange(List<Character> Characters)
    {
        List<Character> charactersInRange = Characters.FindAll(potentialTarget 
            => owner.IsInRange(owner.SquareDistanceToTarget(owner, potentialTarget), owner.Stats.AttackRange) && !potentialTarget.IsDead);
        return charactersInRange;
    }

    /// <summary>
    /// This method finds the player with least health remaining. 
    /// </summary>
    /// <param name="Characters">The list of all active characters.</param>
    ///  <param name="targetHealth">The target health to compare to. Starts at maxvalue if not specified.</param>
    /// <returns>Returns the character with least health.</returns>
    protected Character FindMostDamagedPlayer(List<Character> Characters, int targetHealth = int.MaxValue)
    {
        Character newTarget = null;
        foreach (Character potentialTarget in Characters)
        {
            if(potentialTarget.Stats.Health < targetHealth && !potentialTarget.IsDead)
            {
                targetHealth = potentialTarget.Stats.Health;
                newTarget = potentialTarget;
            }
        }
        return newTarget;
    }

    /// <summary>
    /// This method finds the player with most health remaining. 
    /// </summary>
    /// <param name="Characters">The list of all active characters.</param>
    /// <param name="targetHealth">The target health to compare to. Starts at minvalue if not specified.</param>
    /// <returns>Returns the character with most health.</return
    protected Character FindHealthiestPlayer(List<Character> Characters, int targetHealth = int.MinValue)
    {
        Character newTarget = null;
        foreach (Character potentialTarget in Characters)
        {
            if (potentialTarget.Stats.Health > targetHealth && !potentialTarget.IsDead)
            {
                targetHealth = potentialTarget.Stats.Health;
                newTarget = potentialTarget;
            }
        }
        return newTarget;
    }

    #endregion
}
