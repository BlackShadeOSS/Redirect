using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float totalTime = 180f; // 3 minutes in seconds
    public ScoreBoardScript scoreBoardScript;
    
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
        
        // Get score board script
        scoreBoardScript = GameObject.Find("ScoreBoard").GetComponent<ScoreBoardScript>();
        
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
                scoreBoardScript.EndGame();
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
}