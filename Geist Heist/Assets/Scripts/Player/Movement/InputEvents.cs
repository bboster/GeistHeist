/*
 * Contributors: Toby, Alec P, Clare G, Sky B, Tyler B, Josh K
 * Creation Date: Spring 2024
 * Last Modified: 2/10/2026
 * 
 * Connects to PlayerInput map actions and invokes static UnityEvents.
 * Use other scripts to connect to the unityevents.
 */

using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

public class InputEvents : DontDestroyOnLoadSingleton<InputEvents>
{

    // Events

    [SerializeField] private string moveKey = "Move";
    //[SerializeField] private string jumpKey = "Jump";
    [SerializeField] private string pauseKey = "Pause";
    [SerializeField] private string lookKey = "Look";
    [SerializeField] private string actionKey = "Escape Object";
    [SerializeField] private string interactKey = "Interact";
    [SerializeField] private string debugKey = "DebugConsole";
    //[SerializeField] private string spaceKey = "Space";

    public static UnityEvent MoveStarted = new UnityEvent();
    public static UnityEvent<float> MoveHeld = new();
    public static UnityEvent MoveNotHeld = new UnityEvent();
    public static UnityEvent<float> MoveCanceled = new();

    public static UnityEvent ActionStarted = new UnityEvent();
    public static UnityEvent ActionStarted_WhilePaused = new UnityEvent();
    public static UnityEvent<float> ActionHeld = new();
    public static UnityEvent<float> ActionNotHeld = new();
    public static UnityEvent<float> ActionCanceled = new();

    public static UnityEvent InteractStarted = new();
    public static UnityEvent<float> InteractHeld = new();
    public static UnityEvent<float> InteractCanceled = new();

    public static UnityEvent PauseStarted = new UnityEvent();
    public static UnityEvent DebugStarted = new UnityEvent();
    public static UnityAction PauseStartedOverride = null;

    /*public static UnityEvent SpaceStarted = new UnityEvent();
    public static UnityEvent<float> SpaceHeld = new();
    public static UnityEvent<float> SpaceCanceled = new(); */

    public static UnityEvent<Vector2> LookUpdate = new UnityEvent<Vector2>();

    [SerializeField] private float _sensitivity = 1;

    [SerializeField] private bool InitializeAtStart = false; // to override with main menu

    public Vector2 LookDelta => Look.ReadValue<Vector2>() * _sensitivity;
    public Vector3 FirstPersonInputDirection => (
        (movementOrigin.forward * InputDirection2D.y)
        + (movementOrigin.right * InputDirection2D.x))
        .WithY(0)
        .normalized;
    [HideInInspector] public bool CutsceneRunning = false;
    public Vector2 InputDirection2D => (FadeToBlack.Instance == null && !CutsceneRunning) ? Move.ReadValue<Vector2>() : Vector2.zero;
    public static bool MovePressed, /*JumpPressed,*/ ActionPressed, InteractPressed, PausePressed/*, SpacePressed*/;

    public UnityEvent OnControllerChanged = new();

    #region Time Held
    private static float moveTimeStarted = -1f, actionTimeStarted = -1f, interactTimeStarted = -1f; // other inputs can be added but i dont think theyre super necessary.
    private static float actionTimeReleased = -1;
    public static float MoveHeldTime => MovePressed ? Time.time - moveTimeStarted : 0;
    public static float ActionHeldTime => ActionPressed ? Time.time - actionTimeStarted : 0;
    public static float ActionReleasedTime => ActionPressed ? 0 : Time.time - actionTimeReleased;
    public static float InteractHeldTime => InteractPressed ? Time.time - interactTimeStarted : 0;
    //public static float SpaceHeldTime => SpacePressed ? Time.time - spaceTimeStarted : 0; // only needed if space input is restored


    #endregion

    private PlayerInput playerInput;
    private InputAction Move, /*Jump,*/ Look, Pause, DebugA, Action, Interact/*, Space*/;

    private Transform movementOrigin => GetCamera();
    private Transform _movementOrigin;

    private InputControlScheme? _gamepadScheme;
    private InputControlScheme? _kbmScheme;

    private bool isCurrentlyKeyboard;
    private InputDevice _currentDevice = null;
    private bool _canUseControlSwap = true;
    private WaitForEndOfFrame _endOfFrame = null;

    private Coroutine updateCoroutine;

    protected override void Awake()
    {
        // for main menu only
        if (InitializeAtStart)
            Initialize();
    }

    public void Initialize()
    {
        if (Instance != this)
            base.Awake();

        if (Instance != this)
            return;

        playerInput = GetComponent<PlayerInput>();
        InitializeActions();
        CacheControlSchemes();
        
        // Subscribe to device/scheme changes instead of polling
        InputUser.onChange += OnInputUserChanged;
        
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (updateCoroutine == null)
            updateCoroutine = StartCoroutine(UnscaledUpdate());
    }

