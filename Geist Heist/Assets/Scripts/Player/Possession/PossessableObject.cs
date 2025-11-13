

/*
 * Contributors: Toby, Sky, Skylar
 * Creation Date: 9/16/25
 * Last Modified: 11/12/2025
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
using System.Collections.Generic;

public class PossessableObject : MonoBehaviour, IInteractable
{
    [Header("Camera Settings")]
    [Tooltip("If true, camera will be overridden with the CinemachineCamera on this object")]
    [HideIf(nameof(isGhost))] public bool HasCustomCameraBehavior;
    [Required, ShowIf(nameof(showCinemachineCamera))] public CinemachineCamera CinemachineCamera;
    [Required, HideIf(nameof(HasCustomCameraBehavior))] public Transform cameraAnchor;

    [Tooltip("Locations where the ghost could exit the possessable. Keep above exit point as last as a backup. NOT NEEDED FOR GHOST OR TETHERS.")]
    [HideIf(nameof(isGhost))] public List<Transform> ghostExitPoints;

    [Header("Timer Variables")]
    [SerializeField, HideIf(nameof(isGhost))] private bool hasTimer;
    [SerializeField, HideIf(nameof(isGhost))] public float maxChargePercentage = 100;
    [Tooltip("The percentage the timer recharges each interval while the player is not possessing.")]
    [SerializeField, ShowIf(nameof(hasTimerAndIsNotGhost))] private float timerRechargePercentage = 10;
    [Tooltip("The percentage the timer decreases each interval while the player is possessing.")]
    [SerializeField, ShowIf(nameof(hasTimerAndIsNotGhost))] private float timerDischargePercentage = 10;

    [Tooltip("Location where the ghost spawns after leaving the possessable.")]
    [HideIf(nameof(isGhost))] public Transform ghostSpawnPoint;

    [Header("Materials")]
    [SerializeField, Required, ShowAssetPreview(16, 16), HideIf(nameof(isGhost))] private Material PossessedMaterial;
    [SerializeField, Required, ShowAssetPreview(16, 16), HideIf(nameof(isGhost))] private Material UnpossessedMaterial;

    [Header("Other")]
    [SerializeField] private bool isGhost = false;

    [HideInInspector] public bool CanUnPossess = true;
    public IInputHandler InputHandler => GetInputHandler();
    private IInputHandler inputHandler;

    private Coroutine dischargeCoroutine = null;
    private Coroutine rechargeCoroutine;
    private Coroutine unpossessCoroutine=null;

    private MeshRenderer meshRenderer;

    [ReadOnly] private float currentTimerPercentage;
    [HideInInspector] public UnityEvent<float> OnTimerUpdate = new();

    #region Guard Detection Variables

    [ReadOnly] public bool IsMoving = false;

    public static Action OnActionPerformed;
    public static Action OnObjectLeft;

    #endregion

    #region Inspector Debug

    private bool showCinemachineCamera => isGhost || HasCustomCameraBehavior;
    private bool hasTimerAndIsNotGhost => isGhost == false && hasTimer;

    #endregion

    void Start()
    {
        currentTimerPercentage = maxChargePercentage;

        if (ghostExitPoints.Count == 0)
        {
            Debug.Log("No exit points set for " + this);
        }

        meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (UnpossessedMaterial != null)
            meshRenderer.material = UnpossessedMaterial;
        else
            Debug.LogWarning("No unpossession material for " + gameObject.name);

        if(possessableCanvas == null)
            possessableCanvas = gameObject.GetComponentInChildren<Canvas>();

        if (possessableCanvas != null)
        {
            possessableCanvasGroup = possessableCanvas.gameObject.GetOrAddComponent<CanvasGroup>();
            possessableCanvasGroup.alpha = 0;
            possessableCanvas.gameObject.SetActive(false);
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

    /// <summary>
    /// Called in PlayerManager when player enters object
    /// </summary>
    public void OnPossessionStart()
    {
        StaticUtilities.StopAndStartCoroutine(ref fadeOpacityCoroutine, ShowAndEnableCanvas());

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
        StaticUtilities.StopAndStartCoroutine(ref fadeOpacityCoroutine, HideAndDisableCanvas());

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
        OnObjectLeft?.Invoke();

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

    #region Canvas

    [Header("Canvas settings")]
    [SerializeField] private Canvas possessableCanvas;
    [SerializeField] private float showSeconds = 1;
    [SerializeField] private float hideSeconds = 0.3f;

    private CanvasGroup possessableCanvasGroup;
    private Coroutine fadeOpacityCoroutine;

    private IEnumerator ShowAndEnableCanvas()
    {
        if (possessableCanvas == null)
            yield break;

        possessableCanvas.gameObject.SetActive(true);
        while(possessableCanvasGroup.alpha < 1)
        {
            possessableCanvasGroup.alpha += Time.deltaTime / showSeconds;
            yield return null;
        }
    }

    private IEnumerator HideAndDisableCanvas()
    {
        if (possessableCanvas == null)
            yield break;

        while (possessableCanvasGroup.alpha > 0)
        {
            possessableCanvasGroup.alpha -= Time.deltaTime / hideSeconds;
            yield return null;
        }
        possessableCanvas.gameObject.SetActive(false);
    }

    #endregion
}
