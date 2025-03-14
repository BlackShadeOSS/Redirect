using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject mainmenu;
    [SerializeField] private GameObject selectplayer;

    public void Start()
    {
        mainmenu.SetActive(true);
        selectplayer.SetActive(false);
    }

    public void loadNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void loadPeviousScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void loadPlayerSelection()
    {
        mainmenu.SetActive(false);
        selectplayer.SetActive(true);
    }
    public void loadMainMenu()
    {
        mainmenu.SetActive(true);
        selectplayer.SetActive(false);
    }
    
    public void exitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }
}
