using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Class sets up general stats that can be used amongst many different objects, such as enemies and player characters.
/// </summary>
public class Stats : MonoBehaviour
{
    #region Local Variables
    [Space(10)]
    [Header("CombatStats")]
    private int maxhealth;
    public static float minAttackRange = 1.5f;  //Melee range
    [SerializeField] private int currency = 0;
    [SerializeField] private int health = 0, shield = 0;
    [SerializeField] private int collisionDamage = 0;
    [SerializeField] private int attackDamage = 0;
    [SerializeField] private float damageModifier = 0;
    [SerializeField] private float power = 0;
    [SerializeField] private float attackRange = 0;
    [SerializeField] private float knockBack = 5;

    [Space(10)]
    [Header("Speeds")]
    [SerializeField] private float attackSpeed = 0;
    [SerializeField] private float windUpSpeed = 0;
    [SerializeField] private float movementSpeed = 0;
    [SerializeField] private float rotationSpeed = 0;
    [SerializeField] private float chargeSpeed = 10;
    private float baseMovementSpeed = 0;

    [Space(10)]
    [Header("Dot Effects")]
    [SerializeField] private ParticleSystem poisonParticle = null;
    [SerializeField] private ParticleSystem fireParticle = null;

    [SerializeField] private List<StatusEffect> statuses = null;

    #endregion

    #region Properties
    public int MaxHealth { get { return maxhealth; } set { maxhealth = value; } }
    public int Health
    {
        get { return health; }
        private set
        {
            health = value;
            if (health > maxhealth)
                health = maxhealth;
            else if (health <= 0)
            {
                health = 0;
                OnDeath?.Invoke();
            }
        }
    }
    public int Shield { get { return shield; } private set { shield = value; } }
    public int Currency { get { return currency; } set { currency = value; } }
    public int CollisionDamage { get { return collisionDamage; } set { collisionDamage = value; } }
    public int AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    public float DamageModifier { get { return damageModifier; } set { damageModifier = value; } }
    public float Power { get { return power; } set { power = value; } }
    public float AttackRange { get { return attackRange; } set { attackRange = value; } }
    public float KnockBack { get { return knockBack; } set { knockBack = value; } }

    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    public float WindUpSpeed { get { return windUpSpeed; } set { windUpSpeed = value; } }
    public float MovementSpeed { get { return movementSpeed; } set { movementSpeed = value; } }
    public float BaseMovementSpeed { get { return baseMovementSpeed; } set { baseMovementSpeed = value; } }
    public float RotationSpeed { get { return rotationSpeed; } set { rotationSpeed = value; } }
    public float ChargeSpeed { get { return chargeSpeed; } set { chargeSpeed = value; } }

    public ParticleSystem PoisonParticle { get { return poisonParticle; }}
    public ParticleSystem FireParticle { get { return fireParticle; } }

    public List<StatusEffect> Statuses { get { return statuses; } }

    #endregion

    //Tell listners when the object dies/ is destroyed.
    public delegate void Death();
    public event Death OnDeath;

    //Tell listners when a stat has been changed.
    public delegate void StatChanged(StatType stat, float changeValue, float newValue);
    public event StatChanged OnStatChanged;

    void Awake()
    {
        maxhealth = health;
        baseMovementSpeed = movementSpeed;
        statuses = new List<StatusEffect>();
    }

    /// <summary>
    /// Positive values add currency, negative values subtract currency..
    /// </summary>
    /// <param name="value">The value wich will be added or subtracted.</param>
    /// <returns>True if currency is less than 0.</returns>
    public bool ModifyCurrency(int value)
    {
        Currency += value;
        OnStatChanged?.Invoke(StatType.Currency, value, Currency);
        if (Currency < 0)
        {
            Currency = 0;
            return true;
        }
        else if (Currency > 9999)
            Currency = 9999;
        return false;
    }

    /// <summary>
    /// Negative values does damage, positive values heals.
    /// Damage is dealth to active shields first.
    /// </summary>
    /// <param name="value">The value which will be added or subtracted.</param>
    /// <returns>Returns true if object has died.</returns>
    public bool ModifyHealth(int value)
    {
        if (value < 0)
        {
            Health += ModifyShield(value);  //If we are taking damage, reduce the shield first, then deal any leftover damage
        }
        else
            Health += value;                //otherwise increase our health
        OnStatChanged?.Invoke(StatType.Health, value, Health);
        if (Health <= 0)
            return true;
        return false;
    }

    /// <summary>
    /// Sets the movementspeed to a new value.
    /// </summary>
    /// <param name="value">The new movementspeed to set.</param>
    /// <returns>True if the movementspeed is greater than 0.</returns>
    public bool ModifyMovementSpeed(float value)
    {
        MovementSpeed = value;
        OnStatChanged?.Invoke(StatType.MovementSpeed, value, MovementSpeed);
        if (movementSpeed <= 0)
        {
            movementSpeed = 0;
            return false;
        }
        return true;          
    }

    /// <summary>
    /// Negative values does damage to the shield only, positive values adds shield.
    /// </summary>
    /// <param name="value">The value which will be added or subtracted.</param>
    /// <returns>Any potential damage exceeding the shield</returns>
    public int ModifyShield(int value)
    {
        int dmg = 0;    //Return damage if it exceeds the shield limit
        Shield += value;
        OnStatChanged?.Invoke(StatType.Shield, value, Shield);
        if (Shield <= 0)
        {
            dmg = shield;
            shield = 0;
        }
        return dmg;
    }

    /// <summary>
    /// Applies a statusEffect to this object.
    /// </summary>
    /// <param name="statusEffect">The effect to apply.</param>
    public void AddStatuses(StatusEffect statusEffect)
    {
        statuses.Add(statusEffect);
        StartCoroutine(statusEffect.Status(this));
    }

    /// <summary>
    /// Resets the duration of an active statseffect if it is reaplied.
    /// </summary>
    /// <param name="effect">The effect to reset duration on.</param>
    public void ResetTicks(StatusEffect effect)
    {
        if (statuses.Contains(effect))
        {
            effect.ResetTicks();
        }
    }

    /// <summary>
    /// Removes a statuseffect from this object.
    /// </summary>
    /// <param name="effect">The effect to remove.</param>
    public void RemoveEffectFromList(StatusEffect effect)
    {
        if (statuses.Contains(effect))
        {
            statuses.Remove(effect);
        }
    }
}

public enum StatType
{
    Currency,
    Health,
    MovementSpeed,
    Shield
}
