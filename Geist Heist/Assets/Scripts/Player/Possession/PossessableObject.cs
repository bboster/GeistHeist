

/*
 * Contributors: Toby, Sky, Skylar
 * Creation Date: 9/16/25
 * Last Modified: 2/12/2026
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
using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using GuardUtilities;

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
    [SerializeField, HideIf(nameof(isGhost))] public bool hasTimer;
    [SerializeField, HideIf(nameof(isGhost))] public float maxChargePercentage = 100;
    [Tooltip("The percentage the timer recharges each interval while the player is not possessing.")]
    [SerializeField, ShowIf(nameof(hasTimerAndIsNotGhost))] private float timerRechargePercentage = 10;
    [Tooltip("The percentage the timer decreases each interval while the player is possessing.")]
    [SerializeField, ShowIf(nameof(hasTimerAndIsNotGhost))] private float timerDischargePercentage = 10;

    [Tooltip("Location where the ghost spawns after leaving the possessable.")]
    [HideIf(nameof(isGhost))] public Transform ghostSpawnPoint;

    [Header("Materials")]
    [Tooltip("Material on possessable when it is possessed.")]
    [SerializeField, HideIf(nameof(isGhost)), Required, ShowAssetPreview(16, 16)] public Material PossessedMaterial;
    [Tooltip("Material on possessable when it is UNpossessed.")]
    [SerializeField, HideIf(nameof(isGhost)), Required, ShowAssetPreview(16, 16)] public Material UnpossessedMaterial;
    [Tooltip("Material on possessable when it is used or when Ollie enters the possessable while a guard is in chase state.")]
    [SerializeField, HideIf(nameof(isGhost)), Required, ShowAssetPreview(16, 16)] public Material VisiblePossessionMaterial;
    [Tooltip("Material on possessable when a guard sees the possessable in chase state but possessable is NOT possessed.")]
    [SerializeField, HideIf(nameof(isGhost)), Required, ShowAssetPreview(16, 16)] public Material VisibleUnPossessedMaterial;

    [Header("UI")]
    public GameObject AbilityIconPrefab;
    public GameObject PossessableTextPrefab;
    public bool HasChargeAbility;

    [Header("Animation")]
    [SerializeField] public Animator animator;
    private string isPossessingParam = "isPossessing";
    private string tetherPossessParam = "TetherPossess";
    private string modePossessedParam = "modePossessed";
    private string isMovingParam = "isMoving";
    private string isIdleParam = "isIdle";
    [SerializeField] private string possessStateName = "possess";
    [SerializeField] private string tetherPossessStateName = "tetherpossess";
    private bool queueTetherPossessAnimation = false;
    [SerializeField, ShowIf(nameof(isGhost)), Min(0f)] private float possessCameraTransitionDelay = 0.35f;
    [SerializeField, ShowIf(nameof(isGhost)), Min(0f)] private float tetherCameraTransitionDelay = 0.75f;


    [Header("Other")]
    [SerializeField] private bool isGhost = false;

    [HideInInspector] public bool CanUnPossess = true;
    private bool possessionIsSafe = true;
    [HideInInspector] public IInputHandler InputHandler => GetInputHandler();
    private IInputHandler inputHandler;

    private Coroutine dischargeCoroutine = null;
    private Coroutine rechargeCoroutine;
    private Coroutine unpossessCoroutine = null;

    [HideInInspector] public MeshRenderer meshRenderer;

    [ReadOnly] private float currentTimerCharge;
    private float currentTimerChargePercentage => currentTimerCharge / maxChargePercentage;
    [HideInInspector] public UnityEvent<float> OnTimerUpdate = new();
    [HideInInspector] public bool PauseDischargeTimer = false;

    private EventInstance possessionEnter;
    private EventInstance possessionLow;
    private EventInstance possessionOut;    
    private EventInstance possessionRefill;

    #region Guard Detection Variables

    [ReadOnly] public bool IsMoving = false;

    public static Action OnActionPerformed;
    public static Action OnObjectLeft;

    private PlayerManager playerManager => PlayerManager.Instance;

    /// <summary>
    /// using this for unsafe material changing
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //if interaction is vision cone
        if (other.transform.GetComponent<VisionStimulus>() != null)
        {
            GuardController GC = other.transform.GetComponentInParent<GuardController>();

            //if it is a guard in chase state + possessed
            if (GC != null && GC.currentBehavior.StateName == GuardStates.chase && playerManager.CurrentObject.gameObject == this.gameObject)
            {
                possessionIsSafe = false;
                if (VisiblePossessionMaterial != null)
                {
                    this.meshRenderer.material = VisiblePossessionMaterial;
                }
            }

            //if it is a guard in chase state + UNpossessed
            else if (GC != null && GC.currentBehavior.StateName == GuardStates.chase && playerManager.CurrentObject == playerManager.PlayerGhostObject)
            {
                possessionIsSafe = false;
                if (VisibleUnPossessedMaterial != null)
                {
                    this.meshRenderer.material = VisibleUnPossessedMaterial;
                }
            }
        }
    }

    //I'm not seeing a reason to keep this function if Enter and Exit are being used, but if there's some reason I'm unaware of then this is fine to stay
    private void OnTriggerStay(Collider other) 
    {
        //if interaction is vision cone
        if (other.transform.GetComponent<VisionStimulus>() != null)
        {
            GuardController GC = other.transform.GetComponentInParent<GuardController>();

            //if it is a guard in chase state + possessed
            if (GC != null && GC.currentBehavior.StateName == GuardStates.chase && playerManager.CurrentObject.gameObject == this.gameObject)
            {
                if (VisiblePossessionMaterial != null)
                {
                    this.meshRenderer.material = VisiblePossessionMaterial;
                }
            }

            //if it is a guard in chase state + UNpossessed
            else if (GC != null && GC.currentBehavior.StateName == GuardStates.chase && playerManager.CurrentObject == playerManager.PlayerGhostObject)
            {
                if (VisibleUnPossessedMaterial != null)
                {
                    this.meshRenderer.material = VisibleUnPossessedMaterial;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if interaction is vision cone
        if (other.transform.GetComponent<VisionStimulus>() != null)
        {
            //if player is still inside
            if (playerManager.CurrentObject.gameObject == this.gameObject)
            {
                possessionIsSafe = true;
                if (PossessedMaterial != null)
                {
                    this.meshRenderer.material = PossessedMaterial;
                }
            }
            //is player is not possessing
            else if (playerManager.CurrentObject == playerManager.PlayerGhostObject)
            {
                possessionIsSafe = true;
                if (UnpossessedMaterial != null)
                {
                    this.meshRenderer.material = UnpossessedMaterial;
                }
            }
        }
    }

    #endregion

    #region Inspector Debug

    private bool showCinemachineCamera => isGhost || HasCustomCameraBehavior;
    private bool hasTimerAndIsNotGhost => isGhost == false && hasTimer;

    #endregion

    void Start()
    {
        currentTimerCharge = maxChargePercentage;

        if (ghostExitPoints.Count == 0)
        {
            Debug.Log("No exit points set for " + this);
        }

        meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (UnpossessedMaterial != null)
            this.meshRenderer.material = UnpossessedMaterial;
        else
            Debug.LogWarning("No unpossession material for " + gameObject.name);

        // Ghost is disabled while inside possessables; keep animator state so
        // it can transition from possess -> possessExit when re-enabled.
        if (isGhost && animator != null)
        {
            animator.keepAnimatorStateOnDisable = true;
        }

        /*if(possessableCanvas == null)
            possessableCanvas = gameObject.GetComponentInChildren<Canvas>();

        if (possessableCanvas != null)
        {
            possessableCanvasGroup = possessableCanvas.gameObject.GetOrAddComponent<CanvasGroup>();
            possessableCanvasGroup.alpha = 0;
            possessableCanvas.gameObject.SetActive(false);
        }*/

        //It might be worth moving this line into a manager so that we don't get a ton of repeat messages in the console
        if(AudioManager.Instance == null)
        {
            Debug.LogError("No audio manager in scene");
            return;
        }

        //This is a sound issue, but I think all sound setup should be in its own function for organization, especially since some sounds may need additional lines
        //in the future
        possessionEnter = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PossessionEnter);
        possessionLow = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PossessionLow);
        possessionOut = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PossessionOut);
        possessionRefill = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.PossessionRefill);
    }

    public IInputHandler GetInputHandler()
    {
        inputHandler = inputHandler == null ? GetComponent<IInputHandler>(): inputHandler;
        return inputHandler;
    }

    /// <summary>
    /// Sets the animation type used the next time this object exits possession.
    /// </summary>
    public void QueueGhostPossessionAnimation(bool isTetherPossession)
    {
        queueTetherPossessAnimation = isTetherPossession;
    }

    public float GetGhostCameraTransitionDelay(bool isTetherPossession)
    {
        if (!isGhost)
            return 0f;

        return isTetherPossession ? tetherCameraTransitionDelay : possessCameraTransitionDelay;
    }

    public void RotateTowardsTarget(Transform target, float maxDegreesDelta)
    {
        if (target == null)
            return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxDegreesDelta);
    }

    public void FaceTowardsTarget(Transform target)
    {
        RotateTowardsTarget(target, 360f);
    }


    /// <summary>
    /// Called in PlayerManager when player enters object
    /// </summary>
    public void OnPossessionStart()
    {
        possessionEnter.start();

        //StaticUtilities.StopAndStartCoroutine(ref fadeOpacityCoroutine, ShowAndEnableCanvas());

        gameObject.SetActive(true);
        InputHandler.OnPossessionStart();

        if(PossessedMaterial != null && possessionIsSafe)
            this.meshRenderer.material = PossessedMaterial;
        else
            Debug.LogWarning("No possession material for " + gameObject.name);

        if (unpossessCoroutine == null)
            unpossessCoroutine = StartCoroutine(WaitForUnpossess());

        if (isGhost)
        {
            if (animator != null)
            {
                animator.SetBool(isPossessingParam, false);
                animator.SetBool(tetherPossessParam, false);
                animator.SetBool(modePossessedParam, false);
                animator.SetBool(isMovingParam, false);
                animator.SetBool(isIdleParam, true);
            }
            else
            {
                Debug.LogWarning($"No animator assigned for ghost object '{gameObject.name}'.");
            }

            queueTetherPossessAnimation = false;
        }

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
        //StaticUtilities.StopAndStartCoroutine(ref fadeOpacityCoroutine, HideAndDisableCanvas());

        if (!CanUnPossess && !isGhost)
        {
            Debug.LogError("Trying to unpossess early");
            return;
        }

        if (UnpossessedMaterial != null && possessionIsSafe)
            this.meshRenderer.material = UnpossessedMaterial;
        else
            Debug.LogWarning("No unpossession material for " + gameObject.name);

        InputHandler.OnPossessionEnded();
        OnObjectLeft?.Invoke();

        if (isGhost)
        {
            if (animator != null)
            {
                animator.SetBool(tetherPossessParam, queueTetherPossessAnimation);
                animator.SetBool(isPossessingParam, !queueTetherPossessAnimation);
                animator.SetBool(modePossessedParam, true);
                animator.SetBool(isMovingParam, false);
                animator.SetBool(isIdleParam, false);

                // Start the possession clip immediately instead of waiting on exit-time transitions.
                string stateToPlay = queueTetherPossessAnimation ? tetherPossessStateName : possessStateName;
                if (!string.IsNullOrWhiteSpace(stateToPlay))
                {
                    animator.CrossFadeInFixedTime(stateToPlay, 0.05f, 0, 0f);
                }
            }
            else
            {
                Debug.LogWarning($"No animator assigned for ghost object '{gameObject.name}'.");
            }
        }

        if (hasTimer)
        {
            if (dischargeCoroutine != null)
            {
                StopCoroutine(dischargeCoroutine);
                dischargeCoroutine = null;
            }

            if (rechargeCoroutine == null)
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

    #region Interaction
    void IInteractable.Interact()
    {
        PlayerManager.Instance.PossessObject(this);
    }

    bool IInteractable.IsInteractable()
    {
        // interactable if player isnt possessed
        return playerManager.CurrentObject == playerManager.PlayerGhostObject;
    }

    #endregion

    #region Timer

    private IEnumerator StartDischarge()
    {
        if (!hasTimer)
            yield break;

        while(currentTimerCharge > 0)
        {
            if (!PauseDischargeTimer)
            {
                currentTimerCharge = Mathf.Max(currentTimerCharge - (timerDischargePercentage * Time.deltaTime), 0);
                OnTimerUpdate.Invoke(currentTimerChargePercentage);
            }

            yield return null;
        }

        OnTimerFinished();
    }

    private IEnumerator StartRecharge()
    {
        if (!hasTimer)
            yield break;
        possessionRefill.start();

        while (currentTimerCharge < maxChargePercentage)
        {

            currentTimerCharge = Mathf.Min(currentTimerCharge + (timerRechargePercentage * Time.deltaTime), maxChargePercentage);
            OnTimerUpdate.Invoke(currentTimerChargePercentage);
            yield return null;
        }

        possessionRefill.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void OnTimerFinished()
    {
        PlayerManager.Instance.PossessGhost(gameObject.transform.GetComponent<PossessableObject>());

        if (dischargeCoroutine != null)
        {
            OnTimerUpdate.Invoke(currentTimerChargePercentage);
            StopCoroutine(dischargeCoroutine);
            dischargeCoroutine = null;
        }

        possessionOut.start();
    }
    #endregion

    /*
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

        yield return StaticUtilities.FadeToVisible(possessableCanvasGroup, showSeconds);
    }

    private IEnumerator HideAndDisableCanvas()
    {
        if (possessableCanvas == null)
            yield break;

        yield return StaticUtilities.FadeToVisible(possessableCanvasGroup, hideSeconds);

        possessableCanvas.gameObject.SetActive(false);
    }

    #endregion
    */
}
