using System.Collections;
using UnityEngine;

public class PoisonClaymoreMineWeapon : AutoWeap
{
    [SerializeField] float shootDuration = 0;
    bool activated = false;

    public delegate void Death(PoisonClaymoreMineWeapon weapon);
    public event Death OnDeath;

    /// <summary> this method activates the poison mine.
    /// 
    /// </summary>
    public override void Use(bool use)
    {
        if (!activated && use)
        {
            activated = true;
            StartCoroutine(UseCoroutine());
        }  
    }

    /// <summary> Coroutine that starts firing poison darts.
    /// when the coroutine is called it will invoke "Die" method when shootDuration is 0.
    /// </summary>
    protected override IEnumerator UseCoroutine()
    {
        Invoke("Die", shootDuration);
        while (true)
        {
            if (shootEffect != null)
                shootEffect.Play();
            if (shootDropEffect != null)
                shootDropEffect.Play();

            for (int i = 0; i < 3; i++)
            {
                NormalBullet b = Instantiate(bulletPrefab, bulletSpawnpoint.position, transform.root.rotation);
                b.BulletRotation(Random.Range(minStrayFactor, maxStrayFactor));
            }

            shootSound.PlayAudio();

            yield return new WaitForSeconds(1 / usesPerSecond);
        }
    }

    /// <summary> This method destroys this object.
    /// 
    /// </summary>
    void Die()
    {
        OnDeath?.Invoke(this);
    }
}
