/*
 * Contributors: Toby, Josh
 * Creation Date: 10/20/2025
 * Last Modified: 3/1/2026
 * 
 * Brief Description: Handles UI elements for the pause menu.
 * Also listens to escape key input to open and close it.
 */

using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField, Scene] private int MainMenuScene;
    [SerializeField, Scene] private int HubScene;

    [Header("Misc Components")]
    [SerializeField, Required] private RectTransform pauseScreenParent;
    [SerializeField, Required] public CanvasGroup pauseGroup;

    [Header("Tabs")]
    [SerializeField, Required] public SettingsTab settingsTab;
    [SerializeField, Required] public ControlsTab controlsTab;
    [SerializeField, Required] public GeneralTab generalTab;

    [Header("Buttons")]
    [SerializeField, Required] private Button continueGameButton; 
    [SerializeField, Required] private Button quitToHubButton; 
    [SerializeField, Required] private Button quitToMainMenuButton; 

    [Header("Exit Confirmations")]
    [SerializeField, Required] public ConfirmationPopup confirmationPopup;
    [SerializeField] private string exitToHubText = "Are you sure you want to exit to the hub?\nYou will lose all progress in the current level";
    [SerializeField] private string exitToMainMenuText = "Are you sure you want to exit to the main menu?\nYou will lose all progress in the current level";

    [Header("Debug Buttons")]
    [SerializeField, Required] private Button restartLevelButton;
    [SerializeField, Required] private Button resetSaveButton;

    [Foldout("Advanced Settings"), SerializeField] private float wavyTextLetterSpacing = 8;

    private static float timeOfLastPause;
    private InputAction menuBackAction;
    private Color defaultNormalTabTextColor;

    private void OnEnable()
    {
        TrySubscribeToUICancel();
    }

    private void OnDisable()
    {
        if (menuBackAction == null)
            return;

        menuBackAction.started -= OnMenuBackPressed;
        menuBackAction = null;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultNormalTabTextColor = settingsTab.toggleButton.colors.normalColor;

        // disable going to hub if you are at the hub
        if (SceneManager.GetActiveScene().buildIndex == HubScene)
        {
            quitToHubButton.interactable = false;
            quitToHubButton.gameObject.SetActive(false);
        }

        pauseScreenParent.gameObject.SetActive(true);

        InputEvents.PauseStarted.AddListener(OnPauseKeyPressed);

        // tab buttons
        generalTab .toggleButton.onValueChanged.AddListener((isOn) => { if (isOn) OnOpenGeneralButtonSelected(); });
        controlsTab.toggleButton.onValueChanged.AddListener((isOn) => { if (isOn) OnOpenControlsButtonSelected(); });
        settingsTab.toggleButton.onValueChanged.AddListener((isOn) => { if (isOn) OnOpenSettingsButtonSelected(); });

        continueGameButton.onClick.AddListener(OnContinueGameButtonClicked);
        quitToHubButton.onClick.AddListener(OnGoToHubButtonClicked);
        quitToMainMenuButton.onClick.AddListener(OnGoToMainMenuButtonClicked);

        resetSaveButton.onClick.AddListener(ResetSaveDataButtonClicked);
        restartLevelButton.onClick.AddListener(RestartLevelButtonClicked);

        ClosePauseMenu();
    }

    public void OpenPauseMenu()
    {
        confirmationPopup.HideConfirmationPopup();

        Debug.Log("Pause Menu Opened");
        GameManager.Instance.PauseGame();

        // general tab is default tab


        pauseScreenParent.gameObject.SetActive(true);
        StaticUtilities.EnableCanvasGroup(pauseGroup);
        StaticUtilities.ShowCursor();

        settingsTab.CloseTab();
        controlsTab.CloseTab();
        OnOpenGeneralButtonSelected();
        EventSystem.current.SetSelectedGameObject(generalTab.toggleButton.gameObject);
    }

    public void ClosePauseMenu()
    {
        confirmationPopup.HideConfirmationPopup();
        Debug.Log("Pause Menu closed");
        GameManager.Instance.UnpauseGame();
        pauseScreenParent.gameObject.SetActive(false);
        StaticUtilities.HideCursor();
    }

    /// <summary>
    /// When escape key is pressed
    /// </summary>
    void OnPauseKeyPressed()
    {
        // Bandaid solution to a bad problem
        if (Time.unscaledTime - timeOfLastPause < 0.1f)
            return;

        timeOfLastPause = Time.unscaledTime;

        GameManager.Instance.TogglePause();
        Debug.Log("Pause pressed");

        if (GameManager.Instance.IsPaused)
            OpenPauseMenu();
        else
            ClosePauseMenu() ;
    }

    private void OnMenuBackPressed(InputAction.CallbackContext ctx)
    {
        if (!IsPauseMenuOpen() || Time.unscaledTime - timeOfLastPause < 0.1f)
            return;

        if (IsConfirmationPopupOpen())
        {
            confirmationPopup.HideConfirmationPopup();
            SelectDefaultForCurrentTab();
            return;
        }

        bool settingsOpen = settingsTab.canvasGroup != null && settingsTab.canvasGroup.alpha > 0.001f;
        bool controlsOpen = controlsTab.canvasGroup != null && controlsTab.canvasGroup.alpha > 0.001f;
        if (settingsOpen || controlsOpen)
        {
            generalTab.OpenTab();
            EventSystem.current.SetSelectedGameObject(generalTab.toggleButton.gameObject);
            return;
        }

        // Root/general pause page: back closes pause menu.
        ClosePauseMenu();
    }

    private void TrySubscribeToUICancel()
    {
        if (menuBackAction != null)
            return;

        InputSystemUIInputModule uiInputModule = EventSystem.current != null
            ? EventSystem.current.currentInputModule as InputSystemUIInputModule
            : null;

        if (uiInputModule == null)
            uiInputModule = Object.FindFirstObjectByType<InputSystemUIInputModule>();

        if (uiInputModule == null || uiInputModule.cancel == null || uiInputModule.cancel.action == null)
            return;

        menuBackAction = uiInputModule.cancel.action;
        menuBackAction.started += OnMenuBackPressed;
    }

    private bool IsPauseMenuOpen()
    {
        return pauseScreenParent != null
            && pauseScreenParent.gameObject.activeInHierarchy
            && pauseGroup != null
            && pauseGroup.interactable;
    }

    private bool IsConfirmationPopupOpen()
    {
        if (confirmationPopup == null)
            return false;

        CanvasGroup cg = confirmationPopup.GetComponent<CanvasGroup>();
        return cg != null && cg.interactable && cg.alpha > 0.001f;
    }

    private void SelectDefaultForCurrentTab()
    {
        if (settingsTab.canvasGroup != null && settingsTab.canvasGroup.alpha > 0.001f)
            EventSystem.current.SetSelectedGameObject(settingsTab.toggleButton.gameObject);
        else if (controlsTab.canvasGroup != null && controlsTab.canvasGroup.alpha > 0.001f)
            EventSystem.current.SetSelectedGameObject(controlsTab.toggleButton.gameObject);
        else
            EventSystem.current.SetSelectedGameObject(generalTab.toggleButton.gameObject);
    }

    #region Tab Navigation Buttons

    void OnOpenGeneralButtonSelected()
    {
        generalTab.OpenTab();

        DisableAllWavyTexts();
        generalTab.wavyTextAnimation.PlayAnimation = true;
        generalTab.toggleButton.SetColors(normalColor:  Color.white);   
    }

    void OnOpenControlsButtonSelected()
    {
        controlsTab.OpenTab();

        DisableAllWavyTexts();
        controlsTab.wavyTextAnimation.PlayAnimation = true;
        controlsTab.toggleButton.SetColors(normalColor: Color.white);
    }

    void OnOpenSettingsButtonSelected()
    {
        settingsTab.OpenTab();

        DisableAllWavyTexts();
        settingsTab.wavyTextAnimation.PlayAnimation = true;
        settingsTab.toggleButton.SetColors(normalColor: Color.white);
    }

    void DisableAllWavyTexts()
    {
        generalTab.wavyTextAnimation.PlayAnimation = false;
        generalTab.toggleButton.SetColors(normalColor: defaultNormalTabTextColor);
        //generalTab.wavyTextAnimation.textBox.characterSpacing = 0;

        controlsTab.wavyTextAnimation.PlayAnimation = false;
        controlsTab.toggleButton.SetColors(normalColor: defaultNormalTabTextColor);
        //controlsTab.wavyTextAnimation.textBox.characterSpacing = 0;

        settingsTab.wavyTextAnimation.PlayAnimation = false;
        settingsTab.toggleButton.SetColors(normalColor: defaultNormalTabTextColor);
        //settingsTab.wavyTextAnimation.textBox.characterSpacing = 0;
    }

    #endregion

    #region Main Buttons
    void OnContinueGameButtonClicked()
    {
        ClosePauseMenu() ;
    }

    void OnGoToHubButtonClicked()
    {
        confirmationPopup.OpenConfirmationPopup(exitToHubText, OnConfirmQuitToHubButtonClicked);
    }
    void OnGoToMainMenuButtonClicked()
    {
        confirmationPopup.OpenConfirmationPopup(exitToMainMenuText, OnConfirmQuitToMainMenuButtonClicked);
    }
    #endregion

    #region Confirmation Menus

    void OnConfirmQuitToHubButtonClicked()
    {
        ClosePauseMenu();
        Time.timeScale = 1;
        LevelManager.Instance.InstantiateFadeToBlack(() => SceneManager.LoadScene(HubScene));
    }

    void OnConfirmQuitToMainMenuButtonClicked()
    {
        Time.timeScale = 1;
        DialogueUIManager.Instance.StopVoiceLine();
        LevelManager.Instance.InstantiateFadeToBlack(() => SceneManager.LoadScene(MainMenuScene));
    }

    #endregion

    #region Debug UI Buttons
    void RestartLevelButtonClicked()
    {
        LevelManager.Instance.InstantiateFadeToBlack(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    void ResetSaveDataButtonClicked()
    {
        SaveDataManager.Instance.ClearSaveFile();
        Debug.Log("Save data cleared");
    }

    #endregion

}