    #region Controllers
    private void OnInputUserChanged(InputUser user, InputUserChange change, InputDevice device)
    {
        if (device == null || _currentDevice == device || !_canUseControlSwap ||
            change == InputUserChange.DeviceUnpaired)
            return;

        if (!user.valid || playerInput == null)
            return;

        if (device is Gamepad && _gamepadScheme.HasValue && playerInput.currentControlScheme != _gamepadScheme.Value.name)
        {
            playerInput.SwitchCurrentControlScheme(_gamepadScheme.Value.name, device);
            _currentDevice = device;
            _canUseControlSwap = false;

            if(isCurrentlyKeyboard)
                OnControllerChanged.Invoke();
            isCurrentlyKeyboard = false;

            StartCoroutine(PreventControlSwapUntilEndOfFrame());
        }
        else if ((device is Keyboard || device is Mouse) && _kbmScheme.HasValue && playerInput.currentControlScheme != _kbmScheme.Value.name)
        {
            playerInput.SwitchCurrentControlScheme(_kbmScheme.Value.name, Keyboard.current, Mouse.current);
            _currentDevice = device;
            _canUseControlSwap = false;

            if(!isCurrentlyKeyboard)
                OnControllerChanged.Invoke();
            isCurrentlyKeyboard = true;

            StartCoroutine(PreventControlSwapUntilEndOfFrame());
        }
    }

    private IEnumerator PreventControlSwapUntilEndOfFrame()
    {
        yield return _endOfFrame ??= new WaitForEndOfFrame();
        _canUseControlSwap = true;
    }

    private void OnDestroy()
    {
        InputUser.onChange -= OnInputUserChanged;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void CacheControlSchemes()
    {
        if (playerInput?.actions == null)
            return;

        var controlSchemes = playerInput.actions.controlSchemes;
        _gamepadScheme = null;
        _kbmScheme = null;

        foreach (var scheme in controlSchemes)
        {
            bool hasGamepad = false;
            bool hasKeyboardMouse = false;

            foreach (var req in scheme.deviceRequirements)
            {
                if (req.controlPath.Contains("Gamepad"))
                    hasGamepad = true;
                if (req.controlPath.Contains("Keyboard") || req.controlPath.Contains("Mouse"))
                    hasKeyboardMouse = true;
            }

            if (hasGamepad)
                _gamepadScheme = scheme;
            if (hasKeyboardMouse)
                _kbmScheme = scheme;
        }
    }


    #endregion

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
        DebugA = map.FindAction(debugKey);
        //Space = map.FindAction(spaceKey);

        // Reset all inputs
        RemoveAllListeners();

        Move.started += ctx => InputActionStarted(ref MovePressed, MoveStarted, ref moveTimeStarted);
        //Jump.started += ctx => InputActionStarted(ref JumpPressed, JumpStarted);
        Action.started += ctx => InputActionStarted(ref ActionPressed, ActionStarted, ref actionTimeStarted, ignorePaused: false);
        Action.started += ctx => InputActionStarted(ActionStarted_WhilePaused, ignorePaused: true);
        Interact.started += ctx => InputActionStarted(ref InteractPressed, InteractStarted, ref interactTimeStarted);
        //Space.started += ctx => InputActionStarted(ref SpacePressed, SpaceStarted, ref spaceTimeStarted);
        Pause.started += ctx => OnPauseStarted();
        DebugA.started += ctx => { DebugStarted.Invoke(); };

        Move.canceled += ctx => InputActionCanceled(ref MovePressed, MoveCanceled, MoveHeldTime);
        //Jump.canceled += ctx => InputActionCanceled(ref JumpPressed, JumpCanceled);
        Action.canceled += ctx => InputActionCanceled(ref ActionPressed, ActionCanceled, ActionHeldTime, ref actionTimeReleased);
        Interact.canceled += ctx => InputActionCanceled(ref InteractPressed, InteractCanceled, InteractHeldTime);
        //Space.canceled += ctx => InputActionCanceled(ref SpacePressed, SpaceCanceled, SpaceHeldTime);
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
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsPaused && !ignorePaused)
            return;

