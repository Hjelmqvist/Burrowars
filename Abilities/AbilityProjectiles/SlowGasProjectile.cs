using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowGasProjectile : GrenadeProjectile
{
    [SerializeField] int impactDamage = 0;
    [SerializeField] float slowPercentage = 0.2f;
    List<Enemy> checkSlowedEnemies = new List<Enemy>();
    List<Enemy> enemiesInRadius = null;

    /// <summary> This method sets all enemies speed back to normal before calling base method.
    /// 
    /// </summary>
    protected override void Die()
    {
        foreach (Enemy e in checkSlowedEnemies)
            if (e != null)
                e.Stats.ModifyMovementSpeed(e.Stats.BaseMovementSpeed);
        base.Die();
    }

    /// <summary>this method damages enemy in parameter.
    /// 
    /// </summary>
    /// <param name="enemy">enemy will be damaged</param>
    protected override void ImpactEffect(Enemy enemy)
    {
        enemy.Stats.ModifyHealth(-impactDamage);
    }

    /// <summary> OvertimeDamageMethod is a IEnumerator to use coroutine when applying dot damage.
    /// Add all collidiers within the explostion radius to an array and in the foreach we apply
    /// slow to all objects in list with an Ienemy script. Add that enemy to list of enemiesInRadius and checkSlowedEnemies.
    /// In the second foreach we check if enemies in list of checkSlowedEnemies does not exist in radius,
    /// if no then put their speed back to normal. 
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator OvertimeMethod()
    {
        while (true)
        {
            Collider[] colForDamage = Physics.OverlapSphere(transform.position, dotRadius);
            enemiesInRadius = new List<Enemy>();

            foreach (Collider col in colForDamage)
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                    OvertimeEffect(enemy);
            }

            for (int i = checkSlowedEnemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = checkSlowedEnemies[i];
                if (enemy != null && !enemiesInRadius.Contains(enemy))
                {
                    enemy.Stats.ModifyMovementSpeed(enemy.Stats.BaseMovementSpeed);
                    checkSlowedEnemies.Remove(enemy);
                }
            }
            yield return new WaitForSeconds(secondsBetweenTicks);
        }
    }

    /// <summary> apply slow to enemy.
    /// 
    /// </summary>
    /// <param name="enemy">enemy will be slowed</param>
    protected override void OvertimeEffect(Enemy enemy)
    {
        enemy.Stats.ModifyMovementSpeed(enemy.Stats.BaseMovementSpeed * slowPercentage);
        checkSlowedEnemies.Add(enemy);
        enemiesInRadius.Add(enemy);
    }
}
