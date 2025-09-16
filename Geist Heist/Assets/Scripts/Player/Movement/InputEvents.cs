/*
 * Connects to PlayerInput map actions and invokes static UnityEvents.
 * Use other scripts to connect to the unityevents.
 * 
 * - Alec P, Toby S, Clare G, Sky B, Tyler B
 */

using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputEvents : Singleton<InputEvents>
{
    // Events

    [SerializeField] private string moveKey = "Move";
    [SerializeField] private string leftClickKey = "LeftClick";
    [SerializeField] private string jumpKey = "Jump";
    [SerializeField] private string pauseKey = "Pause";
    [SerializeField] private string lookKey = "Look";

    public static UnityEvent MoveStarted = new UnityEvent();
    public static UnityEvent MoveHeld = new UnityEvent();
    public static UnityEvent MoveCanceled = new UnityEvent();

    public static UnityEvent JumpStarted = new UnityEvent();
    public static UnityEvent JumpHeld = new UnityEvent();
    public static UnityEvent JumpCanceled = new UnityEvent();

    public static UnityEvent PauseStarted = new UnityEvent();
    public static UnityEvent PauseCanceled = new UnityEvent();

    public static UnityEvent LeftClickStarted = new UnityEvent();
    public static UnityEvent LeftClickHeld = new UnityEvent();
    public static UnityEvent LeftClickCanceled = new UnityEvent();

    //public static UnityEvent RestartStarted, RespawnStarted;

    [SerializeField] private float _sensitivity=1;

    // Input values and flags
    public Vector2 LookDelta => Look.ReadValue<Vector2>() * _sensitivity;
    public Vector3 InputDirection => movementOrigin.TransformDirection(new Vector3(InputDirection2D.x, 0f, InputDirection2D.y));
    public Vector2 InputDirection2D => Move.ReadValue<Vector2>();
    public static bool MovePressed, JumpPressed, LeftClickPressed, PausePressed;

    private PlayerInput playerInput;
    private InputAction Move, LeftClick, Jump, Look, Pause;

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
        LeftClick = map.FindAction(leftClickKey);

        Move.started += ctx => ActionStarted(ref MovePressed, MoveStarted);
        Jump.started += ctx => ActionStarted(ref JumpPressed, JumpStarted);
        LeftClick.started += ctx => ActionStarted(ref LeftClickPressed, LeftClickStarted);
        Pause.started += ctx => { PausePressed = true; PauseStarted.Invoke(); };

        Move.canceled += ctx => ActionCanceled(ref MovePressed, MoveCanceled);
        Jump.canceled += ctx => ActionCanceled(ref JumpPressed, JumpCanceled);
        LeftClick.canceled += ctx => ActionCanceled(ref LeftClickPressed, LeftClickCanceled);
        Pause.canceled += ctx => { PausePressed = false; PauseCanceled.Invoke(); };
    }
    void ActionStarted(ref bool pressedFlag, UnityEvent actionEvent)
    {
        pressedFlag = true;
        actionEvent?.Invoke();
    }
    void ActionCanceled(ref bool pressedFlag, UnityEvent actionEvent)
    {
        pressedFlag = false;
        actionEvent?.Invoke();
    }
    private void Update()
    {
        if (MovePressed) MoveHeld.Invoke();
        if (JumpPressed) JumpHeld.Invoke();
        if (LeftClickPressed) LeftClickHeld.Invoke();
    }
    private void OnDisable()
    {
        Move.started -= ctx => ActionStarted(ref MovePressed, MoveStarted);
        Jump.started -= ctx => ActionStarted(ref JumpPressed, JumpStarted);
        LeftClick.started -= ctx => ActionStarted(ref LeftClickPressed, LeftClickStarted);
        Pause.started -= ctx => { PausePressed = true; PauseStarted.Invoke(); };

        Move.canceled -= ctx => ActionCanceled(ref MovePressed, MoveCanceled);
        Jump.canceled -= ctx => ActionCanceled(ref JumpPressed, JumpCanceled);
        LeftClick.canceled -= ctx => ActionCanceled(ref LeftClickPressed, LeftClickCanceled);
        Pause.canceled -= ctx => { PausePressed = false; PauseCanceled.Invoke(); };
    }
}