/*
 * Contributors: Toby, Sky, Skylar
 * Creation Date: 9/16/25
 * Last Modified: 10/22/25
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
    [Tooltip("The time in seconds between each percentage update.")]
    [SerializeField, ShowIf(nameof(hasTimer))] private float timerRechargeInterval = 2f;
    [Tooltip("The percentage the timer recharges each interval while the player is not possessing.")]
    [SerializeField, ShowIf(nameof(hasTimer))] [Range(0, 100)] private int timerRechargePercentage = 10;
    [Tooltip("The time in seconds between each percentage update while discharging.")]
    [SerializeField, ShowIf(nameof(hasTimer))] private float timerDischargeInterval = 1f;
    [Tooltip("The percentage the timer decreases each interval while the player is possessing.")]
    [SerializeField, ShowIf(nameof(hasTimer))] [Range(0, 100)] private int timerDischargePercentage = 10;

    private float currentTimerPercentage = 100f;
    private bool canUpdateTimer;
    [SerializeField] private Image timerImage => GameManager.Instance.TimerImage;
    private RawImage timerBackground => GameManager.Instance.TimerBackground;
    private Coroutine dischargeCoroutine = null;
    private Coroutine rechargeCoroutine;

    [HideInInspector] public bool CanUnPossess = true;
    private Coroutine unpossessCoroutine=null;

    [Tooltip("Location where the ghost spawns after leaving the possessable.")]
    public Transform ghostSpawnPoint;

    public IInputHandler GetInputHandler()
    {
        inputHandler = inputHandler == null ? GetComponent<IInputHandler>(): inputHandler;
        return inputHandler;
    }

    void IInteractable.Interact()
    {
        PlayerManager.Instance.PossessObject(this);
    }

    /// <summary>
    /// Called in PlayerManager when player enters object
    /// </summary>
    public void OnPossessionStart()
    {
        InputHandler.OnPossessionStart();


        if (unpossessCoroutine == null)
            unpossessCoroutine = StartCoroutine(WaitForUnpossess());

        if (hasTimer)
        {
            canUpdateTimer = true;
            timerImage?.gameObject.SetActive(true);
            timerBackground?.gameObject.SetActive(true);
            
            if(rechargeCoroutine != null)
            {
                StopCoroutine(rechargeCoroutine);
                rechargeCoroutine = null;
            }

            if(dischargeCoroutine == null)
                dischargeCoroutine = StartCoroutine(StartDischarge());
        }
        else
        {
            timerImage?.gameObject.SetActive(false);
            timerBackground?.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called in PlayerManager when player exits object
    /// </summary>
    public void OnPossessionEnded()
    {
        if (!CanUnPossess)
        {
            Debug.LogError("Trying to unpossess early");
            return;
        }

        InputHandler.OnPossessionEnded();

        if (hasTimer)
        {
            canUpdateTimer = false;
            if (dischargeCoroutine != null)
            {
                StopCoroutine(dischargeCoroutine);
                dischargeCoroutine = null;
            }

            if(rechargeCoroutine == null)
            {
                rechargeCoroutine = StartCoroutine(StartRecharge());
            }
        }
    }

    /// <summary>
    /// Called in PlayerManager while player is in this object
    /// </summary>
    public void WhilePossessingUpdate()
    {
        
        // Nothing right now
        
    }

    public IEnumerator WaitForUnpossess()
    {
        CanUnPossess = false;
        yield return new WaitForEndOfFrame();
        CanUnPossess = true;
        unpossessCoroutine = null;
    }

    #region Timer
    
    private IEnumerator StartDischarge()
    {
        if (!hasTimer)
            yield break;

        while(currentTimerPercentage > 0)
        {
            currentTimerPercentage = Mathf.Max(currentTimerPercentage - timerDischargePercentage, 0);
            if (canUpdateTimer)
            {
                UpdateSlider();
            }
            yield return new WaitForSeconds(timerDischargeInterval);
        }

        OnTimerFinished();
    }

    private IEnumerator StartRecharge()
    {
        if (!hasTimer)
            yield break;

        while(currentTimerPercentage < 100f)
        {
            currentTimerPercentage = Mathf.Min(currentTimerPercentage + timerRechargePercentage, 100f);
            if(canUpdateTimer)
            {
                UpdateSlider();
            }
            yield return new WaitForSeconds(timerRechargeInterval);
        }
    }

    private void UpdateSlider()
    {
        if (timerImage != null)
        {
            Debug.Log(currentTimerPercentage / 100f);
            timerImage.fillAmount = currentTimerPercentage / 100f;
        }
    }

    private void OnTimerFinished()
    {
        PlayerManager.Instance.PossessGhost(gameObject.transform.GetComponent<PossessableObject>());

        if (dischargeCoroutine != null)
        {
            StopCoroutine(dischargeCoroutine);
            dischargeCoroutine = null;
        }
    }
    #endregion
}
