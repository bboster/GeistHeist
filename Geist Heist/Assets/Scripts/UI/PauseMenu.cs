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

    [Header("Quit To Hub Confirmation")]
    [SerializeField, Required] private CanvasGroup confirmQuitToHubPanel;
    [SerializeField, Required] private Button confirmQuitToHubButton;
    [SerializeField, Required] private Button cancelQuitToHubButton;

    [Header("Quit To Main Menu Confirmation")]
    [SerializeField, Required] private CanvasGroup confirmQuitToMainMenuPanel;
    [SerializeField, Required] private Button confirmQuitToMainMenuButton;
    [SerializeField, Required] private Button cancelQuitToMainMenuButton;

    [Header("Debug Buttons")]
    [SerializeField, Required] private Button restartLevelButton;
    [SerializeField, Required] private Button resetSaveButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // disable going to hub if you are at the hub
        if(SceneManager.GetActiveScene().buildIndex == HubScene)
        {
            quitToHubButton.interactable = false;
        }

        // Hide different panels / screens
        confirmQuitToHubPanel.gameObject.SetActive(true);
        StaticUtilities.DisableCanvasGroup(confirmQuitToHubPanel);
        confirmQuitToMainMenuPanel.gameObject.SetActive(true);
        StaticUtilities.DisableCanvasGroup(confirmQuitToMainMenuPanel);

        InputEvents.PauseStarted.AddListener(OnPauseKeyPressed);

        continueGameButton.onClick.AddListener(OnContinueGameButtonClicked);
        quitToHubButton.onClick.AddListener(OnGoToHubButtonClicked);
        quitToMainMenuButton.onClick.AddListener(OnGoToMainMenuButtonClicked);

        confirmQuitToHubButton.onClick.AddListener(OnConfirmQuitToHubButtonClicked) ;
        cancelQuitToHubButton.onClick.AddListener(OnCancelQuitToHubButtonClicked) ;

        confirmQuitToMainMenuButton.onClick.AddListener(OnConfirmQuitToMainMenuButtonClicked);
        cancelQuitToMainMenuButton.onClick.AddListener(OnCancelQuitToMainMenuButtonClicked);

        ClosePauseMenu();
    }

    public void OpenPauseMenu()
    {
        GameManager.Instance.IsPaused = true;
        pauseScreenParent.gameObject.SetActive(true);
        Time.timeScale = 0f;
        StaticUtilities.ShowCursor();
    }

    public void ClosePauseMenu()
    {
        GameManager.Instance.IsPaused = false;
        pauseScreenParent.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
        StaticUtilities.HideCursor();
    }

    /// <summary>
    /// When escape key is pressed
    /// </summary>
    void OnPauseKeyPressed()
    {
        GameManager.Instance.IsPaused = !GameManager.Instance.IsPaused;
        pauseScreenParent.gameObject.SetActive(GameManager.Instance.IsPaused);
    }

#region UI Buttons

    #region Main Buttons
    void OnContinueGameButtonClicked()
    {
        ClosePauseMenu() ;
    }
    void OnGoToHubButtonClicked()
    {
        StaticUtilities.DisableCanvasGroup(confirmQuitToMainMenuPanel);

        StaticUtilities.EnableCanvasGroup(confirmQuitToHubPanel);
    }
    void OnGoToMainMenuButtonClicked()
    {
        StaticUtilities.DisableCanvasGroup(confirmQuitToHubPanel);

        StaticUtilities.EnableCanvasGroup(confirmQuitToMainMenuPanel);
    }
    #endregion

    #region Quit To Hub

    void OnConfirmQuitToHubButtonClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(HubScene);
    }

    void OnCancelQuitToHubButtonClicked()
    {
        StaticUtilities.DisableCanvasGroup(confirmQuitToHubPanel);
    }

    #endregion

    #region Quit To Main Menu

    void OnConfirmQuitToMainMenuButtonClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(MainMenuScene);
    }

    void OnCancelQuitToMainMenuButtonClicked()
    {
        StaticUtilities.DisableCanvasGroup(confirmQuitToMainMenuPanel);
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
