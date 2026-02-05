/*
 * Contributors: Toby
 * Creation Date: 10/20/25
 * Last Modified: 11/5/25
 * 
 * Brief Description: Resusable & modular UI popup for confirming the users choice.
 */

using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ConfirmationPopup : MonoBehaviour
{
    [SerializeField, Required] private TMP_Text confirmationText;
    [SerializeField, Required] private Button cancelButton;
    [SerializeField, Required] private Button confirmButton; 
    [SerializeField] private bool hideOnCreation = true; 

    private CanvasGroup canvasGroup;
    private float oldTimeScale=1;
    private float lastFadeSecondsUsed = -1;
    private UnityAction lastPauseStartedOverride;
    private Coroutine fadeOpacityCoroutine;

    private UnityAction afterCancelClicked = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cancelButton.onClick.AddListener(OnCancelButtonPressed);
        canvasGroup = GetComponent<CanvasGroup>();
        if(hideOnCreation )
            StaticUtilities.DisableCanvasGroup(canvasGroup);
    }



    /// <summary>
    /// Opens confirmation window, can add custom behaviour to the respective buttons
    /// </summary>
    /// <param name="text">Text prompt that displays at text box (not confirmation button)</param>
    /// <param name="fadeSeconds">If greater than 0, fades in and out</param>
    public void OpenConfirmationPopup(string? text = null, UnityAction? OnConfirmationButtonClicked = null, UnityAction? OnCancelButtonClicked = null, float fadeSeconds = -1)
    {
        if(canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        lastFadeSecondsUsed = fadeSeconds;

        // Press esc to close popup
        InputEvents.PauseStartedOverride = HideConfirmationPopup;

        StaticUtilities.ShowCursor();
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        if(text != null && confirmationText != null)
        {
            confirmationText.text = text;
        }

        // Cancel Button
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(OnCancelButtonPressed);
        //if (OnCancelButtonClicked != null)
        //cancelButton.onClick.AddListener(OnCancelButtonClicked);
        afterCancelClicked = OnCancelButtonClicked;
            
            

        // Confirm button
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnConfirmButtonClicked); // may be redundant to remove this listener and then immediately add it back but idk else to do it.
        if(OnConfirmationButtonClicked != null)
            confirmButton.onClick.AddListener(OnConfirmationButtonClicked);

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        StaticUtilities.EnableCanvasGroup(canvasGroup, alpha: 0);

        if (lastFadeSecondsUsed > 0)
            fadeOpacityCoroutine = StaticUtilities.FadeToVisible(canvasGroup, fadeSeconds);
    }

    public void HideConfirmationPopup()
    {
        InputEvents.PauseStartedOverride = lastPauseStartedOverride;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (lastFadeSecondsUsed > 0)
            fadeOpacityCoroutine = StaticUtilities.FadeToHidden(canvasGroup, lastFadeSecondsUsed, 
                currentCoroutineToCancel: fadeOpacityCoroutine, afterFadeCallback: AfterFadeToHidden);

        else
        {
            StaticUtilities.DisableCanvasGroup(canvasGroup);
            if(afterCancelClicked != null)
                afterCancelClicked();
        }
    }

    private void AfterFadeToHidden()
    {
        StaticUtilities.DisableCanvasGroup(canvasGroup);

        if (afterCancelClicked != null)
            afterCancelClicked();
    }

    void OnCancelButtonPressed()
    {
        Time.timeScale = oldTimeScale;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        HideConfirmationPopup();
    }

    void OnConfirmButtonClicked()
    {
        Time.timeScale = oldTimeScale;
        StaticUtilities.DisableCanvasGroup(canvasGroup);
    }
}
