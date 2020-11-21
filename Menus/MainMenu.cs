using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Takes care of the very most main of the menus
/// </summary>
public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance = null;
    [SerializeField] GameObject mainMenuPanel = null, localLobbyPanel = null, settingsPanel = null;
    [SerializeField] Button lastUsedButton = null;
    [SerializeField] AudioSource bgm = null;

    void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        Instance = this;
        Cursor.visible = true;
    }

    public void BackToMain()
    {
        EventSystem.current.SetSelectedGameObject(lastUsedButton.gameObject);
        lastUsedButton.OnSelect(null);

        //Music changes when entering local lobby but not when you open settings, don't restart if it's already playing
        if (bgm && !bgm.isPlaying)
            bgm.Play();

        mainMenuPanel.SetActive(true);
        localLobbyPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void UsedButton(Button button)
    {
        lastUsedButton = button;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
