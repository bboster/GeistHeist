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
    [SerializeField] private int HubScene;
    [SerializeField] private int NewGameScene; // making it seperate because i imagine we will have a tutorial level or a cutscene or something play on a new save.

    [Header("Main Page")]
    [SerializeField, Required] private Button newGameButton;
    [SerializeField, Required] private Button continueGameButton;
    [SerializeField, Required] private Button creditsButton;
    [SerializeField, Required] private Button quitGameButton;

    [Header("Confirm New Save")]
    [SerializeField, Required] private CanvasGroup confirmDeleteSavePanel;
    [SerializeField, Required] private Button confirmDeleteSaveButton;
    [SerializeField, Required] private Button cancelDeleteSaveButton;

    [Header("Credits Page")]
    [SerializeField, Required] private CanvasGroup creditsPage;
    [SerializeField, Required] private Button closeCreditsButton;

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
        confirmDeleteSaveButton.gameObject.SetActive(true);
        StaticUtilities.DisableCanvasGroup(confirmDeleteSavePanel);
        creditsPage.gameObject.SetActive(true);
        StaticUtilities.DisableCanvasGroup(creditsPage);

        newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        continueGameButton.onClick.AddListener(OnContinueButtonClicked);
        creditsButton.onClick.AddListener(OnCreditsButtonClicked);
        quitGameButton.onClick.AddListener(OnQuitButtonClicked);

        confirmDeleteSaveButton.onClick.AddListener(OnConfirmDeleteSaveButtonClicked);
        cancelDeleteSaveButton.onClick.AddListener(OnCancelDeleteSaveButtonClicked);

        closeCreditsButton.onClick.AddListener(OnCreditsBackButtonClicked);
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
        StaticUtilities.EnableCanvasGroup(confirmDeleteSavePanel);
    }

    void OnContinueButtonClicked()
    {
        SceneManager.LoadScene(HubScene);
    }

    void OnCreditsButtonClicked()
    {
        StaticUtilities.EnableCanvasGroup(creditsPage);
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

    void OnCancelDeleteSaveButtonClicked()
    {
        StaticUtilities.DisableCanvasGroup(confirmDeleteSavePanel);
    }

    #endregion

    #region Credits

    void OnCreditsBackButtonClicked()
    {
        StaticUtilities.DisableCanvasGroup(creditsPage);
    }

    #endregion

    #endregion

}
