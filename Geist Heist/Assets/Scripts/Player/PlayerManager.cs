/*
 * Contributors: Toby
 * Creation Date: 9/16/25
 * Last Modified: 9/16/25
 * 
 * Brief Description: dont put this script on the player.
 * handles possession and such.
 */

using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerManager : MonoBehaviour
{
    public PossessableObject PlayerGhostObject;
    [ReadOnly]
    public PossessableObject CurrentObject;

    private InputEvents inputEvents => InputEvents.Instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RegisterInputs(PlayerGhostObject);
    }

    public void PossessObject(PossessableObject possessable)
    {
        PlayerGhostObject.gameObject.SetActive(false);

        RegisterInputs(possessable);
    }

    public void RegisterInputs(PossessableObject possessable)
    {
        //TODO: unregrister old inputs from other object

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
        var cam = possessable.CameraInputHandler;
        InputEvents.LookUpdate.AddListener(cam.OnMouseMove);
    }
}
