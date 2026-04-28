/*
 * Contributors: Toby, Josh
 * Creation Date: 10/20/25
 * Last Modified: 3/1/26
 * 
 * Brief Description: Resusable & modular UI popup for confirming the users choice.
 */

using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ConfirmationPopup : MonoBehaviour
{
    [SerializeField, Required] private TMP_Text confirmationText;
    [SerializeField, Required] public Button cancelButton;
    [SerializeField, Required] public Button confirmButton; 
    [SerializeField] private bool hideOnCreation = true; 

    protected CanvasGroup canvasGroup;
    protected float oldTimeScale=1;
    protected float lastFadeSecondsUsed = -1;
    private bool closeMenuOnConfirm;
    private UnityAction lastPauseStartedOverride;
    private Coroutine fadeOpacityCoroutine;

    protected UnityAction afterCancelClicked = null;
    protected UnityAction onConfirmationButtonClicked = null;
    private GameObject previouslySelectedBeforeOpen;
    private bool shouldRestorePreviousSelectionOnHide;

    private int frameOpened;

    public static bool AnyConfirmationMenuOpen = false;

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
    public virtual void OpenConfirmationPopup(string? text = null, UnityAction? OnConfirmationButtonClicked = null, UnityAction? OnCancelButtonClicked = null,
        float fadeSeconds = -1, bool closeMenuOnConfirm = true, bool freezeTime = true)
    {
        if(canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        AnyConfirmationMenuOpen = true;
        frameOpened = Time.frameCount;

        lastFadeSecondsUsed = fadeSeconds;
        this.closeMenuOnConfirm = closeMenuOnConfirm;
        previouslySelectedBeforeOpen = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        shouldRestorePreviousSelectionOnHide = false;

        // Press esc to close popup
        InputEvents.PauseStartedOverride = OnCancelButtonPressed;

        StaticUtilities.ShowCursor();
        oldTimeScale = Time.timeScale;
        if(freezeTime)
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
        confirmButton?.onClick.RemoveAllListeners();
        confirmButton?.onClick.AddListener(OnConfirmButtonClicked); // may be redundant to remove this listener and then immediately add it back but idk else to do it.
        onConfirmationButtonClicked = OnConfirmationButtonClicked;
        if (OnConfirmationButtonClicked != null)
            confirmButton?.onClick.AddListener(OnConfirmationButtonClicked);

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        StaticUtilities.EnableCanvasGroup(canvasGroup, alpha: 0);

        if(InputEvents.Instance.IsGamepadActive())
            EventSystem.current.SetSelectedGameObject(cancelButton.gameObject);

        if (lastFadeSecondsUsed > 0)
            fadeOpacityCoroutine = StaticUtilities.FadeToVisible(canvasGroup, fadeSeconds, unscaledTime: true);
        else
            canvasGroup.alpha = 1;
    }

    public virtual void HideConfirmationPopup()
    {
        InputEvents.PauseStartedOverride = lastPauseStartedOverride == null ? null : lastPauseStartedOverride;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (lastFadeSecondsUsed > 0)
            fadeOpacityCoroutine = StaticUtilities.FadeToHidden(canvasGroup, lastFadeSecondsUsed, 
                                                currentCoroutineToCancel: fadeOpacityCoroutine, afterFadeCallback: AfterFadeToHidden);

        else
        {
            StaticUtilities.DisableCanvasGroup(canvasGroup);
            RestorePreviousSelectionIfNeeded();
            AnyConfirmationMenuOpen = false;
            if(afterCancelClicked != null)
                afterCancelClicked();
        }
    }

    protected virtual void AfterFadeToHidden()
    {
        StaticUtilities.DisableCanvasGroup(canvasGroup);
        RestorePreviousSelectionIfNeeded();
        AnyConfirmationMenuOpen = false;
        if (afterCancelClicked != null)
            afterCancelClicked();
    }

    void OnCancelButtonPressed()
    {
        // weird controller bug. too close to fuse for a good solution;
        if(Time.frameCount == frameOpened)
        {
            return;
        }

        Time.timeScale = oldTimeScale;
        shouldRestorePreviousSelectionOnHide = true;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        HideConfirmationPopup();
    }

    protected virtual void OnConfirmButtonClicked()
    {
        Time.timeScale = oldTimeScale;
        shouldRestorePreviousSelectionOnHide = false;
        if (closeMenuOnConfirm)
        {
            StaticUtilities.DisableCanvasGroup(canvasGroup);
            AnyConfirmationMenuOpen = false;
        }
        InputEvents.PauseStartedOverride = lastPauseStartedOverride == null ? null : lastPauseStartedOverride;
    }

    private void RestorePreviousSelectionIfNeeded()
    {
        if (!shouldRestorePreviousSelectionOnHide)
            return;

        shouldRestorePreviousSelectionOnHide = false;

        if (EventSystem.current == null || previouslySelectedBeforeOpen == null || !previouslySelectedBeforeOpen.activeInHierarchy)
            return;

        Selectable selectable = previouslySelectedBeforeOpen.GetComponent<Selectable>();
        if (selectable != null && !selectable.IsInteractable())
            return;

        EventSystem.current.SetSelectedGameObject(previouslySelectedBeforeOpen);
    }
}
