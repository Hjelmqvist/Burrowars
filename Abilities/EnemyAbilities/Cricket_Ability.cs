using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class defines the cricket song ability.
/// </summary>
public class Cricket_Ability : EnemyAbility
{
    [SerializeField] private StatusEffectBlueprint blueprint = null;
    [SerializeField] private float radius = 0;
    [SerializeField] ParticleSystem aura = null;

    protected override IEnumerator Attack()
    {
        if(aura != null)
            aura.Play();

        List<Enemy> targetHitList = new List<Enemy>();
        Collider[] hitCols = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in hitCols)
        {
            Enemy hitEnemy = col.GetComponent<Enemy>();
            Enemy alreadyHit = targetHitList.Find(c => c.Equals(hitEnemy));
            if (hitEnemy != null && alreadyHit == null)
            {
                CricketSong effect = (CricketSong)hitEnemy.Stats.Statuses.Find(e => e is CricketSong);
                if (effect == null)
                {
                    effect = new CricketSong(blueprint);
                    hitEnemy.Stats.AddStatuses(effect);
                }
                else
                    hitEnemy.Stats.ResetTicks(effect);

                targetHitList.Add(hitEnemy);
            }
        }
        yield return null;
    }

    public override void AnimationCall()
    {
        throw new System.NotImplementedException();
    }
}
