using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Rigidbody))]
public abstract class AbilityProjectile : MonoBehaviour
{
    [SerializeField] float duration = 0;
    protected bool hasTriggered = false;
    protected bool canTick = true;

    protected Collider col = null;
    protected Renderer rend = null;
    protected Rigidbody rb = null;

    private void Awake()
    {
        Invoke("Die", duration);
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>This method destroys this gameObject.
    /// 
    /// </summary>
    protected virtual void Die() { Destroy(gameObject); }

    /// <summary>This method adds force to projectile.
    /// 
    /// </summary>
    public virtual void Shoot(float height, float force)
    {
        rb.AddForce((Vector3.up * height) + (transform.forward * force), ForceMode.Impulse);
    }

    /// <summary>This methods executes orders on impact with other colliders.
    /// It is for starting the grenades mechanics and partical effects on explosion. 
    /// </summary>
    /// <param name="col"></param>
    protected virtual void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.root.GetComponent<Character>() == null)
        {
            col.enabled = false;
            rend.enabled = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
            transform.rotation = Quaternion.identity;
            ImpactMethod();
        }
    }

    protected abstract void ImpactMethod();
}
