using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// On players healthbars
/// </summary>
public class Healthbar : MonoBehaviour
{
    [SerializeField] Character character = null;
    [SerializeField] Image healthbar = null;

    void Awake()
    {
        character.Stats.OnStatChanged += Stats_OnStatChanged;
    }

    private void Stats_OnStatChanged(StatType stat, float changeValue, float newValue)
    {
        switch (stat)
        {
            case StatType.Health:
                healthbar.fillAmount = 1 / (float)character.Stats.MaxHealth * newValue;
                break;
        }
    }
}
