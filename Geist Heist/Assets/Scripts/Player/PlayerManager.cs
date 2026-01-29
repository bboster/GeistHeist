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
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private PlayerCameraController mainPlayerCameraController;
    private PlayerCameraController currentCameraController; // may be mainCinemachineCamera sometimes
    private StudioListener fmodListener;

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
    }


    public void PossessObject(PossessableObject possessable)
    {
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

        SwapCameras(PlayerGhostObject,possessable);
        PlayerGhostObject.gameObject.SetActive(false);
        UpdateListener(possessable);

        RegisterInputs(possessable);

        if (CurrentObject != null) CurrentObject.OnPossessionEnded();
        possessable.OnPossessionStart();

        DeRegisterInputs(CurrentObject);
        CurrentObject = possessable;
    }

    public void PossessGhost(PossessableObject possessable)
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

        if (!possessable.CanUnPossess)
        {
            return;
        }

        //to decide where ghost exits the possessable
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
                    break;
                }
                
                //if all of them collide, just use the last backup exit point
                if (i == possessable.ghostExitPoints.Count - 1)
                {
                    PlayerManager.Instance.PlayerGhostObject.transform.position = possessable.ghostExitPoints[possessable.ghostExitPoints.Count - 1].position;
                }
            }
        }

        SwapCameras(possessable, PlayerGhostObject);
        PlayerGhostObject.gameObject.SetActive(true);
        UpdateListener(PlayerGhostObject);

        RegisterInputs(PlayerGhostObject);

        possessable.OnPossessionEnded();
        PlayerGhostObject.OnPossessionStart();

        CurrentObject = PlayerGhostObject;

        DeRegisterInputs(possessable);
    }

    private void SwapCameras(PossessableObject oldObject, PossessableObject newObject)
    {
        // if both possessables dont have special behaviour
        if (oldObject == null || (!oldObject.HasCustomCameraBehavior && !newObject.HasCustomCameraBehavior))
        {
            //mainCinemachineCamera.Follow = newObject.cameraAnchor;
            mainPlayerCameraController.SetAnchorPoint(newObject.cameraAnchor);
            currentCameraController = mainPlayerCameraController;
        }
        else if (oldObject.HasCustomCameraBehavior || newObject.HasCustomCameraBehavior)
        {
            // Get rotation values
            var newOrbitalFollow = newObject.CinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
            var oldOrbitalFollow = oldObject.CinemachineCamera.GetComponent<CinemachineOrbitalFollow>();

            newOrbitalFollow.HorizontalAxis.Value = oldOrbitalFollow.HorizontalAxis.Value;

            newObject.CinemachineCamera.gameObject.SetActive(true);
            oldObject.CinemachineCamera.gameObject.SetActive(false);

            currentCameraController = newObject.GetComponent<PlayerCameraController>();
        }

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
    }

    private void Update()
    {
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
        mainPlayerCameraController.UpdateCameraSensitivity();
        if (currentCameraController != mainPlayerCameraController)
            currentCameraController.UpdateCameraSensitivity();
    }

    public void UpdateCamerasInvertLook()
    {
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
