using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinerGame : MonoBehaviour
{
    [SerializeField] private GameObject minigameRendererObject;
    [SerializeField] private GameObject minigameBackgroundRendererObject;
    [SerializeField] private List<Sprite> keyboardPrompts;
    [SerializeField] private List<Sprite> gamepadPrompts;
    [SerializeField] private List<Sprite> completionSprites;
    [SerializeField] private AudioSource dzwienkKopania;
    [SerializeField] private AudioSource dzwienkJebania;
    [SerializeField] private int promptCount = 8;
    
    private SpriteRenderer minigameRenderer;
    private SpriteRenderer backgroundRenderer;
    private playerCrystalManager playerCrystalManager;
    private GameObject crystal;
    
    private bool isGamepad = true;
    private bool gameRunning = false;
    private int currentPrompt = 0;
    private int inputtedPrompt = -1;
    [SerializeField] public int currentPromptCount = 0;
    
    private void Start()
    {
        this.minigameRenderer = minigameRendererObject.GetComponent<SpriteRenderer>();
        this.playerCrystalManager = GetComponent<playerCrystalManager>();
        this.backgroundRenderer = minigameBackgroundRendererObject.GetComponent<SpriteRenderer>();
        this.isGamepad = GetComponent<Player>().isGamepadControl();
        minigameRenderer.sprite = null;
    }
    
    public void startGame(GameObject crystal)
    {
        this.crystal = crystal;
        this.gameRunning = true;
        this.currentPromptCount = 0;
        // Generate the first prompt for a gamepad
        this.generatePrompt( 0); // Assuming the first prompt is correct
    }


    public void abortGame()
    {
        this.gameRunning = false;
        minigameRenderer.sprite = null;
        backgroundRenderer.sprite = null;
        playerCrystalManager.stopMining();
    }
    
    public void endGame()
    {
        this.gameRunning = false;
        minigameRenderer.sprite = null;
        backgroundRenderer.sprite = null;
        playerCrystalManager.stopMining();
        this.crystal.GetComponent<Crystal>().mine(gameObject);
    }
    
    private void generatePrompt(int correctButton)
    {
        if (currentPromptCount >= promptCount)
        {
            endGame();
            return;
        }
        this.currentPrompt = randomizePrompt();
        this.currentPromptCount++;
        dzwienkKopania.Play();
        if (this.isGamepad)
        {
            this.minigameRenderer.sprite = gamepadPrompts[this.currentPrompt];
        }
        else
        {
            this.minigameRenderer.sprite = gamepadPrompts[this.currentPrompt];
        }
        this.backgroundRenderer.sprite = completionSprites[this.currentPromptCount-1];
        Debug.Log($"Current sprite: {this.minigameRenderer.sprite}");
        
        this.inputtedPrompt = -1; // Reset inputted prompt
    }

    private void Update()
    {
        if (gameRunning && inputtedPrompt != -1)
        {
            if (inputtedPrompt == currentPrompt)
            {
                generatePrompt(randomizePrompt());
            }
            else
            {
                dzwienkJebania.Play();
                StartCoroutine(ShakePrompt());
            }
            inputtedPrompt = -1; // Reset inputted prompt
        }
    }

    void OnB1(InputValue value)
    {
        this.inputtedPrompt = 0;
    }
    
    void OnB2(InputValue value)
    {
        this.inputtedPrompt = 1;
    }
    
    void OnB3(InputValue value)
    {
        this.inputtedPrompt = 2;
    }
    
    void OnB4(InputValue value)
    {
        this.inputtedPrompt = 3;
    }
    
    private void OnUnInteract(InputValue value)
    {
        this.abortGame();
    }
    
    private int randomizePrompt()
    {
        return UnityEngine.Random.Range(0, 4);
    }

    IEnumerator ShakePrompt()
    {
        Vector3 originalPosition = minigameRenderer.transform.position;
        for (int i = 0; i < 5; i++)
        {
            minigameRenderer.transform.position = originalPosition + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), 0);
            yield return new WaitForSeconds(0.1f);
        }
        minigameRenderer.transform.position = originalPosition;
    }

}
