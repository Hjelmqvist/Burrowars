using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the input from one player, takes care of character selection
/// </summary>
public class LocalLobbyPanel : MonoBehaviour
{
    [SerializeField] Image panelImage = null;
    [SerializeField] GameObject noPlayerPanel = null, readyPanel = null;
    [SerializeField] ScrollRect scroll = null;

    Player player = new Player();
    float horizontalValue = 0;

    public bool IsReady { get; protected set; }
    public bool IsActive { get; protected set; }
    bool hasChanged = false;

    public delegate void ReadyChanged();
    public event ReadyChanged OnReadyChanged = null;

    public Player GetPlayer() {  return player; }

    void Update()
    {
        //Have to let go of stick/button a little bit before character can change again
        if (hasChanged && Mathf.Abs(horizontalValue) != 1)
            hasChanged = false;   
    }

    /// <summary>
    /// Opens the panel with chosen settings
    /// </summary>
    public void Activate(InputController inputController, Color32 color, CharacterType characterType = CharacterType.Assault)
    {
        if (player.InputController != null)
        {
            player.InputController.OnCurrentAxis -= InputController_OnAxisChanged;
            player.InputController.OnButtonDown -= InputController_OnButtonDown;
        }

        IsActive = true;
        IsReady = false;
        inputController.OnCurrentAxis += InputController_OnAxisChanged;
        inputController.OnButtonDown += InputController_OnButtonDown;

        //Set player settings
        player.SetInputController(inputController);
        player.ChangeCharacterType(characterType);
        player.SetColor(color);

        //Sets character
        StopAllCoroutines();
        float target = 1 / (float)CharacterType.Hunter * (float)player.CharacterType;
        scroll.horizontalNormalizedPosition = target;

        panelImage.color = color;
        noPlayerPanel.SetActive(false);
        readyPanel.SetActive(false);
    }

    /// <summary>
    /// Closes the panel and opens the "Press to play" window
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        if (player.InputController != null)
        {
            player.InputController.OnCurrentAxis -= InputController_OnAxisChanged;
            player.InputController.OnButtonDown -= InputController_OnButtonDown;
            player.SetInputController(null);
        }
        noPlayerPanel.SetActive(true);
        readyPanel.SetActive(false);

        StopAllCoroutines();
        scroll.horizontalNormalizedPosition = 0;
    }

    private void InputController_OnAxisChanged(Axis axis, float value, InputController controller)
    {
        switch (axis)
        {
            //Change character with left stick (left and right) or A/D/Left/Right
            case Axis.MoveHorizontal:
                if (!IsReady)
                {
                    horizontalValue = value;
                    if (Mathf.Abs(horizontalValue) != 1)
                        return;

                    if (!hasChanged)
                    {
                        StopAllCoroutines();
                        StartCoroutine(ChangeCharacter((int)horizontalValue));
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Change character to previous or next if possible, -1 or +1
    /// </summary>
    IEnumerator ChangeCharacter(int i)
    {
        if (player.ChangeCharacterType(player.CharacterType + i))
        {
            hasChanged = true;
            float target = 1 / (float)CharacterType.Hunter * (float)player.CharacterType;
            while (scroll.horizontalNormalizedPosition != target)
            {
                scroll.horizontalNormalizedPosition = Mathf.Lerp(scroll.horizontalNormalizedPosition, target, 0.2f);
                yield return null;
            }
        }  
    }

    void InputController_OnButtonDown(Action button, InputController controller)
    {
        switch (button)
        {
            case Action.Start:
            case Action.Interact:
                if (IsActive)
                    ModifyReady(true);
                break;

            case Action.Cancel:
                if (IsReady) //If ready go back to character select
                    ModifyReady(false);
                else //Else deactivate the panel (player left)
                    InputManager.Instance.RemoveController(controller);
                break;
        }
    }

    /// <summary>
    /// Sets if the player is ready or not
    /// </summary>
    void ModifyReady(bool b)
    {
        if (IsReady == b)
            return;

        IsReady = b;
        readyPanel.SetActive(IsReady);
        OnReadyChanged?.Invoke();
    }
}
