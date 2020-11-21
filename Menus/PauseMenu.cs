using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Takes care of ingame pause menu, only the pausing controller (and mouse) can pick
/// </summary>
public class PauseMenu : MonoBehaviour
{ 
    public static PauseMenu Instance;

    [SerializeField] StandaloneInputModule inputmodule = null;
    [SerializeField] GameObject pausePanel = null;
    [SerializeField] Button firstButton = null;

    bool isPaused;
    Player p;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Pause(InputController pausedController)
    {
        if (!isPaused)
        {
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
            firstButton.OnSelect(null);

            Time.timeScale = 0;
            Cursor.visible = true;
            isPaused = true;
            pausePanel.SetActive(true);

            string controller = pausedController.Controller.ToString();
            inputmodule.horizontalAxis = controller + " MoveHorizontal";
            inputmodule.verticalAxis = controller + " MoveVertical";
            if (pausedController.Controller != ControllerType.Keyboard)
                inputmodule.submitButton = controller + " Interact";
            else
                inputmodule.submitButton = controller + " Start";
            inputmodule.cancelButton = controller + " Cancel";

            foreach (Character c in SpawnManager.characters)
                c.enabled = false;
        }
    }

    public void Unpause()
    {
        inputmodule.horizontalAxis = "Horizontal";
        inputmodule.verticalAxis = "Vertical";
        inputmodule.submitButton = "Submit";
        inputmodule.cancelButton = "Cancel";

        Time.timeScale = 1;
        Cursor.visible = false;
        isPaused = false;
        pausePanel.SetActive(false);

        foreach (Character c in SpawnManager.characters)
            c.enabled = true;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
