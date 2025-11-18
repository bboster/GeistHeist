/*
 * Contributors: Toby, Sky
 * Creation Date: 9/16/25
 * Last Modified: 11/17/25
 * 
 * Brief Description: dont put this script on the player.
 * handles possession and such.
 */

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
    private Camera camera;
    [HideInInspector] public CinemachineCamera mainCinemachineCamera;
    private PlayerCameraController mainPlayerCameraController;
    private CinemachineCamera currentCamera; // may be mainCinemachineCamera sometimes


    // Start is called once before the first execution of WhilePossessingUpdate after the MonoBehaviour is created
    void Start()
    {
        if (PlayerGhostObject == null)
            PlayerGhostObject = GameObject.FindAnyObjectByType<ThirdPersonInputHandler>().GetComponent<PossessableObject>();

        CurrentObject = PlayerGhostObject;
        RegisterInputs(PlayerGhostObject);

        camera = Camera.main;
        mainCinemachineCamera = PlayerGhostObject.CinemachineCamera;
        PlayerGhostObject.CinemachineCamera.transform.SetParent(null);
        mainPlayerCameraController = mainCinemachineCamera.GetComponent<PlayerCameraController>();
        currentCamera = mainCinemachineCamera;
        UpdateCamerasInvertLook();
        UpdateCamerasSensitivity();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (GameManager.Instance.PlayerStart == null)
            Debug.Log("PlayerStart is null in gamemanager");
        else
            LevelManager.Instance.InitializeLevelManager(GameManager.Instance.PlayerStart.position);
    }

    public void InitializePlayerManager()
    {
        if (PlayerGhostObject == null)
            PlayerGhostObject = GameObject.FindAnyObjectByType<ThirdPersonInputHandler>().GetComponent<PossessableObject>();

        CurrentObject = PlayerGhostObject;
        RegisterInputs(PlayerGhostObject);
        camera = Camera.main;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
            currentCamera = mainCinemachineCamera;
        }
        else if (oldObject.HasCustomCameraBehavior || newObject.HasCustomCameraBehavior)
        {
            // Get rotation values
            var newOrbitalFollow = newObject.CinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
            var oldOrbitalFollow = oldObject.CinemachineCamera.GetComponent<CinemachineOrbitalFollow>();

            newOrbitalFollow.HorizontalAxis.Value = oldOrbitalFollow.HorizontalAxis.Value;

            newObject.CinemachineCamera.gameObject.SetActive(true);
            oldObject.CinemachineCamera.gameObject.SetActive(false);

            currentCamera = newObject.CinemachineCamera;
        }
        UpdateCameraSensitivity(currentCamera);
        UpdateCameraInvertLook(currentCamera);
    }

    public void RegisterInputs(PossessableObject possessable)
    {

        var input = possessable.InputHandler;
        InputEvents.MoveStarted.AddListener(input.OnMoveStarted);   
        InputEvents.MoveHeld.AddListener(input.WhileMoveHeld);
        InputEvents.MoveNotHeld.AddListener(input.WhileMoveNotHeld);
        InputEvents.MoveCanceled.AddListener(input.OnMoveCanceled);

        /*InputEvents.JumpStarted.AddListener(input.OnJumpStarted);
        InputEvents.JumpHeld.AddListener(input.WhileJumpHeld);
        InputEvents.JumpCanceled.AddListener(input.OnJumpCanceled);*/

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

    #region Camera Sensitivity

    // all relevant to settings and settingsmenu.cs

    public void UpdateCamerasSensitivity()
    {
        UpdateCameraSensitivity(mainCinemachineCamera);
        if(currentCamera != mainCinemachineCamera)
            UpdateCameraSensitivity(currentCamera);
    }

    private void UpdateCameraSensitivity(CinemachineCamera cam)
    {
        var controller = cam.GetComponent<CinemachineInputAxisController>();
        if(controller == null)
        {
            Debug.LogWarning($"{cam.gameObject.name} has not CinemachineInputAxisController. cant update sensitivity");
            return;
        }
       
        // apply sensitivity to every axis (yes it HAS to be iterated for some reason)
        foreach (var c in controller.Controllers) 
        {
            Debug.Log(c.Name);
            c.Input.LegacyGain = Mathf.Sign(c.Input.LegacyGain) * SettingsProfile.LookSensitivityTransformed;
            c.Input.Gain = Mathf.Sign(c.Input.Gain) * SettingsProfile.LookSensitivityTransformed;
        }
    }

    public void UpdateCamerasInvertLook()
    {
        UpdateCameraInvertLook(mainCinemachineCamera);
        if (currentCamera != mainCinemachineCamera)
            UpdateCameraInvertLook(currentCamera);
    }

    private void UpdateCameraInvertLook(CinemachineCamera cam)
    {
        var controller = cam.GetComponent<CinemachineInputAxisController>();
        if (controller == null)
        {
            Debug.LogWarning($"{cam.gameObject.name} has not CinemachineInputAxisController. Can't update inverted look");
            return;
        }

        // apply sensitivity to every axis (yes it HAS to be iterated for some reason)
        foreach (var c in controller.Controllers)
        {
            var axisName = c.Name;
            // horrible and hard-coded but there is not a better way to do this (that I could find)
            if (axisName == "Look Orbit Y" || axisName == "Mouse Y" || axisName == "Gamepad Right Stick Y") // Adjust axis names as needed
            {
                Debug.Log("inverting look for "+c.Name);
                c.Input.Gain       = (SettingsProfile.InvertLook ? 1 : -1) * SettingsProfile.LookSensitivityTransformed;
                c.Input.LegacyGain = (SettingsProfile.InvertLook ? -1 : 1) * SettingsProfile.LookSensitivityTransformed;
            }
        }
    }

    // wonder if it would be worth it to make a different script for camera controlling

    #endregion

}
