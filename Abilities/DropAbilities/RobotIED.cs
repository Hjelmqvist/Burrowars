using System.Collections;
using UnityEngine;

/// <summary>
/// This script contains the RobotIED ability used by the Technician class.
/// Attach this script to the RobotIED prefab.
/// </summary>
public class RobotIED : AbilityToDrop
{
    [SerializeField] float force = 0;
    [SerializeField] float timeBeforeImpact = 0;

    private bool isAlive = true;

    private void Update()
    {
        if (isAlive)
            Shoot(force); 
    }

    /// <summary>
    /// Kills the robot and trigger the impact effect.
    /// Called after a specified lifetime of the robot.
    /// </summary>
    protected override void Die()
    {
        ImpactMethod();
    }

    /// <summary>
    /// Sets a player as the owner of the robotIED, and adds a listener for button presses.
    /// </summary>
    /// <param name="character">The character which will own the robotIED.</param>
    public override void SetCharacter(Character character)
    {
        this.character = character;
        character.Player.InputController.OnButtonDown += InputController_OnButtonDown;
    }

    /// <summary>
    /// Triggers the robotIED to explode on buttonpress.
    /// </summary>
    /// <param name="button">The button which will trigger the method.</param>
    /// <param name="controller">The controller which we will listen for inputs on.</param>
    private void InputController_OnButtonDown(Action button, InputController controller)
    {
        if (button == Action.Ability)
            ImpactMethod();
    }

    /// <summary>
    /// Explodes the robotIED, playing visual effects and dealing damage after a delay.
    /// </summary>
    protected override void ImpactMethod()
    {
        if (isAlive)
        {
            character.Player.InputController.OnButtonDown -= InputController_OnButtonDown;
            isAlive = false;
            if (impactEffect != null)
            {
                impactEffect.transform.SetParent(null);
                impactEffect.Play();
            }

            if (overtimeEffect != null)
                overtimeEffect.Play();

            rb.velocity = Vector3.zero;
            StartCoroutine(WaitBeforeImpactDamage());
        }
    }

    /// <summary>
    /// Damages enemies in range after a specified time, and aplies effects such as screenshake.
    /// </summary>
    /// <returns>Waits for a specified time.</returns>
    IEnumerator WaitBeforeImpactDamage()
    {
        yield return new WaitForSeconds(timeBeforeImpact);

        if (Effects.Instance != null)
        {
            Effects.Instance.Pause(pausedTime);
            Effects.Instance.Shake(amplitude, frequency, shakeDuration);
        }

        if (soundEffect != null)
            soundEffect.Play();

        Collider[] colForImpactDamage = Physics.OverlapSphere(transform.position, impactRadius);
        foreach (Collider col in colForImpactDamage)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
                ImpactEffect(enemy);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Stops the robot from moving when colliding with a wall.
    /// </summary>
    /// <param name="collider">The hit collider.</param>
    protected virtual void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.root.GetComponent<Character>() == null &&  collider.transform.root.GetComponent<Enemy>() == null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    /// <summary>
    /// This effect is aplied on impact.
    /// </summary>
    /// <param name="enemy">The enemy to effect.</param>
    protected override void ImpactEffect(Enemy enemy)
    {
        enemy.Stats.ModifyHealth(-impactDamage);
    }
}
