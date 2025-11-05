/*
 * Contributors: Toby
 * Creation Date: 10/20/25
 * Last Modified: 10/27/25
 * 
 * Brief Description: Resusable & modular UI popup for confirming the users choice.
 */

using NaughtyAttributes;
using System;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cancelButton.onClick.AddListener(OnCancelButtonPressed);
        canvasGroup = GetComponent<CanvasGroup>();
        if(hideOnCreation )
            StaticUtilities.DisableCanvasGroup(canvasGroup);
    }

    public void OpenConfirmationPopup(string? text = null, UnityAction? OnConfirmationButtonClicked = null, UnityAction? OnCancelButtonClicked = null)
    {
        if(canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        // shelving this for now, but i think its important
        /*if(InputEvents.Instance != null)
        {
            InputEvents.PauseStarted.AddListener(HideConfirmationPopup);
        }

        GameManager.Instance.IsPaused = true;*/
        StaticUtilities.ShowCursor();
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        if(text != null)
        {
            confirmationText.text = text;
        }

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(OnCancelButtonPressed);
        if (OnCancelButtonClicked != null)
            cancelButton.onClick.AddListener(OnCancelButtonClicked);

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnConfirmButtonClicked); // may be redundant to remove this listener and then immediately add it back but idk else to do it.
        if(OnConfirmationButtonClicked != null)
            confirmButton.onClick.AddListener(OnConfirmationButtonClicked);

        StaticUtilities.EnableCanvasGroup(canvasGroup);
    }

    public void HideConfirmationPopup()
    {
        // if player isnt in the pause menu rn
        /*if(InputEvents.Instance != null && !InputEvents.PausePressed)
        {
            GameManager.Instance.IsPaused = false;
            Time.timeScale = oldTimeScale;
            StaticUtilities.HideCursor();
        }*/

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        StaticUtilities.DisableCanvasGroup(canvasGroup);
    }

    void OnCancelButtonPressed()
    {
        Time.timeScale = oldTimeScale;
        StaticUtilities.DisableCanvasGroup(canvasGroup);
    }

    void OnConfirmButtonClicked()
    {
        Time.timeScale = oldTimeScale;
        StaticUtilities.DisableCanvasGroup(canvasGroup);
    }

}
