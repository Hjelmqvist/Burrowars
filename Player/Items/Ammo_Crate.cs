using UnityEngine;

public class Ammo_Crate : MonoBehaviour
{
    [Space(10)]
    [Header("Effects")]
    [SerializeField] ParticleSystem[] rareEffects = null;

    [Space(10)]
    [Header("Mag Strength")]
    private int magCountFill;
    private int crateRarity;
    [SerializeField] int magCountFillCommon = 3;
    [SerializeField] int magCountFillrare = 5;
    [SerializeField] int magCountFillLegendary = 10;

    private void Awake()
    {
        SetCrateStrength();
        PlayCorrectEffectMethod();
    }

    /// <summary> This method plays the right effect depending on its rarity.
    /// 
    /// </summary>
    private void PlayCorrectEffectMethod()
    {
        switch (crateRarity)
        {
            case 0:
                if (rareEffects[0] != null)
                    rareEffects[0].Play();
                break;
            case 1:
                if (rareEffects[1] != null)
                    rareEffects[1].Play();
                break;
            case 2:
                if (rareEffects[2] != null)
                    rareEffects[2].Play();
                break;
            default:
                break;
        }
    }

    /// <summary>This method sets crate rarity.
    /// Depending on the random value in "randomNumber" it sets crate rarity.
    /// </summary>
    private void SetCrateStrength()
    {
        int randomNumber = Random.Range(1,101);
        if (randomNumber <= 11)
        {
            magCountFill = magCountFillLegendary;
            crateRarity = 2;
        }
        if (randomNumber > 11 && randomNumber <= 41)
        {
            magCountFill = magCountFillrare;
            crateRarity = 1;
        }
        if (randomNumber > 41)
        {
            magCountFill = magCountFillCommon;
            crateRarity = 0;
        }
    }

    /// <summary> This method gives characters ammo when colliding with type Character.
    /// All characters alive gets ammo.
    /// Players gets ammo depending on crates rarity.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.root.GetComponent<Character>())
        {
            foreach (Character character in SpawnManager.characters)
            {
                if (character.PrimaryWeapon != null && !character.IsDead)
                {
                    character.PrimaryWeapon.Magazine.AddAmmoFromCrate(magCountFill);
                    character.PrimaryWeapon.OnAmmoChanaged?.Invoke(character.PrimaryWeapon.Magazine);
                } 
            }
            Destroy(gameObject);
        }
    }
}
