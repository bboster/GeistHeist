/*
 * Contributors: Toby, Alec P, Clare G, Sky B, Tyler B
 * Creation Date: 9/16/25
 * Last Modified: 9/16/25
 * 
 * Connects to PlayerInput map actions and invokes static UnityEvents.
 * Use other scripts to connect to the unityevents.
 */

using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Events;
 using UnityEngine.InputSystem;

public class InputEvents : Singleton<InputEvents>
{
    // Events

    [SerializeField] private string moveKey = "Move";
    [SerializeField] private string jumpKey = "Jump";
    [SerializeField] private string pauseKey = "Pause";
    [SerializeField] private string lookKey = "Look";
    [SerializeField] private string actionKey = "Escape Object";
    [SerializeField] private string escapeObjectKey = "Action";

    public static UnityEvent MoveStarted = new UnityEvent();
    public static UnityEvent MoveHeld = new UnityEvent();
    public static UnityEvent MoveCanceled = new UnityEvent();

    public static UnityEvent JumpStarted = new UnityEvent();
    public static UnityEvent JumpHeld = new UnityEvent();
    public static UnityEvent JumpCanceled = new UnityEvent();

    public static UnityEvent ActionStarted = new UnityEvent();
    public static UnityEvent ActionHeld = new UnityEvent();
    public static UnityEvent ActionCanceled = new UnityEvent();

    public static UnityEvent EscapeObjectStarted = new UnityEvent();
    public static UnityEvent EscapeObjectHeld = new UnityEvent();
    public static UnityEvent EscapeObjectCanceled = new UnityEvent();

    public static UnityEvent PauseStarted = new UnityEvent();

    public static UnityEvent<Vector2> LookUpdate = new UnityEvent<Vector2>();

    //public static UnityEvent RestartStarted, RespawnStarted;

    [SerializeField] private float _sensitivity=1;

    // Input values and flags
    public Vector2 LookDelta => Look.ReadValue<Vector2>() * _sensitivity;
    public Vector3 FirstPersonInputDirection => movementOrigin.TransformDirection(new Vector3(InputDirection2D.x, 0f, InputDirection2D.y));
    public Vector3 ThirdPersonInputDirection => new Vector3() /*TODO: i have no fucking idea*/ ;
    public Vector2 InputDirection2D => Move.ReadValue<Vector2>();
    public static bool MovePressed, JumpPressed, ActionPressed, EscapeObjectPressed;

    private PlayerInput playerInput;
    private InputAction Move, Jump, Look, Pause, Action, EscapeObject;

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
        Jump = map.FindAction(jumpKey);
        Look = map.FindAction(lookKey);
        //Respawn = map.FindAction("Respawn");
        Pause = map.FindAction(pauseKey);
        Action = map.FindAction(actionKey);
        EscapeObject = map.FindAction(escapeObjectKey);

        Move.started += ctx => InputActionStarted(ref MovePressed, MoveStarted);
        Jump.started += ctx => InputActionStarted(ref JumpPressed, JumpStarted);
        Action.started += ctx => InputActionStarted(ref ActionPressed, ActionStarted);
        Pause.started += ctx => { PauseStarted.Invoke(); };

        Move.canceled += ctx => InputActionCanceled(ref MovePressed, MoveCanceled);
        Jump.canceled += ctx => InputActionCanceled(ref JumpPressed, JumpCanceled);
        Action.canceled += ctx => InputActionCanceled(ref ActionPressed, ActionCanceled);
    }
    void InputActionStarted(ref bool pressedFlag, UnityEvent actionEvent)
    {
        pressedFlag = true;
        actionEvent?.Invoke();
    }
    void InputActionCanceled(ref bool pressedFlag, UnityEvent actionEvent)
    {
        pressedFlag = false;
        actionEvent?.Invoke();
    }
    private void FixedUpdate()
    {
        if (MovePressed) MoveHeld.Invoke();
        if (JumpPressed) JumpHeld.Invoke();
        if (ActionPressed) ActionHeld.Invoke();
        if (EscapeObjectPressed) EscapeObjectHeld.Invoke();

        LookUpdate.Invoke(LookDelta);
    }
    private void OnDisable()
    {
        Move.Reset();   
        Jump.Reset();
        Pause.Reset();
        Action.Reset();
        EscapeObject.Reset();
        Look.Reset();
    }
}