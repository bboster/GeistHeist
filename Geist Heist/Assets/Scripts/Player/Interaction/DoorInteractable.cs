/*
 * Contributors: Sky, Josh, Toby
 * Creation Date: 9/30/25
 * Last Modified: 10/27/25
 * 
 * Brief Description: Controls interactions for the door, changes scene on button press
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using UnityEngine.Events;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField][Scene] private string sceneName;

    [InfoBox("The loading screen is the parent that manages the loading card\n\nThe loading card should be level-specific, so have one for each level.")]
    // TODO: can this be moved to some cached/shared confirmation canvas? so we dont have to respawn it everytime.
    [SerializeField, Required] private GameObject loadingScreenPrefab;
    [SerializeField, Required("Make sure each level has its own level-specific title card")] 
    private GameObject levelLoadingCardPrefab;

    [Header("Popup text")]
    [SerializeField] private string confirmationText = "Go to _____?";
    [SerializeField, Required] private GameObject confirmationPopupPrefab;

    private static bool anyLevelConfirmScreenOpen = false;

    public void Interact()
    {
        if(anyLevelConfirmScreenOpen == true)
        {
            Debug.Log("can't open new confirm screen, player is already in a confirmation menu");
            return;
        }
        anyLevelConfirmScreenOpen = true;

        // if i didnt have to spawn this in, that would be cool
        var popupCanvas = Instantiate(confirmationPopupPrefab);
        
        ConfirmationPopup popup = popupCanvas.GetComponentInChildren<ConfirmationPopup>();

        popup.OpenConfirmationPopup(text: confirmationText, fadeSeconds: 0.25f,
            OnCancelButtonClicked : () => OnCancelPressed(popupCanvas), OnConfirmationButtonClicked: () => OnConfrimPressed(popupCanvas));

        popup.GetComponentInParent<LevelConfirmationVisualizer>().Initialize();
    }

    void OnCancelPressed(GameObject confirmationPopup)
    {
        anyLevelConfirmScreenOpen = false;
        Destroy(confirmationPopup);
    }

    void OnConfrimPressed(GameObject confirmationPopup)
    {
        anyLevelConfirmScreenOpen = false;
        if (loadingScreenPrefab == null)
        {
            Debug.LogError("No transition card set on " + gameObject.name);
            LevelManager.Instance.ChangeScene(sceneName);
            return;
        }
        var levelTransition = Instantiate(loadingScreenPrefab).GetComponent<LevelTransitionScreen>();
        levelTransition.StartTransition(sceneName, levelLoadingCardPrefab);

    }
}
