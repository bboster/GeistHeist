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

    public IInputHandler currentInputHandler => CurrentObject?.InputHandler;
    public ICameraInputHandler curentCameraInput => CurrentObject?.CameraInputHandler;

    private InputEvents inputEvents => InputEvents.Instance;
    private Camera camera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentObject = PlayerGhostObject;
        RegisterInputs(PlayerGhostObject);
        camera = Camera.main;

        Cursor.visible = false;
    }

    public void PossessObject(PossessableObject possessable)
    {
        PlayerGhostObject.gameObject.SetActive(false);

        RegisterInputs(possessable);

        CurrentObject = possessable;
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
        InputEvents.LookUpdate.AddListener(LookUpdate);
    }

    private void LookUpdate(Vector2 sensitiveMouseDelta)
    {
        if(curentCameraInput == null)
        {
            Debug.LogError("no current camera input");
            return;
        }

        curentCameraInput.OnMouseMove(camera, sensitiveMouseDelta * curentCameraInput.sensitivityScalar);
    }

    public void FixedUpdate()
    {
        var properties = curentCameraInput.GetCameraTransform(camera);
        camera.transform.position = properties.position;
        camera.transform.rotation = properties.rotation;
    }
}
