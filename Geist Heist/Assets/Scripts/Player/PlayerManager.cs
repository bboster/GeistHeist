/*
 * Contributors: Toby, Sky
 * Creation Date: 9/16/25
 * Last Modified: 9/18/25
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
    public PossessableObject PlayerGhostObject;
    [ReadOnly]
    public PossessableObject CurrentObject;

    public IInputHandler currentInputHandler => CurrentObject?.InputHandler;

    private InputEvents inputEvents => InputEvents.Instance;
    private Camera camera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

        CurrentObject = possessable;

        DeRegisterInputs(PlayerGhostObject);
    }

    public void PossessGhost(PossessableObject possessable)
    {
        PlayerGhostObject.CinemachineCamera.gameObject.SetActive(true);
        possessable.CinemachineCamera.gameObject.SetActive(false);
        PlayerGhostObject.gameObject.SetActive(true);


        RegisterInputs(PlayerGhostObject);

        CurrentObject = PlayerGhostObject;

        DeRegisterInputs(possessable);
    }

    public void RegisterInputs(PossessableObject possessable)
    {

        var input = possessable.InputHandler;
        InputEvents.MoveStarted.AddListener(input.OnMoveStarted);
        InputEvents.MoveHeld.AddListener(input.WhileMoveHeld);
        InputEvents.MoveCanceled.AddListener(input.OnMoveCanceled);

        /*InputEvents.JumpStarted.AddListener(input.OnJumpStarted);
        InputEvents.JumpHeld.AddListener(input.WhileJumpHeld);
        InputEvents.JumpCanceled.AddListener(input.OnJumpCanceled);*/

        InputEvents.ActionStarted.AddListener(input.OnActionStarted);
        InputEvents.ActionHeld.AddListener(input.WhileActionHeld);
        InputEvents.ActionCanceled.AddListener(input.OnActionCanceled);

        InputEvents.PossessStarted.AddListener(input.OnPossessStarted);
        InputEvents.PossessHeld.AddListener(input.WhilePossessHeld);
        InputEvents.PossessCanceled.AddListener(input.OnPossessCanceled);

        // camera is really bad pls refactor it.
        //var cam = possessable.CameraInputHandler;
        //InputEvents.LookUpdate.AddListener(LookUpdate);
    }

    public void DeRegisterInputs(PossessableObject possessable)
    {

        var input = possessable.InputHandler;
        InputEvents.MoveStarted.RemoveListener(input.OnMoveStarted);
        InputEvents.MoveHeld.RemoveListener(input.WhileMoveHeld);
        InputEvents.MoveCanceled.RemoveListener(input.OnMoveCanceled);

        InputEvents.ActionStarted.RemoveListener(input.OnActionStarted);
        InputEvents.ActionHeld.RemoveListener(input.WhileActionHeld);
        InputEvents.ActionCanceled.RemoveListener(input.OnActionCanceled);

        InputEvents.PossessStarted.RemoveListener(input.OnPossessStarted);
        InputEvents.PossessHeld.RemoveListener(input.WhilePossessHeld);
        InputEvents.PossessCanceled.RemoveListener(input.OnPossessCanceled);
    }
}
