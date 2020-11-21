using UnityEngine;

public class MolotovProjectile : GrenadeProjectile
{
    [Header("Molotov")] [Space(20)]

    [SerializeField] protected int impactDamage;
    [SerializeField] protected int dotDamage;

    /// <summary>this method damages enemy in parameter.
    /// 
    /// </summary>
    /// <param name="enemy">enemy will be damaged</param>
    protected override void ImpactEffect(Enemy enemy)
    {
        enemy.Stats.ModifyHealth(-impactDamage);
    }

    /// <summary> apply damage to enemy over time.
    /// 
    /// </summary>
    /// <param name="enemy">enemy will be damaged</param>
    protected override void OvertimeEffect(Enemy enemy)
    {
        enemy.Stats.ModifyHealth(-dotDamage);
    }
}
