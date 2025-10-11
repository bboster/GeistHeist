/*
 * Contributors: Toby, Sky
 * Creation Date: 9/16/25
 * Last Modified: 10/2/25
 * 
 * Brief Description: dont put this script on the player.
 * handles possession and such.
 */

using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerManager : Singleton<PlayerManager>
{
    [HideInInspector]
    public PossessableObject PlayerGhostObject;
    [ReadOnly]
    public PossessableObject CurrentObject;

    public IInputHandler currentInputHandler => CurrentObject?.InputHandler;

    private InputEvents inputEvents => InputEvents.Instance;
    private Camera camera;

    // Start is called once before the first execution of WhilePossessingUpdate after the MonoBehaviour is created
    void Start()
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
        possessable.CinemachineCamera.gameObject.SetActive(true);
        PlayerGhostObject.CinemachineCamera.gameObject.SetActive(false);
        PlayerGhostObject.gameObject.SetActive(false);

        RegisterInputs(possessable);

        if (CurrentObject != null) CurrentObject.OnPossessionEnded();
        possessable.OnPossessionStart();

        DeRegisterInputs(CurrentObject);
        CurrentObject = possessable;
    }

    public void PossessGhost(PossessableObject possessable)
    {
        if (!possessable.CanUnPossess)
        {
            return;
        }

        if (possessable.ghostSpawnPoint != null)
        {
            PlayerManager.Instance.PlayerGhostObject.transform.position = possessable.ghostSpawnPoint.position;
        }

        PlayerGhostObject.CinemachineCamera.gameObject.SetActive(true);
        possessable.CinemachineCamera.gameObject.SetActive(false);
        PlayerGhostObject.gameObject.SetActive(true);

        RegisterInputs(PlayerGhostObject);

        possessable.OnPossessionEnded();
        PlayerGhostObject.OnPossessionStart();

        CurrentObject = PlayerGhostObject;

        DeRegisterInputs(possessable);
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

        InputEvents.PossessStarted.AddListener(input.OnInteractStarted);
        InputEvents.PossessHeld.AddListener(input.WhileInteractHeld);
        InputEvents.PossessCanceled.AddListener(input.OnInteractCanceled);
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

        InputEvents.PossessStarted.RemoveListener(input.OnInteractStarted);
        InputEvents.PossessHeld.RemoveListener(input.WhileInteractHeld);
        InputEvents.PossessCanceled.RemoveListener(input.OnInteractCanceled);
    }

    private void Update()
    {
        if (CurrentObject != null)
            CurrentObject.WhilePossessingUpdate();
    }
}
