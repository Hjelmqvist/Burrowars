//From https://forum.unity.com/threads/button-keyboard-and-mouse-highlighting.294147/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Add to all UI buttons, sliders, toggles etc.
/// </summary>
[RequireComponent(typeof(Selectable))]
public class HighlightFix : MonoBehaviour, IPointerEnterHandler, IDeselectHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!EventSystem.current.alreadySelecting)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        GetComponent<Selectable>().OnPointerExit(null);
    }
}
