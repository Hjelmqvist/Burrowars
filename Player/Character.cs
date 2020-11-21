using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Stats))]
public class Character : MonoBehaviour
{
    protected Animator anim = null;
    protected Collider col = null;
    protected Rigidbody rb = null;
    
    protected Vector3 direction, rotation;

    [Header("Ranged Weapons")]
    [Space(20)]
    [SerializeField] LayerMask aimLayer = 0;
    [SerializeField] protected RangedWeapon baseWeapon = null;
    [SerializeField] protected RangedWeapon primaryWeapon = null;
    [SerializeField] protected RangedWeapon currentWeapon = null;
    [SerializeField] protected Transform weaponParent = null;

    [Header("Abilites")]
    [Space(20)]
    [SerializeField] protected MeleeWeapon meleeWeapon = null;
    [SerializeField] protected Ability ability = null;
    [SerializeField] protected Healing_Ability heal = null;

    [Header("Particle systems")]
    [Space(20)]
    [SerializeField] ParticleSystem[] systemsToColor = null;

    [Header("Stats")]
    [Space(20)]
    [SerializeField] Stats stats = null;

    public Player Player { get; protected set; }
    public bool CanMove { get; protected set; } = true;
    public bool CanAttack { get; set; } = true;
    public bool IsDead { get; protected set; }
    public Stats Stats { get { return stats; } protected set{ stats = value; } }
    public RangedWeapon CurrentWeapon { get { return currentWeapon; } }
    public RangedWeapon PrimaryWeapon { get { return primaryWeapon; } }
    public Animator Animator { get { return anim; } }

    public delegate void AmmoChanged(Magazine mag);
    public event AmmoChanged OnAmmoChanged;

    public delegate void CharacterDeath(Character character);
    public event CharacterDeath OnCharacterDeath;

    /// <summary>
    /// For MeleeWeapons to know when to be able to hit
    /// </summary>
    public delegate void MeleeWeaponCanHit(MeleeWeaponAnimationPart part);
    public event MeleeWeaponCanHit OnMeleeWeaponCanHit;

    void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        //To drag in character and test easier
        if (Player == null)
        {
            Player = new Player();
            SetupPlayer(Player);
        }

        //Subscribe to stat events
        stats.OnDeath += Stats_OnDeath;
        stats.OnStatChanged += Stats_OnStatChanged;

