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
    [SerializeField, Required] Slider timerSlider;

    private IInputHandler GetInputHandler()
    {
        return inputHandler = inputHandler == null ? GetComponent<IInputHandler>() : inputHandler;
    }


    private void Update()
    {
        if (isTimerActive && hasTimer)
        {
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
