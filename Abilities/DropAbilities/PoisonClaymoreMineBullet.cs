using UnityEngine;

public class PoisonClaymoreMineBullet : NormalBullet
{
    [SerializeField] StatusEffectBlueprint blueprint = null;

    /// <summary>This method checks colliders hit and acts differently depending on what it hit.
    /// If collider is of Enemy it will damage and set damage over time effect to it.
    /// </summary>
    protected override void OnTriggerEnter(Collider col)
    {
        Enemy enemy = col.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Stats.ModifyHealth(-damage);
            ClaymorePoisonStatusEffect effect = (ClaymorePoisonStatusEffect)enemy.Stats.Statuses.Find(e => e is ClaymorePoisonStatusEffect);
            if (effect == null)
            {
                effect = new ClaymorePoisonStatusEffect(blueprint);
                enemy.Stats.AddStatuses(effect);
            }
            else
                enemy.Stats.ResetTicks(effect);

            if (hitEffect.Length > 0)
                Instantiate(hitEffect[Random.Range(0, hitEffect.Length)], transform.position, transform.rotation).Play();

            Destroy(gameObject);
        }
        else if (col.GetComponent<Character>() != null)
            return;
        else
        {
            Destroy(gameObject);
        }
    }
}