        //Equip base weapon if any
        if (baseWeapon != null)
        {
            baseWeapon.Equip(weaponParent);
            currentWeapon = baseWeapon;
            baseWeapon.OnAmmoChanaged += CurrentWeapon_AmmoChanged;
            OnAmmoChanged?.Invoke(baseWeapon.Magazine);
        }  
    }

    private void Stats_OnStatChanged(StatType stat, float changeValue, float newValue)
    {
        switch (stat)
        {
            case StatType.Health:
                if (changeValue < 0 && Effects.Instance)
                    Effects.Instance.Shake(.5f, 2.5f, .1f);
                break;
        }
    }

    void Update()
    {
        Player.InputController.Update();
    }

    void FixedUpdate()
    {
        Move();
        Rotate();
    }

    /// <summary>
    /// Returns false if Player is null
    /// </summary>
    public bool SetupPlayer(Player p)
    {
        rb.velocity = Vector3.zero;

        if (p == null)
            return false;

        //If a Player already exists unsubscribe to its events, should never happen.
        if (Player != null)
        {
            Player.InputController.OnCurrentAxis -= InputController_OnCurrentAxis;
            Player.InputController.OnButtonDown -= InputController_OnButtonDown;
            Player.InputController.OnButtonUp -= InputController_OnButtonUp;
        }

        Player = p;
        Player.InputController.OnCurrentAxis += InputController_OnCurrentAxis;
        Player.InputController.OnButtonDown += InputController_OnButtonDown;
        Player.InputController.OnButtonUp += InputController_OnButtonUp;

        //Change color on particlesystem to match players color
        Color c = p.Color;
        foreach (ParticleSystem system in systemsToColor)
        {
            ParticleSystem.MainModule main = system.main;
            c.a = main.startColor.color.a;
            main.startColor = c;
        }

        return true;
    }

    /// <summary>
    /// Lock/unlocks characters movement.
    /// </summary>
    /// <param name="b">Locks character movement if true</param>
    public bool LockMovement(bool b)
    {
        return CanMove = !b;
    }

    //TODO: Make one Equip method for each type of equippable
    /// <summary>
    /// Returns false if Weapon is null
    /// </summary>
    public bool EquipWeapon(Weapon weap)
    {
        if (weap == null)
            return false;

        if (weap is RangedWeapon)
        {
            //Equip new weapon
            RangedWeapon rangedWeap = weap as RangedWeapon;
            anim.SetBool("Primary", true);
            currentWeapon?.IsBeingUsed(false);
            rangedWeap.Equip(weaponParent);

            if (PrimaryWeapon != null)
                Destroy(PrimaryWeapon.gameObject);

            currentWeapon = primaryWeapon = rangedWeap;
            primaryWeapon.OnAmmoChanaged += CurrentWeapon_AmmoChanged;
            OnAmmoChanged?.Invoke(primaryWeapon.Magazine);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Calls the OnAmmoChanged event when the current weapons OnAmmoChanged event is called
    /// </summary>
    /// <param name="mag">The current magazine</param>
    void CurrentWeapon_AmmoChanged(Magazine mag)
    {
        OnAmmoChanged?.Invoke(mag);
    }

    void InputController_OnCurrentAxis(Axis axis, float value, InputController controller)
    {
        if (!IsDead)
        {
            switch (axis)
            {
                case Axis.MoveHorizontal:
                    direction.x = value;
                    break;

                case Axis.MoveVertical:
                    direction.z = value;
                    break;

                case Axis.RotateHorizontal:
                    rotation.x = value;
                    break;

                case Axis.RotateVertical:
                    rotation.z = value;
                    break;

                case Axis.Shoot:
                    if (CanAttack && value > 0)
                        currentWeapon?.Use(true);
                    else
                        currentWeapon?.Use(false);
                    break;
            }
        }
    }

    void InputController_OnButtonDown(Action button, InputController controller)
    {
        if (CanAttack && !IsDead)
        {
            switch (button)
            {
                case Action.Start:
                    if (controller.Controller != ControllerType.Keyboard)
                        PauseMenu.Instance?.Pause(Player.InputController);
                    break;

                case Action.Cancel:
                    if (controller.Controller == ControllerType.Keyboard)
                        PauseMenu.Instance?.Pause(Player.InputController);
                    break;

                case Action.Shoot:
                    if (CurrentWeapon)
                        CurrentWeapon.Use(true);
                    break;

                case Action.Reload:
                    if (CurrentWeapon)
                        CurrentWeapon.Reload();
                    break;

                case Action.ChangeWeapon:
                    ChangeWeapon();
                    break;

                case Action.Melee:
                    if (meleeWeapon)
                        meleeWeapon.Use(true);
                    break;

                case Action.Ability:
                    if (ability)
                        ability.Use(this);
                    break;

                case Action.Heal:
                    if (heal)
                        heal.Use(this);
                    break;
            }
        }
    }

    void InputController_OnButtonUp(Action button, InputController controller)
    {
        if (!IsDead)
        {
            switch (button)
            {
                case Action.Shoot:
                    currentWeapon?.Use(false);
                    break;
            }
        }
    }

    void Move()
    {
        if (!IsDead)
        {
            Vector3 direction = this.direction;
            if (CanMove)
            {
                if (Math.Abs(direction.x) != 1)
                    direction.x = 0;
                if (Math.Abs(direction.z) != 1)
                    direction.z = 0;
            }
            else
                direction = Vector3.zero;

            if (direction.x == 0 && direction.z == 0)
                SetRunningAnimBools(false, false, false, 0);
            else
                PlayRunAnimation();

            direction = Vector3.ClampMagnitude(direction, 1);
            Vector3 newPosition = transform.position + direction * stats.MovementSpeed * Time.deltaTime;

            rb.velocity = Vector3.zero;
            rb.MovePosition(newPosition);
        }
    }

    bool PlayRunAnimation()
    {
        float angle = Vector3.Angle(direction, transform.forward);
        bool right = Vector3.Dot(transform.right, direction) > 0;   //we get angle as 0-180 so we must determine which side we are looking towards

        switch (right)
        {
            case true:
                if (angle < 30)
                    SetRunningAnimBools(true, false, false, 1);
                else if (angle < 90)
                    SetRunningAnimBools(false, true, false, 1);
                else if (angle < 150)
                    SetRunningAnimBools(false, true, false, -1);
                else
                    SetRunningAnimBools(true, false, false, -1);
                break;

            case false:
                if (angle < 30)
                    SetRunningAnimBools(true, false, false, 1);
                else if (angle < 90)
                    SetRunningAnimBools(false, false, true, 1);
                else if (angle < 150)
                    SetRunningAnimBools(false, false, true, -1);
                else
                    SetRunningAnimBools(true, false, false, -1);
                break;
        }
        return right;
    }

    /// <summary>
    /// Sets animator parameters for movement
    /// </summary>
    /// <param name="run">Is character running?</param>
    /// <param name="strafeRight">Is character walking sideways to the right?</param>
    /// <param name="strafeLeft">Is character walking sideways to the left?</param>
    /// <param name="direction">Is character moving forwarads or backwards? 1 or -1</param>
    /// <returns>Returns true if character is moving</returns>
    bool SetRunningAnimBools(bool run, bool strafeRight, bool strafeLeft, float direction)
    {
        bool usingBaseWeapon = currentWeapon.WeaponType == WeaponType.Base;
        bool usingPrimaryWeapon = currentWeapon.WeaponType == WeaponType.Primary;

        anim.SetBool("RunBase", usingBaseWeapon && run);
        anim.SetBool("RunPrimary", usingPrimaryWeapon && run);

        anim.SetBool("StrafeRightBase", usingBaseWeapon && strafeRight);
        anim.SetBool("StrafeRightPrimary", usingPrimaryWeapon && strafeRight);

        anim.SetBool("StrafeLeftBase", usingBaseWeapon && strafeLeft);
        anim.SetBool("StrafeLeftPrimary", usingPrimaryWeapon && strafeLeft);

        anim.SetFloat("Direction", direction);

        return !run && !strafeRight && !strafeLeft; //return idle animation= true/ false
    }

    void Rotate()
    {
        if (IsDead)
            return;

        switch (Player.InputController.Controller)
        {
            case ControllerType.Keyboard: //Keyboard is a specialcase because using mouse to aim
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, aimLayer) && Vector3.Distance(hit.point, transform.position) > 1)
                {
                    Vector3 dir = hit.point;
                    transform.LookAt(dir);
                }
                break;

            case ControllerType.Joystick1:
            case ControllerType.Joystick2:
            case ControllerType.Joystick3:
            case ControllerType.Joystick4:
                Vector3 rotation = this.rotation;
                if (Mathf.Abs(rotation.x) < 0.1f)
                    rotation.x = 0;
                if (Mathf.Abs(rotation.z) < 0.1f)
                    rotation.z = 0;

                if (rotation.x != 0 || rotation.z != 0)
                {
                    Vector3 direction = new Vector3(rotation.x, 0, rotation.z);
                    Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);
                    transform.rotation = rot;
                }
                break;
        }
    }

    void ChangeWeapon()
    {
        RangedWeapon otherWeapon;
        if (currentWeapon == primaryWeapon && baseWeapon != null)
        {
            otherWeapon = baseWeapon;
            anim.SetBool("Primary", false);
        }
        else if (currentWeapon == baseWeapon && primaryWeapon != null)
        {
            otherWeapon = primaryWeapon;
            anim.SetBool("Primary", true);
        }
        else        
            return;

        //Deactivate current weapon
        currentWeapon.OnAmmoChanaged -= CurrentWeapon_AmmoChanged;
        currentWeapon.IsBeingUsed(false);

        currentWeapon = otherWeapon;

        //Activate new weapon
        currentWeapon.Reset();
        currentWeapon.OnAmmoChanaged += CurrentWeapon_AmmoChanged;
        currentWeapon.IsBeingUsed(true);
        OnAmmoChanged?.Invoke(CurrentWeapon.Magazine);
    }

    //TODO: Change to waitforseconds in meleeweapons
    /// <summary>
    /// Tells weapon animation what to do at each point of animation
    /// </summary>
    void MeleeWeaponAnimation(int i)
    {
        OnMeleeWeaponCanHit?.Invoke((MeleeWeaponAnimationPart)i);
    }

    //TODO: Change to waitforseconds in abilities
    void AbilityAnimationCall()
    {
        ability?.AnimationCall();
    }

    /// <summary>
    /// Called when character dies
    /// </summary>
    void Stats_OnDeath()
    {
        IsDead = true;
        anim.SetBool("Dead", true);
        col.enabled = false;
        rb.isKinematic = true;

        OnCharacterDeath?.Invoke(this);
    }

    /// <summary>
    /// Used to respawn the character 
    /// </summary>
    /// <param name="spawnpoint"></param>
    public void Respawn(Vector3 spawnpoint)
    {
        if (IsDead)
        {
            IsDead = false;
            anim.SetBool("Dead", false);

            LockMovement(false);
            col.enabled = true;
            rb.isKinematic = false;

            if (primaryWeapon)
            {
                primaryWeapon.gameObject.SetActive(true);
                primaryWeapon.Reset();
            }

            stats.ModifyHealth(int.MaxValue);
            if (currentWeapon != baseWeapon)
                ChangeWeapon();
            transform.position = spawnpoint;
        }
    }
}

public enum CharacterType
{
    NONE = -1,
    Assault,
    Pyro,
    Technician,
    Hunter
}