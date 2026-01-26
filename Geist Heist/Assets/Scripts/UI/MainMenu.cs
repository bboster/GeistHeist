/*
 * Contributors: Toby Schamberger
 * Creation: 10/20/25
 * Last Edited: 11/5/25
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
    [SerializeField, BoxGroup("Hub Scene"), Scene] private string HubScene;
    [SerializeField, BoxGroup("Hub Scene")] private GameObject HubSceneLoadingCardPrefab;

    [SerializeField, BoxGroup("New Game Scene"), Scene] private string NewGameScene; // making it separate because i imagine we will have a tutorial level or a cutscene or something play on a new save.
    [SerializeField, BoxGroup("New Game Scene")] private GameObject NewSceneLoadingCardPrefab;
    [SerializeField, BoxGroup("New Game Scene")] string confirmNewGameText = "Are you sure? Continuing will delete your progress.";

    [Header("Settings")]
    [SerializeField, Required] private ConfirmationPopup confirmationPopup;
    [SerializeField, Required] private GameObject loadingScreenPrefab;

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
        LoadScene(NewGameScene, NewSceneLoadingCardPrefab);
    }

    void LoadScene(string sceneToLoad, GameObject loadingCardPrefab)
    {
        if (loadingScreenPrefab == null)
        {
            Debug.LogError("No transition card set on " + gameObject.name);
            GameManager.Instance.NextLevel(sceneToLoad);
            return;
        }
        var levelTransition = Instantiate(loadingScreenPrefab).GetComponent<LevelTransitionScreen>();
        levelTransition.StartTransition(sceneToLoad, loadingCardPrefab);
    }


#region Buttons OnClicked

    # region Main Page
    void OnNewGameButtonClicked()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.UIClick);

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
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.UIClick);

        //SceneManager.LoadScene(HubScene);
        LoadScene(HubScene, NewSceneLoadingCardPrefab);
    }

    void OnCreditsButtonClicked()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.UIClick);

        StaticUtilities.DisableCanvasGroup(howToPlayPage);
        StaticUtilities.EnableCanvasGroup(creditsPage);
    }

    void OnHowToPlayButtonClicked()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.UIClick);

        StaticUtilities.DisableCanvasGroup(creditsPage);
        StaticUtilities.EnableCanvasGroup(howToPlayPage);
    }

    void OnQuitButtonClicked()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.UIClick);

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
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.UIClick);

        LoadNewGame();
    }

    #endregion

    #region Credits

    void OnCreditsBackButtonClicked()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.UIClick);

        StaticUtilities.DisableCanvasGroup(creditsPage);
    }

    #endregion

    #region Credits

    void OnCloseHowToPlayButtonClicked()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.instance.UIClick);

        StaticUtilities.DisableCanvasGroup(howToPlayPage);
    }

    #endregion

    #endregion


    #region Level transition

    

    #endregion
}
