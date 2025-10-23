/*
 * Contributors: Toby
 * Creation Date: 10/23/25
 * Last Modified: 10/23/25
 * 
 * Brief Description: billboarded. Appears when the player can interact with it.
 * Childed under billboard UI manager.
 * Gets shown / hidden when the player is looking at the base gameobject.
 */

using NaughtyAttributes;
using NaughtyAttributes.Test;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ButtonPromptBillboardUI : IBillboardUI
{
    [SerializeField, Required] private RectTransform popupParent;
    [SerializeField, Required] private TMP_Text interactText;

    private ButtonPromptInteractable buttomPrompt;
    private Coroutine popupAnimation;


    /// <summary>
    /// Called when this objects is initialized
    /// </summary>
    /// <param name="sourceGameObject"></param>
    public override void OnInitialize(GameObject sourceGameObject)
    {
        var buttonPrompter = sourceGameObject.GetComponentInChildren<ButtonPromptInteractable>();
        buttonPrompter.InitializeFromBillboardUI(this);
    }

    public override void Show()
    {
        base.Show();

        StaticUtilities.StopAndStartCoroutine(ref popupAnimation, PopupAnimation());
    }

    public void UpdateButtonPrompt()
    {
        interactText.text = buttomPrompt.buttonText;
    }

    private IEnumerator PopupAnimation()
    {
        //var startRotation
        yield break;
    }

}
