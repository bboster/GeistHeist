/*
 * Contributors: Sky, Josh, Toby
 * Creation Date: 9/30/25
 * Last Modified: 10/27/25
 * 
 * Brief Description: Changes scene on button press
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using UnityEngine.Events;

public class SceneTransitionInteractable : MonoBehaviour, IInteractable
{
    [SerializeField][Scene] private string sceneName;

    [InfoBox("The loading screen is the parent that manages the loading card\n\nThe loading card should be level-specific, so have one for each level.")]
    // TODO: can this be moved to some cached/shared confirmation canvas? so we dont have to respawn it everytime.
    [SerializeField, Required] private GameObject loadingScreenPrefab;
    [SerializeField, Required("Make sure each level has its own level-specific title card")] 
    private GameObject levelLoadingCardPrefab;

    [Header("Popup text")]
    [SerializeField, ShowIf("hasConfirmationPopup")] private string confirmationText = "Go to _____?";
    [SerializeField, ShowIf("hasConfirmationPopup")] private GameObject confirmationPopupPrefab;

    [Foldout("Advanced"), SerializeField, ShowIf("hasConfirmationPopup")] private bool closeMenuOnConfirm = false;
    [Foldout("Advanced"), SerializeField] bool hasAchievement;
    [Foldout("Advanced"), SerializeField] AchievementManager.eAchievements WhatAcheivement;

    //private static bool anyLevelConfirmScreenOpen = false;
    [Tooltip ("Setting this to false means the transition will ONLY do a fade to black.")]
    [SerializeField] private bool hasConfirmationPopup = true;

    public void Interact()
    {
        if(ConfirmationPopup.AnyConfirmationMenuOpen == true)
        {
            Debug.Log("can't open new confirm screen, player is already in a confirmation menu");
            return;
        }

        if (hasAchievement)
        {
            AchievementManager.instance.UnlockAchievement(WhatAcheivement);
        }

        if (!hasConfirmationPopup)
        {
            Debug.Log("Going straight to fade");
            LevelManager.Instance.InstantiateFadeToBlack(() => LevelManager.Instance.ChangeScene(sceneName));
            return;
        }

        //anyLevelConfirmScreenOpen = true;

        // if i didnt have to spawn this in, that would be cool
        var popupCanvas = Instantiate(confirmationPopupPrefab);

        ConfirmationPopup popup = popupCanvas.GetComponentInChildren<ConfirmationPopup>();

        popup.OpenConfirmationPopup(text: confirmationText, fadeSeconds: 0.5f, 
            closeMenuOnConfirm: false, freezeTime: false,
            OnCancelButtonClicked : () => OnCancelPressed(popupCanvas), OnConfirmationButtonClicked: () => OnConfirmPressed(popupCanvas));

        GameManager.Instance.InGodMode = true;

        popup.GetComponentInParent<LevelConfirmationVisualizer>()?.Initialize(sceneName);
    }

    void OnCancelPressed(GameObject confirmationPopup)
    {
        //anyLevelConfirmScreenOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Destroy(confirmationPopup);
        GameManager.Instance.InGodMode = false;
    }

    void OnConfirmPressed(GameObject confirmationPopup)
    {
        Debug.Log("Confirm Pressed");
        SaveDataManager.Instance.MarkSceneAsCompleted(sceneName);
        LevelManager.Instance.ChangeScene(sceneName);
        return;

    }
}
