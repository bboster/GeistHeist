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
        //TODO: unregrister old inputs 

        var input = possessable.InputHandler;
        InputEvents.MoveStarted.AddListener(input.OnMoveStarted);
        InputEvents.MoveHeld.AddListener(input.WhileMoveHeld);
        InputEvents.MoveCanceled.AddListener(input.OnMoveCanceled);

        InputEvents.JumpStarted.AddListener(input.OnJumpStarted);
        InputEvents.JumpHeld.AddListener(input.WhileJumpHeld);
        InputEvents.JumpCanceled.AddListener(input.OnJumpCanceled);

        InputEvents.ActionStarted.AddListener(input.OnActionStarted);
        InputEvents.ActionHeld.AddListener(input.WhileActionHeld);
        InputEvents.ActionCanceled.AddListener(input.OnActionCanceled);

        InputEvents.EscapeObjectStarted.AddListener(input.OnEscapeObjectStarted);
        InputEvents.EscapeObjectHeld.AddListener(input.WhileEscapeObjectHeld);
        InputEvents.EscapeObjectCanceled.AddListener(input.OnEscapeObjectCanceled);

        // camera is really bad pls refactor it.
        var cam = possessable.CameraInputHandler;
        InputEvents.LookUpdate.AddListener(cam.OnMouseMove);
    }
}
