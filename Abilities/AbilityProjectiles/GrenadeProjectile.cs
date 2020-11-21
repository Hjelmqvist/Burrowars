using System.Collections;
using UnityEngine;

public abstract class GrenadeProjectile : AbilityProjectile
{
    [SerializeField] protected ParticleSystem impactEffect;
    [SerializeField] protected ParticleSystem overtimeEffect;
    [SerializeField] protected float impactRadius, dotRadius;
    [SerializeField] protected float secondsBetweenTicks = 1;

    /// <summary> Method affects enemies in radius.
    /// if the grenade has not exploded it will give the enemy an impact damage.
    /// if the grenade has exploded it will call a coroutine.
    /// </summary>
    protected override void ImpactMethod()
    {
        if (impactEffect != null)
            impactEffect.Play();

        if (overtimeEffect != null)
            overtimeEffect.Play();

        Collider[] colForImpactDamage = Physics.OverlapSphere(transform.position, impactRadius);

        foreach (Collider col in colForImpactDamage)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
                ImpactEffect(enemy);
        }
        StartCoroutine(OvertimeMethod());
    }

    protected abstract void ImpactEffect(Enemy enemy);

    /// <summary> OvertimeDamageMethod is a IEnumerator to use coroutine when applying dot damage.
    /// it will add all collidiers within the explostion radius to an array and in the foreach we add
    /// damage to all objects in list with an Ienemy script.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator OvertimeMethod()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsBetweenTicks);
            Collider[] colForDamage = Physics.OverlapSphere(transform.position, dotRadius);
            foreach (Collider col in colForDamage)
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                    OvertimeEffect(enemy);
            }
        }
    }

    protected abstract void OvertimeEffect(Enemy enemy);

    /// <summary>This method is for the developers.
    /// 
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, dotRadius);
    }
}
