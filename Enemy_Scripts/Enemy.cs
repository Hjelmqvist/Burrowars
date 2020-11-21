using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This class defines the games enemies general behaviour.
/// </summary>
[RequireComponent(typeof(Stats))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : MonoBehaviour
{
    #region Local variables
    [SerializeField] protected bool damageOnTouch = true;
    [SerializeField] protected Transform firePoint = null;
    protected bool canAttack = true;
    protected float currentModifier = 0;
    [SerializeField] protected List<EnemyAbility> abilities;

    [Space(10)]
    [SerializeField] protected float deathAnimationLength = 0;

    protected bool dead = false;
    protected bool charged = false;
    protected readonly StateMachine stateMachine = new StateMachine();
    protected Character target = null;
    protected float currentDistanceToTarget = Mathf.Infinity;

    [Space(10)]
    [SerializeField] protected Stats stats = null;
    protected Rigidbody rb = null;
    protected NavMeshAgent agent = null;

    [Space(10)]
    [SerializeField] protected GameObject ammocrate = null;
    [SerializeField] private float dropChance = 1.5f;

    [Space(10)]
    [SerializeField] protected LayerMask navmask = -1; //-1= All navmesh layers
    [SerializeField] protected LayerMask raymask = -1; //-1= All col layers
    protected Animator anim;

    protected List<Character> collisionTargets = new List<Character>();
    protected float resetHitTimer = 0;
    protected float resetHitDelay = 3;

    [Space(10)]
    [Header("ChangeMat On Damage")]
    [SerializeField] protected Material takingDamageMat;
    [SerializeField] protected Renderer objectToChangeMat;
    [SerializeField] protected float timeWithChangedMat = 0.1f;
    protected Material defaultMat;
    Coroutine changeMatCoroutine;

    protected bool updating = true;

    #endregion

    #region Properties
    public List<EnemyAbility> Abilities { get { return abilities; } }
    public bool DamageOnTouch { get { return damageOnTouch; } set { damageOnTouch = value; } }
    public bool CanAttack { get { return canAttack; } set { canAttack = value; } }
    public bool Charged { get { return charged; } set { charged = value; } }
    public float CurrentModifier { get { return currentModifier; } set { currentModifier = value; } }
    public Transform GetFirePoint { get { return firePoint; } }

    public StateMachine StateMachine { get { return stateMachine; } }
    public Character Target { get { return target; } set { target = value; } }
    public Vector3 ChargeTarget { get; set; }
    public float CurrentDistanceToTarget { get { return currentDistanceToTarget; } set { currentDistanceToTarget = value; } }

    public Stats Stats { get { return stats; } protected set { stats = value; } }
    public Rigidbody Rb { get { return rb; } protected set { rb = value; } }
    public GameObject AmmoCrate { get { return ammocrate; } protected set { ammocrate = value; } }
    public NavMeshAgent Agent { get { return agent; } protected set { agent = value; } }
    public int Navmask { get { return navmask; } set { navmask = value; } }
    public int Raymask { get { return raymask; } set { raymask = value; } }
    public Animator Anim { get { return anim; } protected set { anim = value; } }
    #endregion

    //Tell listners that an enemy has died.
    public delegate void EnemyDeath(Enemy thisEnemy);
    public event EnemyDeath OnEnemyDeath;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        resetHitTimer = Time.time;
    }

    protected virtual void Update()
    {
        if (updating)
        {
            if (!dead && !KilledAllPlayers())
                stateMachine.Update();
            else
            {
                Anim.SetBool("Running", false); //Change this later to a uniform bool dancing across all enemies. Victory dance is important!
                Agent.isStopped = true;
                updating = false;
            }
        }
    }

    /// <summary>
    /// Enemies win if all player characters are dead.
    /// </summary>
    /// <returns>True if atleast one player is alive.</returns>
    protected bool KilledAllPlayers()
    {
        Character exists = SpawnManager.characters.Find(c => !c.IsDead);
        if (exists != null)
            return false;
        else
            return true;
    }

    protected void Start()
    {
        agent.speed = stats.MovementSpeed;
        stats.BaseMovementSpeed = stats.MovementSpeed;

        stats.OnDeath += Die;
        stats.OnStatChanged += Stats_OnStatChanged;
    }

    /// <summary>
    /// Updates stuff on the enemy when stats are changed in the corresponding stats script.
    /// </summary>
    /// <param name="stat">The type of stat that changed.</param>
    /// <param name="changeValue">The value the stat was changed by.</param>
    /// <param name="newValue">The new value of the stat.</param>
    private void Stats_OnStatChanged(StatType stat, float changeValue, float newValue)
    {
        switch (stat)
        {
            case StatType.Health:
                if (changeValue < 0)
                {
                    if (changeMatCoroutine != null)
                    {
                        StopCoroutine(changeMatCoroutine);
                        objectToChangeMat.material = defaultMat;
                    }
                    changeMatCoroutine = StartCoroutine(ChangeMatCoroutine());
                }

                break;
            case StatType.MovementSpeed:
                agent.speed = newValue; //Movement speed needs to be set on the agent to change speed.
                break;
        }
    }

    /// <summary>
    /// Changes the enemy material on taking damage.
    /// </summary>
    /// <returns>Waits a specified amount of time before switching back.</returns>
    protected virtual IEnumerator ChangeMatCoroutine()
    {
        defaultMat = objectToChangeMat.material;
        objectToChangeMat.material = takingDamageMat;
        yield return new WaitForSeconds(timeWithChangedMat);
        objectToChangeMat.material = defaultMat;
    }

    /// <summary>
    /// This method gradually rotates the gameobject this script is attached to towards a given target with a specified speed. Use in update.
    /// </summary>
    /// <param name="target">The gameobject we which to look at.</param>
    /// <param name="rotationSpeed">The speed we want to rotate with.</param>
    /// <returns>Returns true if the target is valid, otherwise false.</returns>
    public bool RotateTowards(Vector3 target, float rotationSpeed)
    {
        bool targetfound = false;
        if (target != null)
        {
            target.y = transform.position.y;
            Vector3 dir = target - transform.position;
            float deltaRadians = rotationSpeed * Time.deltaTime;
            Vector3 newRotation = Vector3.RotateTowards(transform.forward, dir, deltaRadians, 0.0f);
            transform.rotation = Quaternion.LookRotation(newRotation);
            targetfound = true;
        }
        return targetfound;
    }

    /// <summary>
    /// Does stuff when colliding with other objects and players.
    /// </summary>
    /// <param name="c">The collider we have collided with.</param>
    /// <returns>True if we hit a playercharachter, otherwise false</returns>
    protected bool OnCollisionEnter(Collision col)
    {
        bool hit = false;
        Character hitPlayer = col.gameObject.GetComponent<Character>();
        Character alreadyHit = collisionTargets.Find(c => c.Equals(hitPlayer));
        if (damageOnTouch && hitPlayer != null && alreadyHit == null)
        {
            collisionTargets.Add(hitPlayer);
            hit = true;
            resetHitTimer = Time.time;
            hitPlayer.Stats.ModifyHealth(-stats.CollisionDamage);
            //Do other stuff here like knockback
        }
        else if (Time.time > resetHitTimer + resetHitDelay)
        {
            collisionTargets.Clear();
        }
        return hit;
    }

    /// <summary>
    /// This method destroys the enemy gameobject this script is attached to. Called when the corresponding stats Script calls OnDeath.
    /// </summary>
    protected virtual void Die()
    {
        agent.isStopped = true;
        dead = true;
        GetComponent<Collider>().enabled = false;
        if (anim != default(Animator))
        {
            anim.SetBool("Dead", true);
        }

        stats.OnDeath -= Die;
        OnEnemyDeath?.Invoke(this); //Tell further listners that it was an enemy that died.

        if (Random.Range(0, 100) <= dropChance && AmmoCrate != null)
            Instantiate(AmmoCrate, new Vector3(transform.position.x, 2, transform.position.z), transform.rotation);
        Destroy(gameObject, deathAnimationLength + 0.1f);
    }

    /// <summary>
    /// Raycasts forward in z-axis attempting to spot any player characters.
    /// </summary>
    /// <param name="target">The closest position on the navmesh of the spotted target. (0,0,0) if it cant be seen.</param>
    /// <param name="length">Defines how far away we can see.</param>
    /// <param name="navmask">Which navmesh layer can we move on. -1 = all layers.</param>
    /// <param name="raymask">The layermask to be used on the raycast. Set to player layer.</param>
    /// <returns>True if a player character can be seen.</returns>
    public bool CanSeeTarget(out Vector3 target, float length, int navmask, int raymask)
    {
        target = Vector3.zero;
        RaycastHit hit;
        bool canSeeTarget = false;

        if (Physics.Raycast(transform.position, transform.forward, out hit, length, raymask))
        {
            Character hitCharacter = hit.collider.gameObject.GetComponent<Character>();
            if (hitCharacter != null && !hitCharacter.IsDead)
            {
                NavMeshHit navHit;
                NavMesh.SamplePosition(hit.point, out navHit, 1, navmask);
                target = navHit.position;
                canSeeTarget = true;
            }
        }
        return canSeeTarget;
    }

    #region SquareDistance Calculations
    //We Compare SquareDistance to avoid calling expensive square root calculations.

    /// <summary>
    /// Determines if we are in attack range.
    /// </summary>
    /// <param name="currentDistanceToTarget">Our distance to the target.</param>
    /// <param name="attackrange">Our attackrange.</param>
    /// <returns>True if in range.</returns>
    public bool IsInRange(float currentDistanceToTarget, float attackrange)
    {
        return currentDistanceToTarget <= attackrange * attackrange || currentDistanceToTarget <= Stats.minAttackRange * Stats.minAttackRange;
    }

    /// <summary>
    /// This method calculates the squaredistance between an enemy and a target character in the game world.
    /// </summary>
    /// <param name="owner">This enemy.</param>
    /// <param name="character">The character we which to calculate squaredistance to.</param>
    /// <returns>The squaredistance as a float.</returns>
    public float SquareDistanceToTarget(Enemy owner, Character character)
    {
        Vector3 directionToTarget = character.transform.position - owner.transform.position;
        return directionToTarget.sqrMagnitude;
    }

    /// <summary>
    /// This helper method sets the distance of the selected target.
    /// </summary>
    /// <param name="shortestDistance">The shortest distance found to a player.</param>
    /// <returns>Returns the current distance to target.</returns>
    public float SetDistance(float shortestDistance)
    {
        return CurrentDistanceToTarget = shortestDistance;
    }
    #endregion

    /// <summary>
    /// This method checks if we died to a melee attack.
    /// Not yet implemented, but planned for use.
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool DeathOnMelee(bool b)
    {
        throw new System.NotImplementedException();
    }
}

