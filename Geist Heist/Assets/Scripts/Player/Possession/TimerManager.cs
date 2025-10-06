/*
 * Contributors: Skylar
 * Creation Date: 10/5/25
 * Last Modified: 10/5/25
 * 
 * Brief Description: Handles possession timer
 */
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimerManager : Singleton<TimerManager>
{
    public event Action OnTimerFinished;

    [SerializeField] private Slider timerSlider;
    [Tooltip("Refers to the time a player can possess an object for")]
    [SerializeField] private float timerTime;
    private float currentTimerTime;
    private bool isTimerActive = false;

    public bool IsTimerActive => currentTimerTime > 0;

    void Update()
    {
        if (isTimerActive)
        {
            currentTimerTime -= Time.deltaTime;
            if (currentTimerTime <= 0)
            {
                currentTimerTime = 0;
                isTimerActive = false;
                OnTimerFinished?.Invoke();
            }
            UpdateSlider();
        }
    }

    public void StartTimer()
    {
        currentTimerTime = timerTime;
        isTimerActive = true;
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        if (timerSlider != null)
        {
            timerSlider.value = currentTimerTime / timerTime;
        }
    }
}
