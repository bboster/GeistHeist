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
using System.Collections;

public class PossessableObject : MonoBehaviour, IInteractable
{
    [HideInInspector] public IInputHandler InputHandler => GetInputHandler();
    [HideInInspector] private IInputHandler inputHandler;
    [Required] public CinemachineCamera CinemachineCamera;
    [Header("Timer Variables")]
    [SerializeField] private bool hasTimer;
    [SerializeField, ShowIf(nameof(hasTimer))] private float timerTime = 5f;
    private float currentTimerTime;
    private bool isTimerActive = false;
    [SerializeField] private Slider timerSlider => GameManager.Instance.TimerSlider;
    private Coroutine timerCoroutine;

    [HideInInspector] public bool CanUnPossess = true;
    private Coroutine unpossessCoroutine;

    private void Update()
    {
        /*
        if (isTimerActive && hasTimer)
        {
            timerSlider.gameObject.SetActive(true);
            currentTimerTime -= Time.deltaTime;
            if (currentTimerTime <= 0)
            {
                currentTimerTime = 0;
                //isTimerActive = false;
                OnTimerFinished();
            }
            UpdateSlider();
        }*/

        if (isTimerActive && hasTimer)
        {
            currentTimerTime -= Time.deltaTime;
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


        if (unpossessCoroutine == null)
        {
            unpossessCoroutine = StartCoroutine(WaitForUnpossess());
        }

        if (!gameObject.TryGetComponent<ThirdPersonInputHandler>(out ThirdPersonInputHandler obj))
        {
            if (timerSlider != null)
            {
                timerSlider.gameObject.SetActive(true);
            }

            isTimerActive = true;


            if (timerCoroutine == null && hasTimer)
            {

                currentTimerTime = timerTime;
                timerCoroutine = StartCoroutine(TimerCountdown());
            }
        }
    }

    // Called in PlayerManager
    public void OnPossessionEnded()
    {
        InputHandler.OnPossessionEnded();

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        if (timerSlider != null)
        {
            ResetTimer();
        }
    }

    public IEnumerator WaitForUnpossess()
    {
        CanUnPossess = false;
        yield return new WaitForEndOfFrame();
        CanUnPossess = true;
        unpossessCoroutine = null;
    }

    #region Timer
    private void OnTimerFinished()
    {
        PlayerManager.Instance.PossessGhost(gameObject.transform.GetComponent<PossessableObject>());

        if(timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

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

    private IEnumerator TimerCountdown()
    {
        /*while (isTimerActive)
        {
            currentTimerTime -= Time.deltaTime;
            Debug.Log("CURRENT TIMER TIME " + currentTimerTime);
            if (currentTimerTime <= 0)
            {
                currentTimerTime = 0;
                OnTimerFinished();
            }

            UpdateSlider();
            yield return null;
        }
        timerCoroutine = null;
        yield break;*/

        yield return new WaitForSeconds(timerTime);
        OnTimerFinished();
    }
    #endregion
}
