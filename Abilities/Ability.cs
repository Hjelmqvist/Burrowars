using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [SerializeField] protected float cooldown = 10;
    protected Character character = null;
    protected float lastTimeUsed = Mathf.NegativeInfinity;
    protected bool usedOnce = false;

    //TODO: Make Use send a generic type instead.
    //Make dropAbilities inherit from ability.
    public abstract bool Use(Character character);

    /// <summary> Returns bool depending on if the player can use its ability.
    /// Checks if cooldown for ability is done.
    /// </summary>
    public bool CanUse()
    {
        return Time.time > lastTimeUsed + cooldown;
    }

    public abstract void AnimationCall();
}

