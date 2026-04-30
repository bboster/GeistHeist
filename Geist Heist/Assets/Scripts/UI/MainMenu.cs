/*
 * Contributors: Toby Schamberger, Joshua Kelly
 * Creation: 10/20/25
 * Last Edited: 4/29/2026
 * Summary: Handles button functionality for main menu.
 * The player will be prompted to delete their save if they press new game after having save data.
 */

using NaughtyAttributes;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField, Required] private RectTransform canvasRectTransform;

    [SerializeField, BoxGroup("Idle Animation")] private float secondsOfInactivityForIdle = 10;
    [SerializeField, BoxGroup("Idle Animation"), Required] private Button pressAnyButtonButton;

    [SerializeField, BoxGroup("Hub Scene"), Scene] private string HubScene;
    [SerializeField, BoxGroup("Hub Scene")] private GameObject HubSceneLoadingCardPrefab;

    [SerializeField, BoxGroup("New Game Scene"), Scene] private string NewGameScene; // making it separate because i imagine we will have a tutorial level or a cutscene or something play on a new save.
    [SerializeField, BoxGroup("New Game Scene")] private GameObject NewSceneLoadingCardPrefab;
    [SerializeField, BoxGroup("New Game Scene")] string confirmNewGameText = "Are you sure? Continuing will delete your progress.";
    [SerializeField, BoxGroup("New Game Scene")] bool confirmationForNewGame = true;

    [Header("Settings")]
    [SerializeField, Required] private ConfirmationPopup confirmationPopup;

    [Header("Fog")]
    [SerializeField, Required] private RenderTexture leftFogRenderTexture;
    [SerializeField, Required] private Camera leftFogRenderCamera;
    [SerializeField, Required] private Material leftFogMaterial;
    [SerializeField, Required] private RawImage leftFogImage;
    [SerializeField, Required] private RenderTexture rightFogRenderTexture;
    [SerializeField, Required] private Camera rightFogRenderCamera;
    [SerializeField, Required] private Material rightFogMaterial;
    [SerializeField, Required] private RawImage rightFogImage;

    [Header("Main Page")]
    [SerializeField, Required] private Button newGameButton;
    [SerializeField, Required] private Button continueGameButton;
    [SerializeField, Required] private Button settingsButton;
    [SerializeField, Required] private Button creditsButton;
    [SerializeField] private Button howToPlayButton;
    [SerializeField, Required] private Button quitGameButton;

    [Header("Credits Page")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float SecondsForFullCreditsScroll = 60;
    [SerializeField] private float CreditsSpeedMultiplierIfButtonHeld = 3;
    [SerializeField, Required] private CanvasGroup creditsPage;
    [SerializeField, Required] private RectTransform creditsScrollArea;
    [SerializeField, Required] private Button closeCreditsButton;

    [Header("How to Play Page")]
    [SerializeField, Required] private CanvasGroup howToPlayPage;
    [SerializeField, Required] private Button closeHowToPlayButton;

    [Header("Settings Page")]
    [SerializeField, Required] private SettingsTab settingsTab;
    [SerializeField, Required] private CanvasGroup settingsPage;
    [SerializeField, Required] private Button closeSettingsButton;

    private static float SECONDS_UNTIL_PLAYER_CAN_PLAY_THE_GAME = 0.25f;

    // this shouldve been an enum
    private bool settingsOpen = false;
    private bool creditsOpen = false;
    private bool confirmNewGameOpen = false;

    // if the player has played before and got past the first level
    private bool playerHasSignificantSaveData;
    private InputAction menuCancelAction;
    private Animator mainMenuAnimator;
    private float TimeOfLastAnyButtonPressed = 0;
    private float? timeOfFirstAnyButton = null;
    private Coroutine waitToDelayCoroutine;
    private bool introAnimationFinished = false;
    private bool menuActive = false;
    private bool? gamepadActive = null;

    // credits
    private float creditsStartY;
    private Coroutine creditsCoroutine;
    private InputAction speedUpCreditsAction;

    private void OnEnable()
    {
        TrySubscribeToUICancel();
    }

    private void Awake()
    {
        MusicManager.Instance.Initialize(); //Music cannot play in menu without this line
    }

    private void Start()
    {
        mainMenuAnimator = GetComponent<Animator>();
        mainMenuAnimator.SetBool("Active", false);

        leftFogRenderTexture = new RenderTexture(3840, 2160, leftFogRenderTexture.depth, leftFogRenderTexture.format);
        leftFogRenderTexture.Create();
        leftFogRenderCamera.targetTexture = leftFogRenderTexture;
        var leftFogMaterialCopy = Instantiate(leftFogMaterial);
        leftFogMaterialCopy.SetTexture("_Render_Texture", leftFogRenderTexture);
        leftFogImage.material = leftFogMaterialCopy;

        rightFogRenderTexture = new RenderTexture(3840, 2160, rightFogRenderTexture.depth, rightFogRenderTexture.format);
        rightFogRenderCamera.targetTexture = rightFogRenderTexture;
        var rightFogMaterialCopy = Instantiate(rightFogMaterial);
        rightFogMaterialCopy.SetTexture("_Render_Texture", rightFogRenderTexture);
        rightFogImage.material = rightFogMaterialCopy;

        TrySubscribeToUICancel();

        playerHasSignificantSaveData =
               SaveDataManager.Instance.DoesSaveDataExist()
            && SaveDataManager.Instance.GetLevelsCompletedCount() > 0;

        creditsStartY = creditsScrollArea.position.y;
        speedUpCreditsAction = playerInput.actions.FindAction("SpeedUpCredits");

        // hide/show continue button based on if save data exists
        continueGameButton.gameObject.SetActive(playerHasSignificantSaveData);
        EventSystem.current.SetSelectedGameObject(pressAnyButtonButton.gameObject);

        // Hide other pages
        // The only reason im setting them active in code instead of having them active in scene is that i do not trust game designers
        howToPlayPage.gameObject.SetActive(true);
        StaticUtilities.DisableCanvasGroup(howToPlayPage);
        settingsPage.gameObject.SetActive(true);
        StaticUtilities.DisableCanvasGroup(settingsPage);
        settingsOpen = false;
        creditsPage.gameObject.SetActive(true);
        // open credits if the player just beat the game
        if (SceneLoadManager.Instance.PlayCreditsQueued)
        {
            Debug.Log("Opening credits since they are queued");
            //StaticUtilities.EnableCanvasGroup(creditsPage);
            //creditsOpen = true;
            OnCreditsButtonClicked();
        }
        else
        {
            StaticUtilities.DisableCanvasGroup(creditsPage);
            creditsOpen = false;
        }
            

        InputEvents.Instance.OnControllerChanged.AddListener(OnControllerChanged);
        OnControllerChanged();

        InputEvents.PauseStarted.AddListener(OnSettingsBackButtonClicked);
        InputEvents.ActionStarted.AddListener(OnSettingsBackButtonClicked); // because its B on controller

        InputSystem.onAnyButtonPress.Call((ctrl) => OnAnyButtonPressed());
        pressAnyButtonButton.onClick.AddListener(OnAnyButtonPressed);


        // Main Menu
        newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        continueGameButton.onClick.AddListener(OnContinueButtonClicked);
        settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        if(creditsButton != null) creditsButton.onClick.AddListener(OnCreditsButtonClicked);
        if(howToPlayButton != null) howToPlayButton.onClick.AddListener(OnHowToPlayButtonClicked);
        quitGameButton.onClick.AddListener(OnQuitButtonClicked);

        // Credits
        if(closeCreditsButton!= null) closeCreditsButton.onClick.AddListener(OnCreditsBackButtonClicked);

        // How to Play
        if (closeHowToPlayButton != null) closeHowToPlayButton.onClick.AddListener(OnCloseHowToPlayButtonClicked);

        // settings
        closeSettingsButton.onClick.AddListener(OnSettingsBackButtonClicked);

        // Confirmation Popup
        confirmationPopup.HideConfirmationPopup();

        StaticUtilities.StartCoroutineIfNotPlaying(ref waitToDelayCoroutine, CheckActiveState());

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void TrySubscribeToUICancel()
    {
        if (menuCancelAction != null)
            return;

        InputSystemUIInputModule uiInputModule = EventSystem.current != null
            ? EventSystem.current.currentInputModule as InputSystemUIInputModule
            : null;

        if (uiInputModule == null)
            uiInputModule = Object.FindFirstObjectByType<InputSystemUIInputModule>();

        if (uiInputModule == null || uiInputModule.cancel == null || uiInputModule.cancel.action == null)
            return;

        menuCancelAction = uiInputModule.cancel.action;
        menuCancelAction.performed += OnMenuCancel;
    }

    private void OnDisable()
    {
        if (menuCancelAction == null)
            return;

        menuCancelAction.performed -= OnMenuCancel;
        menuCancelAction = null;
    }

    /// <summary>
    /// Clears players save data and starts the game
    /// </summary>
    void LoadNewGame()
    {
        SaveDataManager.Instance.ClearSaveFile();
        LoadScene(NewGameScene, NewSceneLoadingCardPrefab);
    }

    void LoadScene(string sceneToLoad, GameObject loadingCardPrefab)
    {
        Debug.LogError("No transition card set on " + gameObject.name);
        //SceneLoadManager.Instance.LoadScene(sceneToLoad);
        LevelManager.Instance.InstantiateFadeToBlack(() => LevelManager.Instance.ChangeScene(sceneToLoad));
    }

    #region Buttons OnClicked

    #region Main Page

    void OnNewGameButtonClicked()
    {
        if (IsOffScreen(newGameButton))
        {
            Debug.LogWarning("newGameButton not on screen yet");
            return;
        }

        if (timeOfFirstAnyButton == null) return;

        // dont let player skip right into gameplay 
        if(InputEvents.Instance.IsGamepadActive() && Time.unscaledTime - timeOfFirstAnyButton.Value < SECONDS_UNTIL_PLAYER_CAN_PLAY_THE_GAME)
        {
            Debug.Log("player pressed play too early");
            return;
        }

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);

        // if first time playing / no save data
        if (!playerHasSignificantSaveData || !confirmationForNewGame)
        {
            LoadNewGame();
            return;
        }

        // if player has save data: open confirmation popup
        StaticUtilities.DisableCanvasGroup(howToPlayPage);
        StaticUtilities.DisableCanvasGroup(creditsPage);

        confirmNewGameOpen = true;
        confirmationPopup.OpenConfirmationPopup(confirmNewGameText, OnConfirmDeleteSaveButtonClicked, OnCancelButtonClicked: OnCancelDeleteSaveButtonClicked);
    }

    void OnContinueButtonClicked()
    {
        if (IsOffScreen(continueGameButton))
        {
            Debug.LogWarning("continueGameButton not on screen yet");
            return;
        }

        if (timeOfFirstAnyButton == null) return;

        // dont let player skip right into gameplay 
        if (InputEvents.Instance.IsGamepadActive() && Time.unscaledTime - timeOfFirstAnyButton.Value < SECONDS_UNTIL_PLAYER_CAN_PLAY_THE_GAME)
        {
            Debug.Log("player pressed play too early");
            return;
        }

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);

        //SceneManager.LoadScene(HubScene);
        LoadScene(HubScene, NewSceneLoadingCardPrefab);
    }

    void OnSettingsButtonClicked()
    {
        if (IsOffScreen(settingsButton))
        {
            Debug.LogWarning("settings button not on screen yet");
            return;
        }

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);
        settingsOpen = true;
        PauseMenuTab.currentOpenTab = null;
        settingsTab.OpenTab();

        StaticUtilities.EnableCanvasGroup(settingsPage);

        if (InputEvents.Instance.IsGamepadActive())
        {
            EventSystem.current.SetSelectedGameObject(closeSettingsButton.gameObject);
        }
    }

    void OnCreditsButtonClicked()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);
        creditsOpen = true;

        StaticUtilities.DisableCanvasGroup(howToPlayPage);
        StaticUtilities.EnableCanvasGroup(creditsPage);

        InitializeCredits();
    }

    void OnHowToPlayButtonClicked()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);

        StaticUtilities.DisableCanvasGroup(creditsPage);
        StaticUtilities.EnableCanvasGroup(howToPlayPage);
        EventSystem.current.SetSelectedGameObject(closeHowToPlayButton.gameObject);
    }

    void OnQuitButtonClicked()
    {
        // check if button is on screen.
        if (IsOffScreen(quitGameButton))
        {
            Debug.LogWarning("quit button not on screen yet");
            return;
        }

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);

