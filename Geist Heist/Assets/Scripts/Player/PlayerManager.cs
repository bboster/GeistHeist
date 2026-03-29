/*
 * Contributors: Toby, Sky
 * Creation Date: 9/16/25
 * Last Modified: 11/17/25
 * 
 * Brief Description: dont put this script on the player.
 * handles possession and such.
 */

using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : Singleton<PlayerManager>
{
    [HideInInspector]
    public PossessableObject PlayerGhostObject;
    [ReadOnly]
    public PossessableObject CurrentObject;

    public IInputHandler currentInputHandler => CurrentObject?.InputHandler;

    private InputEvents inputEvents => InputEvents.Instance;
    [HideInInspector] public Camera camera;
    [HideInInspector] public CinemachineCamera mainCinemachineCamera;
    [SerializeField] private float possessionFacingRotationSpeed = 540f;
    private PlayerCameraController mainPlayerCameraController;
    private PlayerCameraController currentCameraController; // may be mainCinemachineCamera sometimes
    private StudioListener fmodListener;
    private Coroutine possessionTransitionCoroutine;
    private bool isTransitioningPossession = false;

    [HideInInspector] public static UnityEvent<PossessableObject> OnPossessionObjectChanged = new();

    // Start is called once before the first execution of WhilePossessingUpdate after the MonoBehaviour is created
    public void Start()
    {
        // This all used to be start function, mostly just getting variables:

        if (PlayerGhostObject == null)
            PlayerGhostObject = GameObject.FindAnyObjectByType<ThirdPersonInputHandler>().GetComponent<PossessableObject>();

        CurrentObject = PlayerGhostObject;
        RegisterInputs(PlayerGhostObject);

        camera = Camera.main;
        mainCinemachineCamera = PlayerGhostObject.CinemachineCamera;
        PlayerGhostObject.CinemachineCamera.transform.SetParent(null);
        mainPlayerCameraController = mainCinemachineCamera.GetComponent<PlayerCameraController>();
        currentCameraController = mainPlayerCameraController;
        currentCameraController.UpdateAllSettings();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (GameManager.Instance.PlayerStart == null)
            Debug.Log("PlayerStart is null in gamemanager");
        else
            LevelManager.Instance.Initialize(GameManager.Instance.PlayerStart.position);

        fmodListener = camera.GetComponent<StudioListener>();
        UpdateListener(CurrentObject);

        OnPossessionObjectChanged.Invoke(CurrentObject);
    }


    public void PossessObject(PossessableObject possessable)
    {
        if (isTransitioningPossession)
            return;

        if(possessable == null)
        {
            Debug.LogError("Possessable is null");
            return;
        }
        if (PlayerGhostObject == null)
        {
            Debug.LogError("Player Ghost Object is null");
            return;
        }

        // Store reference to old object before changing CurrentObject
        PossessableObject oldObject = CurrentObject;
        bool isTetherPossession = possessable.InputHandler is TetherPossessable;
        if (oldObject != null && oldObject == PlayerGhostObject)
        {
            oldObject.QueueGhostPossessionAnimation(isTetherPossession);
        }

        if (possessionTransitionCoroutine != null)
            StopCoroutine(possessionTransitionCoroutine);

        possessionTransitionCoroutine = StartCoroutine(PossessObjectAfterAnimation(possessable, oldObject, isTetherPossession));
    }

    private IEnumerator PossessObjectAfterAnimation(PossessableObject possessable, PossessableObject oldObject, bool isTetherPossession)
    {
        isTransitioningPossession = true;

        if (oldObject != null)
            DeRegisterInputs(oldObject);
        if (oldObject != null)
            oldObject.OnPossessionEnded();

        if (oldObject != null && oldObject == PlayerGhostObject)
        {
            float cameraDelay = oldObject.GetGhostCameraTransitionDelay(isTetherPossession);
            if (cameraDelay > 0f)
            {
                float elapsed = 0f;
                while (elapsed < cameraDelay)
                {
                    oldObject.RotateTowardsTarget(possessable.transform, possessionFacingRotationSpeed * Time.deltaTime);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }

            oldObject.FaceTowardsTarget(possessable.transform);
        }

        PlayerGhostObject.gameObject.SetActive(false);

        // Update listener for audio and register inputs
        UpdateListener(possessable);
        RegisterInputs(possessable);

        // Start possession and swap cameras
        possessable.OnPossessionStart();
        CurrentObject = possessable;
        SwapCameras(oldObject, possessable);

        OnPossessionObjectChanged.Invoke(CurrentObject);

        isTransitioningPossession = false;
        possessionTransitionCoroutine = null;
    }

    public void PossessGhost(PossessableObject possessable)
    {
        if (isTransitioningPossession)
            return;

        if (possessable == null)
        {
            Debug.LogError("Possessable is null");
            return;
        }
        if (PlayerGhostObject == null)
        {
            Debug.LogError("Player Ghost Object is null");
            return;
        }

        if (!possessable.CanUnPossess)
        {
            return;
        }

        // Store reference to old object
        PossessableObject oldObject = CurrentObject;
        CheckGhostExitPoints(possessable);
        if (oldObject != null)
            DeRegisterInputs(oldObject);
        if (oldObject != null)
            oldObject.OnPossessionEnded();
        PlayerGhostObject.gameObject.SetActive(true);

        // Update listener for audio and register inputs
        UpdateListener(PlayerGhostObject);
        RegisterInputs(PlayerGhostObject);

        // Call ghost's start possession
        PlayerGhostObject.OnPossessionStart();
        CurrentObject = PlayerGhostObject;
        SwapCameras(oldObject, PlayerGhostObject);

        //DeRegisterInputs(possessable);

        OnPossessionObjectChanged.Invoke(CurrentObject);
    }

    /// <summary>
    /// Decides where the ghost exits the possessable, returns the new position
    /// </summary>
    /// <param name="possessable"></param>
    private Vector3 CheckGhostExitPoints(PossessableObject possessable)
    {
        if (possessable.ghostExitPoints != null)
        {
            //go through spawn points until one of them doesn't collide
            for (int i = 0; i < possessable.ghostExitPoints.Count; i++)
            {
                Collider[] collisions = Physics.OverlapSphere(possessable.ghostExitPoints[i].transform.position, 0.2f);

                //no collision = use this point
                if (collisions.Length <= 0)
                {
                    PlayerManager.Instance.PlayerGhostObject.transform.position = possessable.ghostExitPoints[i].position;
                    return possessable.ghostExitPoints[i].position;
                }
            }

            //if all of them collide, just use the last backup exit point
            PlayerManager.Instance.PlayerGhostObject.transform.position = possessable.ghostExitPoints[possessable.ghostExitPoints.Count - 1].position;
            return possessable.ghostExitPoints[possessable.ghostExitPoints.Count - 1].position;
        }

        Debug.Log("No ghost exit points available.");
        return Vector3.zero;
    }

    private void SwapCameras(PossessableObject oldObject, PossessableObject newObject)
    {
        // Null safety check
        if (newObject == null)
            return;

        // If neither has custom camera behavior, use the main camera
        if ((oldObject == null || !oldObject.HasCustomCameraBehavior) && !newObject.HasCustomCameraBehavior)
        {
            mainCinemachineCamera.gameObject.SetActive(true);
            mainPlayerCameraController.SetAnchorPoint(newObject.cameraAnchor);
            currentCameraController = mainPlayerCameraController;
        }
        // If new object has custom camera, switch to it
        else if (newObject.HasCustomCameraBehavior && newObject.CinemachineCamera != null)
        {
            // Deactivate old camera if it exists
            if (oldObject != null && oldObject.HasCustomCameraBehavior && oldObject.CinemachineCamera != null)
            {
                oldObject.CinemachineCamera.gameObject.SetActive(false);
            }
            else
            {
                mainCinemachineCamera.gameObject.SetActive(false);
            }

            // Activate new custom camera
            newObject.CinemachineCamera.gameObject.SetActive(true);
            
            // Get PlayerCameraController from the CinemachineCamera GameObject, not the possessable
            currentCameraController = newObject.CinemachineCamera.GetComponent<PlayerCameraController>();
            
            if (currentCameraController == null)
            {
                currentCameraController = newObject.CinemachineCamera.GetComponentInParent<PlayerCameraController>();
            }
           
            if (currentCameraController == null)
            {
                Debug.LogError($"[PlayerManager] Could not find PlayerCameraController on or near {newObject.CinemachineCamera.gameObject.name}", newObject.gameObject);
                currentCameraController = mainPlayerCameraController;
            }
        }
        // Switching FROM custom camera back to main camera
        else if (oldObject != null && oldObject.HasCustomCameraBehavior && oldObject.CinemachineCamera != null && !newObject.HasCustomCameraBehavior)
        {
            oldObject.CinemachineCamera.gameObject.SetActive(false);
            mainCinemachineCamera.gameObject.SetActive(true);
            mainPlayerCameraController.SetAnchorPoint(newObject.cameraAnchor);
            currentCameraController = mainPlayerCameraController;
        }
        // Fallback: HasCustomCameraBehavior is true but CinemachineCamera is null
        else if (newObject.HasCustomCameraBehavior && newObject.CinemachineCamera == null)
        {
            Debug.LogError($"'{newObject.gameObject.name}' has HasCustomCameraBehavior checked but CinemachineCamera is not assigned!", newObject);
            mainCinemachineCamera.gameObject.SetActive(true);
            currentCameraController = mainPlayerCameraController;
        }

        if (currentCameraController != null)
            currentCameraController.UpdateAllSettings();
    }

    public void RegisterInputs(PossessableObject possessable)
    {

        var input = possessable.InputHandler;
        InputEvents.MoveStarted.AddListener(input.OnMoveStarted);   
        InputEvents.MoveHeld.AddListener(input.WhileMoveHeld);
        InputEvents.MoveNotHeld.AddListener(input.WhileMoveNotHeld);
        InputEvents.MoveCanceled.AddListener(input.OnMoveCanceled);

        InputEvents.ActionStarted.AddListener(input.OnActionStarted);
        InputEvents.ActionHeld.AddListener(input.WhileActionHeld);
        InputEvents.ActionNotHeld.AddListener(input.WhileActionNotHeld);
        InputEvents.ActionCanceled.AddListener(input.OnActionCanceled);

        InputEvents.InteractStarted.AddListener(input.OnInteractStarted);
        InputEvents.InteractHeld.AddListener(input.WhileInteractHeld);
        InputEvents.InteractCanceled.AddListener(input.OnInteractCanceled);

        /*InputEvents.SpaceStarted.AddListener(input.OnSpaceStarted);
        InputEvents.SpaceHeld.AddListener(input.WhileSpaceHeld);
        InputEvents.SpaceCanceled.AddListener(input.OnSpaceCanceled); */

    }

    public void DeRegisterInputs(PossessableObject possessable)
    {
        var input = possessable.InputHandler;
        InputEvents.MoveStarted.RemoveListener(input.OnMoveStarted);
        InputEvents.MoveHeld.RemoveListener(input.WhileMoveHeld);
        InputEvents.MoveNotHeld.RemoveListener(input.WhileMoveNotHeld);
        InputEvents.MoveCanceled.RemoveListener(input.OnMoveCanceled);

        InputEvents.ActionStarted.RemoveListener(input.OnActionStarted);
        InputEvents.ActionHeld.RemoveListener(input.WhileActionHeld);
        InputEvents.ActionNotHeld.RemoveListener(input.WhileActionNotHeld);
        InputEvents.ActionCanceled.RemoveListener(input.OnActionCanceled);

        InputEvents.InteractStarted.RemoveListener(input.OnInteractStarted);
        InputEvents.InteractHeld.RemoveListener(input.WhileInteractHeld);
        InputEvents.InteractCanceled.RemoveListener(input.OnInteractCanceled);


        /* InputEvents.SpaceStarted.RemoveListener(input.OnSpaceStarted);
        InputEvents.SpaceHeld.RemoveListener(input.WhileSpaceHeld);
        InputEvents.SpaceCanceled.RemoveListener(input.OnSpaceCanceled); */
    }

    private void Update()
    {
        if (isTransitioningPossession)
            return;

        if (CurrentObject != null)
            CurrentObject.WhilePossessingUpdate();
        if (currentInputHandler != null)
            currentInputHandler.WhilePossessingUpdate();
    }

    private void UpdateListener(PossessableObject currentListener)
    {
        // change listener
        if (fmodListener == null)
            Debug.LogError("The main camera does not have a FMOD Studio Listener. please remove the current listener and add a FMOD Studio Listener component");
        else
            fmodListener.AttenuationObject = currentListener.gameObject;
    }

    #region Camera Sensitivity

    // all relevant to settings and settingsmenu.cs

    public void UpdateCamerasSensitivity()
    {
        if (mainPlayerCameraController == null) return;

        mainPlayerCameraController.UpdateCameraSensitivity();
        if (currentCameraController != mainPlayerCameraController)
            currentCameraController.UpdateCameraSensitivity();
    }

    public void UpdateCamerasInvertLook()
    {
        if (mainPlayerCameraController == null) return;

        mainPlayerCameraController.UpdateCameraInvertLook();
        if (currentCameraController != mainPlayerCameraController)
            currentCameraController. UpdateCameraInvertLook();
    }


    // wonder if it would be worth it to make a different script for camera controlling

    #endregion


    public void PossessFreecam(PossessableObject possessable)
    {
        if (possessable == null)
        {
            Debug.LogError("Possessable is null");
            return;
        }
        if (PlayerGhostObject == null)
        {
            Debug.LogError("Player Ghost Object is null");
            return;
        }

        SwapCameras(PlayerGhostObject, possessable);
        //PlayerGhostObject.gameObject.SetActive(false);

        RegisterInputs(possessable);

        if (CurrentObject != null) CurrentObject.OnPossessionEnded();
        possessable.OnPossessionStart();

        DeRegisterInputs(CurrentObject);
        CurrentObject = possessable;
    }
}
