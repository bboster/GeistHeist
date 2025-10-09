/*
 * Contributors: Toby, Sky
 * Creation Date: 9/16/25
 * Last Modified: 9/18/25
 * 
 * Brief Description: On every possessable object, and the player for simplicity. 
 * Contains reference to input scripts and other stuff.
 *  
 *  TODO:
 */
using UnityEngine;
using Unity.Cinemachine;
using NaughtyAttributes;
using UnityEngine.UI;
using System;

public class PossessableObject : MonoBehaviour, IInteractable
{
    [HideInInspector] public IInputHandler InputHandler => GetInputHandler();
    [HideInInspector] private IInputHandler inputHandler;
    [Required] public CinemachineCamera CinemachineCamera;
    [Header("Timer Variables")]
    [SerializeField] private bool hasTimer;
    [SerializeField] private float timerTime = 5f;
    private float currentTimerTime;
    private bool isTimerActive;
    [SerializeField] private Slider timerSlider => GameManager.Instance.TimerSlider;

    private void Update()
    {
        if (isTimerActive && hasTimer)
        {
            timerSlider.gameObject.SetActive(true);
            currentTimerTime -= Time.deltaTime;
            if (currentTimerTime <= 0)
            {
                currentTimerTime = 0;
                isTimerActive = false;
                OnTimerFinished();
            }
            UpdateSlider();
        }
    }

    public IInputHandler GetInputHandler()
    {
        inputHandler = inputHandler == null ? GetComponent<IInputHandler>(): inputHandler;
        return inputHandler;
    }

    void IInteractable.Interact()
    {
        PlayerManager.Instance.PossessObject(this);
    }

    // Called in PlayerManager
    public void OnPossessionStarted()
    {
        InputHandler.OnPossessionStarted();
        if(timerSlider != null)
        {
            timerSlider.gameObject.SetActive(true);
        }
        isTimerActive = true;
        currentTimerTime = timerTime;
    }

    // Called in PlayerManager
    public void OnPossessionEnded()
    {
        InputHandler.OnPossessionEnded();
        if(timerSlider != null)
        {
            timerSlider.gameObject.SetActive(false);
            ResetTimer();
        }
    }

    #region Timer
    private void OnTimerFinished()
    {
        PlayerManager.Instance.PossessGhost(gameObject.transform.GetComponent<PossessableObject>());
        ResetTimer();
        timerSlider.gameObject.SetActive(false);
    }

    private void UpdateSlider()
    {
        if (timerSlider != null)
        {
            timerSlider.value = currentTimerTime / timerTime;
        }
    }

    private void ResetTimer()
    {
        currentTimerTime = timerTime;
        isTimerActive = false;
        timerSlider.gameObject.SetActive(false);
    }
    #endregion
}