#if UNITY_EDITOR
        //EditorApplication.isPlaying = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
#else
        Application.Quit();
#endif
    }

    #endregion

    #region Confirm Delete Save

    void OnConfirmDeleteSaveButtonClicked()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);

        confirmNewGameOpen = false;
        LoadNewGame();
    }

    void OnCancelDeleteSaveButtonClicked()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);
        confirmNewGameOpen = false;
        EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
    }

    #endregion

    

    #region Settings

    void OnSettingsBackButtonClicked()
    {
        // since player can activate this by pressing esc
        if(settingsOpen == false) { return; }   

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);

        StaticUtilities.DisableCanvasGroup(settingsPage);
        if(InputEvents.Instance.IsGamepadActive())
            EventSystem.current.SetSelectedGameObject(settingsButton.gameObject);

        settingsOpen = false;

        settingsTab.CloseTab();
    }

    #endregion

    #region How To Play

    void OnCloseHowToPlayButtonClicked()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);

        StaticUtilities.DisableCanvasGroup(howToPlayPage);
        EventSystem.current.SetSelectedGameObject(howToPlayButton.gameObject);
    }

    #endregion

    #endregion

    #region Animations

    void OnAnyButtonPressed()
    {
        // ignore input for a tiny bit
        if (introAnimationFinished == false)
        {
            if (InputEvents.Instance.IsGamepadActive())
            {
                EventSystem.current?.SetSelectedGameObject(pressAnyButtonButton.gameObject);
            }
            return;
        }

        /*
        // it counts moving your mouse as an input (sob)
        if (device != null && device.ToString().Contains("Mouse")) return;

        // because gamepad inputs (like stick movements) should count on active menu but not idle menu
        if (!menuActive && device != null && device is Gamepad gamepad)
        {
            float stickMagnitude =
                gamepad.leftStick.ReadValue().magnitude +
                gamepad.rightStick.ReadValue().magnitude;

            if (stickMagnitude < 0.2f)
                return;
        }*/

        Debug.Log("Any Button pressed");

        TimeOfLastAnyButtonPressed = Time.unscaledTime;

        if(timeOfFirstAnyButton == null) timeOfFirstAnyButton = Time.unscaledTime;
    }


    IEnumerator CheckActiveState()
    {
        while (introAnimationFinished == false) yield return null;

        bool wasActive = false;
        while (true)
        {
            if (mainMenuAnimator == null)
                yield break;

            // refresh timer so menu doesnt go back to idle while user is in submenu
            if (settingsOpen || creditsOpen)
                TimeOfLastAnyButtonPressed = Time.unscaledTime;

            //Debug.Log(Time.unscaledTime - TimeOfLastAnyButtonPressed);
            menuActive = (Time.unscaledTime - TimeOfLastAnyButtonPressed <= secondsOfInactivityForIdle);
            if(mainMenuAnimator != null) mainMenuAnimator.SetBool("Active", menuActive);

            // frame that menu became inactive
            if (!menuActive &&  wasActive) OnMenuEnterIdle();
            if ( menuActive && !wasActive) OnMenuEnterActive();

            wasActive = menuActive;

            yield return null;
        }
    }

    void OnMenuEnterIdle()
    {
        if (InputEvents.Instance.IsGamepadActive())
        {
            EventSystem.current?.SetSelectedGameObject(pressAnyButtonButton.gameObject);
            timeOfFirstAnyButton = null;
        }
    }

    void OnMenuEnterActive()
    {
        if (InputEvents.Instance.IsGamepadActive())
        {
            if (continueGameButton.gameObject.activeSelf) EventSystem.current?.SetSelectedGameObject(continueGameButton.gameObject);
            else EventSystem.current?.SetSelectedGameObject(newGameButton.gameObject);
        }
    }

    public void OnMainMenuIntroFinished() => introAnimationFinished = true;


    #endregion

    void OnMenuCancel(InputAction.CallbackContext _)
    {
        CanvasGroup popupGroup = confirmationPopup != null ? confirmationPopup.GetComponent<CanvasGroup>() : null;
        bool popupOpen = popupGroup != null && popupGroup.interactable && popupGroup.alpha > 0.001f;
        if (popupOpen)
        {
            confirmationPopup.HideConfirmationPopup();
            EventSystem.current.SetSelectedGameObject(
                playerHasSignificantSaveData ? continueGameButton.gameObject : newGameButton.gameObject);
            return;
        }

        if (howToPlayPage != null && howToPlayPage.interactable && howToPlayPage.alpha > 0.001f)
        {
            OnCloseHowToPlayButtonClicked();
            return;
        }

        if (creditsPage != null && creditsPage.interactable && creditsPage.alpha > 0.001f && !SceneLoadManager.Instance.PlayCreditsQueued)
        {
            OnCreditsBackButtonClicked();
            return;
        }
    }

    #region Controller / Keyboard

    void OnControllerChanged()
    {
        if (InputEvents.Instance.IsGamepadActive())
            OnGamepadInputActivated();
        else
            OnKeyboardInputActivated();
    }

    void OnKeyboardInputActivated()
    {
        // if we already know its active
        if (gamepadActive.HasValue && gamepadActive.Value == false)
            return;

        gamepadActive = false;

        Debug.Log("Mouse input activated");

        EventSystem.current.SetSelectedGameObject(null);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnGamepadInputActivated()
    {
        // bad solution, refreshes ui countdown
        //OnAnyButtonPressed(null, null);

        if (!menuActive && !settingsOpen && !creditsOpen)
        {
            EventSystem.current?.SetSelectedGameObject(pressAnyButtonButton.gameObject);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("Gamepad input detected");

        // ==== First frame after swapped to controller only: ====

        if (gamepadActive.HasValue && gamepadActive.Value == true) 
            return;

        gamepadActive = true;

        if(settingsOpen)
        {
            EventSystem.current?.SetSelectedGameObject(closeSettingsButton.gameObject);
            return;
        }

        if (creditsOpen)
        {
            EventSystem.current?.SetSelectedGameObject(closeCreditsButton.gameObject);
            return;
        }

        if(confirmNewGameOpen)
        {
            EventSystem.current?.SetSelectedGameObject(confirmationPopup.cancelButton.gameObject);
            return;
        }

        if (menuActive)
        {
            if (continueGameButton.gameObject.activeSelf) EventSystem.current?.SetSelectedGameObject(continueGameButton.gameObject);
            else EventSystem.current?.SetSelectedGameObject(newGameButton.gameObject);
        }

        
    }

    #endregion

    #region Credits

    void InitializeCredits()
    {
        if (InputEvents.Instance.IsGamepadActive())
        {
            EventSystem.current.SetSelectedGameObject(closeCreditsButton.gameObject);
        }

        StaticUtilities.StopAndStartCoroutine(ref creditsCoroutine, ScrollCredits());
    }

    IEnumerator ScrollCredits()
    {
        float creditsPivotX = creditsScrollArea.pivot.x;
        float creditsXPos = creditsScrollArea.position.x;

        // reset position, in case player quit-mid credits and then went back to them
        creditsScrollArea.pivot = new Vector2(creditsPivotX, 1);
        creditsScrollArea.position = new Vector2(creditsXPos, 0);

        // hard coded delay so the credits are tasteful
        yield return new WaitForSecondsRealtime(0.5f);

        // wait an extra hard-coded second to account for the fade to black.
        if (SceneLoadManager.Instance.PlayCreditsQueued)
            yield return new WaitForSecondsRealtime(1);

        float timeElapsed = 0;
        float t;
        while (timeElapsed < SecondsForFullCreditsScroll)
        {
            float speedMultiplier = speedUpCreditsAction.IsPressed() ? CreditsSpeedMultiplierIfButtonHeld : 1;

            timeElapsed += Time.unscaledDeltaTime * speedMultiplier; 
            t = timeElapsed / SecondsForFullCreditsScroll; // 0-1

            // move the anchor point to scroll the credits, keep the y position the same. 
            // this guarantees that the credits will always play the whole thing through. even if 
            creditsScrollArea.pivot = new Vector2(creditsPivotX, 1 - t);
            creditsScrollArea.position = new Vector2(creditsXPos, creditsStartY);

            yield return null;
        }

        AchievementManager.Instance.UnlockAchievement(AchievementManager.eAchievements.Credits);
    }


    void OnCreditsBackButtonClicked()
    {
        Debug.Log("close credits");

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);
        creditsOpen = false;

        StaticUtilities.DisableCanvasGroup(creditsPage);
        EventSystem.current.SetSelectedGameObject(creditsButton.gameObject);

        SceneLoadManager.Instance.PlayCreditsQueued = false;
    }

    #endregion

    #region Utility

    /// i took this code from the goog tbh
    bool IsOffScreen(Button button)
    {
        Vector3[] corners = new Vector3[4];
        button.GetComponent<RectTransform>().GetWorldCorners(corners);

        foreach (Vector3 corner in corners)
        {
            // Screen.width and Screen.height define the visible area
            if (corner.x < 0 || corner.x > Screen.width ||
                corner.y < 0 || corner.y > Screen.height)
            {
                return true; // At least one corner is outside
            }
        }
        return false;
    }

    #endregion
}
