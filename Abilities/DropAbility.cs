using UnityEngine;

public class DropAbility : Ability
{
    [SerializeField] float force = 0;
    [SerializeField] AbilityToDrop[] traps = null;
    Renderer rend = null;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    /// <summary> This methods sets values to this character and then starts the use of ability.
    /// "character.Animator.SetBool("Ability", true);" = sätter characters ability parameter till true och den startar ability.
    /// </summary>
    /// <param name="character">Sets to this character to use</param>
    public override bool Use(Character character)
    {
        if (CanUse())
        {
            this.character = character;
            character.CanAttack = false;
            character.LockMovement(true);
            character.CurrentWeapon.gameObject.SetActive(false);
            lastTimeUsed = Time.time;
            character.Animator.SetBool("Ability", true);
            return true;
        }
        return false;
    }

    /// <summary>This method sets character values and instatiates a random trap.
    /// 
    /// </summary>
    public override void AnimationCall()
    {
        character.CanAttack = true;
        character.LockMovement(false);
        character.CurrentWeapon.gameObject.SetActive(true);
        character.CurrentWeapon.Reset();
        character.Animator.SetBool("Ability", false);

        Vector3 pos = new Vector3(transform.root.position.x, 0.5f, transform.root.position.z);
        AbilityToDrop a = Instantiate(traps[Random.Range(0, traps.Length)], pos, transform.root.rotation);
        a.SetCharacter(character);
        a.Shoot(force);
    }
}
