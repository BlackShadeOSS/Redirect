using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{

    [Header("Menu Pannels")]
    public GameObject MenuPanel;
    public GameObject OptionsPanel;
    public GameObject CreditsPanel;
    public GameObject PadPanel;
    public GameObject MultiplierManager;
    public LocalMultiplayerControllerSelection localMultiplayerControllerSelection;
    public void Start()
    {
        CloseAllPanels();
        MenuPanel.SetActive(true);
        localMultiplayerControllerSelection = MultiplierManager.GetComponent<LocalMultiplayerControllerSelection>();
        Debug.LogWarning(localMultiplayerControllerSelection);
    }

    public void StartGame()
    {
        Debug.LogWarning(localMultiplayerControllerSelection.SelectedDevices);
        localMultiplayerControllerSelection.SaveSelectedDevices();
        Debug.LogWarning(GlobalInputDeviceManager.SelectedDevices);
        SceneManager.LoadScene("Gra"); //do nazwy dopraw/ zostaw 1 zeby działalo/ daj scene gry jako 1 w play specs
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
        PadPanel.SetActive(false);
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
    public void OpenController()
    {
        CloseAllPanels();
        PadPanel.SetActive(true);
    }
       
    
}
