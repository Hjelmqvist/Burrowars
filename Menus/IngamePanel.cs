using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sits on the panels that shows players information in the game
/// </summary>
public class IngamePanel : MonoBehaviour
{
    [SerializeField] Image coloredBorder = null; //Border to color with players colors.
    [SerializeField] TextMeshProUGUI health = null, currency = null, ammo = null;
    [SerializeField] Sprite magazineOn = null, magazineOff = null;
    [SerializeField] Image[] magazines = null;
    Character c = null;

    const string XOUTOFY = "{0}/{1}"; //Used to faster change formatting

    /// <summary>
    /// Sets values to characters and subscribes to events</para>
    /// Updates values in UI when events are invoked
    /// </summary>
    /// <param name="character">The character the panel will listen to</param>
    public void SetCharacter(Character character)
    {
        c = character;

        //Subscribe to events
        c.Stats.OnStatChanged += Stats_OnStatChanged;
        c.OnAmmoChanged += Weapon_OnAmmoChanged;

        //Change color of panel/border
        Color color = c.Player.Color;
        color.a = coloredBorder.color.a;
        coloredBorder.color = color;

        //Set all values
        health.text = string.Format(XOUTOFY, c.Stats.Health, c.Stats.MaxHealth);
        currency.text = c.Stats.Currency.ToString();
        ammo.text = string.Format(XOUTOFY, c.CurrentWeapon.Magazine.currentBullets, c.CurrentWeapon.Magazine.size);
    }

    /// <summary>
    /// Update UI when stats changes
    /// </summary>
    /// <param name="stat">Stat that changed</param>
    private void Stats_OnStatChanged(StatType stat, float changeValue, float newValue)
    {
        switch (stat)
        {
            case StatType.Health:
                health.text = string.Format(XOUTOFY, newValue, c.Stats.MaxHealth);
                break;

            case StatType.Currency:
                currency.text = newValue.ToString();
                break;
        }
    }

    /// <summary>
    /// Update UI when ammo changed
    /// </summary>
    /// <param name="mag">The current weapons magazine</param>
    private void Weapon_OnAmmoChanged(Magazine mag)
    {
        ammo.text = string.Format(XOUTOFY, mag.currentBullets, mag.size);
        int fullMags = mag.currentAmmoCount / mag.size;
        for (int i = 0; i < magazines.Length; i++)
        {
            if (i < fullMags)
                magazines[i].sprite = magazineOn;
            else
                magazines[i].sprite = magazineOff;
        }
    }
}
