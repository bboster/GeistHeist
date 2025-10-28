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

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField][Scene] private string sceneName;

    // TODO: can this be moved to some cached/shared confirmation canvas? so we dont have to respawn it everytime.
    [SerializeField, Required] private GameObject loadingCardPrefab;

    [Header("Popup text")]
    [SerializeField] private string confirmationText = "Go to _____?";
    [SerializeField, Required] private GameObject confirmationPopupPrefab;

    public void Interact()
    {
        // if i didnt have to spawn this in, that would be cool
        var popupCanvas = Instantiate(confirmationPopupPrefab);
        
        ConfirmationPopup popup = popupCanvas.GetComponentInChildren<ConfirmationPopup>();

        popup.OpenConfirmationPopup(text: confirmationText, OnCancelButtonClicked : () => OnCancelPressed(popupCanvas), OnConfirmationButtonClicked: () => OnConfrimPressed(popupCanvas));
    }

    void OnCancelPressed(GameObject confirmationPopup)
    {
        Destroy(confirmationPopup);
    }

    void OnConfrimPressed(GameObject confirmationPopup)
    {

        if (loadingCardPrefab == null)
        {
            Debug.LogError("No transition card set on " + gameObject.name);
            GameManager.Instance.NextLevel(sceneName);
            return;
        }
        Instantiate(loadingCardPrefab);

    }
}
