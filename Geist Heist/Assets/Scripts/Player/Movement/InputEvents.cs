/*
 * Contributors: Toby, Alec P, Clare G, Sky B, Tyler B
 * Creation Date: Spring 2024
 * Last Modified: 10/27/25
 * 
 * Connects to PlayerInput map actions and invokes static UnityEvents.
 * Use other scripts to connect to the unityevents.
 */

using System.Net.Http.Headers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
 using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class InputEvents : Singleton<InputEvents>
{
    // Events

    [SerializeField] private string moveKey = "Move";
    //[SerializeField] private string jumpKey = "Jump";
    [SerializeField] private string pauseKey = "Pause";
    [SerializeField] private string lookKey = "Look";
    [SerializeField] private string actionKey = "Escape Object";
    [SerializeField] private string interactKey = "Interact";

    public static UnityEvent MoveStarted = new UnityEvent();
    public static UnityEvent<float> MoveHeld = new();
    public static UnityEvent MoveNotHeld = new UnityEvent();
    public static UnityEvent<float> MoveCanceled = new();

    /*public static UnityEvent JumpStarted = new UnityEvent();
    public static UnityEvent JumpHeld = new UnityEvent();
    public static UnityEvent JumpCanceled = new UnityEvent();*/

    public static UnityEvent ActionStarted = new UnityEvent();
    public static UnityEvent<float> ActionHeld = new();
    public static UnityEvent<float> ActionNotHeld = new();
    public static UnityEvent<float> ActionCanceled = new();

    public static UnityEvent InteractStarted = new UnityEvent();
    public static UnityEvent<float> InteractHeld = new();
    public static UnityEvent<float> InteractCanceled = new();

    public static UnityEvent PauseStarted = new UnityEvent();

    public static UnityEvent<Vector2> LookUpdate = new UnityEvent<Vector2>();

    [SerializeField] private float _sensitivity=1;

    public static bool IsHeld = false;

    // Input values and flags
    public Vector2 LookDelta => Look.ReadValue<Vector2>() * _sensitivity;
    public Vector3 FirstPersonInputDirection => (
        (movementOrigin.forward * InputDirection2D.y)
        + (movementOrigin.right * InputDirection2D.x))
        .WithY(0)
        .normalized;

    public Vector2 InputDirection2D => Move.ReadValue<Vector2>();
    public static bool MovePressed, /*JumpPressed,*/ ActionPressed, InteractPressed, PausePressed;

    #region Time Held
    private static float moveTimeStarted, actionTimeStarted, interactTimeStarted = -1; // other inputs can be added but i dont think theyre super necessary.
    private static float actionTimeReleased = -1;
    public static float MoveHeldTime => MovePressed ? Time.time - moveTimeStarted : 0;
    public static float ActionHeldTime => ActionPressed ? Time.time - actionTimeStarted : 0;
    public static float ActionReleasedTime => ActionPressed ? 0: Time.time - actionTimeReleased;
    public static float InteractHeldTime => InteractPressed ? Time.time - interactTimeStarted : 0;


    #endregion

    private PlayerInput playerInput;
    private InputAction Move, /*Jump,*/ Look, Pause, Action, Interact;


    private Transform movementOrigin;

    private void Start()
    {
        movementOrigin = Camera.main.transform;
        playerInput = GetComponent<PlayerInput>();
        InitializeActions();
    }

    void InitializeActions()
    {
        var map = playerInput.currentActionMap;
        Move = map.FindAction(moveKey);
        //Jump = map.FindAction(jumpKey);
        Look = map.FindAction(lookKey);
        //Respawn = map.FindAction("Respawn");
        Pause = map.FindAction(pauseKey);
        Action = map.FindAction(actionKey);
        Interact = map.FindAction(interactKey);

        // Reset all inputs
        RemoveAllListeners();

        Move.started += ctx => InputActionStarted(ref MovePressed, MoveStarted, ref moveTimeStarted);
        //Jump.started += ctx => InputActionStarted(ref JumpPressed, JumpStarted);
        Action.started += ctx => InputActionStarted(ref ActionPressed, ActionStarted, ref actionTimeStarted);
        Interact.started += ctx => InputActionStarted(ref InteractPressed, InteractStarted);
        Pause.started += ctx => PauseStarted.Invoke();

        Move.canceled += ctx => InputActionCanceled(ref MovePressed, MoveCanceled, MoveHeldTime);
        //Jump.canceled += ctx => InputActionCanceled(ref JumpPressed, JumpCanceled);
        Action.canceled += ctx => InputActionCanceled(ref ActionPressed, ActionCanceled, ActionHeldTime, ref actionTimeReleased);
        Interact.canceled += ctx => InputActionCanceled(ref InteractPressed, InteractCanceled, InteractHeldTime);
    }
    void InputActionStarted(ref bool pressedFlag, UnityEvent actionEvent, bool ignorePaused = false)
    {
        if (GameManager.Instance.IsPaused && !ignorePaused)
            return;

        pressedFlag = true;
        actionEvent?.Invoke();
    }

    void InputActionStarted(ref bool pressedFlag, UnityEvent actionEvent, ref float timeStartedFlag, bool ignorePaused = false)
    {
        if (GameManager.Instance.IsPaused && !ignorePaused)
            return;

        timeStartedFlag = Time.time;

        pressedFlag = true;
        actionEvent?.Invoke();
    }

    void InputActionCanceled(ref bool pressedFlag, UnityEvent actionEvent)
    {
        actionEvent?.Invoke();
        pressedFlag = false;
    }

    void InputActionCanceled(ref bool pressedFlag, UnityEvent<float> actionEvent, float timeHeld)
    {
        actionEvent?.Invoke(timeHeld);
        pressedFlag = false;
    }

    void InputActionCanceled(ref bool pressedFlag, UnityEvent<float> actionEvent, float timeHeld, ref float timeStartedFlag)
    {
        timeStartedFlag = Time.time;

        actionEvent?.Invoke(timeHeld);
        pressedFlag = false;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsPaused)
            return;

        if (MovePressed) MoveHeld.Invoke(MoveHeldTime);
        else MoveNotHeld.Invoke();
        //if (JumpPressed) JumpHeld.Invoke();
        if (ActionPressed) ActionHeld.Invoke(ActionHeldTime);
        else ActionNotHeld.Invoke(ActionReleasedTime);
        if (InteractPressed) InteractHeld.Invoke(InteractHeldTime);

        LookUpdate.Invoke(LookDelta);
    }

    private void RemoveAllListeners()
    {
        MoveStarted.RemoveAllListeners();
        ActionStarted.RemoveAllListeners();
        InteractStarted.RemoveAllListeners();
        PauseStarted.RemoveAllListeners();

        MoveCanceled.RemoveAllListeners();
        ActionCanceled.RemoveAllListeners();
        InteractCanceled.RemoveAllListeners();
    }
    private void OnDisable()
    {
        Debug.Log("On Disable");
        Move?.Reset();   
        //Jump.Reset();
        Pause?.Reset();
        Action?.Reset();
        Interact?.Reset();
        Look?.Reset();
    }
}