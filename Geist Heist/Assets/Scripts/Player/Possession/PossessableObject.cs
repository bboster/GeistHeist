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
using UnityEngine.Events;
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
    [SerializeField] public float maxChargePercentage = 100;
    [Tooltip("The percentage the timer recharges each interval while the player is not possessing.")]
    [SerializeField, ShowIf(nameof(hasTimer))] private float timerRechargePercentage = 10;
    [Tooltip("The percentage the timer decreases each interval while the player is possessing.")]
    [SerializeField, ShowIf(nameof(hasTimer))] private float timerDischargePercentage = 10;


    private Coroutine dischargeCoroutine = null;
    private Coroutine rechargeCoroutine;

    [Tooltip("Location where the ghost spawns after leaving the possessable.")]
    public Transform ghostSpawnPoint;

    [Header("Materials")]
    [SerializeField, Required, ShowAssetPreview(16, 16)] private Material PossessedMaterial;
    [SerializeField, Required, ShowAssetPreview(16, 16)] private Material UnpossessedMaterial;

    [HideInInspector] public bool CanUnPossess = true;
    private Coroutine unpossessCoroutine=null;
    private MeshRenderer meshRenderer;

    [ReadOnly] private float currentTimerPercentage = 100f;
    [HideInInspector] public UnityEvent<float> OnTimerUpdate = new();

    void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (UnpossessedMaterial != null)
            meshRenderer.material = UnpossessedMaterial;
        else
            Debug.LogWarning("No unpossession material for " + gameObject.name);
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

    /// <summary>
    /// Called in PlayerManager when player enters object
    /// </summary>
    public void OnPossessionStart()
    {
        gameObject.SetActive(true);
        InputHandler.OnPossessionStart();

        if(PossessedMaterial != null)
            meshRenderer.material = PossessedMaterial;
        else
            Debug.LogWarning("No possession material for "+gameObject.name);

        if (unpossessCoroutine == null)
            unpossessCoroutine = StartCoroutine(WaitForUnpossess());

        if (hasTimer)
        {
            if(rechargeCoroutine != null)
            {
                StopCoroutine(rechargeCoroutine);
                rechargeCoroutine = null;
            }

            if(dischargeCoroutine == null)
                dischargeCoroutine = StartCoroutine(StartDischarge());
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

        if (PossessedMaterial != null)
            meshRenderer.material = UnpossessedMaterial;
        else
            Debug.LogWarning("No unpossession material for " + gameObject.name);

        InputHandler.OnPossessionEnded();

        if (hasTimer)
        {
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
            currentTimerPercentage = Mathf.Max(currentTimerPercentage - (timerDischargePercentage * Time.deltaTime), 0);
            OnTimerUpdate.Invoke(currentTimerPercentage);
            yield return null;
        }

        OnTimerFinished();
    }

    private IEnumerator StartRecharge()
    {
        if (!hasTimer)
            yield break;

        while(currentTimerPercentage < maxChargePercentage)
        {
            currentTimerPercentage = Mathf.Min(currentTimerPercentage + (timerRechargePercentage * Time.deltaTime), maxChargePercentage);
            OnTimerUpdate.Invoke(currentTimerPercentage);
            yield return null;
        }
    }

    private void OnTimerFinished()
    {
        PlayerManager.Instance.PossessGhost(gameObject.transform.GetComponent<PossessableObject>());

        if (dischargeCoroutine != null)
        {
            OnTimerUpdate.Invoke(currentTimerPercentage);
            StopCoroutine(dischargeCoroutine);
            dischargeCoroutine = null;
        }
    }
    #endregion
}
