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


    public void Start()
    {
        CloseAllPanels();
        MenuPanel.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Gra"); // game scene name
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
