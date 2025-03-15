using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float totalTime = 180f; // 3 minutes in seconds
    
    private float remainingTime;
    private bool isTimerRunning = true;

    void Start()
    {
        // If timer text reference is not set in inspector, try to get it from this GameObject
        if (timerText == null)
        {
            timerText = GetComponent<TextMeshProUGUI>();
            if (timerText == null)
            {
                Debug.LogError("Timer script requires a TextMeshProUGUI component.");
                enabled = false;
                return;
            }
        }
        
        // Initialize timer
        remainingTime = totalTime;
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            // Decrease time
            remainingTime -= Time.deltaTime;
            
            // Check if time is up
            if (remainingTime <= 0)
            {
                remainingTime = 0;
                isTimerRunning = false;
                EndGame();
            }
            
            // Update the display
            UpdateTimerDisplay();
        }
    }
    
    void UpdateTimerDisplay()
    {
        // Format time as minutes:seconds
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    void EndGame()
    {
        Debug.Log("Koniec czasu! Gra zakończona.");
        
        // You can implement your game ending logic here:
        // Option 1: Load a "Game Over" scene
        // SceneManager.LoadScene("GameOver");
        
        // Option 2: Show a game over panel
        // gameOverPanel.SetActive(true);
        
        // Option 3: Just quit the game (for testing)
        // Application.Quit();
    }
}