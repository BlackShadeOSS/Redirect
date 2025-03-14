using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{

    [Header("Menu Pannels")]
    public GameObject MenuPanel;
    public GameObject OptionsPanel;
    public GameObject CreditsPanel;

    public void StartGame()
    {
        SceneManager.LoadScene(1); //do nazwy dopraw/ zostaw 1 zeby działalo/ daj scene gry jako 1 w play specs
        Debug.Log("Vrum v");
    }
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
       #endif 
    }

    public void CloseAllPanels()
    {
        MenuPanel.SetActive(false);
        OptionsPanel.SetActive(false);
        CreditsPanel.SetActive(false);
    }

    public void OpenMenuPanel()
    {
        CloseAllPanels();
        MenuPanel.SetActive(true);
    }

    public void OpenOptionsPanel()
    {
        CloseAllPanels();
        OptionsPanel.SetActive(true);
    }

    public void OpenCreditsPanel()
    {
        CloseAllPanels();
        CreditsPanel.SetActive(true);
    }

    public void Return()
    {
        CloseAllPanels();
        MenuPanel.SetActive(true);
    }
    
}
