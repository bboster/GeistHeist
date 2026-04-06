/*
 * Contributors: Toby Schamberger, Joshua Kelly
 * Creation: 10/20/25
 * Last Edited: 4/7/2026
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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
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
    [SerializeField, Required] private GameObject loadingScreenPrefab;

    [Header("Fog")]
    [SerializeField, Required] private RenderTexture leftFogRenderTexture;
    [SerializeField, Required] private RenderTexture rightFogRenderTexture;

    [Header("Main Page")]
    [SerializeField, Required] private Button newGameButton;
    [SerializeField, Required] private Button continueGameButton;
    [SerializeField, Required] private Button settingsButton;
    [SerializeField, Required] private Button creditsButton;
    [SerializeField] private Button howToPlayButton;
    [SerializeField, Required] private Button quitGameButton;

    [Header("Credits Page")]
    [SerializeField, Required] private CanvasGroup creditsPage;
    [SerializeField, Required] private Button closeCreditsButton;

    [Header("How to Play Page")]
    [SerializeField, Required] private CanvasGroup howToPlayPage;
    [SerializeField, Required] private Button closeHowToPlayButton;

    [Header("Settings Page")]
    [SerializeField, Required] private SettingsTab settingsTab;
    [SerializeField, Required] private CanvasGroup settingsPage;
    [SerializeField, Required] private Button closeSettingsButton;
    private bool settingsOpen = false;
    private bool creditsOpen = false;

    // if the player has played before and got past the first level
    private bool playerHasSignificantSaveData;
    private InputAction menuCancelAction;
    private Animator mainMenuAnimator;
    private float TimeOfLastAnyButtonPressed = 0;
    private Coroutine waitToDelayCoroutine;
    private bool introAnimationFinished = false;
    private bool menuActive = false;
    private bool gamepadActive = false;

    private void OnEnable()
    {
        TrySubscribeToUICancel();
    }

    private void Start()
    {
        mainMenuAnimator = GetComponent<Animator>();
        mainMenuAnimator.SetBool("Active", false);

        TrySubscribeToUICancel();

        playerHasSignificantSaveData =
               SaveDataManager.Instance.DoesSaveDataExist()
            && SaveDataManager.Instance.GetLevelsCompletedCount() > 0;

        // hide/show continue button based on if save data exists
        continueGameButton.gameObject.SetActive(playerHasSignificantSaveData);
        EventSystem.current.SetSelectedGameObject(pressAnyButtonButton.gameObject);

        // Hide other pages
        // The only reason im setting them active in code instead of having them active in scene is that i do not trust game designers
        creditsPage.gameObject.SetActive(true);
        StaticUtilities.DisableCanvasGroup(creditsPage);
        howToPlayPage.gameObject.SetActive(true);
        StaticUtilities.DisableCanvasGroup(howToPlayPage);
        settingsPage.gameObject.SetActive(true);
        StaticUtilities.DisableCanvasGroup(settingsPage);
        settingsOpen = false;
        creditsOpen = false;

        InputEvents.Instance.OnControllerChanged.AddListener(OnControllerChanged);
        OnControllerChanged();

        InputEvents.PauseStarted.AddListener(OnSettingsBackButtonClicked);
        InputEvents.ActionStarted.AddListener(OnSettingsBackButtonClicked); // because its B on controller

        InputSystem.onEvent += OnAnyButtonPressed;
        pressAnyButtonButton.onClick.AddListener(() => OnAnyButtonPressed(null, null));

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
        if (loadingScreenPrefab == null)
        {
            Debug.LogError("No transition card set on " + gameObject.name);
            LevelManager.Instance.ChangeScene(sceneToLoad);
            return;
        }
        var levelTransition = Instantiate(loadingScreenPrefab).GetComponent<LevelTransitionScreen>();
        levelTransition.StartTransition(sceneToLoad, loadingCardPrefab);
    }


    #region Buttons OnClicked

    #region Main Page

    void OnNewGameButtonClicked()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);

        if (!playerHasSignificantSaveData || !confirmationForNewGame)
        {
            LoadNewGame();
            return;
        }

        // if player has save data: open confirmation popup
        StaticUtilities.DisableCanvasGroup(howToPlayPage);
        StaticUtilities.DisableCanvasGroup(creditsPage);

        confirmationPopup.OpenConfirmationPopup(confirmNewGameText, OnConfirmDeleteSaveButtonClicked);
    }

    void OnContinueButtonClicked()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);

        //SceneManager.LoadScene(HubScene);
        LoadScene(HubScene, NewSceneLoadingCardPrefab);
    }

    void OnSettingsButtonClicked()
    {
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

        if (InputEvents.Instance.IsGamepadActive())
        {
            EventSystem.current.SetSelectedGameObject(closeCreditsButton.gameObject);
        }
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
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion

    #region Confirm Delete Save

    void OnConfirmDeleteSaveButtonClicked()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);

        LoadNewGame();
    }

    #endregion

    #region Credits

    void OnCreditsBackButtonClicked()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);
        creditsOpen = false;

        StaticUtilities.DisableCanvasGroup(creditsPage);
        EventSystem.current.SetSelectedGameObject(creditsButton.gameObject);
    }

    #endregion

    #region Settings

    void OnSettingsBackButtonClicked()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIClick);

        StaticUtilities.DisableCanvasGroup(settingsPage);
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

    void OnAnyButtonPressed(UnityEngine.InputSystem.LowLevel.InputEventPtr eventPtr, InputDevice device)
    {
        // ignore input for a tiny bit
        if (introAnimationFinished == false) return;
        // it counts moving your mouse as an input (sob)
        if (device != null && device.ToString().Contains("Mouse")) return;

        if ( !menuActive && device != null && device is Gamepad) return;

        Debug.Log("Any Button pressed");

        TimeOfLastAnyButtonPressed = Time.unscaledTime;

        StaticUtilities.StartCoroutineIfNotPlaying(ref waitToDelayCoroutine, CheckActiveState());
    }


    IEnumerator CheckActiveState()
    {
        bool wasActive = false;
        while (true)
        {
            if (settingsOpen || creditsOpen)
                TimeOfLastAnyButtonPressed = Time.unscaledTime;

            //Debug.Log(Time.unscaledTime - TimeOfLastAnyButtonPressed);
            menuActive = (Time.unscaledTime - TimeOfLastAnyButtonPressed < secondsOfInactivityForIdle);
            mainMenuAnimator.SetBool("Active", menuActive);

            if (!menuActive && wasActive)
            {
                EventSystem.current?.SetSelectedGameObject(pressAnyButtonButton.gameObject);
            }
            if (menuActive && !wasActive)
            {
                if (continueGameButton.gameObject.activeSelf) EventSystem.current?.SetSelectedGameObject(continueGameButton.gameObject);
                else EventSystem.current?.SetSelectedGameObject(newGameButton.gameObject);
            }

            wasActive = menuActive;

            yield return null;
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

        if (creditsPage != null && creditsPage.interactable && creditsPage.alpha > 0.001f)
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
        if (! gamepadActive)
            return;

        gamepadActive = false;

        Debug.Log("Mouse input activated");

        EventSystem.current.SetSelectedGameObject(null);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnGamepadInputActivated()
    {
        // bad solution, im so tired
        OnAnyButtonPressed(null, null);

        if (!menuActive)
        {
            EventSystem.current?.SetSelectedGameObject(pressAnyButtonButton.gameObject);
        }

        // dont do it twice
        if (gamepadActive) 
            return;

        Debug.Log("Gamepad input activated");
        gamepadActive = true;

        if (menuActive == true)
        {
            if (continueGameButton.gameObject.activeSelf) EventSystem.current?.SetSelectedGameObject(continueGameButton.gameObject);
            else EventSystem.current?.SetSelectedGameObject(newGameButton.gameObject);
        }

        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    #endregion

    #region Resolution
    Vector2 lastResolution;

    void Update()
    {
        if (Screen.width != lastResolution.x || Screen.height != lastResolution.y)
        {
            lastResolution = new Vector2(Screen.width, Screen.height);
            OnResolutionChanged();
        }
    }

    void OnResolutionChanged()
    {
        Debug.Log($"new resolution: {Screen.width} x {Screen.height}");
        leftFogRenderTexture.width  = Screen.width;
        leftFogRenderTexture.height = Screen.height;
        rightFogRenderTexture.width = Screen.width;
        rightFogRenderTexture.height = Screen.height;
    }
    #endregion
}