        timeStartedFlag = Time.time;
        pressedFlag = true;
        actionEvent?.Invoke();
    }

    void InputActionStarted(UnityEvent actionEvent, bool ignorePaused = false)
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsPaused && !ignorePaused)
            return;

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

    void OnPauseStarted()
    {
        if (ConfirmationPopup.AnyConfirmationMenuOpen) //Should prevent pause menu from opening over a confirmation menu
            return;

        if (PauseStartedOverride != null)
            PauseStartedOverride();
        else
            PauseStarted.Invoke();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && (GameManager.Instance.IsPaused || GameManager.Instance.IsPlayerInMenu))
            return;

        if (MovePressed) MoveHeld.Invoke(MoveHeldTime);
        else MoveNotHeld.Invoke();
        //if (JumpPressed) JumpHeld.Invoke();
        if (ActionPressed) ActionHeld.Invoke(ActionHeldTime);
        else ActionNotHeld.Invoke(ActionReleasedTime);
        if (InteractPressed) InteractHeld.Invoke(InteractHeldTime);
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPaused)
            LookUpdate.Invoke(LookDelta);
    }

    IEnumerator UnscaledUpdate()
    {
        while (true)
        {
            yield return null;

            // Polling-based device switching (catches input not yet in InputUser.onChange)
            if (playerInput == null || playerInput.actions == null || !_canUseControlSwap)
                continue;

            if (!WasAnySwitchRelevantDeviceUpdatedThisFrame())
                continue;

            //OnControllerChanged.Invoke();

            string currentScheme = playerInput.currentControlScheme;

            if (TrySwitchToKeyboardMouseScheme(currentScheme))
                continue;

            TrySwitchToGamepadScheme(currentScheme);
        }
    }

    private static bool WasAnySwitchRelevantDeviceUpdatedThisFrame()
    {
        return (Keyboard.current?.wasUpdatedThisFrame ?? false) ||
               (Mouse.current?.wasUpdatedThisFrame ?? false) ||
               (Gamepad.current?.wasUpdatedThisFrame ?? false);
    }

    private bool TrySwitchToKeyboardMouseScheme(string currentScheme)
    {
        if (!_gamepadScheme.HasValue || currentScheme != _gamepadScheme.Value.name)
            return false;

        if ((Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) ||
            (Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.01f))
        {
            if (_kbmScheme.HasValue)
            {
                playerInput.SwitchCurrentControlScheme(_kbmScheme.Value.name, Keyboard.current, Mouse.current);
                _currentDevice = Keyboard.current;
                _canUseControlSwap = false;

                if(!isCurrentlyKeyboard)
                    OnControllerChanged.Invoke();
                isCurrentlyKeyboard = true;

                StartCoroutine(PreventControlSwapUntilEndOfFrame());
            }
            return true;
        }

        return false;
    }

    private bool TrySwitchToGamepadScheme(string currentScheme)
    {
        if (!_kbmScheme.HasValue || currentScheme != _kbmScheme.Value.name)
            return false;

        if (Gamepad.current != null && IsInputFromGamepad())
        {
            if(isCurrentlyKeyboard)
                OnControllerChanged.Invoke();

            if (_gamepadScheme.HasValue)
            {
                isCurrentlyKeyboard = false;
                playerInput.SwitchCurrentControlScheme(_gamepadScheme.Value.name, Gamepad.current);
                _currentDevice = Gamepad.current;
                _canUseControlSwap = false;
                StartCoroutine(PreventControlSwapUntilEndOfFrame());
            }
            return true;
        }

        return false;
    }

    // Raw gamepad activity detector for scheme switching.
    // This must not rely on action.activeControl while on KBM scheme.
    private bool IsInputFromGamepad()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
            return false;

        return gamepad.leftStick.ReadValue().sqrMagnitude > 0.15f ||
               gamepad.rightStick.ReadValue().sqrMagnitude > 0.15f ||
               gamepad.dpad.ReadValue().sqrMagnitude > 0.1f ||
               gamepad.leftTrigger.ReadValue() > 0.1f ||
               gamepad.rightTrigger.ReadValue() > 0.1f ||
               gamepad.buttonSouth.isPressed ||
               gamepad.buttonNorth.isPressed ||
               gamepad.buttonEast.isPressed ||
               gamepad.buttonWest.isPressed ||
               gamepad.leftShoulder.isPressed ||
               gamepad.rightShoulder.isPressed ||
               gamepad.startButton.isPressed ||
               gamepad.selectButton.isPressed;
    }

    // Detects move-input source only (use this for movement-tuned behavior).
    public bool IsMoveInputFromGamepad()
    {
        return Move?.activeControl?.device is Gamepad;
    }

    // UI scripts should use this to decide which prompts to show.
    public bool IsGamepadActive()
    {
        if (playerInput == null)
            return false;

        string currentScheme = playerInput.currentControlScheme;
        if (string.IsNullOrEmpty(currentScheme))
            return false;

        if (_gamepadScheme.HasValue)    
            return currentScheme == _gamepadScheme.Value.name;

        return currentScheme.IndexOf("Gamepad", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void RemoveAllListeners()
    {
        MoveStarted.RemoveAllListeners();
        MoveHeld.RemoveAllListeners();
        MoveNotHeld.RemoveAllListeners();
        MoveCanceled.RemoveAllListeners();

        ActionStarted.RemoveAllListeners();
        ActionHeld.RemoveAllListeners();
        ActionNotHeld.RemoveAllListeners();
        ActionCanceled.RemoveAllListeners();

        InteractStarted.RemoveAllListeners();
        InteractHeld.RemoveAllListeners();
        InteractCanceled.RemoveAllListeners();

        PauseStarted.RemoveAllListeners();
        DebugStarted.RemoveAllListeners();
        LookUpdate.RemoveAllListeners();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Move?.Reset();
        Pause?.Reset();
        Action?.Reset();
        Interact?.Reset();
        Look?.Reset();
        DebugA?.Reset();
        //Space?.Reset();

        RemoveAllListeners();
    }

    #region Camera

    Transform GetCamera()
    {
        if (_movementOrigin == null)
            _movementOrigin = Camera.main.transform;

        return _movementOrigin;
    }

    #endregion
}
