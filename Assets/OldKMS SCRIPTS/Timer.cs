using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private float maxtime = 150.0f; 
    [SerializeField] private float time = 30.0f; // czas w sekundach
    [SerializeField] private float timeFactor = 1.0f; // współczynnik przyspieszenia czasu
    [SerializeField] private bool isTimerPaused = false;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private float timeToAdd = 10.0f;
    [SerializeField] private Player player;
    private float totalTime = 0.0f;
    void Start()
    {
        
    }
    void Update()
    {
        if (!isTimerPaused)
        {
            time -= Time.deltaTime * timeFactor;
            if (time <= 0)
            {
                time = 0;
                isTimerPaused = true;
                Debug.Log("Time's up!");
                player.playerDead();
            }
            
            // update text
            timeText.text = time.ToString("F2");
            
            // update slider
            timeSlider.value = time;
            timeSlider.maxValue = maxtime;
        }
    }
    public void addTime()
    {
        time += timeToAdd;
        totalTime += timeToAdd;
        time = Mathf.Clamp(time, 0, maxtime);
    }
    public void pauseTimer()
    {
        isTimerPaused = true;
    }
    public void resumeTimer()
    {
        isTimerPaused = false;
    }

    public float getTotalTime()
    {
        return this.totalTime;
    }
}
