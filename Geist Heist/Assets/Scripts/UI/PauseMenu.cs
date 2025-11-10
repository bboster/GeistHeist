/*
 * Contributors: Toby
 * Creation Date: 10/20/2025
 * Last Modified: 10/20/2025
 * 
 * Brief Description: Handles UI elements for the pause menu.
 * Also listens to escape key input to open and close it.
 */

using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField, Scene] private int MainMenuScene;
    [SerializeField, Scene] private int HubScene;

    [Header("Misc Components")]
    [SerializeField, Required] private RectTransform pauseScreenParent;

    [Header("Buttons")]
    [SerializeField, Required] private Button continueGameButton; 
    [SerializeField, Required] private Button quitToHubButton; 
    [SerializeField, Required] private Button quitToMainMenuButton;

    [Header("Exit Confirmations")]
    [SerializeField, Required] private ConfirmationPopup confirmationPopup;
    [SerializeField] private string exitToHubText = "Are you sure you want to exit to the hub?\nYou will lose all progress in the current level";
    [SerializeField] private string exitToMainMenuText = "Are you sure you want to exit to the main menu?\nYou will lose all progress in the current level";

    [Header("Debug Buttons")]
    [SerializeField, Required] private Button restartLevelButton;
    [SerializeField, Required] private Button resetSaveButton;

    private static float timeOfLastPause;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // disable going to hub if you are at the hub
        if(SceneManager.GetActiveScene().buildIndex == HubScene)
        {
            quitToHubButton.interactable = false;
        }

        pauseScreenParent.gameObject.SetActive(true);

        InputEvents.PauseStarted.AddListener(OnPauseKeyPressed);

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
        GameManager.Instance.IsPaused = true;
        pauseScreenParent.gameObject.SetActive(true);
        Time.timeScale = 0f;
        StaticUtilities.ShowCursor();
        AudioManager.instance.PauseSFX();
    }

    public void ClosePauseMenu()
    {
        confirmationPopup.HideConfirmationPopup();
        Debug.Log("Pause Menu closed");
        GameManager.Instance.IsPaused = false;
        pauseScreenParent.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
        StaticUtilities.HideCursor();
        AudioManager.instance.UnpauseSFX();
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

        GameManager.Instance.IsPaused = !GameManager.Instance.IsPaused;
        Debug.Log("Pause pressed");

        if (GameManager.Instance.IsPaused)
            OpenPauseMenu();
        else
            ClosePauseMenu() ;
    }

#region UI Buttons

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
        SceneManager.LoadScene(HubScene);
    }

    void OnConfirmQuitToMainMenuButtonClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(MainMenuScene);
    }

    #endregion

    #region Debug UI Buttons
    void RestartLevelButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ResetSaveDataButtonClicked()
    {
        SaveDataManager.Instance.ClearSaveFile();
        Debug.Log("Save data cleared");
    }

    #endregion

    #endregion

}
