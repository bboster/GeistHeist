/*
 * Contributors: Toby, Sky
 * Creation Date: 9/16/25
 * Last Modified: 10/28/25
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

    [Tooltip("Location where the ghost spawns after leaving the possessable.")]
    public Transform ghostSpawnPoint;
    [Header("Timer Variables")]
    [SerializeField] private bool hasTimer;
    [SerializeField, ShowIf(nameof(hasTimer))] private float timerTime = 5f;

    [Header("Materials")]
    [SerializeField, Required, ShowAssetPreview(16, 16)] private Material PossessedMaterial;
    [SerializeField, Required, ShowAssetPreview(16, 16)] private Material UnpossessedMaterial;

    private float currentTimerTime;
    private Slider timerSlider => GameManager.Instance.TimerSlider;
    private Coroutine timerCoroutine;

    [HideInInspector] public bool CanUnPossess = true;
    private Coroutine unpossessCoroutine=null;
    private MeshRenderer meshRenderer;

    #region Guard Detection Variables

    public bool IsMoving = false;

    public static Action OnActionPerformed;
    public static Action OnObjectLeft;

    #endregion

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
            timerSlider?.gameObject.SetActive(true);

            currentTimerTime = timerTime;
            if(timerCoroutine == null)
                timerCoroutine = StartCoroutine(TimerCountdown());
        }
        else
        {
            timerSlider?.gameObject.SetActive(false);
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
        OnObjectLeft?.Invoke();

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
    
    private IEnumerator TimerCountdown()
    {
        if (!hasTimer)
            yield break;

        currentTimerTime = timerTime;

        while(currentTimerTime > 0)
        {
            currentTimerTime -= Time.deltaTime;
            UpdateSlider();
            yield return null;
        }

        OnTimerFinished();
    }

    private void UpdateSlider()
    {
        if (timerSlider != null)
        {
            timerSlider.value = currentTimerTime / timerTime;
        }
    }

    private void OnTimerFinished()
    {
        PlayerManager.Instance.PossessGhost(gameObject.transform.GetComponent<PossessableObject>());

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        ResetTimer();
    }

    private void ResetTimer()
    {
        currentTimerTime = timerTime;
        timerSlider.gameObject.SetActive(false);
    }

    #endregion
}
