using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// Handles the local lobby where players choose characters
/// </summary>
public class LocalLobby : MonoBehaviour
{
    public static LocalLobby Instance { get; protected set; }

    [SerializeField] List<LocalLobbyPanel> playerPanels = new List<LocalLobbyPanel>();
    [SerializeField] Text startTimer = null;
    bool controlsUpdatedThisFrame = false;

    [SerializeField] List<Color32> playerColors = null;

    void Awake()
    {
        //Singleton
        if (Instance != null)
            Destroy(gameObject);
        Instance = this;
    }

    void Update()
    {
        //Check start input for keyboard and joystick 1-4
        for (ControllerType i = ControllerType.Keyboard; i <= ControllerType.Joystick4; i++)
        {
            if (Input.GetButtonDown(string.Format("{0} {1}", i, Action.Start.ToString())) || 
                Input.GetButtonDown(string.Format("{0} {1}", i, Action.Interact.ToString())))
                AddController(i);
        }

        //Need a check to not go back to main menu immediately when pressing the Cancel button
        if (controlsUpdatedThisFrame)
            controlsUpdatedThisFrame = false;
        else if (InputManager.Instance.GetControllers().Count == 0)
        {
            //Check if it should go back to main menu
            for (ControllerType i = ControllerType.Keyboard; i <= ControllerType.Joystick4; i++)
            {
                if (Input.GetButtonDown(string.Format("{0} {1}", i, Action.Cancel.ToString())))
                    MainMenu.Instance.BackToMain();
            }
        }
    }

    /// <summary>
    /// Adds a new InputController for controller
    /// </summary>
    /// <param name="controller">Which controller it is, Keyboard or Joystick1-4</param>
    void AddController(ControllerType controller)
    {
        List<InputController> playerControllers = InputManager.Instance.GetControllers();

        //Fails if the controller is already being used
        foreach (InputController player in playerControllers)
            if (player != null && player.Controller == controller)
                return;

        //Else create a new controller
        InputManager.Instance.AddController(new InputController(controller));
    }

    /// <summary>
    /// Subscribes to panels and InputManager
    /// </summary>
    void OnEnable()
    {
        foreach (LocalLobbyPanel panel in playerPanels)
            panel.OnReadyChanged += StartCheck;
        InputManager.Instance.OnChange += Refresh;
    }

    /// <summary>
    /// Unsubscribes to panels and InputManager
    /// </summary>
    void OnDisable()
    {
        foreach (LocalLobbyPanel panel in playerPanels)
            panel.OnReadyChanged -= StartCheck;
        InputManager.Instance.OnChange -= Refresh;
    }

    /// <summary>
    /// Starts the countdown if all players are ready</para>
    /// Called from panels OnReadyChanged event
    /// </summary>
    void StartCheck()
    {
        //Stop current countdown if there is one
        StopAllCoroutines();
        startTimer.gameObject.SetActive(false);
        List<LocalLobbyPanel> activePanels = playerPanels.Where((p) => p.IsActive).ToList();

        //Controls that at least one panel is active and all active panels are ready
        if (activePanels.Count > 0 && activePanels.TrueForAll((p) => p.IsReady)) 
            StartCoroutine(StartGame());
    }

    /// <summary>
    /// Start the game countdown
    /// </summary>
    IEnumerator StartGame()
    {
        //Start from 3.99 to get 3, 2, 1
        float timeLeft = 3.99f;
        startTimer.gameObject.SetActive(true);
        while (timeLeft > 1)
        {
            startTimer.text = string.Format("{0}", (int)timeLeft);
            yield return null;
            timeLeft -= Time.deltaTime;
        }

        //Takes all the players and adds them to SpawnManagers list of players
        List<Player> players = new List<Player>();
        foreach (LocalLobbyPanel panel in playerPanels)
        {
            if (panel.IsActive)
            {
                Player p = panel.GetPlayer();
                p.InputController.UnsubscribeAll();
                players.Add(p);
            }   
        }
        SpawnManager.players = players;
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Updates the panels info when controllers are added or removed.
    /// </summary>
    void Refresh(List<InputController> inputControllers)
    {
        controlsUpdatedThisFrame = true;
        for (int i = 0; i < playerPanels.Count; i++)
        {
            //If panel should be activated
            if (i < inputControllers.Count)
            {
                //If player moved to a lower number panel. Ex. move player 3 to panel 2 when player 2 leaves
                if (i + 1 < playerPanels.Count && inputControllers[i] == playerPanels[i + 1].GetPlayer().InputController)
                {
                    Player p = playerPanels[i].GetPlayer();
                    if (!PlayerWasMoved(p, inputControllers))
                        playerColors.Add(p.Color);
                    p = playerPanels[i + 1].GetPlayer();
                    playerPanels[i].Activate(p.InputController, p.Color, p.CharacterType);
                } 
                //Else just activate it
                else if (playerPanels[i].GetPlayer().InputController != inputControllers[i])
                {
                    Color32 randomColor = playerColors[Random.Range(0, playerColors.Count - 1)];
                    playerColors.Remove(randomColor);
                    playerPanels[i].Activate(inputControllers[i], randomColor);
                } 
            }
            //Else deactivate panel
            else if (playerPanels[i].IsActive)
            {
                Player p = playerPanels[i].GetPlayer();
                if (!PlayerWasMoved(p, inputControllers))
                    playerColors.Add(p.Color);
                playerPanels[i].Deactivate();
            }   
        }
        StartCheck();
    }

    /// <summary>
    /// Returns true if player was moved, false if player was removed.
    /// </summary>
    bool PlayerWasMoved(Player p, List<InputController> inputControllers)
    {
        return inputControllers.Any((controller) => controller == p.InputController);
    }
}
