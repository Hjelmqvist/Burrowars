using System.Collections;
using UnityEngine;

public abstract class AbilityToDrop : MonoBehaviour
{
    [SerializeField] float duration = 0;
    [SerializeField] protected float impactRadius = 0;
    [SerializeField] protected int impactDamage = 0;
    [SerializeField] protected float pausedTime = 0;

    [Header("Camera shake")]
    [Space(20)]
    [SerializeField] protected float amplitude = 1;
    [SerializeField] protected float frequency = 5;
    [SerializeField] protected float shakeDuration = 0.1f;

    [Header("Particle Systems")]
    [Space(20)]

    [SerializeField] protected ParticleSystem impactEffect = null;
    [SerializeField] protected ParticleSystem overtimeEffect = null;

    [SerializeField] protected AudioSource soundEffect = null;

    protected Collider col = null;
    protected Renderer rend = null;
    protected Rigidbody rb = null;

    protected Character character = null;
    protected bool hasTriggered = false;

    private void Awake()
    {
        Invoke("Die", duration);
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>This method adds forward momentum to ability object.
    /// 
    /// </summary>
    /// <param name="force">is value is used to add momentum to ability object</param>
    public virtual void Shoot(float force)
    {
        rb.velocity = transform.forward * force;
    }

    /// <summary>Sets this.character to characters sent here
    /// 
    /// </summary>
    /// <param name="character">Sets this.character to characters sent here</param>
    public virtual void SetCharacter(Character character)
    {
        this.character = character;
    }

    /// <summary> this method is used for developers.
    /// 
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }

    protected abstract void ImpactEffect(Enemy enemy);
    protected virtual void Die() { Destroy(gameObject); }
    protected abstract void ImpactMethod();

    /// <summary> coroutine used to pouse game time for enhanced effect(feeling of big explosion)
    /// 
    /// </summary>
    protected IEnumerator TimePause()
    {
        float f = Time.unscaledTime + pausedTime;
        Time.timeScale = 0;
        yield return new WaitUntil(() => Time.unscaledTime > f);
        Time.timeScale = 1;
    }
    
    /// <summary>This method is used for camera shake.
    /// Enhance feeling of explosion.
    /// </summary>
    protected void Shake()
    {
        Effects.Instance.Shake(amplitude, frequency, shakeDuration);
    }
}
