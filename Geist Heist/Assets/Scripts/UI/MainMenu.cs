/*
 * Contributors: Toby Schamberger
 * Creation: 10/20/25
 * Last Edited: 10/20/25
 * Summary: Handles button functionality for main menu.
 * The player will be prompted to delete their save if they press new game after having save data.
 */

using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Scene] private int HubScene;
    [SerializeField, Scene] private int NewGameScene; // making it seperate because i imagine we will have a tutorial level or a cutscene or something play on a new save.
    [SerializeField, Required] private ConfirmationPopup confirmationPopup;
    [SerializeField] string confirmNewGameText = "Are you sure? Continuing will delete your progress.";

    [Header("Main Page")]
    [SerializeField, Required] private Button newGameButton;
    [SerializeField, Required] private Button continueGameButton;
    [SerializeField, Required] private Button creditsButton;
    [SerializeField, Required] private Button howToPlayButton;
    [SerializeField, Required] private Button quitGameButton;

    [Header("Credits Page")]
    [SerializeField, Required] private CanvasGroup creditsPage;
    [SerializeField, Required] private Button closeCreditsButton;

    [Header("How to Play Page")]
    [SerializeField, Required] private CanvasGroup howToPlayPage;
    [SerializeField, Required] private Button closeHowToPlayButton;

    // if the player has played before and got past the first level
    private bool playerHasSignificantSaveData;

    void Start()
    {
        playerHasSignificantSaveData = 
               SaveDataManager.Instance.DoesSaveDataExist() 
            && SaveDataManager.Instance.GetLevelsCompletedCount() > 0;

        // hide/show continue button based on if save data exists
        continueGameButton.gameObject.SetActive(playerHasSignificantSaveData == true);

        // Hide other pages
        // The only reason im setting them active in code instead of having them active in scene is that i do not trust game designers
        creditsPage.gameObject.SetActive(true);
        StaticUtilities.DisableCanvasGroup(creditsPage);
        howToPlayPage.gameObject.SetActive(true);
        StaticUtilities.DisableCanvasGroup(howToPlayPage);

        // Main Menu
        newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        continueGameButton.onClick.AddListener(OnContinueButtonClicked);
        creditsButton.onClick.AddListener(OnCreditsButtonClicked);
        howToPlayButton.onClick.AddListener(OnHowToPlayButtonClicked);
        quitGameButton.onClick.AddListener(OnQuitButtonClicked);

        // Credits
        closeCreditsButton.onClick.AddListener(OnCreditsBackButtonClicked);
        
        // How to Play
        closeHowToPlayButton.onClick.AddListener(OnCloseHowToPlayButtonClicked);

        // Confirmation Popup
        confirmationPopup.HideConfirmationPopup();
    }

    /// <summary>
    /// Clears players save data and starts the game
    /// </summary>
    void LoadNewGame()
    {
        SaveDataManager.Instance.ClearSaveFile();
        SceneManager.LoadScene(NewGameScene);
    }

#region Button OnClicked

    # region Main Page
    void OnNewGameButtonClicked()
    {
        if (!playerHasSignificantSaveData)
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
        SceneManager.LoadScene(HubScene);
    }

    void OnCreditsButtonClicked()
    {
        StaticUtilities.DisableCanvasGroup(howToPlayPage);
        StaticUtilities.EnableCanvasGroup(creditsPage);
    }

    void OnHowToPlayButtonClicked()
    {
        StaticUtilities.DisableCanvasGroup(creditsPage);
        StaticUtilities.EnableCanvasGroup(howToPlayPage);
    }

    void OnQuitButtonClicked()
    {
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
        LoadNewGame();
    }

    #endregion

    #region Credits

    void OnCreditsBackButtonClicked()
    {
        StaticUtilities.DisableCanvasGroup(creditsPage);
    }

    #endregion

    #region Credits

    void OnCloseHowToPlayButtonClicked()
    {
        StaticUtilities.DisableCanvasGroup(howToPlayPage);
    }

    #endregion

    #endregion

}
