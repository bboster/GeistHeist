/*
 * Contributors: Toby
 * Creation Date: 10/20/25
 * Last Modified: 10/20/25
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

    private CanvasGroup canvasGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cancelButton.onClick.AddListener(OnCancelButtonPressed);
        canvasGroup = GetComponent<CanvasGroup>();
        StaticUtilities.DisableCanvasGroup(canvasGroup);
    }

    public void OpenConfirmationPopup(string text, UnityAction OnConfirmationButtonClicked)
    {
        confirmationText.text = text;
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnConfirmationButtonClicked);
        confirmButton.onClick.AddListener(OnConfirmButtonClicked); // may be redundant to remove this listener and then immediately add it back but idk else to do it.
        StaticUtilities.EnableCanvasGroup(canvasGroup);
    }

    void OnCancelButtonPressed()
    {
        StaticUtilities.DisableCanvasGroup(canvasGroup);
    }

    void OnConfirmButtonClicked()
    {
        StaticUtilities.DisableCanvasGroup(canvasGroup);
    }

}
