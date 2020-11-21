using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Takes care of ingame panels which shows player information
/// </summary>
public class IngamePanelManager : MonoBehaviour
{
    public static IngamePanelManager Instance { get; protected set; }

    [SerializeField] IngamePanel[] ingamePanels = null;
    [SerializeField] GameObject victoryScreen = null, defeatScreen = null;
    [SerializeField] Button victoryFirst = null, defeatFirst = null;

    void Awake()
    {
        //Singleton
        if (Instance != null)
            Destroy(gameObject);
        Instance = this;
    }

    /// <summary>
    /// Sets all IngamePanels
    /// </summary>
    /// <param name="characters">Players characters</param>
    public void AddCharacters(List<Character> characters)
    {
        for (int i = 0; i < ingamePanels.Length; i++)
        {
            if (i < characters.Count)
                ingamePanels[i].SetCharacter(characters[i]);
            else
                ingamePanels[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Call when players win
    /// </summary>
    public void Victory()
    {
        EventSystem.current.SetSelectedGameObject(victoryFirst.gameObject);
        victoryFirst.OnSelect(null);

        foreach (IngamePanel panel in ingamePanels)
            panel.gameObject.SetActive(false);
        victoryScreen.SetActive(true);
    }

    /// <summary>
    /// Call when players lose
    /// </summary>
    public void Defeat()
    {
        EventSystem.current.SetSelectedGameObject(defeatFirst.gameObject);
        defeatFirst.OnSelect(null);

        foreach (IngamePanel panel in ingamePanels)
            panel.gameObject.SetActive(false);
        defeatScreen.SetActive(true);
    }

    #region Buttons

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit()
    {
        Application.Quit();
    }

    #endregion
}
