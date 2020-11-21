using UnityEngine;

public class PoisonClaymoreMine : AbilityToDrop
{
    [SerializeField] PoisonClaymoreMineWeapon claymoreWeapon = null;
    bool used = false;

    protected override void Die()
    {
        FirePoisonArrows();
    }

    /// <summary>
    /// Sets this.character to characters sent here and subscribes to event. 
    /// </summary>
    /// <param name="character">Sets this.character to characters sent here</param>
    public override void SetCharacter(Character character)
    {
        this.character = character;
        character.Player.InputController.OnButtonDown += InputController_OnButtonDown;
    }

    /// <summary>This method is subscibed to controller onButtonDown event.
    /// If button for ability is pressed call ability method.
    /// </summary>
    /// <param name="button">Button that is pressed</param>
    private void InputController_OnButtonDown(Action button, InputController controller)
    {
        if (button == Action.Ability)
            FirePoisonArrows();
    }

    /// <summary>
    /// This method calls the "use" method of weapon attached to its ability.
    /// </summary>
    private void FirePoisonArrows()
    {
        if (!used)
        {
            used = true;
            claymoreWeapon.Use(true);
            claymoreWeapon.OnDeath += ClaymoreWeapon_OnDeath;
        }
    }

    /// <summary>
    /// This method unsubscribes "InputController_OnButtonDown" from event and destroys itself.
    /// </summary>
    private void ClaymoreWeapon_OnDeath(PoisonClaymoreMineWeapon weapon)
    {
        character.Player.InputController.OnButtonDown -= InputController_OnButtonDown;
        Destroy(gameObject);
    }

    protected override void ImpactEffect(Enemy enemy)
    {
        throw new System.NotImplementedException();
    }

    protected override void ImpactMethod()
    {
        throw new System.NotImplementedException();
    }
}
